using System.Collections.Generic;

namespace Instagram_Scraper.Utility
{
    public class UriNameDictionary : Dictionary<string, string>
    {
        public new void Add(string name, string uri)
        {
            base.Add(name, uri);
        }

        public new string this[string name]
        {
            get => base[name];
            set => base[name] = value;
        }
    }
}