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

        public FinderRadioSetting(string name, string text, string description, string group, bool defaultCheck, FindBetween findBetween, bool multipleSearch, BaseSetting[] searchSettings) : base(name, text, description,  defaultCheck, findBetween, multipleSearch, searchSettings)
        {
            this.group = group;
        }

        public FinderRadioSetting(string name, string text, string description, string group, bool defaultCheck, FindBetween[] findBetween, bool multipleSearch, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, findBetween, multipleSearch, searchSettings)
        {
            this.group = group;
        }
    }
}
