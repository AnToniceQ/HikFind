using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIKFind.Search.Webscraper;

namespace HIKFind.Search
{
    public class CheckboxSetting : SearchSetting
    {
        public CheckboxSetting(string text, bool defaultCheck) : base(text, defaultCheck)
        {
        }

        public CheckboxSetting(string text) : base(text)
        {
        }
    }
}
