using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StudentLeague
    {
        public int LeagueRank { get; set; }
        public string StudentName { get; set; }
        public int TimeSpentMinutes { get; set; }

        public bool IsCurrentStudent { get; set; } = false;
    }
}
