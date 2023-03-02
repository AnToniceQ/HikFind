using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind.Search
{
    public class SettingCategory
    {
        SearchSetting[] searchSettings;
        public SearchSetting[] SearchSettings { get { return searchSettings; } }

        protected private string productCategory;
        public string ProductCategory { get { return productCategory; } }

        string name;
        public string Name { get { return name; } }

        public SettingCategory(string name, SearchSetting[] checkboxSettings)
        {
            this.name = name;
            this.searchSettings = checkboxSettings;
        }

        public SettingCategory(string name, string productCategory, SearchSetting[] checkboxSettings)
        {
            this.name = name;
            this.productCategory = productCategory;
            this.searchSettings = checkboxSettings;
        }
    }
}
