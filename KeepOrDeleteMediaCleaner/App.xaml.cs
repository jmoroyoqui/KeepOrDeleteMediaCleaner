using KeepOrDeleteMediaCleaner.Views;

namespace KeepOrDeleteMediaCleaner
{
    public partial class App : Application
    {
        public App(MediaViewerMainPage page)
        {
            InitializeComponent();

            MainPage = new NavigationPage(page);
        }
    }
}
