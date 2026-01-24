using KeepOrDeleteMediaCleaner.Services;
using KeepOrDeleteMediaCleaner.ViewModels;
using KeepOrDeleteMediaCleaner.Views;
#if ANDROID
using KeepOrDeleteMediaCleaner.Platforms.Android.Services;
#endif
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection; // Add this using directive
namespace KeepOrDeleteMediaCleaner
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
            //builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MediaViewerMainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID

            builder.Services.AddSingleton<IImageScannerService, ImageScannerService>();
            builder.Services.AddSingleton<ImageViewerViewModel>();
            builder.Services.AddTransient<ImageScannerPage>();
#endif

            return builder.Build();
        }
    }
}
