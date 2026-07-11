using Dapper;
using GymManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymManagement.Controllers
{
    [Authorize] // 🔒 CRITICAL: This locks down the entire controller. Only logged-in users with a valid JWT token can get in!
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MemberController(IConfiguration config)
        {
            _config = config;
        }

        // 1. GET ALL MEMBERS
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sqlQuery = "SELECT * FROM Members";
            var members = await connection.QueryAsync<Member>(sqlQuery);

            return Ok(members);
        }

        // 2. CREATE A NEW MEMBER (With strict validation rules)
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] Member member)
        {
            // Check basic rules (like phone number formatting or empty fields)
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Special Assignment Rule: Date of Birth cannot be in the future
            if (member.DateOfBirth > DateTime.Today)
            {
                return BadRequest("Date of Birth cannot be in the future.");
            }

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            // Special Assignment Rule: Verify that the assigned MembershipPlanId actually exists!
            string checkPlanSql = "SELECT COUNT(1) FROM MembershipPlans WHERE MembershipPlanId = @PlanId";
            var planExists = await connection.ExecuteScalarAsync<bool>(checkPlanSql, new { PlanId = member.MembershipPlanId });

            if (!planExists)
            {
                return BadRequest("The selected Membership Plan does not exist. Please assign a valid Plan ID.");
            }

            // Insert into Database
            string insertSql = @"INSERT INTO Members (FullName, Email, Phone, Gender, DateOfBirth, JoinDate, MembershipPlanId, IsActive) 
                                 VALUES (@FullName, @Email, @Phone, @Gender, @DateOfBirth, @JoinDate, @MembershipPlanId, @IsActive)";

            await connection.ExecuteAsync(insertSql, member);
            return Ok(new { message = "Gym member added successfully!" });
        }

        // 3. UPDATE MEMBER DETAILS
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] Member member)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (member.DateOfBirth > DateTime.Today) return BadRequest("Date of Birth cannot be in the future.");

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string updateSql = @"UPDATE Members 
                                 SET FullName = @FullName, 
                                     Email = @Email, 
                                     Phone = @Phone, 
                                     Gender = @Gender, 
                                     DateOfBirth = @DateOfBirth, 
                                     MembershipPlanId = @MembershipPlanId, 
                                     IsActive = @IsActive 
                                 WHERE MemberId = @MemberId";

            member.MemberId = id;
            int rowsAffected = await connection.ExecuteAsync(updateSql, member);

            if (rowsAffected == 0) return NotFound("Member not found.");
            return Ok(new { message = "Member details updated successfully!" });
        }

        // 4. DELETE A MEMBER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string deleteSql = "DELETE FROM Members WHERE MemberId = @Id";
            int rowsAffected = await connection.ExecuteAsync(deleteSql, new { Id = id });

            if (rowsAffected == 0) return NotFound("Member not found.");
            return Ok(new { message = "Member deleted successfully!" });
        }
    }
}