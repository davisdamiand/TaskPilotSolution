using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class TodoGetDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 3, ErrorMessage = "Priority must be between 1 (Low), 2 (Medium), 3 (High)")]
        public int PrioritySelection { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Time spent must be greater than 0 minutes")]
        public TimeOnly TimeSpent { get; set; }   // e.g. 100 = 100 minutes

        [Required(ErrorMessage = "Due date is required")]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [CustomValidation(typeof(TodoGetDto), nameof(ValidateEndTime))]
        public TimeOnly EndTime { get; set; }

        public static ValidationResult ValidateEndTime(TimeOnly endTime, ValidationContext context)
        {
            var instance = (TodoGetDto)context.ObjectInstance;
            if (endTime <= instance.StartTime)
            {
                return new ValidationResult("End time must be later than start time");
            }
            return ValidationResult.Success!;
        }
    }
}
