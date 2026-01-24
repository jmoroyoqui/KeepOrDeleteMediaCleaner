using KeepOrDeleteMediaCleaner.Models;
using KeepOrDeleteMediaCleaner.Permissions;
using KeepOrDeleteMediaCleaner.Services;
using KeepOrDeleteMediaCleaner.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KeepOrDeleteMediaCleaner.Views;

public partial class ImageScannerPage : ContentPage
{
	private readonly ImageViewerViewModel _viewModel;

    public ImageScannerPage(ImageViewerViewModel viewModel)
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
            "La app necesita acceso a tus imágenes",
            "OK");

            return;
        }
        base.OnAppearing();
		await _viewModel.InitializeAsync();
    }
}