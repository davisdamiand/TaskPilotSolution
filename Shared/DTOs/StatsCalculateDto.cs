using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StatsCalculateDto
    {
        public int StudentID { get; set; }
        public DateOnly LastAccessedDay { get; set; }
    }
}
