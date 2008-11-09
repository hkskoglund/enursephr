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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CCCBrowser.SoapAPI;
using System.Threading;
using System.ComponentModel;

namespace TestSDS
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
       
        CCCSoapAPI soapAPI = new CCCSoapAPI("hkscloudtest", "oglu94");

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            loadFramework();
            
        }

        private void loadFramework()
        {
            pbProgress.Visibility = Visibility.Visible;
            
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
          //  bw.RunWorkerAsync();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            tbStatus.Text = e.UserState as string;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            soapAPI.readCCCFramework(sender as BackgroundWorker,"nb-NO", "2.0");
            
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbProgress.Visibility = Visibility.Collapsed;
           // lbDiagnoses.ItemsSource = soapAPI.Diagnoses;
        }
    }
}
