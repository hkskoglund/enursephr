using System;
using System.Net;

namespace CCCBrowser.Support
{
    public abstract class FrameworkElement
    {
        public virtual string Concept { get; set; }
        public virtual string Definition { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public int PatternId { get; set; }

        
        
       
    }
}
