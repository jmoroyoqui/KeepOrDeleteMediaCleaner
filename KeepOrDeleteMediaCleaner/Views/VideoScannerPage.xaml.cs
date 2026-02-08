using Android.Webkit;
using KeepOrDeleteMediaCleaner.Permissions;
using KeepOrDeleteMediaCleaner.ViewModels;
using Plugin.Maui.VideoPlayer;

namespace KeepOrDeleteMediaCleaner.Views;

public partial class VideoScannerPage : ContentPage
{
	private readonly VideoViewerViewModel _viewModel;

    public VideoScannerPage(VideoViewerViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
	{
        var status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<ReadImagePermission>();

        if (status != PermissionStatus.Granted)
        {
            await Application.Current.MainPage.DisplayAlert(
            "Permiso requerido",
            "La app necesita acceso a tus videos",
            "OK");

            return;
        }
        base.OnAppearing();
		await _viewModel.InitializeAsync();
    }

    private void VideoCarousel_PositionChanged(object sender, PositionChangedEventArgs e)
    {
        if(sender is CarouselView carousel)
        {
            foreach(var view in carousel.VisibleViews)
            {
                if(view is Grid grid)
                {
                    foreach(var child in grid.Children)
                    {
                        if(child is VideoPlayer player)
                        {
                            player.Stop();
                        }
                    }
                }
            }   
        }
    }
}