namespace HIKFind.Settings
{
    public class RadioSetting : BaseSearchSetting
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

        public RadioSetting(string name, string text, string description, bool defaultCheck, string group, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, searchSettings)
        {
            this.group = group;
        }
        public RadioSetting(string name, string text, string description, string group, BaseSetting[] searchSettings) : base(name, text, description, searchSettings)
        {
            this.group = group;
        }
    }
}
