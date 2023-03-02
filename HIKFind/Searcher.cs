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

namespace HIKFind
{
    class Searcher
    {
        int maxSearch = 10;
        WebScraper[] webScrapers;
        WebScraper webScraperWEB;

        public static bool stop = false;

        public Searcher(int maxSearch,WebScraper[] webScrapers, string webKeywordHelp)
        {
            this.maxSearch = maxSearch;
            this.webScrapers = webScrapers;
            this.webScraperWEB = new WebScraper("WEB", "https://spapi.searchencrypt.com/api/search?st=web&q=", "+"+ webKeywordHelp + "&types=web");
            Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("FolderResultPath"));
        }

        List<Product> results = new List<Product>();

        public async Task Find(string search, LoadingWindow loading)
        {
            Stopwatch download = new Stopwatch();
            Stopwatch fullWork = new Stopwatch();

            fullWork.Start();

            List<SearchedResultItem> resultItems = new List<SearchedResultItem>();

            string[] rawResults = new string[0];

            if (!HikFindSearch.modZoufalosti.Check)
            {
                for (int i = 0; i < webScrapers.Length; i++)
                {
                    try
                    {
                        loading.Text = "Stahuji vyhledávání z [" + webScrapers[i].Name + "] hikvisionu";
                        download.Start();
                        await webScrapers[i].Search(search);
                        download.Stop();
                    }
                    catch (System.Net.WebException ex)
                    {
                        loading.Close();
                        AlertWindow.ShowUp(ex.Message);
                        return;
                    }

                    loading.Text = "Rozděluji stažený JSON do podJSNŮ o produktech ze stažených výsledků";
                    rawResults = webScrapers[i].ScrapeEverythingBetween(new FindBetween("{\"filetype\":", "}"));

                    loading.Text = "Získávám čitelné data z podJSNŮ produktů";
                    for (int y = 0; y < rawResults.Length; y++)
                    {
                        webScrapers[i].SetTempScrape(rawResults[y]);
                        string name = "[" + webScrapers[i].Name + "] | " + webScrapers[i].ScrapeBetween(new FindBetween("\"fess_title\":\"", "\""));
                        string url = webScrapers[i].ScrapeBetween(new FindBetween("\"url_link\":\"", "\",\""));
                        string category = webScrapers[i].ScrapeBetween(new FindBetween("\"content_category\":\"", "\""));

                        if (category == null)
                        {
                            continue;
                        }

                        category = category.ToLower();


                        if (category.Contains("||"))
                        {
                            webScrapers[i].SetTempScrape(category);
                            category = webScrapers[i].ScrapeBetween(new FindBetween("", "||"));
                        }

                        webScrapers[i].ClearTempScrape();

                        if (category.Equals("products"))
                        {
                            loading.Text = "Přidávám produkt do zobrazených výsledků";
                            resultItems.Add(new SearchedResultItem(name, url));
                        }

                        if (resultItems.Count - i * maxSearch >= maxSearch)
                        {
                            break;
                        }
                    }
                }
            }



            if ((resultItems.Count <= 0 && HikFindSearch.webPoHledani.Check) || HikFindSearch.modZoufalosti.Check)
            {
                webScraperWEB.ResetWebClient();
                try
                {
                    loading.Text = "Stahuji vyhledávání z WEBU";
                    download.Start();
                    await webScraperWEB.Search(search);
                    download.Stop();
                }
                catch (System.Net.WebException ex)
                {
                    AlertWindow.ShowUp(ex.Message);
                    return;
                }

                loading.Text = "Rozděluji stažený JSON do podJSNŮ o produktech ze stažených výsledků";
                rawResults = webScraperWEB.ScrapeEverythingBetween(new FindBetween("{\"ClickUrl\":\"", "}"));

                loading.Text = "Získávám čitelné data z podJSNŮ produktů";
                for (int i = 0; i < rawResults.Length; i++)
                {
                    webScraperWEB.SetTempScrape(rawResults[i]);
                    string name = "[" + webScraperWEB.Name + "] | " + webScraperWEB.ScrapeBetween(new FindBetween("\"Title\":\"", "\""));
                    string url = webScraperWEB.ScrapeBetween(new FindBetween("\"ActualClickUrl\":\"", "\",\""));
                    webScraperWEB.ClearTempScrape();

                    name = name.Replace("<b>", "");
                    name = name.Replace("</b>", "");


                    if (url != null && url.Contains("https://www.hikvision.com/"))
                    {
                        loading.Text = "Přidávám produkt do zobrazených výsledků";
                        resultItems.Add(new SearchedResultItem(name, url));
                    }

                    if (resultItems.Count - i * maxSearch >= maxSearch)
                    {
                        break;
                    }
                }
            }

            if(stop)
            {
                stop = false;
            }

            fullWork.Stop();
            Trace.WriteLine("[SEARCH] Download Time: " + download.ElapsedMilliseconds);
            Trace.WriteLine("[SEARCH] Time the program was working (W/O Download): " + (fullWork.ElapsedMilliseconds - download.ElapsedMilliseconds));
            loading.Close();

            var resultWindow = new SearchedResult(resultItems.ToArray());
        }

