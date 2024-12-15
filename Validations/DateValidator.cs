using System.ComponentModel.DataAnnotations;

namespace CollegeAppWebAPI.Validations
{
    public static class DateValidator
    {
        public static ValidationResult ValidatePastDate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("Date of Birth cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
