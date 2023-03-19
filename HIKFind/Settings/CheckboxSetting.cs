namespace HIKFind.Settings
{
    public class CheckboxSetting : SearchSetting
    {
        public CheckboxSetting(string text, string description, bool defaultCheck) : base(text, description, defaultCheck)
        {
        }

        public CheckboxSetting(string text, string description) : base(text, description)
        {
        }

        public CheckboxSetting()
        {

        }
    }
}
