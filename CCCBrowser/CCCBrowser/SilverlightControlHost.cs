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
using System.Windows.Browser;

namespace CCCBrowser
{
    // Interface with silverControlHost element in Html page
    public class SilverlightControlHost
    {
        string Width { get; set; }
        string Height { get; set; }
        string Border { get; set; }

        public HtmlElement Host;

        public SilverlightControlHost()
        {
            Host  = HtmlPage.Document.GetElementById("silverlightControlHost");
            Width = Host.GetStyleAttribute("width");
            Height = Host.GetStyleAttribute("height");
            Border = Host.GetStyleAttribute("border");
        }
        
        public void hide()
        {
            Host.SetStyleAttribute("visibility", "hidden");
            Host.SetStyleAttribute("width","0");
            Host.SetStyleAttribute("height","0");
            Host.SetStyleAttribute("border","opx");
        }

        public void show()
        {
            Host.SetStyleAttribute("visibility", "visible");
            Host.SetStyleAttribute("width", Width);
            Host.SetStyleAttribute("height", Height);
            Host.SetStyleAttribute("border", Border);
        }
    }
}
