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
               

               
               return "https://localhost:7192/";
            }
        }
    }
}