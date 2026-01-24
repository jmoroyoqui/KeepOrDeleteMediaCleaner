namespace KeepOrDeleteMediaCleaner.Views;

public partial class MediaViewerMainPage : ContentPage
{
	public MediaViewerMainPage()
	{
		InitializeComponent();
	}

	private async void OnScanImagesClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(
			App.Current.MainPage.Handler.MauiContext.Services.GetRequiredService<ImageScannerPage>()
            );
    }
}