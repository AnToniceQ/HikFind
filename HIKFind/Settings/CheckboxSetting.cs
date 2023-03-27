namespace HIKFind.Settings
{
    public class CheckboxSetting : BaseSearchSetting
    {
        public CheckboxSetting(string name, string text, string description, bool defaultCheck, BaseSetting[] searchSettings) : base(name, text, description, defaultCheck, searchSettings)
        {
        }

        public CheckboxSetting(string name, string text, string description, BaseSetting[] searchSettings) : base(name, text, description, searchSettings)
        {
        }
    }
}
