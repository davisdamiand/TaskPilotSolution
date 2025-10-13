using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class TodoCreateDto : IValidatableObject
    {
        [Required(ErrorMessage = "StudentId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Task name is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Task name must be between 3 and 20 characters")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeOnly EndTime { get; set; }

        [Range(0, 5, ErrorMessage = "Priority level must be between 0 (lowest) and 5 (highest)")]
        public int PriorityLevel { get; set; } = 0;

        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be later than start time",
                    new[] { nameof(EndTime), nameof(StartTime) });
            }

            if (DueDate < DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "Due date cannot be in the past",
                    new[] { nameof(DueDate) });
            }
        }
    }
}
