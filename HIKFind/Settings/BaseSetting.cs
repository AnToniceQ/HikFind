using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind.Settings
{
    public class BaseSetting
    {
        protected BaseSetting[] searchSettings = new BaseSetting[0];
        public BaseSetting[] SearchSettings
        {
            get
            {
                return searchSettings;
            }
        }

        protected BaseSetting(BaseSetting[] searchSettings)
        {
            this.searchSettings = searchSettings;
        }
    }
}
