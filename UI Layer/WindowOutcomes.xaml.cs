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
using Microsoft.Samples.KMoore.WPFSamples.DateControls;

using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.PHR;

namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for WindowOutcomes.xaml
    /// </summary>
    public partial class WindowOutcomes : Window
    {
        public WindowOutcomes()
        {
            InitializeComponent();

           
        }

        TagLangageConverter tagConverter = new TagLangageConverter();

        private void btnNewOutcome_Click(object sender, RoutedEventArgs e)
        {
            Tag selTag = this.DataContext as Tag;
            // Default to outcome 1, user must change it
            Outcome newOutcome = Outcome.CreateOutcome(Guid.NewGuid(), 1, (tagConverter.getTaxonomyGuidOutcomeType(1, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version)));
            newOutcome.Tag = selTag;
            History newHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
            newOutcome.History = newHistory;
            
            App.s_carePlan.DB.AddToOutcome(newOutcome);
            App.s_carePlan.DB.AddToHistory(newHistory);
            ((WindowMain)App.Current.MainWindow).SaveCarePlan();
            
        }

        private void btnDeleteOutcome_Click(object sender, RoutedEventArgs e)
        {
            Outcome selOutcome = (Outcome)lvOutcome.SelectedItem;
            if (selOutcome != null)
            {
                App.s_carePlan.DB.DeleteObject(selOutcome);
                ((WindowMain)App.Current.MainWindow).SaveCarePlan();
            }
            else
                MessageBox.Show("No outcome selected to delete, please select an outcome if possible", "No outcome selected", MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Outcome selOutcome = (Outcome)lvOutcome.SelectedItem;
            //if (selOutcome == null)
            //    MessageBox.Show("You have noe s
            //if (!selOutcome.HistoryReference.IsLoaded)
            //    selOutcome.HistoryReference.Load();
            //selOutcome.History.UpdatedDate = DateTime.Now;
            //selOutcome.History.UpdatedBy = System.Environment.UserName;

            
            ((WindowMain)App.Current.MainWindow).SaveCarePlan();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
              // Based on info: http://joshsmithonwpf.wordpress.com/2007/06/28/how-to-use-findname-with-a-contentcontrol/
                // Accessed : 10 march 2008

              ContentPresenter contentPresenter = VisualTreeHelper.GetChild(ccOutcome, 0) as ContentPresenter;

              ComboBox cbOutcomePlan = (ComboBox)ccOutcome.ContentTemplate.FindName("cbOutcomePlan", contentPresenter);
              ComboBox cbOutcomeEvaluate = (ComboBox)ccOutcome.ContentTemplate.FindName("cbOutcomeEvaluate", contentPresenter);

              cbOutcomePlan.ItemsSource = App.s_cccFrameWork.Outcomes;
              cbOutcomeEvaluate.ItemsSource = App.s_cccFrameWork.cvOutcomeTypes;
            
        }

        
    }
}
