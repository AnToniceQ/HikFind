namespace HIKFind.Settings
{
    public class BaseSearchSetting : BaseSetting
    {
        protected private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        protected private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        protected private string description;
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        protected private bool check = false;
        public bool Check
        {
            get
            {
                return check;
            }
            set
            {
                check = value;
            }
        }

        public BaseSearchSetting(string name, string text, string description, bool defaultCheck, BaseSetting[] searchSettings) : base(searchSettings)
        {
            this.name = name;
            this.check = defaultCheck;
            this.text = text;
            this.description = description;
        }

        public BaseSearchSetting(string name, string text, string description, BaseSetting[] searchSettings) : base(searchSettings)
        {
            this.name = name;
            this.text = text;
            this.description = description;
        }
    }
}
