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


               // return "https://taskpilot-asp-api-a0edfzeqctaubyf8.southafricanorth-01.azurewebsites.net/";

               return "https://localhost:7192/";
            }
        }
    }
}