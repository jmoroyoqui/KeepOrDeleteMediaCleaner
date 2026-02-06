
using KeepOrDeleteMediaCleaner.Permissions;
using KeepOrDeleteMediaCleaner.ViewModels;

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
}