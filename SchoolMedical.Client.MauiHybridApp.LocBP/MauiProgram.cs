using Microsoft.Extensions.Logging;


namespace SchoolMedical.Client.MauiHybridApp.LocBP
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddHttpClient(/*"Ocelot", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7261"); // ✅ real base URL, e.g., Ocelot Gateway
            }*/);


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

#endif
            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("********** OMG! FirstChanceException **********");
                System.Diagnostics.Debug.WriteLine(e.Exception);
            };
            return builder.Build();
        }
    }
}
