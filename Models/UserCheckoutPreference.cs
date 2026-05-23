using System.ComponentModel.DataAnnotations;

namespace SugboGo.Models;

public sealed class UserCheckoutPreference
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    public string UserId { get; set; } = string.Empty;

    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(180)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(90)]
    public string City { get; set; } = string.Empty;

    [StringLength(90)]
    public string StateProvince { get; set; } = string.Empty;

    [StringLength(24)]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(160)]
    public string EmailAddress { get; set; } = string.Empty;

    [StringLength(100)]
    public string CardholderName { get; set; } = string.Empty;

    [StringLength(4)]
    public string CardLast4 { get; set; } = string.Empty;

    [StringLength(7)]
    public string CardExpiry { get; set; } = string.Empty;

    [StringLength(40)]
    public string PaymentMethod { get; set; } = "Card";

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
