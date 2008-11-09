using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Browser;
using System.IO;
using System.IO.IsolatedStorage;

namespace CCCBrowser
{
    public partial class Page : UserControl
    {
        

      
        
        SilverlightControlHost silverlightHost = new SilverlightControlHost();

     
        public Page()
        {
            InitializeComponent();
           
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
          
           
        //   silverlightHost.hide();
        //   System.Threading.Thread.Sleep(3000);
        //   silverlightHost.show();
        }

        
        private void printTest()
        {
            HtmlElement he = HtmlPage.Document.CreateElement("testDiv");
            he.SetProperty("innerHtml", "<table><thead><tr><th>Overskrift 1</th></tr></thead></table></paragraph>");
            HtmlPage.Document.Body.AppendChild(he);

            HtmlElement s = HtmlPage.Document.GetElementById("silverlightControlHost");
            s.SetStyleAttribute("visibility", "hidden");
            s.SetStyleAttribute("height", "0");
            s.SetStyleAttribute("width", "0");
            s.SetStyleAttribute("border","0px");
        }

        
       

    }
}
