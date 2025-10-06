using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class TodoCreateDto
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage ="Task name is required")]
        [StringLength(20)]
        public string Name { get; set; }
        
        [StringLength(50)]
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int PriorityLevel { get; set; } = 0;
        
    }
}
