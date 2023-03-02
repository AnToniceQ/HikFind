using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HIKFind
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
            this.Close();
            await Searcher.ScrapeFinalProduct(browser.Source.AbsoluteUri,loading); 
        }

        private void Storno_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
