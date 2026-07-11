using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models
{
    public class MembershipPlan
    {
        public int MembershipPlanId { get; set; }

        [Required(ErrorMessage = "Plan Name is required.")]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [Range(1, 24, ErrorMessage = "Duration must be between 1 and 24 months.")]
        public int DurationInMonths { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}