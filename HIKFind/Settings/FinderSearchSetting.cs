using HIKFind.Search.Webscraper;

namespace HIKFind.Settings
{
    class FinderSearchSetting : SearchSetting
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

        public FinderSearchSetting(string text, FindBetween findBetween, bool defaultCheck, string description, bool multipleResults) : base(text, description, defaultCheck)
        {
            this.text = text;
            this.findBetweens = new FindBetween[1];
            this.findBetweens[0] = findBetween;
            this.check = defaultCheck;
            this.multipleResults = multipleResults;
        }

        public FinderSearchSetting(string text, FindBetween[] findBetween, bool defaultCheck, string description, bool multipleResults) : base(text, description, defaultCheck)
        {
            this.text = text;
            this.findBetweens = findBetween;
            this.check = defaultCheck;
            this.multipleResults = multipleResults;
        }

        public FinderSearchSetting()
        {

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
