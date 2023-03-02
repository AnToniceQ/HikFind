using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIKFind
{
    public class SearchedResultItem
    {
        string name;

        public string Name { get { return name; } }

        string url;

        public string Url { get { return url; } }

        public SearchedResultItem(string name, string url)
        {
            this.name = name;
            this.url = url;
        }

        public override string ToString()
        {
            return url;
        }
    }
}