        public static async Task ScrapeFinalProduct(string url, LoadingWindow loading)
        {
            Product tempProduct = new Product();

            if(stop)
            {
                stop = false;
            }

            Stopwatch download = new Stopwatch();
            Stopwatch fullWork = new Stopwatch();

            fullWork.Start();

            WebScraper resultScraper = new WebScraper("resultScraper", url);

            tempProduct.Url = url;

            try
            {
                download.Start();
                await resultScraper.Search("");
                download.Stop();
            }
            catch (System.Net.WebException ex)
            {
                AlertWindow.ShowUp(ex.Message);
                return;
            }

            loading.Text = "Získávám jméno produktu";

            string name = resultScraper.ScrapeBetween(new FindBetween[]
            { new FindBetween("<h1 class=\"", "h1>"), new FindBetween("\">", "</") }).Trim();

            if (name == null)
            {
                loading.Close();
                AlertWindow.ShowUp("Požadovaná stránka buďto není hikvision.com, nebo se nejedná o stránku s produktem.");
                return;
            }

            tempProduct.Name = name;
            name = string.Join("-", name.Split(System.IO.Path.GetInvalidFileNameChars()));

            string folderPath = ConfigurationManager.AppSettings.Get("FolderResultPath") + @name + "\\";
            tempProduct.FolderPath = folderPath;

            if (Directory.Exists(folderPath))
            {
                loading.Close();
                AlertWindow.ShowUp("Pro tento předmět již existuje složka s informacemi vevnitř.");
                return;
            }

            int imageCount = 0;
            int documentCount = 0;

            loading.Text = "Získávám URL k požadovaným informacím";

            for (int i = 0; i < HikFindSearch.settingCategories.Length; i++)
            {
                SearchSetting[] searchSettings = HikFindSearch.settingCategories[i].SearchSettings;
                for (int y = 0; y < searchSettings.Length; y++)
                {
                    if (searchSettings[y] is FinderSearchSetting && searchSettings[y].Check)
                    {

                        loading.Text = "Snažím se získat URL k " + searchSettings[y].Text;
                        object results = searchSettings[y].GetType().GetMethod("Find").Invoke(searchSettings[y], new object[] { resultScraper });
                        string[] stringResults = results as string[];

                        if (stringResults == null || stringResults.Length < 1 || stringResults[0] == null)
                        {
                            continue;
                        }

                        for (int z = 0; z < stringResults.Length; z++)
                        {

                            if (stringResults[z].EndsWith(".png") || stringResults[z].EndsWith(".jpg") || stringResults[z].EndsWith(".jpeg"))
                            {
                                loading.Text = "Binárně stahuji obrázek " + (imageCount+1);
                                download.Start();

                                byte[] result = await WebScraper.DownloadData("https://www.hikvision.com" + stringResults[z]);

                                try
                                {
                                    using (MemoryStream ms = new MemoryStream(result))
                                    {
                                        download.Stop();
                                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms, true);

                                        Bitmap bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                        string format;
                                        System.Drawing.Imaging.ImageFormat imageFormat;
                                        System.Drawing.Color background;

                                        if (HikFindSearch.jpgFormat.Check)
                                        {
                                            loading.Text = "Převádím na .jpg";
                                            format = ".jpg";
                                            imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                            background = System.Drawing.Color.White;
                                        }
                                        else
                                        {
                                            format = ".png";
                                            imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                                            background = System.Drawing.Color.Transparent;
                                        }

                                        loading.Text = "Vytvářím složku, pokud již neexistuje";
                                        Directory.CreateDirectory(folderPath);
                                        loading.Text = "Manipuluji s obrázkem pro správné zobrazení";
                                        bmp = ImageEditor.FillWithBackground(bmp, img, background);
                                        bmp = ImageEditor.Crop(bmp);

                                        loading.Text = "Ukládám obrázek";

                                        bmp.Save(folderPath + (imageCount + 1) + "-" + @name + format, imageFormat);
                                        imageCount++;
                                    }
                                }
                                catch(System.ArgumentException)
                                {

                                }
                                
                            }
                            else
                            {
                                loading.Text = "Vytvářím složku, pokud již neexistuje";
                                Directory.CreateDirectory(folderPath);
                                documentCount++;
                                download.Start();
                                loading.Text = "Stahuji a ukládám dokument " + searchSettings[y].Text;
                                await WebScraper.DownloadFile("https://www.hikvision.com" + stringResults[z], folderPath + HikFindSearch.settingCategories[i].SearchSettings[y].Text + "_" + @name + ".pdf");
                                download.Stop();
                            }

                            if (stop)
                            {
                                if(Directory.Exists(folderPath))
                                {
                                    Directory.Delete(folderPath, true);
                                }
                                stop = false;
                                return;
                            }
                        }

                    }
                }
            }
            if (documentCount + imageCount < 1)
            {
                loading.Close();
                AlertWindow.ShowUp("Požadovaná stránka buďto není hikvision.com, nebo se nejedná o stránku s produktem.");
                return;
            }

            HikFindSearch.products.Add(tempProduct);
            Trace.WriteLine(HikFindSearch.products.Count());

            fullWork.Stop();
            Trace.WriteLine("[PRODUCT] Download Time: " + download.ElapsedMilliseconds);
            Trace.WriteLine("[PRODUCT] Time the program was working (W/O Download): " + (fullWork.ElapsedMilliseconds - download.ElapsedMilliseconds));
            loading.LoadingFinished();
            return;
        }

    }
}
