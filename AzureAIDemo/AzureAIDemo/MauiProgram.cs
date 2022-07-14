using AzureAIDemo.Data;
using AzureAIDemo.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace AzureAIDemo
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
            /*
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".dae", "text/xml");
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });*/
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<FaceServices>();
            builder.Services.AddSingleton<OCRService>();
            builder.Services.AddSingleton<ComputerVisionService>();
            builder.Services.AddSingleton<TextToSpeechService>();
            builder.Services.AddSingleton<SpeechService>();
            builder.Services.AddSingleton<TranslateService>();
            builder.Services.AddSingleton<FormRecognizersService>();
            builder.Services.AddSingleton<LanguageService>();

            return builder.Build();
        }
    }
}