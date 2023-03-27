using HIKFind.Search.Webscraper;

namespace HIKFind.Settings
{
    class FinderSearchSetting : BaseSearchSetting
    {
        protected private FindBetween[] findBetweens = null;
        public FindBetween[] FindBetweens
        {
            get
            {
                return findBetweens;
            }
            set
            {
                findBetweens = value;
            }
        }

        protected private bool multipleResults;
        public bool MultipleResults
        {
            get
            {
                return multipleResults;
            }
        }

        public FinderSearchSetting(string name, string text, string description, bool defaultCheck, FindBetween findBetween, bool multipleResults, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, searchSettings)
        {
            this.text = text;
            this.findBetweens = new FindBetween[1];
            this.findBetweens[0] = findBetween;
            this.check = defaultCheck;
            this.multipleResults = multipleResults;
        }

        public FinderSearchSetting(string name, string text, string description, bool defaultCheck, FindBetween[] findBetween, bool multipleResults, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, searchSettings)
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
