using HIKFind.Settings;
using HIKFind.Search.Webscraper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Configuration;

namespace HIKFind
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (File.Exists("products.json"))
            {
                HikFindSearch.products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(File.ReadAllText("products.json"));
            }

            if(Directory.Exists(ConfigurationManager.AppSettings.Get("FolderResultPath") + "temp"))
            {
                Directory.Delete(ConfigurationManager.AppSettings.Get("FolderResultPath") + "temp", true);
            }

            if (File.Exists("settings.json"))
            {
                using (StreamReader rd = new StreamReader("settings.json"))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                    HikFindSearch.settingCategories = JsonConvert.DeserializeObject<ObservableCollection<SettingCategory>>(rd.ReadToEnd(), settings);
                }
            }
            else
            {

                HikFindSearch.settingCategories.Add(new SettingCategory("Možnosti vyhledávání", new Dictionary<string, SearchSetting>
                    {

                    { "obrazky",new FinderCheckboxSetting("Obrázky",
                    new FindBetween[]{ new FindBetween("<picture data-mob-src=\"", "</picture"), new FindBetween(" data-src=\"","\"") },
                    true, "Nastavení, zda má HikFind stahovat obrázky produktu.", true) },
                    { "datasheet",new FinderCheckboxSetting("Datasheet",new FindBetween[]
                    { new FindBetween("id=\"Data_Sheet\"","</div"), new FindBetween("href=\"","\"") },
                    true, "Nastavení, zda má HikFind stahovat datasheet produktu.", false) },
                    { "usermanual",new FinderCheckboxSetting("User-Manual",new FindBetween[]
                    { new FindBetween("id=\"User_Manual\"","</div"), new FindBetween("href=\"", "\"") },
                    true, "Nastavení, zda má HikFind stahovat User Manual produktu.", false) },
                    { "quickstart", new FinderCheckboxSetting("Quick-Start-Guide", new FindBetween[]
                    { new FindBetween("id=\"Quick_Start_Guide\"","</div"), new FindBetween("href=\"","\"") },
                    true, "Nastavení, zda má HikFind stahovat Quick Start Guide produktu.", false) },

                    }));

                HikFindSearch.settingCategories.Add(new SettingCategory("Pomocník vyhledávání", new Dictionary<string, SearchSetting>
            {
                    { "webpohik", new CheckboxSetting("Web po hikvisionu", "Nastavení, zda má HikFind použít API internetového vyhledávače za účelem poslední šance nalezení produktu, když nic nenalezne na EN/EU Hikvision stránkách.",
                true) },
                    { "zadnapomoc", new RadioSetting("Žádná pomoc", "vyhledavani", "Nastavení, zda nemá HikFind upravovat váš vstupní parametr za účelem efektivnějšího vyhledávání."
                    ) },
                    { "zakladnipomoc", new RadioSetting("Základní pomoc",
                true, "Nastavení, zda má HikFind upravovat váš vstupní parametr za účelem efektivnějšího vyhledávání.", "vyhledavani")},
                    { "modzoufalosti", new RadioSetting("Webové vyhledávání", "vyhledavani", "Nastavení, že HikFind využije API internetového vyhledávače rovnou, místo hledání na EN/EU Hikvision stránek."
                )}
            }));

                HikFindSearch.settingCategories.Add(new SettingCategory("Obrázky", new Dictionary<string, SearchSetting>
            {
                    { "jpgformat", new RadioSetting("Formát JPG",
                true, "Nastavení, zda se stažené obrázky mají překonvertovat do formátu JPG" ,"formatObraz") },
                    { "pngformat", new RadioSetting("Formát PNG",
                false, "Nastavení, zda se stažené obrázky mají překonvertovat do formátu PNG", "formatObraz") },
                    { "crop", new CheckboxSetting("Oříznout", "Nastavení, zda se stažené obrázky mají oříznout dle obsahu v obrázku.",
                true) },
                    { "transparent", new CheckboxSetting("Transparent", "Nastavení, zda se staženým obrázkům má zanechat transparentní pozadí.",
                false) }
            }));
            }

            HikFindSearch mainWindow = new HikFindSearch();
            mainWindow.ShowDialog();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            HikFindSearch.SaveSettings();
        }
    }
}
