using System;
using System.Collections.Generic;
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
    /// Interakční logika pro InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private InfoWindow(string infoText)
        {
                InitializeComponent();
                this.InfoText.Text = infoText;
                this.ShowDialog();
        }

        public static void ShowUp(string infoText)
        {
            InfoWindow infoWindow = new InfoWindow(infoText);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
