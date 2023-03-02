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
using System.Configuration;
using System.IO;
using HIKFind.Search.Webscraper;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Drawing;

namespace HIKFind
{
    public partial class SearchedResult : Window
    {
        SearchedResultItem[] results;

        public SearchedResult(SearchedResultItem[] results)
        {
            this.results = results;
            InitializeComponent();
            ListBoxResults.MouseDoubleClick += new MouseButtonEventHandler(ListBoxResults_Click);
            this.PocetVyhledanychPolozek.Text = results.Length.ToString();
            this.ListBoxResults.ItemsSource = results;
            this.Show();

            if(results.Length <= 0)
            {
                this.InfoBar.Text = "Žádný výsledek nenalezen. Zkus input frázovat jinak! (Velké, malé písmena, ...)";
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ListBoxResults_Click(object sender, EventArgs e)
        {
            
            ListBox lb = sender as ListBox;
            LoadingWindow loading = new LoadingWindow("Získávám data z: " + lb.SelectedItem.ToString(), "Začínám stahovat stránku produktu");

            try
            {
                if (lb != null)
                {
                    this.Hide();
                        await Searcher.ScrapeFinalProduct(lb.SelectedItem.ToString(), loading);
                }
                this.Close();
            }
            catch (System.NotSupportedException ex)
            {
                loading.Close();
                this.Show();
                AlertWindow.ShowUp("Počkejte než se vyhledá první požadavek! " + ex.Message);
            }
        }

        private void CopyUrl(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            Clipboard.SetText(item.ToolTip.ToString());
        }
    }
}
