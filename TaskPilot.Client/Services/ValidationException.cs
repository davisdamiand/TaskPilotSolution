using System;
using System.Collections.Generic;
using System.Text;
using Shared.Security;
namespace TaskPilot.Client.Services
{
    public class ValidationException : Exception
    {
        public ErrorResponse Errors { get; }

        public ValidationException(ErrorResponse errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
