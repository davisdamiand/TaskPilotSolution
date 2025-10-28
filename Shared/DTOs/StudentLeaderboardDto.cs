using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StudentLeaderboardDto
    {
        public int TimeSpentInSeconds { get; set; } = 0;
        public string StudentName { get; set; } = string.Empty;
        public string StudentSurname { get; set; } = string.Empty;
    }
}
