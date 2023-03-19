using HIKFind.InfoWindows;
using HIKFind.Search.Webscraper;
using HIKFind.Search.window;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HIKFind
{
    internal class BaseSearcher
    {
        protected int maxSearch = 10;
        protected WebScraper[] webScrapers;
        protected WebScraper webScraperWEB;

        public static bool stop = false;

        protected BaseSearcher(int maxSearch, WebScraper[] webScrapers, string webKeywordHelp)
        {
            this.maxSearch = maxSearch;
            this.webScrapers = webScrapers;
            this.webScraperWEB = new WebScraper("WEB", "https://spapi.searchencrypt.com/api/search?st=web&q=", "+" + webKeywordHelp + "&types=web");
            Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("FolderResultPath"));
        }

        public async Task Find(string search, LoadingWindow loading)
        {
            Stopwatch download = new Stopwatch();
            Stopwatch fullWork = new Stopwatch();

            fullWork.Start();

            List<SearchedResultItem> resultItems = new List<SearchedResultItem>();

            string[] rawResults = new string[0];

            if (!HikFindSearch.settings["modzoufalosti"].Check)
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
                        InfoWindow.ShowUp(ex.Message, InfoWindows.Helpers.InfoWindowEnum.Alert);
                        return;
                    }

                    loading.Text = "Rozděluji stažený JSON do podJSNŮ o produktech ze stažených výsledků";
                    rawResults = webScrapers[i].ScrapeEverythingBetween(new FindBetween("{\"filetype\":", "}"));

                    loading.Text = "Získávám čitelné data z podJSNŮ produktů";
                    for (int y = 0; y < rawResults.Length; y++)
                    {
                        webScrapers[i].SetTempScrape(rawResults[y]);
                        string tempName = "[" + webScrapers[i].Name + "] | " + webScrapers[i].ScrapeBetween(new FindBetween("\"title\":\"", "\""));
                        tempName = tempName.Replace("<strong>", "");
                        tempName = tempName.Replace("<\\/strong>", "");
                        string name = tempName;
                        string url = webScrapers[i].ScrapeBetween(new FindBetween("\"url\":\"", "\",\""));
                        string category = webScrapers[i].ScrapeBetween(new FindBetween("\"category\":\"", "\""));

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
                            bool alreadyExists = false;

                            loading.Text = "Přidávám produkt do zobrazených výsledků";
                            foreach(Product realProduct in HikFindSearch.products)
                            {
                                if(realProduct.Name == name)
                                {
                                    alreadyExists = true;
                                    break;
                                }
                            }
                            resultItems.Add(new SearchedResultItem(name, url, alreadyExists));
                        }

                        if (resultItems.Count - i * maxSearch >= maxSearch)
                        {
                            break;
                        }
                    }
                }
            }



            if ((resultItems.Count <= 0 && HikFindSearch.settings["webpohik"].Check) || HikFindSearch.settings["modzoufalosti"].Check)
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
                    InfoWindow.ShowUp(ex.Message, InfoWindows.Helpers.InfoWindowEnum.Alert);
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
                        bool alreadyExists = false;

                        loading.Text = "Přidávám produkt do zobrazených výsledků";
                        foreach (Product realProduct in HikFindSearch.products)
                        {
                            if (realProduct.Name == name)
                            {
                                alreadyExists = true;
                                break;
                            }
                        }
                        resultItems.Add(new SearchedResultItem(name, url, alreadyExists));
                    }

                    if (resultItems.Count - i * maxSearch >= maxSearch)
                    {
                        break;
                    }
                }
            }

            if (stop)
            {
                stop = false;
            }

            fullWork.Stop();
            Trace.WriteLine("[SEARCH] Download Time: " + download.ElapsedMilliseconds);
            Trace.WriteLine("[SEARCH] Time the program was working (W/O Download): " + (fullWork.ElapsedMilliseconds - download.ElapsedMilliseconds));
            loading.Close();

            var resultWindow = new SearchedResult(resultItems.ToArray());
        }
    }
}
