using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StatsSendDto
    {
        public int TotalPomodoroSessions { get; set; }
        public int Streak { get; set; }
        public int TotalCompletedTasks { get; set; }

        public int TotalInCompletedTasks { get; set; }
    }
}
