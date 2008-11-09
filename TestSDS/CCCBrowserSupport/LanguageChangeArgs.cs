using System;
using System.Net;

namespace CCCBrowser.Support
{
    public class LanguageChangeArgs : EventArgs
    {

        public string Language { get; set; }
        public string Version { get; set; }
    }
}
