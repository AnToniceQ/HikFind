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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Drawing;
using System.ComponentModel;

using HIKFind.Search;
using HIKFind.Search.Webscraper;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;

namespace HIKFind
{
    public partial class HikFindSearch : Window
    {

        public static readonly CheckboxSetting webPoHledani = new CheckboxSetting("Web po hikvisionu", true);
        public static readonly RadioSetting zakladniPomoc = new RadioSetting("Základní pomoc",true, "vyhledavani");
        public static readonly RadioSetting modZoufalosti = new RadioSetting("Webové vyhledávání", "vyhledavani");

        public static readonly RadioSetting jpgFormat = new RadioSetting("Obrázek: JPG (CROP)",true,"formatObraz");
        public static readonly RadioSetting pngFormat = new RadioSetting("Obrázek: PNG (RAW)", false, "formatObraz");

        public static ObservableCollection<Product> products = new ObservableCollection<Product>();

        public static readonly SettingCategory[] settingCategories =
        {
            new SettingCategory("Možnosti vyhledávání",new SearchSetting[]
            {
                new FinderCheckboxSetting("Obrázky",new FindBetween("show-image\" data-original=\"", "\""),true,true),
                new FinderCheckboxSetting("Datasheet",new FindBetween[]
                    { new FindBetween("id=\"Data_Sheet\"","</div"), new FindBetween("href=\"","\"") },true,false),
                new FinderCheckboxSetting("User-Manual",new FindBetween[]
                    {new FindBetween("id=\"User_Manual\"","</div"), new FindBetween("href=\"", "\"") },true,false),
                new FinderCheckboxSetting("Quick-Start-Guide", new FindBetween[]
                    { new FindBetween("id=\"Quick_Start_Guide\"","</div"), new FindBetween("href=\"","\"") },true,false)
            }),
            
            new SettingCategory("Pomocník vyhledávání",new SearchSetting[] 
            {
                webPoHledani,
                new RadioSetting("Žádná pomoc","vyhledavani"),
                zakladniPomoc,
                modZoufalosti
            }),

            new SettingCategory("Formáty", new SearchSetting[]
            {
                jpgFormat,
                pngFormat
            })
        };

        Searcher searcher = new Searcher(10, new WebScraper[]
        {
            new WebScraper("EN", "https://www.hikvision.com/content/hikvision/en/search/jcr:content/root/responsivegrid/filter_web.json?keyword=", "&subcategory=&start=&num=20&daterange=undefined"),
            new WebScraper("EU", "https://www.hikvision.com/content/hikvision/europe/search/jcr:content/root/responsivegrid/filter_web_1355781697.json?keyword=", "&subcategory=&start=&num=20&daterange=undefined")
    }, "hikvision.com");

        public HikFindSearch()
        {
            InitializeComponent();
            SearchSettingsCategories.ItemsSource = settingCategories;
            productsList.DataContext = products;
        }

        private async void Hledej_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchBox.Text;

            LoadingWindow loading = new LoadingWindow("Vyhledávám: " + search, "Začínám s hledáním");

            if (search.Contains("www."))
            {
                await Searcher.ScrapeFinalProduct(search, loading);
                return;
            }

            try
            {
                if (zakladniPomoc.Check)
                {
                    loading.Text = "Upravuji Input";
                    if (search.Contains("-"))
                    {
                        int indexOfFirstDash = search.IndexOf("-");
                        int additionalDash = search.IndexOf("-", indexOfFirstDash + 1);

                        if (additionalDash > -1)
                        {
                            search = search.Remove(additionalDash, search.Length - additionalDash);
                        }
                    }

                    if (search.Contains("("))
                    {
                        int indexOfDash = search.IndexOf("(");

                        search = search.Remove(indexOfDash, search.Length - indexOfDash);
                    }
                }

                await searcher.Find(search, loading);
            }
            catch(System.NotSupportedException ex)
            {
                loading.Close();
                AlertWindow.ShowUp("Počkejte než se vyhledá první požadavek! " + ex.Message);
            }
        }

        private void OnKeyDownHandler(Object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Hledej_Click(sender,e);
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

            Trace.WriteLine(actualPath);
            if(Directory.Exists(actualPath))
            {
                Process.Start(actualPath);
            }
            else
            {
                AlertWindow.ShowUp("Složka neexistuje. Jak se to stalo? Nevím. Zkus informovat Tondu, protože tohle by se fakt nikdy nemělo stát. (Pokud se někdo nehrabal v \"Results\" složce, nebo nepřesunul .exe soubor bez \"Results\" složky :,) )");
            }
            
        }

        private void SaveProducts(object sender, RoutedEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Product>));
            using (StreamWriter wr = new StreamWriter("products.xml"))
            {
                xs.Serialize(wr, HikFindSearch.products);
            }

            InfoWindow.ShowUp("Úspěšně uloženo.");
        }

        private void DeleteProducts(object sender, RoutedEventArgs e)
        {
            string resultFolderPath = ConfigurationManager.AppSettings.Get("FolderResultPath");

            products.Clear();

            if (File.Exists("products.xml"))
            {
                FileInfo productFile = new FileInfo("products.xml");
                productFile.Delete();
            }

            if(Directory.Exists(resultFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(resultFolderPath);
                FileInfo[] files = di.GetFiles();
                
                foreach(FileInfo file in files)
                {
                    file.Delete();
                }

                DirectoryInfo[] subDirectories = di.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    subDirectory.Delete(true);
                }

                InfoWindow.ShowUp("Všechny výsledky i se složkami úspěšně smazány.");
            }
            else
            {
                InfoWindow.ShowUp("Složka \"" + resultFolderPath + "\" byla již prázdná.");
            }
        }

        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            /**
            Button btn = sender as Button;
            string folderProductPath = @btn.ToolTip.ToString();

            if(Directory.Exists(folderProductPath))
            {
                Directory.Delete(folderProductPath, true);
            }
            else
            {
                AlertWindow.ShowUp("Složka již neexistovala. Pokusím se smazat produkt z menu \"Výsledky\".");
            }
            **/

            //TO-DO
        }
    }
}
