using System.Security.Principal;
using System.Windows;
using System.Windows.Media.Imaging;
using HIKFind.InfoWindows.Helpers;

namespace HIKFind.InfoWindows
{
    /// <summary>
    /// Interakční logika pro InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private InfoWindow(string infoText, InfoWindowEnum windowType)
        {
            InitializeComponent();
            this.InfoText.Text = infoText;
            if(windowType == InfoWindowEnum.Info)
            {
                // DO NOTHING, already set
            }
            else if(windowType == InfoWindowEnum.Alert)
            {
                this.Title = "Alert";
                this.img.Source = new BitmapImage(new System.Uri(@"\Images\warning.png", System.UriKind.Relative));
                this.Border.BorderBrush = System.Windows.Media.Brushes.Orange;

            }
            this.ShowDialog();
        }

        public static void ShowUp(string infoText, InfoWindowEnum windowType)
        {
            InfoWindow infoWindow = new InfoWindow(infoText, windowType);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
