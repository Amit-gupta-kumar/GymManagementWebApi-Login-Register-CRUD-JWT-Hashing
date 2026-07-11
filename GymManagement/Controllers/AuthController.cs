using Dapper;
using GymManagement.Models;
using GymManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // 1. REGISTER USER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            // Check if the email already exists in the database
            string checkUserSql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            var userExists = await connection.ExecuteScalarAsync<bool>(checkUserSql, new { Email = model.Email });

            if (userExists)
            {
                return BadRequest("A user with this email already exists.");
            }

            // Securely hash the plain-text password before saving it
            string hashedPassword = PasswordHasher.HashPassword(model.Password);

            // Insert user into the database using inline SQL
            string insertSql = @"INSERT INTO Users (FullName, Email, PasswordHash, Role, IsActive) 
                                 VALUES (@FullName, @Email, @PasswordHash, @Role, 1)";

            await connection.ExecuteAsync(insertSql, new
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = model.Role
            });

            return Ok(new { message = "Registration successful!" });
        }

        // 2. LOGIN USER
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            // Find the user by their email
            string findUserSql = "SELECT * FROM Users WHERE Email = @Email";
            var user = await connection.QueryFirstOrDefaultAsync<dynamic>(findUserSql, new { Email = model.Email });

            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }

            // Verify the entered password against the hashed password in the database
            bool isPasswordValid = PasswordHasher.VerifyPassword(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return BadRequest("Invalid email or password.");
            }

            // Generate JWT Token if login is successful
            var token = GenerateJwtToken(user.Email, user.Role);
            return Ok(new { token = token, message = "Login successful!" });
        }

        // Helper Method to build a JWT Token string
        private string GenerateJwtToken(string email, string role)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // Small supporting model just for processing incoming logins
    public class UserLoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}