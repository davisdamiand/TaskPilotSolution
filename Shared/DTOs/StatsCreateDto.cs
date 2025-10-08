using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class StatsCreateDto
    {
        [Required(ErrorMessage ="Student ID is required")]
        public int StudentID { get; set; }
    }
}
