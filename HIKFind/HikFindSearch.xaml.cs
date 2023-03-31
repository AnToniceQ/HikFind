using HIKFind.Settings;
using HIKFind.Search.Webscraper;
using HIKFind.InfoWindows;
using HIKFind.InfoWindows.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using HIKFind.Search.window;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HIKFind.Helpers;
using System.Windows.Shapes;

namespace HIKFind
{
    public partial class HikFindSearch : Window
    {
        public static ObservableCollection<Product> products = new ObservableCollection<Product>();

        public static ObservableCollection<BaseSetting> settings;
        public static Dictionary<string, BaseSearchSetting> dictSettings = new Dictionary<string, BaseSearchSetting>();

        Searcher searcher = new Searcher(10, new WebScraper[]
        {
            new WebScraper("EN", "https://www.hikvision.com/content/hikvision/en/search/jcr:content/root/responsivegrid/new_filter_search.json?keyword=", "&subcategory=Products&start=&num=20&daterange=undefined"),
            new WebScraper("EU", "https://www.hikvision.com/content/hikvision/europe/search/jcr:content/root/responsivegrid/new_filter_search.json?keyword=", "&subcategory=Products&start=&num=20&daterange=undefined")
    }, "hikvision.com");

        public HikFindSearch()
        {
            InitializeComponent();
            SearchSettingsCategories.ItemsSource = settings;
            productsList.DataContext = products;


        }

        public static Dictionary<string, BaseSearchSetting> FindAllSettings(BaseSetting category)
        {
            Dictionary<string, BaseSearchSetting> tempSettings = new Dictionary<string, BaseSearchSetting>();
            if(!(category.SearchSettings is null))
            foreach (BaseSetting setting in category.SearchSettings)
            {
                if (!(setting.SearchSettings is null) && setting.SearchSettings.Count() > 0)
                {
                    FindAllSettings(setting).ToList().ForEach(x => tempSettings.Add(x.Key, x.Value));
                }
                if(!(setting is SettingCategory))
                    {
                        BaseSearchSetting realSetting = setting as BaseSearchSetting;
                        tempSettings.Add(realSetting.Name, realSetting);
                    }
            }
            return tempSettings;
        }

        private async void Hledej_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchBox.Text;

            LoadingWindow loading = new LoadingWindow("Vyhledávám: " + search, "Začínám s hledáním");

            if (search.Contains("www."))
            {
                SaveProduct(await Searcher.ScrapeFinalProduct(search, loading, true));
                return;
            }

            try
            {
                if (dictSettings["upravavstupu"].Check)
                {
                    loading.Text = "Upravuji Input";
                    search = await InputHelper.OptimizeInput(search);
                }

                SaveSettings();

                await searcher.Find(search, loading);
            }
            catch (System.NotSupportedException ex)
            {
                loading.Close();
                InfoWindow.ShowUp("Počkejte než se vyhledá první požadavek! " + ex.Message, InfoWindowEnum.Alert);
            }
        }

        private void OnKeyDownHandler(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Hledej_Click(sender, e);
            }
        }

        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            var webBrowserWindow = new WebBrowserWindow();
            webBrowserWindow.ShowDialog();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string folderExePath = @Assembly.GetEntryAssembly().Location;
            string folderWOExePath = @System.IO.Path.GetDirectoryName(folderExePath);

            string folderProductPath = @btn.ToolTip.ToString();

            string actualPath = folderWOExePath + @"\" + @folderProductPath;

            if (Directory.Exists(actualPath))
            {
                Process.Start(actualPath);
            }
            else
            {
                InfoWindow.ShowUp("Složka neexistuje. Jak se to stalo? Nevím. Zkus informovat Tondu, protože tohle by se fakt nikdy nemělo stát. (Pokud se někdo nehrabal v \"Results\" složce, nebo nepřesunul .exe soubor bez \"Results\" složky :,) )", InfoWindowEnum.Alert);
            }

        }

        public static void SaveProduct(Product product)
        {
            if(product is null)
            {
                return;
            }

            products.Insert(0, product);

            File.WriteAllText("products.json", JsonConvert.SerializeObject(products));

            string tempFolderPath = ConfigurationManager.AppSettings.Get("FolderResultPath") + "\\temp\\";

            Directory.CreateDirectory(product.FolderPath);
            string[] fileTempList = System.IO.Directory.GetFiles(tempFolderPath);

            string fileName = "";
            foreach (string file in fileTempList)
            {
                fileName = System.IO.Path.GetFileName(file);

                string fileToMove = tempFolderPath + fileName;
                string moveTo = product.FolderPath + fileName;
                File.Move(fileToMove, moveTo);
            }
        }

        public static void DeleteProduct(Product product)
        {
            if (Directory.Exists(product.FolderPath))
            {
                Directory.Delete(product.FolderPath, true);
            }
            else
            {
                InfoWindow.ShowUp("Složka již neexistovala. Pokusím se smazat produkt z menu \"Výsledky\".", InfoWindowEnum.Alert);
            }

            try
            {
                products.Remove(product);
            }
            catch (Exception)
            {
                InfoWindow.ShowUp("Něco velice špatného se stalo... kontaktuj Tondu, nebo vymaž všechny výsledky.", InfoWindowEnum.Alert);
            }

            File.WriteAllText("products.json", JsonConvert.SerializeObject(products));
        }

        public static void SaveSettings()
        {
            Dictionary<string, bool> savedSettings = new Dictionary<string, bool>();

            foreach(KeyValuePair<string, BaseSearchSetting> setting in dictSettings)
            {
                savedSettings.Add(setting.Key, setting.Value.Check);
            }

            File.WriteAllText("settings.json", JsonConvert.SerializeObject(savedSettings));
        }

        private void DeleteProductsClick(object sender, RoutedEventArgs e)
        {
            string resultFolderPath = ConfigurationManager.AppSettings.Get("FolderResultPath");

            products.Clear();

            if (File.Exists("products.json"))
            {
                FileInfo productFile = new FileInfo("products.json");
                productFile.Delete();
            }

            if (Directory.Exists(resultFolderPath))
            {
                FileHelper.RemoveDirectoryContents(resultFolderPath);

                InfoWindow.ShowUp("Všechny výsledky i se složkami úspěšně smazány.", InfoWindowEnum.Info);
            }
            else
            {
                InfoWindow.ShowUp("Ve složce není co mazat.", InfoWindowEnum.Info);
            }
        }

        private void DeleteProductClick(object sender, RoutedEventArgs e)
        {
            Product product = ((Button)sender).Tag as Product;
            DeleteProduct(product);
        }

        private async void RefreshProductClick(object sender, RoutedEventArgs e)
        {
            Product product = ((Button)sender).Tag as Product;
            LoadingWindow loading = new LoadingWindow("Aktualizuji: " + product.Name, "Začínám s hledáním");

            Product tempProduct = await Searcher.ScrapeFinalProduct(product.Url, loading, false);
            if(tempProduct != null)
            {
                DeleteProduct(product);
                SaveProduct(tempProduct);
            }
        }

        private void CopyUrl(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            Clipboard.SetText(item.ToolTip.ToString());
        }
    }
}
