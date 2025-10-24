using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
