using HIKFind.Search.Webscraper;

namespace HIKFind.Settings
{
    class FinderRadioSetting : FinderSearchSetting
    {
        protected string group;
        public string Group
        {
            get
            {
                return group;
            }
        }

        public FinderRadioSetting(string text, string group, string description, FindBetween findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, description, multipleSearch)
        {
            this.group = group;
        }

        public FinderRadioSetting(string text, string group, string description, FindBetween[] findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, description, multipleSearch)
        {
            this.group = group;
        }

        public FinderRadioSetting()
        {

        }
    }
}
