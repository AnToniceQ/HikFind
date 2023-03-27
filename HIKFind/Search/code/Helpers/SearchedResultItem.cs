namespace HIKFind
{
    public class SearchedResultItem
    {
        string resultName;

        public string ResultName
        {
            get
            {
                return resultName;
            }
        }

        string name;

        public string Name 
        { 
            get 
            { 
                return name; 
            } 
        }

        bool exists;

        public bool Exists 
        { 
            get 
            { 
                return exists; 
            } 
        }

        string url;

        public string Url 
        { get 
            { 
                return url; 
            } 
        }

        public SearchedResultItem(string webClientLang, string name, string url, bool exists)
        {
            this.resultName = "["+webClientLang+"] | " + name;
            this.name = name;
            this.url = url;
            this.exists = exists;
        }

        public override string ToString()
        {
            return url;
        }
    }
}
