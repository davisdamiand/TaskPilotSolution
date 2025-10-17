using Microsoft.Extensions.Logging;
using TaskPilot.Client.Services;
using TaskPilot.Client.ViewModels;

namespace TaskPilot.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register HttpClient with base address
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(Config.BaseUrl)
            });

            // Register TodoService
            builder.Services.AddSingleton<TodoService>();

            // Register ViewModel
            builder.Services.AddTransient<TodoViewModel>();

            builder.Services.AddSingleton<StudentService>();

            builder.Services.AddTransient<RegisterViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
