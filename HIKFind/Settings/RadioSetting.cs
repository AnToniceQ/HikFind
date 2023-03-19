namespace HIKFind.Settings
{
    public class RadioSetting : SearchSetting
    {
        protected string group;
        public string Group
        {
            get
            {
                return group;
            }
            set
            {
                this.group = value;
            }
        }

        public RadioSetting(string text, bool defaultCheck, string description, string group) : base(text, description, defaultCheck)
        {
            this.group = group;
        }
        public RadioSetting(string text, string group, string description) : base(text, description)
        {
            this.group = group;
        }

        public RadioSetting()
        {
        }
    }
}
