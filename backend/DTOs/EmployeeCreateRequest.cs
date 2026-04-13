namespace ServiceApi.DTOs;

public class EmployeeCreateRequest
{
    public string UserType { get; set; } = "user";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateOnly Dob { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ScannerId { get; set; } = string.Empty;
    public decimal? Salary { get; set; }
}
