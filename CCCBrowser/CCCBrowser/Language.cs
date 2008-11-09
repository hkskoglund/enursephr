using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCCBrowser
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
