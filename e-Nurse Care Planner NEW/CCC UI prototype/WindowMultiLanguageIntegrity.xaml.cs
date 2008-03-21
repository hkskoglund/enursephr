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
using CCC.BusinessLayer;
using System.ComponentModel;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowMultiLanguageIntegrity : Window
    {
        BackgroundWorker worker;
        string version;
        string languageName;
        CCCFrameworkLanguageAnalysis langAnalysis;
           
        public WindowMultiLanguageIntegrity()
        {
            InitializeComponent();
        }

        public void worker_DoWork(object sender, DoWorkEventArgs a)
        {
            langAnalysis.doDLAnalysis(this.version, this.languageName, worker);
        }

        public void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs a)
        {
            pbBackground.Value = 0;
            pbBackground.Visibility = Visibility.Collapsed;
             spDLSummary.Visibility = Visibility.Visible;
            worker = null;
        }

        public void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbBackground.Value = e.ProgressPercentage;
            lbFramework.Items.Add(e.UserState);
            spDLFrameworkElement.Visibility = Visibility.Visible;
            pbBackground.Visibility = Visibility.Visible;
        }

        public void showDLA()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = false;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            spDLA.Visibility  = Visibility.Visible;
            worker.RunWorkerAsync();
        }

        private void lbFramework_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedItem == null)
                return;
            Type t = l.SelectedItem.GetType();

            spDLMissingCodes.Visibility = Visibility.Collapsed;

            switch (t.Name)
            {
                case "MCareComponent": lbDLMissingCodes.ItemTemplate = (DataTemplate)this.Resources["CodeTemplate"];
                    lbDLMissingCodes.ItemsSource = ((MCareComponent)l.SelectedItem).MissingList;
                    if (((MCareComponent)l.SelectedItem).MissingListCount > 0)
                        spDLMissingCodes.Visibility = Visibility.Visible;
                    break;

                case "MOutcomeType": lbDLMissingCodes.ItemTemplate = (DataTemplate)this.Resources["CodeTemplate"];
                    lbDLMissingCodes.ItemsSource = ((MOutcomeType)l.SelectedItem).MissingList;
                    if (((MOutcomeType)l.SelectedItem).MissingListCount > 0)
                        spDLMissingCodes.Visibility = Visibility.Visible;
                    break;

                case "MNursingDiagnosis": lbDLMissingCodes.ItemTemplate = (DataTemplate)this.Resources["BigCodeTemplate"];
                    lbDLMissingCodes.ItemsSource = ((MNursingDiagnosis)l.SelectedItem).MissingList;
                    if (((MNursingDiagnosis)l.SelectedItem).MissingListCount > 0)
                        spDLMissingCodes.Visibility = Visibility.Visible;

                    break;

                case "MActionType": lbDLMissingCodes.ItemTemplate = (DataTemplate)this.Resources["CodeTemplate"];
                    lbDLMissingCodes.ItemsSource = ((MActionType)l.SelectedItem).MissingList;
                    if (((MActionType)l.SelectedItem).MissingListCount > 0)
                        spDLMissingCodes.Visibility = Visibility.Visible;

                    break;

                case "MNursingIntervention": lbDLMissingCodes.ItemTemplate = (DataTemplate)this.Resources["BigCodeTemplate"];
                    lbDLMissingCodes.ItemsSource = ((MNursingIntervention)l.SelectedItem).MissingList;
                    if (((MNursingIntervention)l.SelectedItem).MissingListCount > 0)
                        spDLMissingCodes.Visibility = Visibility.Visible;
                    break;

            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            langAnalysis= new CCCFrameworkLanguageAnalysis(Properties.Settings.Default.Version);

            spCCCMultiLanguageIntegrity.DataContext = langAnalysis;

            //gvcLanguage.Header = "Language (" + langAnaylsis.Version + ")";
            //gvcCarePattern.Header = "Care patterns (" + langAnaylsis.ExpectedNumberOfCarePatterns + ")";
            //gvcCareComponent.Header = "Care components (" + langAnaylsis.ExpectedNumberOfCareComponents + ")";
            //gvcDiagnosis.Header = "Diagnoses (" + langAnaylsis.ExpectedNumberOfDiagnoses + ")";
            //gvcIntervention.Header = "Interventions (" + langAnaylsis.ExpectedNumberOfInterventions + ")";

            lvLanguageShallow.ItemsSource = langAnalysis.LanguageFrameworkList;
           // gvView.ColumnHeaderToolTip = langAnaylsis.

            //cbLanguage.ItemsSource = langAnaylsis.LanguageFrameworkList;

            // for (int i = 0; i < langAnaylsis.LanguageFrameworkList.Count; i++)
            //    if (langAnaylsis.LanguageFrameworkList[i].LanguageName.Contains(Properties.Settings.Default.LanguageName))
            //        cbLanguage.SelectedItem = langAnaylsis.LanguageFrameworkList[i];

        }

        private void DLA_Click(object sender, RoutedEventArgs e)
        {
            this.showDLA();
            e.Handled = true;
        }

        private void lvLanguageShallow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;

            if (worker == null) // If not worker is running
            {
                ContextMenu ctxm = new ContextMenu();
                languageName = langAnalysis.LanguageFrameworkList[lv.SelectedIndex].LanguageName;
                version = Properties.Settings.Default.Version;
         
                MenuItem mi = new MenuItem();
                mi.Header = "Check integrity of codes";
                ctxm.Items.Add(mi);
                lvLanguageShallow.ContextMenu = ctxm;
                mi.Click += new RoutedEventHandler(DLA_Click);
                lbFramework.Items.Clear();
                spDLA.Visibility = Visibility.Collapsed;
                  }
            else
                lvLanguageShallow.ContextMenu = null;
           
        }

        //private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    langFramework selItem = (langFramework)(sender as ComboBox).SelectedItem;
        //    if (selItem != null)
        //    {
        //        Properties.Settings.Default.LanguageName = selItem.LanguageName.Trim();
              
        //        Properties.Settings.Default.Save();

        //        App.cccFrameWork.DB.Dispose();
        //        App.cccFrameWork = new ViewCCCFrameWork(Properties.Settings.Default.LanguageName);
        //   }
            
        //}

        
    }
}
