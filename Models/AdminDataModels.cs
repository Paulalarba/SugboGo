namespace SugboGo.Models;

public sealed class AdminGem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int FlashpackerScore { get; set; }
    public string QualityCheckDate { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MapX { get; set; }
    public int MapY { get; set; }
}

public sealed class ItineraryTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public string Stops { get; set; } = string.Empty;
    public string AvgDuration { get; set; } = string.Empty;
}

public sealed class AdminPartner
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Commission { get; set; } = string.Empty;
    public string LastAudit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class AdminDataContainer
{
    public List<AdminGem> Gems { get; set; } = [];
    public List<ItineraryTemplate> Templates { get; set; } = [];
    public List<AdminPartner> Partners { get; set; } = [];
}
