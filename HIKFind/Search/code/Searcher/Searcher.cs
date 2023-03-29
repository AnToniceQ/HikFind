using HIKFind.InfoWindows;
using HIKFind.Settings;
using HIKFind.Search.Webscraper;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HIKFind.Helpers;
using HIKFind.ImageEditing;

namespace HIKFind
{
    class Searcher : BaseSearcher
    {
        public Searcher(int maxSearch, WebScraper[] webScrapers, string webKeywordHelp) : base(maxSearch, webScrapers, webKeywordHelp)
        {
        }

        public static async Task<Product> ScrapeFinalProduct(string url, LoadingWindow loading, bool checkForProductDirectory)
        {
            Product tempProduct = new Product();

            if (stop)
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
                InfoWindow.ShowUp(ex.Message, InfoWindows.Helpers.InfoWindowEnum.Alert);
                return null;
            }

            loading.Text = "Získávám jméno produktu";

            
            string name = resultScraper.ScrapeBetween(new FindBetween[]
            { new FindBetween("<h1 class=\"", "h1>"), new FindBetween("\">", "</") });

            if (name == null)
            {
                loading.Close();
                InfoWindow.ShowUp("Požadovaná stránka buďto není hikvision.com, nebo se nejedná o stránku s produktem.", InfoWindows.Helpers.InfoWindowEnum.Alert);
                return null;
            }

            name = name.Trim();

            tempProduct.Name = name;
            name = string.Join("-", name.Split(System.IO.Path.GetInvalidFileNameChars()));

            string tempFolderPath = ConfigurationManager.AppSettings.Get("FolderResultPath") + "temp" + "\\";
            tempProduct.FolderPath = ConfigurationManager.AppSettings.Get("FolderResultPath") + @name + "\\";

            loading.Text = "Vytvářím TEMP složku, pokud již neexistuje";
            DirectoryInfo dict = Directory.CreateDirectory(tempFolderPath);
            dict.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            if (Directory.Exists(tempProduct.FolderPath) && checkForProductDirectory)
            {
                loading.Close();
                InfoWindow.ShowUp("Pro tento předmět již existuje složka s informacemi vevnitř.", InfoWindows.Helpers.InfoWindowEnum.Alert);
                return null;
            }

            int imageCount = 0;
            int documentCount = 0;

            loading.Text = "Získávám URL k požadovaným informacím";

            for (int i = 0; i < HikFindSearch.dictSettings.Count; i++)
            {
                if (HikFindSearch.dictSettings.ElementAt(i).Value is FinderSearchSetting && HikFindSearch.dictSettings.ElementAt(i).Value.Check)
                {

                    loading.Text = "Snažím se získat URL k " + HikFindSearch.dictSettings.ElementAt(i).Value.Text;

                    FinderSearchSetting searchSetting = (FinderSearchSetting) HikFindSearch.dictSettings.ElementAt(i).Value;
                    string[] stringResults = searchSetting.Find(resultScraper);

                    if (stringResults == null || stringResults.Length < 1 || stringResults[0] == null)
                    {
                        continue;
                    }

                    for (int z = 0; z < stringResults.Length; z++)
                    {
                        if (stringResults[z].EndsWith("png") || stringResults[z].EndsWith("jpg") || stringResults[z].EndsWith("jpeg") || stringResults[z].EndsWith("webp"))
                        {
                            loading.Text = "Binárně stahuji obrázek " + (imageCount + 1);
                            download.Start();

                            Bitmap bmp = await WebScraper.DownloadImage("https://www.hikvision.com" + stringResults[z]);

                            try
                            {
                                    download.Stop();
                                    string format;
                                    System.Drawing.Imaging.ImageFormat imageFormat;

                                    if (HikFindSearch.dictSettings["jpgformat"].Check)
                                    {
                                        format = ".jpg";
                                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    }
                                    else
                                    {
                                        format = ".png";
                                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                                    }
                                    loading.Text = "Manipuluji s obrázkem pro správné zobrazení";

                                    if (HikFindSearch.dictSettings["crop"].Check)
                                    {
                                    loading.Text = "Ořezávám obrázek";
                                    if (HikFindSearch.dictSettings["fastcrop"].Check)
                                    {
                                        bmp = await ImageEditor.FastCrop(bmp);
                                    }
                                    else
                                    {

                                    }
                                    bmp = await ImageEditor.FullCrop(bmp);

                                }

                                if (!HikFindSearch.dictSettings["transparent"].Check || imageFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                                {
                                    loading.Text = "Dávám bílé pozadí";
                                    bmp = await ImageEditor.WhiteBackground(bmp);
                                }

                                loading.Text = "Ukládám obrázek a převádím formát";
                                    bmp.Save(tempFolderPath + (imageCount + 1) + "-" + @name + format, imageFormat);
                                tempProduct.AddImage(bmp);    
                                imageCount++;
                            }
                            catch (System.ArgumentException ex)
                            {
                                InfoWindow.ShowUp(ex.Message, InfoWindows.Helpers.InfoWindowEnum.Alert);
                            }

                        }
                        else
                        {
                            documentCount++;
                            download.Start();
                            loading.Text = "Stahuji a ukládám dokument " + HikFindSearch.dictSettings.ElementAt(i).Value.Text;
                            Console.WriteLine(stringResults[z]);
                            await WebScraper.DownloadFile("https://www.hikvision.com" + stringResults[z], tempFolderPath + HikFindSearch.settings.ElementAt(i).Value.Text + "-" + @name + ".pdf");
                            download.Stop();
                        }

                        if (stop)
                        {
                            if (Directory.Exists(tempFolderPath))
                            {
                                FileHelper.RemoveDirectoryContents(tempFolderPath);
                            }
                            stop = false;
                            return null;
                        }
                    }

                }
            }

            if (documentCount + imageCount < 1)
            {
                loading.Close();
                InfoWindow.ShowUp("Požadovaná stránka buďto není hikvision.com, nebo se nejedná o stránku s produktem.", InfoWindows.Helpers.InfoWindowEnum.Alert);
                return null;
            }

            tempProduct.DateSearched = DateTime.Now;

            fullWork.Stop();
            Trace.WriteLine("[PRODUCT] Download Time: " + download.ElapsedMilliseconds);
            Trace.WriteLine("[PRODUCT] Time the program was working (W/O Download): " + (fullWork.ElapsedMilliseconds - download.ElapsedMilliseconds));
            loading.LoadingFinished();
            return tempProduct;
        }

    }
}
