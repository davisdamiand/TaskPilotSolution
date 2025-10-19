using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class TodoUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDateTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int PriorityLevel { get; set; }
    }

}
