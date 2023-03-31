using HIKFind.Settings;
using HIKFind.Search.Webscraper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Configuration;
using Microsoft.SqlServer.Server;
using HIKFind.Properties;
using System.Linq;

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

            if (Directory.Exists(ConfigurationManager.AppSettings.Get("FolderResultPath") + "temp"))
            {
                Directory.Delete(ConfigurationManager.AppSettings.Get("FolderResultPath") + "temp", true);
            }

            HikFindSearch.settings = new ObservableCollection<BaseSetting> {
                    new SettingCategory("Stahování", new BaseSetting[]
                    {
                    new FinderCheckboxSetting("obrazky","Obrázky","Nastavení, zda má HikFind stahovat Obrázky produktu.",
                    true, new FindBetween[]{ new FindBetween("<picture data-mob-src=\"", "</picture"), new FindBetween(" data-src=\"","\"") }, true,
                    null),

                    new FinderCheckboxSetting("datasheet","Datasheet","Nastavení, zda má HikFind stahovat Datasheet produktu.",
                    true, new FindBetween[]{ new FindBetween("id=\"Data_Sheet\"", "</div"), new FindBetween("href=\"", "\"") }, true,
                    null),

                    new FinderCheckboxSetting("usermanual","User-Manual", "Nastavení, zda má HikFind stahovat User Manual produktu.",
                    true, new FindBetween[]{ new FindBetween("id=\"User_Manual\"","</div"), new FindBetween("href=\"", "\"") }, false,
                    null),

                    new FinderCheckboxSetting("quickstart","Quick-Start-Guide", "Nastavení, zda má HikFind stahovat Quick Start Guide produktu.",
                    true, new FindBetween[]{ new FindBetween("id=\"Quick_Start_Guide\"", "</div"), new FindBetween("href=\"", "\"") }, false,
                    null),


                    new SettingCategory("NVR/DVR", new BaseSetting[]
                    {
                    new FinderCheckboxSetting("commmatrix","Communication-Matrix","Nastavení, zda má HikFind stahovat Communication Matrix produktu.",
                    true,new FindBetween[]{ new FindBetween("id=\"Communication_Matrix\"", "</div>"), new FindBetween("href=\"", "\"") }, false,
                    null)
                    })
                    }),


                    new SettingCategory("Vyhledávání", new BaseSetting[]
                    { new RadioSetting("normalhledani","Klasické hledání","Nastavení, že HikFind bude hledat produkty svou normální formou (Specifické hikvision stránky).",true,"typhledani",
                    new BaseSetting[]
                    {
                        new CheckboxSetting("webpohik","Prohlížeč po HIK","Nastavení, že HikFind použije API internetového prohlížeče pokud nic nenalezne na specifických hikvision stránkách.", true,
                        null)
                    }),
                    new RadioSetting("webhledani","Hledání prohlížečem","Nastavení, že HikFind bude hledat pomocí API internetového prohlížeče a nebude hledat na specifických hikvision stránkách.","typhledani",
                    null),
                    new CheckboxSetting("upravavstupu","Úprava vstupu","Nastavení, že HikFind upraví váš vstup (text) za účelem efektevnějšího vyhledávání.",true,
                    null)
                    }),


                    new SettingCategory("Obrázky", new BaseSetting[]
                    {
                        new RadioSetting("jpgformat","Formát JPG","Nastavení, zda se stažené obrázky mají překonvertovat do formátu JPG",true,"formatObraz",
                        null),
                        new RadioSetting("pngformat","Formát PNG","Nastavení, zda se stažené obrázky mají překonvertovat do formátu PNG","formatObraz",
                        new BaseSetting[]
                        {
                            new CheckboxSetting("transparent","Transparentní","Nastavení, zda se staženým obrázkům má zanechat transparentní pozadí.",false,
                            null)
                        }),
                        new CheckboxSetting("crop","Oříznout", "Nastavení, zda se stažené obrázky mají oříznout dle obsahu v obrázku.",true,
                        new BaseSetting[]
                        {
                            new RadioSetting("fullcrop","FULL Oříznutí","Nastavení, zda má HikFind zkontrolovat každý pixel pro oříznutí (Zabírá čas, ale 100% úspěšnost)", true,"typoriznuti",
                            null),
                            new RadioSetting("fastcrop","FAST Oříznutí", "Nastavení, zda má HikFind použít rychlejší kontrolování pixelů (až 4x rychlejší jak FULL crop, ale neefektivní když je na obrázku více (nespojených) předmětů)", "typoriznuti",
                            null),
                        })
                    })
            };

            foreach (BaseSetting mainSearch in HikFindSearch.settings)
            {
                HikFindSearch.FindAllSettings(mainSearch).ToList().ForEach(x => HikFindSearch.dictSettings.Add(x.Key, x.Value));
            }

            if (File.Exists("settings.json"))
            {
                Dictionary<string, bool> loadedSettings = new Dictionary<string, bool>();
                using (StreamReader rd = new StreamReader("settings.json"))
                {
                    loadedSettings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(rd.ReadToEnd());
                }


                List<string> usedRadios = new List<string>();
                foreach(KeyValuePair<string, bool> kvp in loadedSettings)
                {
                    BaseSetting setting = HikFindSearch.dictSettings[kvp.Key];
                    if (setting is RadioSetting)
                    {
                        RadioSetting rdioSetting = setting as RadioSetting;
                        if(usedRadios.Contains(rdioSetting.Group)) {
                            continue;
                        }
                        usedRadios.Add(rdioSetting.Group);
                    }
                    HikFindSearch.dictSettings[kvp.Key].Check = kvp.Value;
                }
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