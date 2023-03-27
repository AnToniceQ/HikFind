using System.Collections.Generic;
using System.Linq;

namespace HIKFind.Settings
{
    public class SettingCategory : BaseSetting
    {
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

        public SettingCategory(string name, BaseSetting[] searchSettings) : base(searchSettings)
        {
            this.name = name;
            this.searchSettings = searchSettings;
        }

        public SettingCategory(string name, string productCategory, BaseSetting[] searchSettings) : base(searchSettings)
        {
            this.name = name;
            this.productCategory = productCategory;
            this.searchSettings = searchSettings;
        }
    }
}
