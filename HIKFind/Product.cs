using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HIKFind
{
    public class Product
    {
        public string Name { get; set; }
        public Bitmap[] Images { get; set; }
        public string Url { get; set; }
        public string FolderPath { get; set; }
    }
}
