using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;

namespace eNurseCP.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for WindowInsertHyperlink.xaml
    /// </summary>
    public partial class WindowInsertHyperlink : Window
    {

        public string URL { get; set; }
        public string WebTitle { get; set; }

        public WindowInsertHyperlink()
        {
            InitializeComponent();
        }


// Based on: http://blogs.msdn.com/noahc/archive/2007/02/19/get-a-web-page-s-title-from-a-url-c.aspx
// Accessed : 5 april 2008
        public string GetWebPageTitle(string url)
        {
            // Create a request to the url
            HttpWebRequest request;

            try
            {
                request = HttpWebRequest.Create(url) as HttpWebRequest;
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Please choose a valid web-address, could not understand the format", "Invalid web-address", MessageBoxButton.OK);
                return null;
            }

            // If the request wasn't an HTTP request (like a file), ignore it
            if (request == null) return null;

            // Use the user's credentials
            request.UseDefaultCredentials = true;
            request.Method = "HEAD";

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse; 
            }
            catch (WebException)
            {
                return null;
            }
                
            // Regular expression for an HTML title
            string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            if (new List<string>(response.Headers.AllKeys).Contains("Content-Type"))
                if (response.Headers["Content-Type"].StartsWith("text/html"))
                {
                    // Download the page
                    WebClient web = new WebClient();
                    web.UseDefaultCredentials = true;
                    string page = web.DownloadString(url);

                    // Extract the title
                    Regex ex = new Regex(regex, RegexOptions.IgnoreCase);
                    return ex.Match(page).Value.Trim();
                }

            // Not a valid HTML page
            return null;
        }

        private void btnGetTitle_Click(object sender, RoutedEventArgs e)
        {
            tbTitle.Text = GetWebPageTitle(tbAddress.Text);
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
