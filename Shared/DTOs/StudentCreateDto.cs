using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    // Prevent exposing the Server Entity Framework model to the client in full
    public class StudentCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [StringLength(50, ErrorMessage = "Surname cannot exceed 50 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [CustomValidation(typeof(StudentCreateDto), nameof(ValidateDOB))]
        public DateOnly DOB { get; set; }

        public static ValidationResult ValidateDOB(DateOnly dob, ValidationContext context)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (dob > today)
                return new ValidationResult("Date of Birth cannot be in the future");
            if (today.Year - dob.Year < 16)
                return new ValidationResult("Student must be at least 16 years old");

            return ValidationResult.Success;
        }
    }
}
