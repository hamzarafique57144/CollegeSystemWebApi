using CollegeAppWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace CollegeAppWebAPI.Models
{
    public class StudentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(50, ErrorMessage = "Student name cannot exceed 50 characters.")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [CustomValidation(typeof(DateValidator), nameof(DateValidator.ValidatePastDate))]
        public DateTime DateOfBirth { get; set; }

    }
}
