using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Shared.DTOs
{
    public class TodoGetDto: INotifyPropertyChanged
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public double PrioritySelection { get; set; }

        public ICommand ToggleCompleteCommand { get; set; }

        public int PriorityLevel { get; set; }

        public int TimeSpentMinutes { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDateTime { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [CustomValidation(typeof(TodoGetDto), nameof(ValidateEndTime))]
        public DateTime EndDateTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Simpler, framework-agnostic OnPropertyChanged to avoid referencing MAUI runtime types here.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // CustomValidation expects a method with signature (object value, ValidationContext context)
        public static ValidationResult ValidateEndTime(object value, ValidationContext context)
        {
            if (value == null) return new ValidationResult("End time is required");

            if (!(value is DateTime endTime)) return new ValidationResult("Invalid end time");

            var instance = context.ObjectInstance as TodoGetDto;
            if (instance == null) return new ValidationResult("Validation context is invalid");

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

                // ensure double division and double arguments for Math.Clamp
                return Math.Clamp((double)TimeSpentMinutes / totalMinutes, 0.0, 1.0);
            }
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted == value) return;

                // Only allow marking as completed (true). Ignore attempts to unmark.
                if (!value && _isCompleted)
                {
                    // Ignore un-check requests to avoid toggle loops and UI issues.
                    return;
                }

                _isCompleted = value;
                OnPropertyChanged(); // This tells the UI to refresh!
            }
        }
    }
}