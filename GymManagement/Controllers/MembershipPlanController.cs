using Dapper;
using GymManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipPlanController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MembershipPlanController(IConfiguration config)
        {
            _config = config;
        }

        // 1. GET ALL PLANS (Read)
        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sqlQuery = "SELECT * FROM MembershipPlans";
            var plans = await connection.QueryAsync<MembershipPlan>(sqlQuery);

            return Ok(plans);
        }

        // 2. CREATE A NEW PLAN (Create)
        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] MembershipPlan plan)
        {
            // Validates model rules (e.g., Price > 0, Duration 1-24 months)
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sqlQuery = @"INSERT INTO MembershipPlans (PlanName, DurationInMonths, Price, IsActive) 
                                VALUES (@PlanName, @DurationInMonths, @Price, @IsActive)";

            await connection.ExecuteAsync(sqlQuery, plan);
            return Ok(new { message = "Membership plan created successfully!" });
        }

        // 3. UPDATE AN EXISTING PLAN (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] MembershipPlan plan)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sqlQuery = @"UPDATE MembershipPlans 
                                SET PlanName = @PlanName, 
                                    DurationInMonths = @DurationInMonths, 
                                    Price = @Price, 
                                    IsActive = @IsActive 
                                WHERE MembershipPlanId = @MembershipPlanId";

            // Make sure the object has the correct ID from the URL path parameter
            plan.MembershipPlanId = id;

            int rowsAffected = await connection.ExecuteAsync(sqlQuery, plan);

            if (rowsAffected == 0) return NotFound("Membership plan not found.");
            return Ok(new { message = "Membership plan updated successfully!" });
        }

        // 4. DELETE A PLAN (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sqlQuery = "DELETE FROM MembershipPlans WHERE MembershipPlanId = @Id";

            int rowsAffected = await connection.ExecuteAsync(sqlQuery, new { Id = id });

            if (rowsAffected == 0) return NotFound("Membership plan not found.");
            return Ok(new { message = "Membership plan deleted successfully!" });
        }
    }
}