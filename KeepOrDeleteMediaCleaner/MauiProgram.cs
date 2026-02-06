using KeepOrDeleteMediaCleaner.Services;
using KeepOrDeleteMediaCleaner.ViewModels;
using KeepOrDeleteMediaCleaner.Views;
#if ANDROID
using KeepOrDeleteMediaCleaner.Platforms.Android.Services;
#endif
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection; // Add this using directive
using CommunityToolkit.Maui; // Add this using directive
using Microsoft.Maui.Controls; // Add this using directive for MediaElement
using Microsoft.Maui.Controls.Handlers; // Add this using directive for MediaElementHandler
using CommunityToolkit.Maui.Views; // For MediaElement
using CommunityToolkit.Maui.Core.Handlers; // For MediaElementHandler
using CommunityToolkit.Maui;

namespace KeepOrDeleteMediaCleaner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().UseMauiCommunityToolkit() // Chain this directly after .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkitMediaElement();
            //builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MediaViewerMainPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
#if ANDROID
            builder.Services.AddSingleton<IImageScannerService, ImageScannerService>();
            builder.Services.AddSingleton<IVideoScannerService, VideoScannerService>();
            builder.Services.AddSingleton<ImageViewerViewModel>();
            builder.Services.AddTransient<ImageScannerPage>();
            builder.Services.AddSingleton<VideoViewerViewModel>();
            builder.Services.AddTransient<VideoScannerPage>();
#endif
            return builder.Build();
        }
    }
}