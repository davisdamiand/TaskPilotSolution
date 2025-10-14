using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Security
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }

    }
}
