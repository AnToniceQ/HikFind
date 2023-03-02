using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIKFind.Search.Webscraper;

namespace HIKFind.Search
{
    public class SearchSetting
    {
        protected private string text;
        public string Text { get { return this.text; } }

        protected private bool check = false;
        public bool Check { get { return check; } set { check = value; } }

        public SearchSetting(string text, bool defaultCheck)
        {
            this.check = defaultCheck;
            this.text = text;
        }

        public SearchSetting(string text)
        {
            this.text = text;
        }

        
    }
}
