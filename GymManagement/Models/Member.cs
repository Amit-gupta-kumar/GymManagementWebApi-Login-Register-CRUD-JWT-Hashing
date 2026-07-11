using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Plz Fill the name section")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Plz Fill the Email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(\+977)?9\d{9}$", ErrorMessage = "Plz enter the correct phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Required]
        public int MembershipPlanId { get; set; } // Links to our plan table (Foreign Key)

        public bool IsActive { get; set; } = true;
    }
}