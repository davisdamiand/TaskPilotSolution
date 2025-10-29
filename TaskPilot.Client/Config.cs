using System;
using System.Collections.Generic;
using System.Text;

namespace TaskPilot
{
    public static class Config // It's good practice to make a class with only static members static
    {
        public static string BaseUrl
        {
            get
            {
#if ANDROID
                // Use the HTTP address of your laptop for Android
                return "http://10.0.0.113:5123/"; 
#elif WINDOWS
                // Use the standard HTTPS localhost for Windows
                return "https://localhost:7192/";
#else
                // Fallback for other platforms like iOS or Mac (simulators can use localhost)
                return "http://localhost:7192/";
#endif
            }
        }
    }
}