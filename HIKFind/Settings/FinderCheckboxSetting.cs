using HIKFind.Search.Webscraper;

namespace HIKFind.Settings
{
    class FinderCheckboxSetting : FinderSearchSetting
    {
        public FinderCheckboxSetting(string name, string text, string description, bool defaultCheck, FindBetween findBetween, bool multipleSearch, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, findBetween, multipleSearch, searchSettings)
        {

        }

        public FinderCheckboxSetting(string name, string text, string description, bool defaultCheck, FindBetween[] findBetween, bool multipleSearch, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, findBetween, multipleSearch, searchSettings)
        {

        }
    }
}
