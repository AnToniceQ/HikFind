using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIKFind.Search.Webscraper;

namespace HIKFind.Search
{
    public class RadioSetting : SearchSetting
    {
        protected string group;
        public string Group { get { return group; } }

        public RadioSetting(string text, bool defaultCheck, string group) : base(text, defaultCheck)
        {
            this.group = group;
        }

        public RadioSetting(string text, string group) : base(text)
        {
            this.group = group;
        }
    }
}
