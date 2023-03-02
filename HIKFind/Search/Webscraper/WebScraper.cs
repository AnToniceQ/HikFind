using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Threading;
using System.IO;

namespace HIKFind.Search.Webscraper
{
    class WebScraper
    {
        string name;
        public string Name { get { return name; } }

        string baseStartSearchURL;
        string endURL;
        string scrape = null;

        string tempScrape = "";

        public string SearchedResult { get { return scrape; } set { scrape = value; } }

        private static WebClient wClient = new WebClient();

        public WebScraper(string name, string baseStartSearchURL)
        {
            this.baseStartSearchURL = baseStartSearchURL;
            this.scrape = "";
            this.endURL = "";
            this.name = name;

            wClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            wClient.Proxy = WebRequest.DefaultWebProxy;
            wClient.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
        }

        public WebScraper(string name, string baseStartSearchURL, string endURL)
        {
            this.baseStartSearchURL = baseStartSearchURL;
            this.scrape = "";
            this.endURL = endURL;
            this.name = name;

            wClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            wClient.Proxy = WebRequest.DefaultWebProxy;
            wClient.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
        }


        public async Task Search(string search)
        {
            try
            {
                        scrape = await wClient.DownloadStringTaskAsync(new Uri(baseStartSearchURL + search + endURL));
                
            }
            catch(System.Net.WebException)
            {
                scrape = "";
                throw;
            }
        }

        public void ResetWebClient()
        {
            wClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            wClient.Proxy = WebRequest.DefaultWebProxy;
            wClient.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
        }

        public string[] ScrapeEverythingBetween(FindBetween[] findBetween)
        {
            List<string> results = new List<string>();

            string tempScrape = scrape;
            string middleTempScrape;

            int startResultAt = -1;
            int resultLength = 0;

            bool noProblem;

            while(tempScrape.IndexOf(findBetween[0].Start) > -1 && tempScrape.IndexOf(findBetween[0].End) > -1)
            {
                noProblem = true;

                startResultAt = tempScrape.IndexOf(findBetween[0].Start) + findBetween[0].Start.Length;
                resultLength = tempScrape.IndexOf(findBetween[0].End, startResultAt) - startResultAt;

                middleTempScrape = tempScrape.Substring(startResultAt,resultLength);
                tempScrape = tempScrape.Remove(0, resultLength + startResultAt + 1);

                for(int i = 1; i < findBetween.Length; i++)
                {
                    if (middleTempScrape.IndexOf(findBetween[i].Start) < 0 || middleTempScrape.IndexOf(findBetween[i].End) < 0)
                    {
                        noProblem = false;
                        break;
                    }

                    startResultAt = tempScrape.IndexOf(findBetween[i].Start) + findBetween[i].Start.Length;
                    resultLength = tempScrape.IndexOf(findBetween[i].End, startResultAt) - startResultAt;

                    middleTempScrape = middleTempScrape.Substring(startResultAt, resultLength);
                }

                if(noProblem)
                {
                    results.Add(middleTempScrape);
                }
            }

            return results.ToArray();
        }

        public string[] ScrapeEverythingBetween(FindBetween findBetween)
        {
            List<string> results = new List<string>();

            string tempScrape;
            if (this.tempScrape.Equals(""))
            {
                tempScrape = scrape;
            }
            else
            {
                tempScrape = this.tempScrape;
            }

            int startResultAt;
            int resultLength;
                    
            while (tempScrape.IndexOf(findBetween.Start) != -1 && tempScrape.IndexOf(findBetween.End) != -1)
            {
                startResultAt = tempScrape.IndexOf(findBetween.Start) + findBetween.Start.Length;
                resultLength = tempScrape.IndexOf(findBetween.End, startResultAt) - startResultAt;

                results.Add(tempScrape.Substring(startResultAt, resultLength));
                tempScrape = tempScrape.Remove(0, resultLength + startResultAt + 1);
            }

            return results.ToArray();
        }

        public string ScrapeBetween(FindBetween[] findBetween)
        {
            string tempScrape;
            if (this.tempScrape.Equals(""))
            {
                tempScrape = scrape;
            }
            else
            {
                tempScrape = this.tempScrape;
            }

            for (int i = 0; i < findBetween.Length; i++)
            {
                if (tempScrape.IndexOf(findBetween[i].Start) < 0 || tempScrape.IndexOf(findBetween[i].End) < 0)
                {
                    return null;
                }

                int startResultAt = tempScrape.IndexOf(findBetween[i].Start) + findBetween[i].Start.Length;
                int resultLength = tempScrape.IndexOf(findBetween[i].End, startResultAt) - startResultAt;

                tempScrape = tempScrape.Substring(startResultAt, resultLength);
            }
            return tempScrape;
        }

        public string ScrapeBetween(FindBetween findBetween)
        {
            string tempScrape;

            if (this.tempScrape.Equals(""))
            {
                tempScrape = scrape;
            }
            else
            {
                tempScrape = this.tempScrape;
            }

            if(tempScrape.IndexOf(findBetween.Start) < 0 || tempScrape.IndexOf(findBetween.End) < 0)
            {
                return null;
            }

            int startResultAt = tempScrape.IndexOf(findBetween.Start) + findBetween.Start.Length;
            int resultLength = tempScrape.IndexOf(findBetween.End, startResultAt) - startResultAt;

            return tempScrape.Substring(startResultAt, resultLength);
        }

        public void SetTempScrape(string scrape)
        {
            tempScrape = scrape;
        }

        public void ClearTempScrape()
        {
            tempScrape = "";
        }

        public static async Task DownloadFile(string url, string path)
        {
            try
            {
                try
                {
                    await wClient.DownloadFileTaskAsync(url, path);
                }
                catch(TaskCanceledException ex)
                {
                    AlertWindow.ShowUp(ex.Message);
                }
            }
            catch (System.Net.WebException ex)
            {
                AlertWindow.ShowUp(ex.Message);
            }

        }

        public async static Task<byte[]> DownloadData(string url)
        {
            try
            {
                try
                {
                    byte[] content = await wClient.DownloadDataTaskAsync(new Uri(url));

                    return content;
                }
                catch (TaskCanceledException ex)
                {
                    AlertWindow.ShowUp(ex.Message);
                    return new byte[0];
                }
            }
            catch (System.Net.WebException ex)
            {
                AlertWindow.ShowUp(ex.Message);
                return new byte[0];
            }
        }

        public static void StopDownload()
        {
            Searcher.stop = true;
            wClient.CancelAsync();
        }
    }
}
