using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Dashboard;

public sealed class SupabaseDestinationPostStore : IDestinationPostStore
{
    private readonly HttpClient _httpClient;
    private readonly SupabaseOptions _options;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SupabaseDestinationPostStore(HttpClient httpClient, IOptions<SupabaseOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.Url.TrimEnd('/') + "/rest/v1/");
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceRoleKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ServiceRoleKey);
    }

    public async Task<List<DestinationPost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?select=*&order=created_at.desc", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabasePostRow>>(stream, _jsonOptions, cancellationToken) ?? [];
        return rows.Select(row => row.ToPost(_jsonOptions)).ToList();
    }

    public async Task<List<DestinationPost>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var encodedUserId = Uri.EscapeDataString(userId);
        using var response = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?user_id=eq.{encodedUserId}&select=*&order=created_at.desc", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var rows = await JsonSerializer.DeserializeAsync<List<SupabasePostRow>>(stream, _jsonOptions, cancellationToken) ?? [];
        return rows.Select(row => row.ToPost(_jsonOptions)).ToList();
    }

    public async Task<DestinationPost> CreateAsync(DestinationPost post, CancellationToken cancellationToken = default)
    {
        var row = SupabasePostRow.FromPost(post, _jsonOptions);
        using var response = await _httpClient.PostAsJsonAsync(_options.DestinationPostsTable, row, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return post;
    }

    public async Task<DestinationPost?> IncrementLikesAsync(string postId, CancellationToken cancellationToken = default)
    {
        // First get the post to know current likes
        var encodedPostId = Uri.EscapeDataString(postId);
        using var getResponse = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?id=eq.{encodedPostId}&select=likes", cancellationToken);
        getResponse.EnsureSuccessStatusCode();

        var rows = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>(cancellationToken: cancellationToken);
        if (rows == null || rows.Count == 0) return null;

        var currentLikes = rows[0].GetProperty("likes").GetInt32();
        var newLikes = currentLikes + 1;

        // Patch the likes
        using var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"{_options.DestinationPostsTable}?id=eq.{encodedPostId}")
        {
            Content = JsonContent.Create(new { likes = newLikes }, options: _jsonOptions)
        };
        using var patchResponse = await _httpClient.SendAsync(patchRequest, cancellationToken);
        patchResponse.EnsureSuccessStatusCode();

        // Get full post to return
        using var finalResponse = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?id=eq.{encodedPostId}&select=*", cancellationToken);
        finalResponse.EnsureSuccessStatusCode();
        var finalRows = await finalResponse.Content.ReadFromJsonAsync<List<SupabasePostRow>>(options: _jsonOptions, cancellationToken: cancellationToken);
        
        return finalRows?.FirstOrDefault()?.ToPost(_jsonOptions);
    }

    public async Task<DestinationPost?> AddCommentAsync(string postId, PostComment comment, CancellationToken cancellationToken = default)
    {
        var encodedPostId = Uri.EscapeDataString(postId);
        using var getResponse = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?id=eq.{encodedPostId}&select=comments_json", cancellationToken);
        getResponse.EnsureSuccessStatusCode();

        var rows = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>(cancellationToken: cancellationToken);
        if (rows == null || rows.Count == 0) return null;

        var commentsJson = rows[0].GetProperty("comments_json").GetString() ?? "[]";
        var comments = JsonSerializer.Deserialize<List<PostComment>>(commentsJson, _jsonOptions) ?? [];
        
        comments.Add(comment);
        var updatedCommentsJson = JsonSerializer.Serialize(comments, _jsonOptions);

        // Patch the comments and update comment count
        using var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"{_options.DestinationPostsTable}?id=eq.{encodedPostId}")
        {
            Content = JsonContent.Create(new 
            { 
                comments_json = updatedCommentsJson,
                comments = comments.Count
            }, options: _jsonOptions)
        };
        using var patchResponse = await _httpClient.SendAsync(patchRequest, cancellationToken);
        patchResponse.EnsureSuccessStatusCode();

        // Get full post to return
        using var finalResponse = await _httpClient.GetAsync($"{_options.DestinationPostsTable}?id=eq.{encodedPostId}&select=*", cancellationToken);
        finalResponse.EnsureSuccessStatusCode();
        var finalRows = await finalResponse.Content.ReadFromJsonAsync<List<SupabasePostRow>>(options: _jsonOptions, cancellationToken: cancellationToken);
        
        return finalRows?.FirstOrDefault()?.ToPost(_jsonOptions);
    }

    private sealed class SupabasePostRow
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; } = string.Empty;

        [JsonPropertyName("author_email")]
        public string AuthorEmail { get; set; } = string.Empty;

        [JsonPropertyName("destination_name")]
        public string DestinationName { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("caption")]
        public string Caption { get; set; } = string.Empty;

        [JsonPropertyName("tag")]
        public string Tag { get; set; } = string.Empty;

        [JsonPropertyName("image_file_name")]
        public string ImageFileName { get; set; } = string.Empty;

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("comments")]
        public int Comments { get; set; }

        [JsonPropertyName("comments_json")]
        public string CommentsJson { get; set; } = "[]";

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        public static SupabasePostRow FromPost(DestinationPost post, JsonSerializerOptions jsonOptions)
        {
            return new SupabasePostRow
            {
                Id = post.Id,
                UserId = post.UserId,
                AuthorName = post.AuthorName,
                AuthorEmail = post.AuthorEmail,
                DestinationName = post.DestinationName,
                Location = post.Location,
                Description = post.Description,
                Caption = post.Caption,
                Tag = post.Tag,
                ImageFileName = post.ImageFileName,
                Likes = post.Likes,
                Comments = post.Comments,
                CommentsList = post.CommentsList,
                CommentsJson = JsonSerializer.Serialize(post.CommentsList, jsonOptions),
                CreatedAt = post.CreatedAt
            };
        }

        [JsonIgnore]
        public List<PostComment> CommentsList { get; set; } = [];

        public DestinationPost ToPost(JsonSerializerOptions jsonOptions)
        {
            return new DestinationPost
            {
                Id = Id,
                UserId = UserId,
                AuthorName = AuthorName,
                AuthorEmail = AuthorEmail,
                DestinationName = DestinationName,
                Location = Location,
                Description = Description,
                Caption = Caption,
                Tag = Tag,
                ImageFileName = ImageFileName,
                Likes = Likes,
                Comments = Comments,
                CommentsList = JsonSerializer.Deserialize<List<PostComment>>(CommentsJson, jsonOptions) ?? [],
                CreatedAt = CreatedAt
            };
        }
    }
}
