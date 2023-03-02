using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace HIKFind
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if(File.Exists("products.xml"))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Product>));
                using (StreamReader rd = new StreamReader("products.xml"))
                {
                    HikFindSearch.products = xs.Deserialize(rd) as ObservableCollection<Product>;
                }
            }

            HikFindSearch mainWindow = new HikFindSearch();
            mainWindow.ShowDialog();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Product>));
            using (StreamWriter wr = new StreamWriter("products.xml"))
            {
                xs.Serialize(wr, HikFindSearch.products);
            }
        }
    }
}
