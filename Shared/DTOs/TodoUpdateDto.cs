using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class TodoUpdateDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDateTime { get; set; }
        public int PriorityLevel { get; set; }
    }

}
