using Microsoft.Extensions.Logging;
using TaskPilot.Client.Services;
using TaskPilot.Client.ViewModels;

namespace TaskPilot.Client
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }
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

            builder.Services.AddSingleton<AppShell>();

            // Register pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<TodoPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();

            builder.Services.AddTransient<TodoViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Register Services
            builder.Services.AddSingleton<TodoService>();
            builder.Services.AddSingleton<StudentService>();
            builder.Services.AddSingleton<ProfileService>();
            builder.Services.AddSingleton<StatsService>();


            // Register Profile related services and viewmodel so Shell casn resolve ProfilePage

           

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Capture the service provider so we can use it anywhere
            Services = app.Services;

            return app;
        }
    }
}
