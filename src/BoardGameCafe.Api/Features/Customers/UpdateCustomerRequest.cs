using System.ComponentModel.DataAnnotations;

namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Request to update customer profile
/// </summary>
public class UpdateCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }
}
