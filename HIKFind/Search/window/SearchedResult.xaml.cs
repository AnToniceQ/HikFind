using HIKFind.InfoWindows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HIKFind.Search.window
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

            if (results.Length <= 0)
            {
                this.InfoBar.Text = "Žádný výsledek nenalezen. Zkus input frázovat jinak! (Velká, malé písmena, ...)";
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
                    HikFindSearch.SaveProduct(await Searcher.ScrapeFinalProduct(lb.SelectedItem.ToString(), loading, true));
                }
                this.Close();
            }
            catch (System.NotSupportedException ex)
            {
                loading.Close();
                this.Show();
                InfoWindow.ShowUp("Počkejte než se vyhledá první požadavek! " + ex.Message, InfoWindows.Helpers.InfoWindowEnum.Alert);
            }
        }

        private void CopyUrl(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            Clipboard.SetText(item.ToolTip.ToString());
        }
    }
}
