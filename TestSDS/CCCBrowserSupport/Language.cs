using System;
using System.Net;

namespace CCCBrowser.Support
{
    public class CCCLanguage
        
    {
        public CCCLanguage(string language, string friendlyLangauge, string version, string imageURL)
        {
            Language = language;
            Version = version;
            ImageURL = imageURL;
            FriendlyLanguage = friendlyLangauge;
        }

        public string Language { get; set; }
        public string FriendlyLanguage { get; set; }
        public string Version { get; set; }
        public string ImageURL { get; set; }
    }
}
