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
using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.CCC_Translations;
using System.Windows.Annotations;
using System.Xml;

namespace eNursePHR.userInterfaceLayer
{

    public class LoadEventArgs : EventArgs
    {
        public System.Exception Exception;

        public LoadEventArgs(Exception e)
        {
            Exception = e;
        }
    }

    /// <summary>
    /// Interaction logic for CCCTaxonomyControl.xaml
    /// </summary>
    public partial class CCCTaxonomyControl : UserControl
    {

        private string FilterSearch;
        
        public delegate void LoadEventHandler(object sender,LoadEventArgs e);

        public event LoadEventHandler CCCLoadFail;

        public CCCTaxonomyControl()
        {
            InitializeComponent();
        }
        #region Border color change for framework care component, diagnosis and interventions

        private void ccFrameworkElement_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = (Brush)this.TryFindResource("FrameworkElementBorderSelected");
            (sender as Border).Cursor = Cursors.Hand;
        }

        private void ccFrameworkElement_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = (Brush)this.TryFindResource("FrameworkElementBorder");
            (sender as Border).Cursor = null;
        }
        #endregion

        private void lbCareComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbSearch.Text != "")
                return; // Make sure were not filtering diagnoses while search is active

            ListBox lb = sender as ListBox;

            if (lb.SelectedIndex == -1) // Tom selected item-liste
                return;


            activateNursingDiagnosisFilter();

            activateNursingInterventionFilter();

        }

        private void activateNursingDiagnosisFilter()
        {
            App.s_cccFrameWork.cvDiagnoses.Filter = new Predicate<object>(FilterOutDiagnoses);
            App.s_cccFrameWork.cvDiagnoses.Refresh();
            // Master
            lbNursingDiagnosis.ItemsSource = App.s_cccFrameWork.cvDiagnoses;

            // Detail

            ccNursingDiagnosis.Content = App.s_cccFrameWork.cvDiagnoses.CurrentItem;

        }

        private void activateNursingInterventionFilter()
        {
            App.s_cccFrameWork.cvInterventions.Filter = new Predicate<object>(FilterOutInterventions);
            App.s_cccFrameWork.cvInterventions.Refresh();
            lbNursingInterventions.ItemsSource = App.s_cccFrameWork.cvInterventions;

            // Detail

            // ccNursingDiagnosis.Content = App.cccFrameWork.cvDiagnoses.CurrentItem;

        }

        private bool FilterOutDiagnoses(object item)
        { // Based on example from Beatrize Costa blog, accessed 25 november 2007

            Nursing_Diagnosis nd = item as Nursing_Diagnosis;


            if (nd == null)
                return false;


            int result = nd.ComponentCode.CompareTo(getFilterCareComponentCode(lbCareComponent).ToString());

            if (result == 0) return true;

            return false;


        }

        private bool FilterOutInterventions(object item)
        {
            // Based on example from Beatrize Costa blog, accessed 25 november 2007

            Nursing_Intervention nd = item as Nursing_Intervention;


            if (nd == null)
                return false;

            int result = nd.ComponentCode.CompareTo(getFilterCareComponentCode(lbCareComponent).ToString());

            if (result == 0) return true;

            return false;

        }

        private char getFilterCareComponentCode(ListBox lbCareComponent)
        {
            Care_component cc = lbCareComponent.SelectedItem as Care_component;
            return cc.Code.ToCharArray()[0];
        }


        private void lbOutcomeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OutcomeType selOutcomeType = (OutcomeType)lbOutcomeType.SelectedItem;


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

            tbShowOutcomeConcept.DataContext = selOutcomeType;


        }

        private void lbActionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActionType selActionType = lbActionType.SelectedItem as ActionType;
            // char [] delemiter = {'/'};
            string[] actionTypeSeparate = selActionType.Concept.Split('/');
            if (actionTypeSeparate != null)
            {
                tbConceptActionType.Text = selActionType.Concept;
                tbDefinitionActionType.Text = selActionType.Definition;
                cbActionType.ItemsSource = actionTypeSeparate;
                cbActionType.SelectedIndex = 0;
            }

        }

        private void cbAttachToDiagnosis_Checked(object sender, RoutedEventArgs e)
        {

            spShowOutcome.Visibility = Visibility.Visible;
            //tbShowOutcomeConcept.DataContext = lbOutcomeType.SelectedItem as FrameworkOutcomeType;

        }

        private void cbAttachToDiagnosis_UnChecked(object sender, RoutedEventArgs e)
        {
            spShowOutcome.Visibility = Visibility.Collapsed;
        }

        private void tbReasonDiagnosis_TextChanged(object sender, TextChangedEventArgs e)
        {
            Nursing_Diagnosis selDiag = lbNursingDiagnosis.SelectedItem as Nursing_Diagnosis;
            if (selDiag == null)
                return;

            selDiag.Comment = tbReasonDiagnosis.Text;

        }

        private void tbReasonIntervention_TextChanged(object sender, TextChangedEventArgs e)
        {
            Nursing_Intervention selInterv = lbNursingInterventions.SelectedItem as Nursing_Intervention;
            if (selInterv == null)
                return;

            selInterv.Comment = tbReasonIntervention.Text;

            
        }

        private void btnAcquireDiagnosticInformation_Click(object sender, RoutedEventArgs e)
        {
            tbReasonDiagnosis.Text += (App.Current.MainWindow as WindowMain).AnnotationControl.getAquiredDiagnosticInfo();

            
             }

        private void btnAcquireInterventionalInformation_Click(object sender, RoutedEventArgs e)
        {
            tbReasonIntervention.Text += (App.Current.MainWindow as WindowMain).AnnotationControl.getAquiredInterventionalInfo();
        }

        #region Nursing Diagnosis handling

        private void lbNursingDiagnosis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ListBox lb = sender as ListBox;
            Nursing_Diagnosis nd = lb.SelectedItem as Nursing_Diagnosis;

            ccNursingDiagnosis.Content = nd; // Update detail view

            tbReasonDiagnosis.Text = String.Empty;

            if (nd != null)
                nd.Comment = String.Empty;
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

            Nursing_Diagnosis selectedNursingDiagnosis = (Nursing_Diagnosis)ccNursingDiagnosis.Content;
            OutcomeType selectedOutcomeType = (OutcomeType)lbOutcomeType.SelectedItem;

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
                txtCode += "(" + selectedNursingDiagnosis.ComponentCode + "." + selectedNursingDiagnosis.MajorCode.ToString("00") + ".";
                if (hasMinorDiagnosis)
                    txtCode += selectedNursingDiagnosis.MinorCode;
                else
                    txtCode += "0";

                if (hasOutcomeAttached)
                    txtCode += "." + selectedOutcomeType.Code.ToString();

                // Diagnosis

                txtDiagnosis = txtCode + ") " + selectedNursingDiagnosis.Concept + "\n" + selectedNursingDiagnosis.Definition + "\n";

                if (hasOutcomeAttached)
                    txtDiagnosis += "Outcome :\n" + "(" + selectedOutcomeType.Code + ") " + selectedOutcomeType.Concept + "\n" + selectedOutcomeType.Definition;

                transfer.SetData(DataFormats.UnicodeText, txtDiagnosis);

                // Let's do a drag n drop

                DragDrop.DoDragDrop(sender as Border, transfer, DragDropEffects.Copy);
            }

        }


        #endregion


        #region Nursing Intervention handling

        private void ccNursingIntervention_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border dragSource = sender as Border;
            string txtIntervention;

            bool hasActionTypeAttached = (bool)cbAttachToIntervention.IsChecked;


            Nursing_Intervention selectedNursingIntervention = (Nursing_Intervention)lbNursingInterventions.SelectedItem;
            ActionType selectedActionType = (ActionType)lbActionType.SelectedItem;

            if (selectedNursingIntervention != null)
            {
                DataObject transfer = new DataObject();
                transfer.SetData("CCC/NursingIntervention", selectedNursingIntervention);

                if (hasActionTypeAttached)
                {
                    selectedActionType.SingleConcept = tbConceptActionType.Text;
                    transfer.SetData("CCC/ActionType", selectedActionType);
                }

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
            Nursing_Intervention nursingIntervention = lb.SelectedItem as Nursing_Intervention;
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

        /// <summary>
        /// Updates UI when a new intervention is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbNursingInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;

            // Get the nursing intervention that is selected
            Nursing_Intervention fi = (Nursing_Intervention)lb.SelectedItem;
            if (fi == null)
                return;

            if ((bool)cbAttachToIntervention.IsChecked)
            {
                // Update action modifier 
                tbNursingInterventionActionModifier.Text = tbConceptActionType.Text;
                tbNursingInterventionConcept.Text = fi.Concept;
            }
            else
            {
                tbNursingInterventionConcept.Text = fi.Concept;
                tbNursingInterventionActionModifier.Text = null;
            }

            tbNursingInterventionConcept.ToolTip = fi.Definition;

            tbReasonIntervention.Text = String.Empty;
        }


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

        private void ccOutcomeType_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag handling
        }

        private void ccActionType_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag handling
        }


        private void cbActionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((bool)cbAttachToIntervention.IsChecked)
                if (cbActionType.SelectedValue != null)
                {
                    tbConceptActionType.Text = cbActionType.SelectedValue.ToString();
                    tbNursingInterventionActionModifier.Text = tbConceptActionType.Text;
                    tbNursingInterventionConcept.Text = ((Nursing_Intervention)(lbNursingInterventions.SelectedItem)).Concept;
                }

        }

        private void cbAttachToIntervention_Checked(object sender, RoutedEventArgs e)
        {
            gcActionType.Width = new GridLength(75);
            tbNursingInterventionActionModifier.Visibility = Visibility.Visible;
            tbConceptActionType.Text = cbActionType.Text;
            tbNursingInterventionActionModifier.Text = cbActionType.Text;
            tbNursingInterventionConcept.Text = ((Nursing_Intervention)(lbNursingInterventions.SelectedItem)).Concept;
        }

        private void cbAttachToIntervention_UnChecked(object sender, RoutedEventArgs e)
        {
            gcActionType.Width = new GridLength(0);
            tbNursingInterventionActionModifier.Visibility = Visibility.Collapsed;
            tbConceptActionType.Text = ((ActionType)(lbActionType.SelectedItem)).Concept;
            tbNursingInterventionActionModifier.Text = null;
            tbNursingInterventionConcept.Text = ((Nursing_Intervention)lbNursingInterventions.SelectedItem).Concept;
        }


        

 
        // Refactored to usercontrol
        private void refreshOutcomeTypes()
        {
            lbOutcomeType.ItemsSource = App.s_cccFrameWork.cvOutcomeTypes;
            ccOutcomeType.Content = App.s_cccFrameWork.cvOutcomeTypes;

        }

        // Refactored to usercontrol
        private void refreshActionTypes()
        {
            lbActionType.ItemsSource = App.s_cccFrameWork.cvActionTypes;
        }

        // Refactored to usercontrol
        private void refreshMetaInformation()
        {
            spCopyright.DataContext = App.s_cccFrameWork;


        }

        public void turnOffUI()
        {
            exFramework.Visibility = Visibility.Collapsed;
            exFramework.Header = "Clinical Care Classification";
            exFramework.IsExpanded = false;
            exFramework.IsEnabled = false;
             
        }


        /// <summary>
        /// Loads the CCC framework and updates user interface
        /// </summary>
        public void loadCCCFramework()
        {
            bool loadFail = false;
            // Load CCC Framework 

            try
            {
                App.s_cccFrameWork = new ViewCCCFrameWork(eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName,
                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);

                // Meta information
                App.s_cccFrameWork.loadMetaInformation(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version, eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                refreshMetaInformation();

                // Care Component and Pattern
                App.s_cccFrameWork.loadCareComponentAndPatternView(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version,
                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                refreshCareComponentAndPattern();

                // Diagnoses
                App.s_cccFrameWork.loadDiagnosesView(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version,
                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                activateNursingDiagnosisFilter();

                // Outcome types
                App.s_cccFrameWork.loadOutcomeTypesView(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version, eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                refreshOutcomeTypes();

                // Interventions
                App.s_cccFrameWork.loadInterventionsView(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version,
                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                activateNursingInterventionFilter();

                // Action types
                App.s_cccFrameWork.loadActionTypesView(eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version,
                                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName);
                refreshActionTypes();

            }
            catch (Exception exception)
            {
                loadFail = true;
                CCCLoadFail(this, new LoadEventArgs(exception));
            }

            if (!loadFail)
                refreshFrameworkLanguageAnalysis();
            else
            {
                exFramework.IsExpanded = false;
                exFramework.IsEnabled = false;
                tbSearch.IsEnabled = false;
            }
        }

        private void pickDefaultCareComponent()
        {
            lbCareComponent.SelectedIndex = 1;

        }

        public void refreshCareComponentAndPattern()
        {

            lbCareComponent.ItemsSource = App.s_cccFrameWork.cvComponents; // Master
            ccCareComponent.Content = App.s_cccFrameWork.cvComponents;     // Detail

            pickDefaultCareComponent();
        }

        public void emptySearchBox()
        {
            lblNoMatchCareComponent.Visibility = Visibility.Collapsed;
            lblNoMatchNursingDiagnoses.Visibility = Visibility.Collapsed;
            lblNoMatchNursingInterventions.Visibility = Visibility.Collapsed;

            spCareComponent.Visibility = Visibility.Visible;
            App.s_cccFrameWork.cvComponents.Filter = null;

            lbCareComponent.SelectedIndex = 1;
                
        }

        /// <summary>
        /// Filters the interventions
        /// </summary>
        private void filterInterventions()
        {
            App.s_cccFrameWork.cvInterventions.Filter = new Predicate<object>(FilterOutInterventionsSearch);
            App.s_cccFrameWork.cvInterventions.Refresh();

            if (App.s_cccFrameWork.cvInterventions.Count == 0)
            {
                lblNoMatchNursingInterventions.Visibility = Visibility.Visible;
                spNursingInterventions.Visibility = Visibility.Hidden;
            }
            else
            {
                lblNoMatchNursingInterventions.Visibility = Visibility.Collapsed;
                spNursingInterventions.Visibility = Visibility.Visible;
            }

            lbNursingInterventions.ItemsSource = App.s_cccFrameWork.cvInterventions;
            lbNursingInterventions.SelectedIndex = 0;

        }

        /// <summary>
        /// Filters diagnoses
        /// </summary>
        private void filterDiagnoses()
        {
            App.s_cccFrameWork.cvDiagnoses.Filter = new Predicate<object>(FilterOutDiagnosesSearch);
            App.s_cccFrameWork.cvDiagnoses.Refresh();

            if (App.s_cccFrameWork.cvDiagnoses.Count == 0)
            {
                lblNoMatchNursingDiagnoses.Visibility = Visibility.Visible;
                spNursingDiagnoses.Visibility = Visibility.Hidden;
            }
            else
            {
                lblNoMatchNursingDiagnoses.Visibility = Visibility.Collapsed;
                spNursingDiagnoses.Visibility = Visibility.Visible;
            }

            lbNursingDiagnosis.ItemsSource = App.s_cccFrameWork.cvDiagnoses;
            lbNursingDiagnosis.SelectedIndex = 0;
        }

        /// <summary>
        /// Filters care components
        /// </summary>
        private void filterCareComponents()
        {
            App.s_cccFrameWork.cvComponents.Filter = new Predicate<object>(FilterOutComponentsSearch);
            App.s_cccFrameWork.cvComponents.Refresh();
            if (App.s_cccFrameWork.cvComponents.Count == 0)
            {
                lblNoMatchCareComponent.Visibility = Visibility.Visible;
                spCareComponent.Visibility = Visibility.Hidden;

            }
            else
            {
                lblNoMatchCareComponent.Visibility = Visibility.Collapsed;
                spCareComponent.Visibility = Visibility.Visible;
            }
        }

        private bool FilterOutDiagnosesSearch(object item)
        { // Based on example from Beatrize Costa blog, accessed 25 november 2007

            Nursing_Diagnosis nd = item as Nursing_Diagnosis;

            if (nd == null)
                return false;

            if (nd.Concept.ToLower().Contains(FilterSearch.ToLower()) || nd.Definition.ToLower().Contains(FilterSearch.ToLower()))
                return true;
            else
                return false;



        }

        private bool FilterOutInterventionsSearch(object item)
        {// Based on example from Beatrize Costa blog, accessed 25 november 2007

            Nursing_Intervention nd = item as Nursing_Intervention;

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


         public void refreshFrameworkLanguageAnalysis()
        {
            // Load language analysis information
            App.s_cccFrameWork.loadFrameworkLanguageAnalysis();
            // Bind to UI
            cbLanguage.ItemsSource = App.s_cccFrameWork.ActualLanguageAnalysisResult;
            
            if (App.s_cccFrameWork.ActualLanguageAnalysisResult != null)
                cbLanguage.ToolTip = "Last language integrity check was run on " +
                    App.s_cccFrameWork.ActualLanguageAnalysisResult[0].Date.ToString();

            // Choose last saved setting language as the selected language
            for (int i = 0; i < App.s_cccFrameWork.ActualLanguageAnalysisResult.Count; i++)
            {
                if (App.s_cccFrameWork.ActualLanguageAnalysisResult[i].Language_Name == 
                    eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName)
                {
                    cbLanguage.SelectedItem = App.s_cccFrameWork.ActualLanguageAnalysisResult[i];
                    break;
                }
            }

           
           
        }

        /// <summary>
        /// Handles new selection of language and loads CCC framework based on the new selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkActual selItem = (FrameworkActual)(sender as ComboBox).SelectedItem;
            
            if (selItem == null)
                return;

            if (selItem.Language_Name == Properties.Settings.Default.LanguageName)
                return;

            if (App.s_cccFrameWork.LogoURL == null)
                imgLogo.Visibility = Visibility.Collapsed;
            else
                imgLogo.Visibility = Visibility.Visible;

            // Save the new langaugename in properties
            Properties.Settings.Default.LanguageName = selItem.Language_Name.Trim();
            Properties.Settings.Default.Save();

            // Recreates new CCC framework for given language name and version
            loadCCCFramework();

            
            (App.Current.MainWindow as  WindowMain).refreshLanguageTranslationForItemTags();

            (App.Current.MainWindow  as WindowMain).buildTagsOverview(App.s_carePlan.ActiveCarePlan);
     

            
        }

        
        #region Explorer search

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (App.s_cccFrameWork == null)
            {
                MessageBox.Show("Cannot search in the clinical care classification, because database is not loaded", "Cannot search", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            FilterSearch = tb.Text;
            if (FilterSearch == "")
            {
                emptySearchBox();
                return;
            }

            //   spCareComponent.Visibility = Visibility.Collapsed;


            filterCareComponents();

            filterDiagnoses();

            filterInterventions();
        }

        #endregion

       
        


    }
}
