namespace HIKFind.Settings
{
    public class SearchSetting
    {

        protected private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        protected private string description;
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
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

        public SearchSetting(string text, string description, bool defaultCheck)
        {
            this.check = defaultCheck;
            this.text = text;
            this.description = description;
        }

        public SearchSetting(string text, string description)
        {
            this.text = text;
            this.description = description;
        }

        public SearchSetting()
        {

        }
    }
}
