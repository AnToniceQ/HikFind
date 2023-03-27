using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace HIKFind
{
    public class Product
    {
        private List<Bitmap> images = new List<Bitmap>();
        private BitmapImage guiMainImage = null;

        public string Name 
        { 
            get;
            set;
        }
        
        public BitmapImage GUIMainImage 
        {
            get
            {
                return guiMainImage;
            }
        }
        public string Url 
        { 
            get; 
            set; 
        }
        public string FolderPath 
        { 
            get; 
            set; 
        }
        public DateTime DateSearched 
        { 
            get; 
            set; 
        }
        public string DateSearchedString
        {
            get
            {
                return DateSearched.ToString("dd.MM.yyyy H:mm");
            }
        }

        public void AddImage(Bitmap bmp)
        {
            if(images.Count == 0)
            {
                guiMainImage = ImageToGUI(bmp);
            }
            images.Add(bmp);
        }

        private BitmapImage ImageToGUI(Bitmap image)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
