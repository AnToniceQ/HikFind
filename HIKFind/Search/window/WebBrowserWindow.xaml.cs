using System;
using System.Windows;
using HIKFind.InfoWindows;

namespace HIKFind.Search.window
{
    /// <summary>
    /// Interakční logika pro WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window
    {
        public WebBrowserWindow()
        {
            InitializeComponent();
            browser.Source = new Uri("https://www.google.com");
        }

        private void Google_Click(object sender, RoutedEventArgs e)
        {
            browser.Source = new Uri("https://www.google.com");
        }

        private async void Ziskat_Informace_Click(object sender, RoutedEventArgs e)
        {
            LoadingWindow loading = new LoadingWindow("Získávám data z: " + browser.Source.AbsoluteUri, "Začínám stahovat stránku produktu");
            HikFindSearch.SaveProduct(await Searcher.ScrapeFinalProduct(browser.Source.AbsoluteUri, loading, true));
        }

        private void Storno_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
