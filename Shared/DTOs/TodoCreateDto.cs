using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Prevent exposing the Server Entity Framework model to the client in full
    public class TodoCreateDto : IValidatableObject
    {
        // Foreign key to associate the todo with a student
        [Required(ErrorMessage = "StudentId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Task name is required")]
        [StringLength(30, ErrorMessage = "Task name must be between 3 and 30 characters")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        public DateTime DueDateTime { get; set; }

        [Range(0, 5, ErrorMessage = "Priority level must be between 0 (lowest) and 5 (highest)")]
        public int PriorityLevel { get; set; } = 0;

        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDateTime < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Due date cannot be in the past",
                    new[] { nameof(DueDateTime) });
            }

        }
    }
}
