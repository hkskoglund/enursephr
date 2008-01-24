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

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowLangShallowAnalysis : Window
    {
        public WindowLangShallowAnalysis()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CCCFrameworkLanguageAnalysis langAnaylsis = 
                new CCCFrameworkLanguageAnalysis(Properties.Settings.Default.Version);

            spCCCLanguageShallowAnalysis.DataContext = langAnaylsis;

            //gvcLanguage.Header = "Language (" + langAnaylsis.Version + ")";
            //gvcCarePattern.Header = "Care patterns (" + langAnaylsis.ExpectedNumberOfCarePatterns + ")";
            //gvcCareComponent.Header = "Care components (" + langAnaylsis.ExpectedNumberOfCareComponents + ")";
            //gvcDiagnosis.Header = "Diagnoses (" + langAnaylsis.ExpectedNumberOfDiagnoses + ")";
            //gvcIntervention.Header = "Interventions (" + langAnaylsis.ExpectedNumberOfInterventions + ")";

            lvLanguageShallow.ItemsSource = langAnaylsis.LanguageFrameworkList;
           // gvView.ColumnHeaderToolTip = langAnaylsis.

            //cbLanguage.ItemsSource = langAnaylsis.LanguageFrameworkList;

            // for (int i = 0; i < langAnaylsis.LanguageFrameworkList.Count; i++)
            //    if (langAnaylsis.LanguageFrameworkList[i].LanguageName.Contains(Properties.Settings.Default.LanguageName))
            //        cbLanguage.SelectedItem = langAnaylsis.LanguageFrameworkList[i];

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
