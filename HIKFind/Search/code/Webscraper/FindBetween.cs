namespace HIKFind.Search.Webscraper
{
    public class FindBetween
    {
        string start;
        public string Start { get { return start; } }
        string end;
        public string End { get { return end; } }

        public FindBetween(string start, string end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
