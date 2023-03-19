using System;
using System.Drawing;

namespace HIKFind
{
    public class Product
    {
        public string Name { get; set; }
        public Bitmap[] Images { get; set; }
        public string Url { get; set; }
        public string FolderPath { get; set; }
        public DateTime DateSearched { get; set; }
        public string DateSearchedString
        {
            get
            {
                return DateSearched.ToString("dd.MM.yyyy H:mm");
            }
        }
    }
}
