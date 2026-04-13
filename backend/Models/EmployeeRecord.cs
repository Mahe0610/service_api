namespace ServiceApi.Models;

public class EmployeeRecord
{
    public int Id { get; set; }
    public string UserType { get; set; } = "user"; // admin or user
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateOnly Dob { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ScannerId { get; set; } = string.Empty;
    public decimal? Salary { get; set; }
    public string CertificateCode { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
