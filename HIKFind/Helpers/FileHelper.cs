using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind.Helpers
{
    internal class FileHelper
    {
        public static void RemoveDirectoryContents(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo file in files)
            {
                file.Delete();
            }

            DirectoryInfo[] subDirectories = di.GetDirectories();
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                subDirectory.Delete(true);
            }
        }
    }
}
