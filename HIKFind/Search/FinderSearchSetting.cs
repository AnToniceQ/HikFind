using HIKFind.Search.Webscraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind.Search
{
    class FinderSearchSetting : SearchSetting
    {
        protected private FindBetween[] findBetweens = null;
        public FindBetween[] FindBetweens { get { return findBetweens; } }

        protected private bool multipleResults;
        public bool MultipleResults { get { return multipleResults; } }

        public FinderSearchSetting(string text, FindBetween findBetween, bool defaultCheck, bool multipleResults) : base(text, defaultCheck)
        {
            this.text = text;
            this.findBetweens = new FindBetween[1];
            this.findBetweens[0] = findBetween;
            this.check = defaultCheck;
            this.multipleResults = multipleResults;
        }

        public FinderSearchSetting(string text, FindBetween[] findBetween, bool defaultCheck, bool multipleResults) : base(text, defaultCheck)
        {
            this.text = text;
            this.findBetweens = findBetween;
            this.check = defaultCheck;
            this.multipleResults = multipleResults;
        }

        public string[] Find(WebScraper webScraper)
        {
            if (multipleResults)
            {
                return webScraper.ScrapeEverythingBetween(findBetweens);
            }
            return new string[] { webScraper.ScrapeBetween(findBetweens) };
        }
    }
}
