using System.Collections.Generic;
using System.Linq;

namespace HIKFind.Settings
{
    public class SettingCategory
    {
        Dictionary<string, SearchSetting> searchSettings;
        public Dictionary<string, SearchSetting> SearchSettings
        {
            get
            {
                return searchSettings;
            }
            set
            {
                searchSettings = value;
            }
        }

        public SearchSetting[] GUISearchSettings
        {
            get
            {
                return SearchSettings.Values.ToArray();
            }
        }

        protected private string productCategory;
        public string ProductCategory
        {
            get
            {
                return productCategory;
            }
            set
            {
                productCategory = value;
            }
        }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public SettingCategory(string name, Dictionary<string, SearchSetting> checkboxSettings)
        {
            this.name = name;
            this.searchSettings = checkboxSettings;
        }

        public SettingCategory(string name, string productCategory, Dictionary<string, SearchSetting> checkboxSettings)
        {
            this.name = name;
            this.productCategory = productCategory;
            this.searchSettings = checkboxSettings;
        }

        public SettingCategory()
        {

        }
    }
}
