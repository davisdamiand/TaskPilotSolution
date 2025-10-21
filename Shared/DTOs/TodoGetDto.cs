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

        public double PrioritySelection { get; set; }

        public int PriorityLevel { get; set; }

        public int TimeSpentMinutes { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDateTime { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [CustomValidation(typeof(TodoGetDto), nameof(ValidateEndTime))]
        public DateTime EndDateTime { get; set; }

        public static ValidationResult ValidateEndTime(DateTime endTime, ValidationContext context)
        {
            var instance = (TodoGetDto)context.ObjectInstance;
            if (endTime <= instance.StartDateTime)
            {
                return new ValidationResult("End time must be later than start time");
            }
            return ValidationResult.Success!;
        }
        public double Progress
        {
            get
            {
                var totalMinutes = (EndDateTime - StartDateTime).TotalMinutes;
                if (totalMinutes <= 0) return 0;

                return Math.Clamp(TimeSpentMinutes / totalMinutes, 0, 1);
            }
        }
    }
}