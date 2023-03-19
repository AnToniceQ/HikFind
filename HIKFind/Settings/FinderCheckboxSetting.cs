using HIKFind.Search.Webscraper;

namespace HIKFind.Settings
{
    class FinderCheckboxSetting : FinderSearchSetting
    {
        public FinderCheckboxSetting(string text, FindBetween findBetween, bool defaultCheck, string description, bool multipleSearch) : base(text, findBetween, defaultCheck, description, multipleSearch)
        {

        }

        public FinderCheckboxSetting(string text, FindBetween[] findBetween, bool defaultCheck, string description, bool multipleSearch) : base(text, findBetween, defaultCheck, description, multipleSearch)
        {

        }

        public FinderCheckboxSetting()
        {

        }
    }
}
