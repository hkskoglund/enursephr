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
    public abstract class FrameworkElement
    {
        public virtual string Concept { get; set; }
        public virtual string Definition { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public int PatternId { get; set; }

        
        
       
    }
}
