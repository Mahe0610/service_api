using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceApi.Data;
using ServiceApi.DTOs;
using ServiceApi.Models;

namespace ServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext dbContext) : ControllerBase
{
    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin@123";

    [HttpPost("register")]
    public async Task<ActionResult<object>> Register([FromBody] EmployeeCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        if (await dbContext.EmployeeRecords.AnyAsync(e => e.Username == request.Username))
            return BadRequest("Username already exists.");

        if (await dbContext.EmployeeRecords.AnyAsync(e => e.Email == request.Email))
            return BadRequest("Email already exists.");

        var record = new EmployeeRecord
        {
            UserType = "user",
            Username = request.Username.Trim(),
            Password = request.Password,
            EmployeeName = request.EmployeeName,
            Age = request.Age,
            Dob = request.Dob,
            Email = request.Email,
            ScannerId = request.ScannerId,
            Salary = null
        };

        dbContext.EmployeeRecords.Add(record);
        await dbContext.SaveChangesAsync();

        return Ok(new { role = "user", userId = record.Id, name = record.EmployeeName, username = record.Username });
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
    {
        if (request.Username == AdminUsername && request.Password == AdminPassword)
            return Ok(new { role = "admin", userId = 0, name = "Administrator", username = AdminUsername });

        var user = await dbContext.EmployeeRecords.FirstOrDefaultAsync(e => e.Username == request.Username && e.Password == request.Password);
        if (user is null)
            return Unauthorized("Invalid username or password.");

        return Ok(new { role = "user", userId = user.Id, name = user.EmployeeName, username = user.Username });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
