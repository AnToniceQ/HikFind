using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIKFind.Search.Webscraper;

namespace HIKFind.Search
{
    class FinderRadioSetting : FinderSearchSetting
    {
        protected string group;
        public string Group { get { return group; } }

        public FinderRadioSetting(string text, string group, FindBetween findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, multipleSearch)
        {
            this.group = group;
        }

        public FinderRadioSetting(string text, string group, FindBetween[] findBetween, bool defaultCheck, bool multipleSearch) : base(text, findBetween, defaultCheck, multipleSearch)
        {
            this.group = group;
        }
    }
}
