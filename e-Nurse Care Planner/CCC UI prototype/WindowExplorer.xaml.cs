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
using CCC.BusinessLayer;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for WindowExplorer.xaml
    /// </summary>
    public partial class WindowExplorer : Window
    {
        private char FilterCode;
        private string FilterSearch;

        public WindowExplorer()
        {
            InitializeComponent();
            
        }

        #region Language handling

        private void MenuItemLanguageIntegrity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowMultiLanguageIntegrity winMultiLangIntegrity = new WindowMultiLanguageIntegrity();
                winMultiLangIntegrity.ShowDialog();
                App.cccFrameWork.loadFrameworkAnalysis();
                refreshCCCFramework();
            }
            catch (Exception exception)
            {
                showDatabaseError(exception);
            }
        }

        public void refreshCCCFramework()
        {

            
            cbLanguage.ItemsSource = App.cccFrameWork.FrameworkActual;
            if (App.cccFrameWork.FrameworkActual != null)
                cbLanguage.ToolTip = "Last language integrity check was run on " +
                    App.cccFrameWork.FrameworkActual[0].Date.ToString();
            for (int i = 0; i < App.cccFrameWork.FrameworkActual.Count; i++)
            {
                if (App.cccFrameWork.FrameworkActual[i].Language_Name == Properties.Settings.Default.LanguageName)
                {
                    cbLanguage.SelectedItem = App.cccFrameWork.FrameworkActual[i];
                    break;
                }
            }

            spCopyright.DataContext = App.cccFrameWork;
            //tbFrameworkAuthors.DataContext = App.cccFrameWork;
            //tbFrameworkName.DataContext = App.cccFrameWork;
            //tbFrameworkVersion.DataContext = App.cccFrameWork;
            //tbFrameworkLastUpdate.DataContext = App.ccc

            //lbCareComponent.GroupStyle.Add(new GroupStyle());
            
            // Care component
            
            lbCareComponent.ItemsSource = App.cccFrameWork.cvComponents; // Master
            ccCareComponent.Content = App.cccFrameWork.cvComponents;     // Detail
            lbCareComponent.SelectedIndex = 1; // Default to selfcare

            // Outcome type
            lbOutcomeType.ItemsSource = App.cccFrameWork.cvOutcomeTypes;
            ccOutcomeType.Content = App.cccFrameWork.cvOutcomeTypes;

            // Action type

            lbActionType.ItemsSource = App.cccFrameWork.cvActionTypes;
            ccActionType.Content = App.cccFrameWork.cvActionTypes;
            
        }


        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkActual selItem = (FrameworkActual)(sender as ComboBox).SelectedItem;

            if (App.cccFrameWork.LogoURL == null)
                imgLogo.Visibility = Visibility.Collapsed;
            else
                imgLogo.Visibility = Visibility.Visible;

            if (selItem != null)
            {

                if (selItem.Language_Name != Properties.Settings.Default.LanguageName)
                {
                    Properties.Settings.Default.LanguageName = selItem.Language_Name.Trim();
                    Properties.Settings.Default.Save();

                    App.cccFrameWork.DB.Dispose();
                    App.cccFrameWork = new ViewCCCFrameWork(Properties.Settings.Default.LanguageName,
                        Properties.Settings.Default.Version);
                    refreshCCCFramework();

                    ((WindowMain)App.Current.MainWindow).refreshTags();
                }

            }

        }

        #endregion

        #region Explorer search

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            FilterSearch = tb.Text;
            if (FilterSearch == "")
            {
                spCareComponent.Visibility = Visibility.Visible;
                App.cccFrameWork.cvComponents.Filter = null;

                lbCareComponent.SelectedIndex = 1; // Default to selfcare-component
                return;
            }

            //   spCareComponent.Visibility = Visibility.Collapsed;


            App.cccFrameWork.cvComponents.Filter = new Predicate<object>(FilterOutComponentsSearch);
            App.cccFrameWork.cvComponents.Refresh();
            if (App.cccFrameWork.cvComponents.Count == 0)
            {
                lblNoMatchCareComponent.Visibility = Visibility.Visible;
                spCareComponent.Visibility = Visibility.Hidden;

            }
            else
            {
                lblNoMatchCareComponent.Visibility = Visibility.Collapsed;
                spCareComponent.Visibility = Visibility.Visible;
            }

            App.cccFrameWork.cvDiagnoses.Filter = new Predicate<object>(FilterOutDiagnosesSearch);
            App.cccFrameWork.cvDiagnoses.Refresh();

            if (App.cccFrameWork.cvDiagnoses.Count == 0)
            {
                lblNoMatchNursingDiagnoses.Visibility = Visibility.Visible;
                spNursingDiagnoses.Visibility = Visibility.Hidden;
            }
            else
            {
                lblNoMatchNursingDiagnoses.Visibility = Visibility.Collapsed;
                spNursingDiagnoses.Visibility = Visibility.Visible;
            }

            lbNursingDiagnosis.ItemsSource = App.cccFrameWork.cvDiagnoses;
            lbNursingDiagnosis.SelectedIndex = 0;

            App.cccFrameWork.cvInterventions.Filter = new Predicate<object>(FilterOutInterventionsSearch);
            App.cccFrameWork.cvInterventions.Refresh();

            if (App.cccFrameWork.cvInterventions.Count == 0)
            {
                lblNoMatchNursingInterventions.Visibility = Visibility.Visible;
                spNursingInterventions.Visibility = Visibility.Hidden;
            }
            else
            {
                lblNoMatchNursingInterventions.Visibility = Visibility.Collapsed;
                spNursingInterventions.Visibility = Visibility.Visible;
            }

            lbNursingInterventions.ItemsSource = App.cccFrameWork.cvInterventions;
            lbNursingInterventions.SelectedIndex = 0;
        }

        private bool FilterOutDiagnosesSearch(object item)
        { // Based on example from Beatrize Costa blog, accessed 25 november 2007

            FrameworkDiagnosis nd = item as FrameworkDiagnosis;

            if (nd == null)
                return false;

            if (nd.Concept.ToLower().Contains(FilterSearch.ToLower()) || nd.Definition.ToLower().Contains(FilterSearch.ToLower()))
                return true;
            else
                return false;



        }

        private bool FilterOutInterventionsSearch(object item)
        {// Based on example from Beatrize Costa blog, accessed 25 november 2007

            FrameworkIntervention nd = item as FrameworkIntervention;

            if (nd == null)
                return false;

            if (nd.Concept.ToLower().Contains(FilterSearch.ToLower()) || nd.Definition.ToLower().Contains(FilterSearch.ToLower()))
                return true;
            else
                return false;



        }

        private bool FilterOutComponentsSearch(object item)
        {// Based on example from Beatrize Costa blog, accessed 25 november 2007

            Care_component cc = item as Care_component;

            if (cc == null)
                return false;

            if (cc.Component.ToLower().Contains(FilterSearch.ToLower()) || cc.Definition.ToLower().Contains(FilterSearch.ToLower()))
                return true;
            else
                return false;



        }


        private bool FilterOutDiagnoses(object item)
        { // Based on example from Beatrize Costa blog, accessed 25 november 2007

            FrameworkDiagnosis nd = item as FrameworkDiagnosis;


            if (nd == null)
                return false;


            int result = nd.ComponentCode.CompareTo(FilterCode.ToString());

            if (result == 0) return true;

            return false;


        }

        private bool FilterOutInterventions(object item)
        {
            // Based on example from Beatrize Costa blog, accessed 25 november 2007

            FrameworkIntervention nd = item as FrameworkIntervention;


            if (nd == null)
                return false;

            int result = nd.ComponentCode.CompareTo(FilterCode.ToString());

            if (result == 0) return true;

            return false;

        }


        #endregion

        #region Care component handling


        private void ccCareComponent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentControl ccFrom = sender as ContentControl;
            Care_component selectedCareComponent = (Care_component)((ListCollectionView)ccFrom.Content).CurrentItem;

            if (selectedCareComponent != null)
            {
                DataObject transfer = new DataObject();

                transfer.SetData("CCC/CareComponent", selectedCareComponent);
                string pretty = "(" + selectedCareComponent.Code + ") " + selectedCareComponent.Component + "\n" + selectedCareComponent.Definition + "\n";
                transfer.SetData(DataFormats.UnicodeText, pretty);
                DragDrop.DoDragDrop(ccFrom, transfer, DragDropEffects.Copy);
            }

            
        }
      
        private void lbCareComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbSearch.Text != "")
                return; // Make sure were not filtering diagnoses while search is active

            ListBox lb = sender as ListBox;

            if (lb.SelectedIndex == -1) // Tom selected item-liste
                return;

            Care_component cc = lb.SelectedItem as Care_component;
            FilterCode = cc.Code.ToCharArray()[0];

            App.cccFrameWork.cvDiagnoses.Filter = new Predicate<object>(FilterOutDiagnoses);
            App.cccFrameWork.cvDiagnoses.Refresh();

            App.cccFrameWork.cvInterventions.Filter = new Predicate<object>(FilterOutInterventions);
            App.cccFrameWork.cvInterventions.Refresh();


            // Master
            lbNursingDiagnosis.ItemsSource = App.cccFrameWork.cvDiagnoses;
            lbNursingInterventions.ItemsSource = App.cccFrameWork.cvInterventions;

            // Detail

            ccNursingDiagnosis.Content = App.cccFrameWork.cvDiagnoses.CurrentItem;
            ccNursingIntervention.Content = App.cccFrameWork.cvInterventions.CurrentItem;
           
         

        }


        #endregion

        #region Nursing Diagnosis handling

        private void lbNursingDiagnosis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
           ListBox lb = sender as ListBox;
           FrameworkDiagnosis nd = lb.SelectedItem as FrameworkDiagnosis;

           ccNursingDiagnosis.Content = nd; // Update detail view
        }

        private void lbNursingDiagnosis_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //FrameworkDiagnosis frameworkDiagnosis = (sender as ListBox).SelectedItem as FrameworkDiagnosis;
            //CarePlanDiagnosis diag = new CarePlanDiagnosis();

            //// Copy from framework diagnosis to careplan diagnosis

            ////Ediag.CarePlan.Id = 1; // Må endres etterhvert !!!!

            //diag.CarePlan = App.carePlan.DB.CarePlan.First(c => c.Id == 1);

            //diag.Concept = frameworkDiagnosis.Concept; //Persistence Ignorance = PI
            //diag.Title = diag.Concept; // Default title is the same as the concept
            //diag.ComponentName = frameworkDiagnosis.Care_component.Component; // PI
            //diag.ComponentCode = frameworkDiagnosis.ComponentCode;
            //diag.MajorCode = (short)frameworkDiagnosis.MajorCode;
            //diag.MinorCode = frameworkDiagnosis.MinorCode;
            //diag.Version = (string)App.Current.Properties["Version"];

            //diag.CreationDate = DateTime.Now;
            //diag.CreationDateString = diag.CreationDate.Value.ToLongDateString(); // PI
            //diag.Definition = frameworkDiagnosis.Definition; //PI

            //App.carePlan.InsertDiagnosis(diag);

            //App.carePlan.cvDiagnoses.Refresh();


            ////+reasondiagnosis!!

            ////        myNursingDiagnosis my = new myNursingDiagnosis(nursingDiagnosis.Care_component.Component,
            ////        nursingDiagnosis.Concept, cccFrameWork.Outcomes);

            ////carePlan.Diagnoses.Add(diag);


            //lbCarePlanDiagnoses.ItemsSource = App.carePlan.cvDiagnoses;
            //lbCarePlanDiagnoses.Visibility = Visibility.Visible;

            //App.carePlan.PrettyCarePlan_Update(lcoll);
        }

        private void ccNursingDiagnosis_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string txtDiagnosis;
            string txtCode;

            bool hasOutcomeAttached = (bool)cbAttachToDiagnosis.IsChecked;
            bool hasMinorDiagnosis;

            FrameworkDiagnosis selectedNursingDiagnosis = (FrameworkDiagnosis)ccNursingDiagnosis.Content;
            FrameworkOutcomeType selectedOutcomeType = (FrameworkOutcomeType)lbOutcomeType.SelectedItem;
            
            if (selectedNursingDiagnosis != null)
            {

                if (selectedNursingDiagnosis.MinorCode == null)
                    hasMinorDiagnosis = false;
                else
                    hasMinorDiagnosis = true;
        
                DataObject transfer = new DataObject();

                if (hasOutcomeAttached)
                    transfer.SetData("CCC/OutcomeType", selectedOutcomeType);

                transfer.SetData("CCC/NursingDiagnosis", selectedNursingDiagnosis);

                // Code
                txtCode = "Diagnosis :\n";
                txtCode += "(" + selectedNursingDiagnosis.ComponentCode +"."+selectedNursingDiagnosis.MajorCode.ToString("00")+".";
                if (hasMinorDiagnosis)
                    txtCode += selectedNursingDiagnosis.MinorCode;
                else
                    txtCode += "0";

                if (hasOutcomeAttached)
                    txtCode += "."+selectedOutcomeType.Code.ToString();

                // Diagnosis

                txtDiagnosis = txtCode+ ") " + selectedNursingDiagnosis.Concept + "\n" + selectedNursingDiagnosis.Definition + "\n";

                if (hasOutcomeAttached)
                    txtDiagnosis += "Outcome :\n"+"(" + selectedOutcomeType.Code+") "+ selectedOutcomeType.Concept + "\n" + selectedOutcomeType.Definition;
                
                transfer.SetData(DataFormats.UnicodeText, txtDiagnosis);
               
                // Let's do a drag n drop
                
                DragDrop.DoDragDrop(sender as Border, transfer, DragDropEffects.Copy);
            }
            
        }


        #endregion

        #region Nursing Intervention handling

        private void ccNursingIntervention_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentControl dragSource = sender as ContentControl;
            string txtIntervention;

            FrameworkIntervention selectedNursingIntervention = (FrameworkIntervention)ccNursingIntervention.Content;

            if (selectedNursingIntervention != null)
            {
                DataObject transfer = new DataObject();
                transfer.SetData("CCC/NursingIntervention", selectedNursingIntervention);
                

                // Build text data drop

                if (selectedNursingIntervention.MinorCode != null)
                    txtIntervention = "(" + selectedNursingIntervention.ComponentCode + "." + selectedNursingIntervention.MajorCode.ToString("00") +
                     "." + selectedNursingIntervention.MinorCode.ToString() +
                     ") " + selectedNursingIntervention.Concept + "\n" + selectedNursingIntervention.Definition + "\n";
                else
                    txtIntervention = "(" + selectedNursingIntervention.ComponentCode + "." + selectedNursingIntervention.MajorCode.ToString("00") +

                                        ") " + selectedNursingIntervention.Concept + "\n" + selectedNursingIntervention.Definition + "\n";


                transfer.SetData(DataFormats.UnicodeText, txtIntervention);
                DragDrop.DoDragDrop(dragSource, transfer, DragDropEffects.Copy);
            }
            
        }


        
        
        
        private void lbNursingInterventions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            FrameworkIntervention nursingIntervention = lb.SelectedItem as FrameworkIntervention;
            /*
                        if (lbCarePlanDiagnoses.SelectedIndex == -1) // User has not selected diagnose to add intervention to
                            return;

                        myNursingIntervention my = new myNursingIntervention(nursingIntervention.Care_component.Component,
                            nursingIntervention.Concept);

                        ((myNursingDiagnosis)lbCarePlanDiagnoses.SelectedItem).Intervention.Add(my);

                        carePlan.Interventions.Add(my);
                        cvcarePlanInterventions.Refresh();


                        lbCarePlanInterventions.ItemsSource = cvcarePlanInterventions;
                        lbCarePlanInterventions.Visibility = Visibility.Visible;

                        PrettyCarePlan_Update();
                        */
        }

          
        #endregion

        private void showDatabaseError(Exception ex)
        {

            WindowDatabaseError wDatabaseError = new WindowDatabaseError();
            //  wDatabaseError.tbDatabaseError.Text = "Cannot access database server : " + conn.DataSource;
            wDatabaseError.tbDatabaseErrorDetail.Text = ex.Source + ": " + ex.Message + "\n";
            if (ex.InnerException != null)
                wDatabaseError.tbDatabaseErrorDetail.Text += ex.InnerException.Source + ": " + ex.InnerException.Message + "\n";
            wDatabaseError.tbDatabaseErrorDetail.Text += ex.StackTrace;

            wDatabaseError.ShowDialog();

        }

        private void lbNursingInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ccNursingIntervention.Content = (FrameworkIntervention)lb.SelectedItem;
        }

        #region Border color change for framework care component, diagnosis and interventions
       
        private void ccFrameworkElement_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = (Brush)this.TryFindResource("FrameworkElementBorderSelected");
        }

        private void ccFrameworkElement_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = (Brush)this.TryFindResource("FrameworkElementBorder");
        }
        #endregion

        private void ccOutcomeType_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag handling
        }

        private void ccActionType_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag handling
        }

        
        private void cbAttachToDiagnosis_Checked(object sender, RoutedEventArgs e)
        {
           
            imgOutcomeType.Visibility = Visibility.Visible;
        
        }

        private void cbAttachToDiagnosis_UnChecked(object sender, RoutedEventArgs e)
        {
            imgOutcomeType.Visibility = Visibility.Collapsed;
        }

        private void lbOutcomeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkOutcomeType selOutcomeType = (FrameworkOutcomeType)lbOutcomeType.SelectedItem;


            string imgFileName = null;

            if (selOutcomeType == null)
                return;

            switch (selOutcomeType.Code)
            {
                case 1: imgFileName = "Outcome Types\\Improved.png"; break;
                case 2: imgFileName = "Outcome Types\\Stabilized.png"; break; 
                case 3: imgFileName = "Outcome Types\\Worsened.png"; break;
            }

            if (imgFileName == null)
                return;

            // Load image
            imgOutcomeType.BeginInit();
            imgOutcomeType.Source = new BitmapImage(new Uri(imgFileName, UriKind.Relative));
            imgOutcomeType.EndInit();


        }

        private void cbAttachToIntervention_Checked(object sender, RoutedEventArgs e)
        {
            imgActionType.Visibility = Visibility.Visible;
        }

        private void cbAttachToIntervention_UnChecked(object sender, RoutedEventArgs e)
        {
            imgActionType.Visibility = Visibility.Collapsed;
        }

    }
}
