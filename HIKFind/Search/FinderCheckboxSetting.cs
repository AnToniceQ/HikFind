using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIKFind.Search.Webscraper;

namespace HIKFind.Search
{
    class FinderCheckboxSetting : FinderSearchSetting
    {
        public FinderCheckboxSetting(string text, FindBetween findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, multipleSearch)
        {
        }

        public FinderCheckboxSetting(string text, FindBetween[] findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, multipleSearch)
        {
        }
    }
}
