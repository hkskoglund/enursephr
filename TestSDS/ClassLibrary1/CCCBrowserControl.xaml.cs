//#define SILVERLIGHT
#define SDSCLOUD
//#define XMLAPI
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
using System.IO.IsolatedStorage;
using CCCBrowser.Support;
using CCCBrowser.SoapAPI;


namespace CCCBrowser
{
    public partial class CCCBrowserControl : UserControl
    {
       

#if XMLAPI
        // XML api for working for the CCC XML-tree to get diagnoses, interventions, action types, outcomes types
        private CCCXMLAPI xAPI = new CCCXMLAPI();
#endif

#if SDSCLOUD
        private CCCSoapAPI soapAPI = new CCCSoapAPI("yourcloud", "yourpassword"); // CHANGE BEFORE USE
#endif

        // Defines which languages/versions that are supported and reference to flags icon resource files
        ObservableCollection<CCCLanguage> languages = new ObservableCollection<CCCLanguage>();

#if SILVERLIGHT
        // Stores default language and version 
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
#endif
        private IEnumerable<Pattern> Patterns;
        public int ActivePattern { get; set; }

        // True if this diagnosis/intervention has an outcome attached
        private bool? AttachOutcomeToDiagnosis { get; set; }
        private bool? AttachActionTypeToIntervention { get; set; }

      

        #region Events
        // Allow clients to get a new intervention or diagnosis
        public delegate void DiagnosisCreatedHandler(object sender, Diagnosis diagnosis);
        public event DiagnosisCreatedHandler OnDiagnosisCreated;

        public delegate void InterventionCreatedHandler(object sender, Intervention intervention);
        public event InterventionCreatedHandler OnInterventionCreated;

        public delegate void ComponentChangedHandler(object sender, Component component);
        public event ComponentChangedHandler OnComponentChanged;


        // Allow clients to update language
        public delegate void LanguageChangeHandler(object sender, LanguageChangeArgs e);
        public event LanguageChangeHandler LanguageChange;
        #endregion

        public CCCBrowserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Propagate language selection
            this.LanguageChange += new LanguageChangeHandler(InterventionLanguageChange);
            this.LanguageChange += new LanguageChangeHandler(DiagnosisLanguageChange);
            this.LanguageChange += new LanguageChangeHandler(OutcomesLanguageChange);
            this.LanguageChange += new LanguageChangeHandler(ActionTypesLanguageChange);
            
            languages.Add(new CCCLanguage("nb-NO", "Norwegian", "2.0", "Flags/nb-NO.jpg"));
            languages.Add(new CCCLanguage("en-US", "English", "2.0", "Flags/en-US.jpg"));
            languages.Add(new CCCLanguage("ko-KR", "Korean", "2.0", "Flags/ko-KR.jpg"));
            languages.Add(new CCCLanguage("tr-TR", "Turkish", "2.0", "Flags/tr-TR.jpg"));
            languages.Add(new CCCLanguage("pt-BR", "Brazil (Portugese)", "1.0", "Flags/pt-BR.jpg"));
            languages.Add(new CCCLanguage("de-DE", "German", "1.0", "Flags/de-DE.jpg"));
            languages.Add(new CCCLanguage("sl-SI", "Slovenia", "1.0", "Flags/sl-SI.jpg"));
            languages.Add(new CCCLanguage("es-AR", "Argentina", "1.0", "Flags/es-AR.jpg"));

            cbLanguage.ItemsSource = languages;

            lbCareComponents.SelectionChanged += new SelectionChangedEventHandler(DiagnosislbCareComponents_SelectionChanged);
            lbCareComponents.SelectionChanged += new SelectionChangedEventHandler(InterventionlbCareComponents_SelectionChanged);

            setLanguage();

            setupMetaInformation();

            //soapAPI.readCCCFramework("en-US", "2.0");

        }

        void ActionTypesLanguageChange(object sender, LanguageChangeArgs e)
        {
#if XMLAPI
            cbActionTypes.ItemsSource = xAPI.getActionTypesV2(e.Language,e.Version);
#endif
#if SDSCLOUD
           
            soapAPI.readActionTypes(e.Language, e.Version);
            cbActionTypes.ItemsSource = soapAPI.ActionTypes;
#endif
        }

        void OutcomesLanguageChange(object sender, LanguageChangeArgs e)
        {
#if XMLAPI
            acOutcomes.ItemsSource = xAPI.getOutcomeTypesV2(e.Language, e.Version);
#endif

#if SDSCLOUD
            soapAPI.readOutcomeTypes(e.Language, e.Version);
            acOutcomes.ItemsSource = soapAPI.OutcomeTypes;
#endif
        }
        
        public void turnOffInterventionDiagnosisDetails()
        {
            btnDetailsDiagnosis.Visibility = Visibility.Collapsed;
            btnDetailsIntervention.Visibility = Visibility.Collapsed;

        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageChangeArgs a = new LanguageChangeArgs();


            CCCLanguage selectedLanguage = (sender as ComboBox).SelectedItem as CCCLanguage;

            a.Language = selectedLanguage.Language;
            a.Version = selectedLanguage.Version;

            CareComponentLanguageChange(a.Language, a.Version);
            updatePatterns(a.Language, a.Version);

#if SILVERLIGHT
            // Persist to isolated storage
            try
            {
                userSettings["Language"] = a.Language;
            }
            catch (KeyNotFoundException e2)
            {
                userSettings.Add("Language", a.Language);
            }
            finally
            {
                userSettings["Language"] = a.Language;
                userSettings.Save();
            }


            // Persist to isolated storage
            try
            {
                userSettings["Version"] = a.Version;
            }
            catch (KeyNotFoundException e3)
            {
                userSettings.Add("Version", a.Version);
            }
            finally
            {
                userSettings["Version"] = a.Version;
                userSettings.Save();
            }

#endif
            if (LanguageChange != null) // Check for listeners
                LanguageChange(sender, a);
            // Update page with language specific details
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(a.Language);

            setupMetaInformation();
        }

        /// <summary>
        /// Gets default startup language from isolated storage usersettings
        /// </summary>
        private void setLanguage()
        {


            string startupLanguage;
            string startupVersion;

#if SILVERLIGHT
            try
            {
                startupLanguage = userSettings["Language"] as string; // Fetch for isolated storage
            }
            catch (KeyNotFoundException e)
            {
                startupLanguage = "en-US";
            }

            try
            {
                startupVersion = userSettings["Version"] as string; // Fetch for isolated storage
            }
            catch (KeyNotFoundException e)
            {
                startupVersion = "2.0";
            }

#else
            // TO DO : .NET Property.Settings.Default

            startupLanguage = "en-US";
            startupVersion = "2.0";

            

#endif
            
            updatePatterns(startupLanguage, startupVersion);
            selectLanguage(startupLanguage, startupVersion);

        }

        /// <summary>
        /// Updates cbLanguage combobox with selected language
        /// </summary>
        /// <param name="newLanguage"></param>
        private void selectLanguage(string newLanguage, string newVersion)
        {
            int indx = -1;
            for (int i = 0; i < languages.Count; i++)

                if (languages[i].Language == newLanguage && languages[i].Version == newVersion)
                { indx = i; break; }

            cbLanguage.SelectedIndex = indx;

        }

        private void setupMetaInformation()
        {
            if (cbLanguage == null)
                return;

            CCCLanguage lang = cbLanguage.SelectedItem as CCCLanguage;

            if (lang == null)
                return;

#if XMLAPI
            MetaInformation mi = xAPI.getMetaInformationV2(lang.Language, lang.Version);
#endif

#if SDSCLOUD
             soapAPI.readMetaInformation(lang.Language,lang.Version);
            MetaInformation mi = soapAPI.MetaInformation;
#endif
             spMetaInformation.DataContext = mi;
        }

        /// <summary>
        /// Hurray, my first generic method, find the last selected item in a listbox
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lastSelected"></param>
        /// <param name="listBox"></param>
        public void findLastSelected<T>(T lastSelected, ListBox listBox)
        {
            string tName = typeof(T).Name;

            if (lastSelected == null || listBox.ItemsSource == null)
                return;

            // Find last selected diagnosis
            int indx = -1;

            foreach (T d in listBox.ItemsSource)
            {
                indx++;
                switch (tName)
                {
                    case "Diagnosis":
                        if ((d as Diagnosis).ComponentCode == (lastSelected as Diagnosis).ComponentCode &&
                            (d as Diagnosis).MajorCode == (lastSelected as Diagnosis).MajorCode &&
                            (d as Diagnosis).MinorCode == (lastSelected as Diagnosis).MinorCode)
                        {
                            listBox.SelectedIndex = indx;
                            return;
                        }
                        break;

                    case "CareComponent":
                        if ((d as Component).ComponentCode == (lastSelected as Component).ComponentCode)
                        {
                            listBox.SelectedIndex = indx;
                            return;
                        }
                        break;
                }
            }



        }

        /// <summary>
        /// Reads available patterns from XML api and updates access buttons
        /// </summary>
        /// <param name="language"></param>
        /// <param name="version"></param>
        private void updatePatterns(string language, string version)
        {
#if XMLAPI
            Patterns = xAPI.getCarePatternsV2(language, version);
#endif

#if SDSCLOUD
            soapAPI.readPatterns(language, version);
            Patterns = soapAPI.Patterns;
#endif

            if (Patterns == null)
                return;

            btnPattern1.Tag = 1;
            btnPattern2.Tag = 2;
            btnPattern3.Tag = 3;
            btnPattern4.Tag = 4;

            if (Patterns.Count() < 4)
            {
                MessageBox.Show("Found only " + Patterns.Count() + " patterns for " + language);
                btnPattern1.Content = "Health Behavioral";
                btnPattern2.Content = "Functional";
                btnPattern3.Content = "Physiological";
                btnPattern4.Content = "Psychological";
            }
            else
            {
                btnPattern1.Content = Patterns.ElementAt(0).Concept;
                btnPattern2.Content = Patterns.ElementAt(1).Concept;
                btnPattern3.Content = Patterns.ElementAt(2).Concept;
                btnPattern4.Content = Patterns.ElementAt(3).Concept;
            }

        }

        //public void LanguageChange(object sender, LanguageChangeArgs e)
        //{
        //    updatePatterns(e.Language, e.Version);
        //}

        private void firePatternChanged(object sender, RoutedEventArgs e)
        {
            ActivePattern = (int)(sender as Button).Tag;  // PatternID

            CCCLanguage lang = cbLanguage.SelectedItem as CCCLanguage;
            if (lang == null)
                return;
#if SDSCLOUD
            soapAPI.readComponentsForPattern(ActivePattern,lang.Language, lang.Version);
            lbCareComponents.ItemsSource = soapAPI.Components;
#endif
#if XMLAPI
            lbCareComponents.ItemsSource = xAPI.getCareComponentsV2(

                ActivePattern,
                lang.Language,
                lang.Version);
#endif
            exCareComponent.IsExpanded = true;

            turnOffInterventionDiagnosisDetails();
            gridActionTypes.Visibility = Visibility.Collapsed;
            gridOutcomeTypes.Visibility = Visibility.Collapsed;
       

        }

        private void lbCareComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Component comp = lbCareComponents.SelectedItem as Component;
            if (comp == null)
                return;

            if (OnComponentChanged != null)   // Allow external code to be notified of which component is selected
                OnComponentChanged(this, comp);

            detailsCareComponent.Content = comp; // Update details

            if (detailsCareComponent.Visibility == Visibility.Collapsed) // Make sure details are visible
                detailsCareComponent.Visibility = Visibility.Visible;

            turnOffInterventionDiagnosisDetails(); // Hide previous diagnosis/intervention details

            gridOutcomeTypes.Visibility = Visibility.Collapsed;
            gridActionTypes.Visibility = Visibility.Collapsed;
        }

        public void CareComponentLanguageChange(string language, string version)
        {
            Component comp = lbCareComponents.SelectedItem as Component;
#if SDSCLOUD      
            soapAPI.readComponentsForPattern(ActivePattern,language, version);
            lbCareComponents.ItemsSource = soapAPI.Components;
#endif
#if XMLAPI            
            lbCareComponents.ItemsSource = xAPI.getCareComponentsV2(ActivePattern, language, version);
#endif

            if (comp == null) // No need to update selection if it is empty
                return;

            // Find last selected item
            int indx = -1;
            foreach (Component cc in lbCareComponents.ItemsSource as IEnumerable<Component>)
            {
                indx++;
                if (cc.ComponentCode == comp.ComponentCode)
                {
                    lbCareComponents.SelectedIndex = indx;
                    break;
                }
            }


        }

        private void exCareComponent_Expanded(object sender, RoutedEventArgs e)
        {
            if (lbCareComponents.Items.Count == 0)
                exCareComponent.IsExpanded = false;
        }
        
        private string getComponentSelected()
        {
            if (this.detailsCareComponent.Content == null)
            {
                MessageBox.Show("Please select a component first", "Select component", MessageBoxButton.OK);
                return null;
            }
            
            string componentCode = (this.detailsCareComponent.Content as Component).ComponentCode;
            
            return componentCode;
        }

        /// <summary>
        /// Adds diagnosis to quick plan datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetailsDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            Diagnosis diag = btnDetailsDiagnosis.Content as Diagnosis;
          
            OutcomeType outcome = null;

            if (acOutcomes.SelectedItem as OutcomeType != null)
            {
                OutcomeType selOutcome = acOutcomes.SelectedItem as OutcomeType;
                outcome = new OutcomeType();
                outcome.Code = selOutcome.Code;
                outcome.Concept = acOutcomes.Text;
                outcome.Definition = selOutcome.Definition;
                outcome.Language = selOutcome.Language;
                outcome.Version = selOutcome.Version;
                outcome.PatternId = selOutcome.PatternId;
            }
            
            AttachOutcomeToDiagnosis = chAttachOutcome.IsChecked;
            if ((bool)AttachOutcomeToDiagnosis)
                diag.Outcome = outcome;
            else
                diag.Outcome = null;

            if (OnDiagnosisCreated != null)
                OnDiagnosisCreated(this, diag);
        }

        private void exDiagnosis_Expanded(object sender, RoutedEventArgs e)
        {
            if (lbDiagnoses.Items.Count == 0)    // Refuse to open expander if empty collection
                exDiagnosis.IsExpanded = false;
        }

        private void lbDiagnoses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Diagnosis diagInterv = this.lbDiagnoses.SelectedItem as Diagnosis;
            if (diagInterv == null)
                return;

            btnDetailsDiagnosis.Content = diagInterv;
            btnDetailsDiagnosis.Visibility = Visibility.Visible;
            gridOutcomeTypes.Visibility = Visibility.Visible;

        }

        public void DiagnosisLanguageChange(object sender, LanguageChangeArgs e)
        {
            string componentCode;

            
            Diagnosis selDiagnosis = lbDiagnoses.SelectedItem as Diagnosis;
            if (selDiagnosis == null)
                return;
            else
                componentCode = selDiagnosis.ComponentCode;
#if SDSCLOUD
            soapAPI.readDiagnosesForComponent(componentCode, e.Language, e.Version);
            lbDiagnoses.ItemsSource = soapAPI.Diagnoses;
#endif

#if XMLAPI
            lbDiagnoses.ItemsSource = xAPI.getDiagnosesV2(componentCode, e.Language, e.Version);
#endif
            findLastSelected(selDiagnosis, lbDiagnoses);


        }
        
        /// <summary>
        /// Updates diagnoses according to componentcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DiagnosislbCareComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbCareComponents = sender as ListBox;
            Component selComponent = lbCareComponents.SelectedItem as Component;

            Diagnosis selDiagnosis = lbDiagnoses.SelectedItem as Diagnosis;

            if (selComponent == null)
                return;
#if SDSCLOUD
            soapAPI.readDiagnosesForComponent(selComponent.ComponentCode, selComponent.Language, selComponent.Version);
            lbDiagnoses.ItemsSource = soapAPI.Diagnoses;
#endif

#if XMLAPI
            lbDiagnoses.ItemsSource = xAPI.getDiagnosesV2(selComponent.ComponentCode, selComponent.Language, selComponent.Version);
#endif
            findLastSelected(selDiagnosis, lbDiagnoses);
        }

        private void btnDetailsIntervention_Click(object sender, RoutedEventArgs e)
        {
            Intervention interv = btnDetailsIntervention.Content as Intervention;
            ActionType actionT = null;

            AttachActionTypeToIntervention = chAttachActionType.IsChecked;
            if (!(bool)AttachActionTypeToIntervention || cbActionTypes.SelectedItem == null)
              return;

            ActionType selectedActionType = cbActionTypes.SelectedItem as ActionType;
            // Copy selected action type
            actionT = new ActionType();
            actionT.Code = selectedActionType.Code;
            actionT.Definition = selectedActionType.Definition;
            actionT.Language = selectedActionType.Language;
            actionT.Version = selectedActionType.Version;
            actionT.PatternId = selectedActionType.PatternId;

            if (acActionTypeSynonyms.ItemsSource != null)
                actionT.Concept = acActionTypeSynonyms.Text;
            else
                actionT.Concept = selectedActionType.Concept;

            interv.ActionType = actionT;

            if (OnInterventionCreated != null)
              OnInterventionCreated(this,interv);

        }

        private void lbInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Intervention Interv = lbInterventions.SelectedItem as Intervention;
            if (Interv == null)
                return;

            btnDetailsIntervention.Content = Interv;
            btnDetailsIntervention.Visibility = Visibility.Visible;
            gridActionTypes.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Updates diagnoses according to componentcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InterventionlbCareComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbCareComponents = sender as ListBox;
            Component selComponent = lbCareComponents.SelectedItem as Component;

            Intervention selIntervention = lbInterventions.SelectedItem as Intervention;

            if (selComponent == null)
                return;
#if SDSCLOUD
            soapAPI.readInterventionsForComponent(selComponent.ComponentCode, selComponent.Language, selComponent.Version);
            lbInterventions.ItemsSource = soapAPI.Interventions;
#endif
#if XMLAPI
            lbInterventions.ItemsSource =
                xAPI.getInterventionsV2(selComponent.ComponentCode, selComponent.Language, selComponent.Version);
#endif
            findLastSelected(selIntervention, lbInterventions);

        }

        public void InterventionLanguageChange(object sender, LanguageChangeArgs e)
        {
            string componentCode;

            Intervention selInterv = lbInterventions.SelectedItem as Intervention;
            if (selInterv == null)
                return;
            else
                componentCode = selInterv.ComponentCode;

#if SDSCLOUD
            soapAPI.readInterventionsForComponent(componentCode,e.Language, e.Version);
            lbInterventions.ItemsSource = soapAPI.Interventions;
#endif
#if XMLAPI
          lbInterventions.ItemsSource = xAPI.getInterventionsV2(componentCode, e.Language, e.Version);
#endif
          findLastSelected(selInterv, lbInterventions);

        }

        private void exIntervention_Expanded(object sender, RoutedEventArgs e)
        {
            if (lbInterventions.Items.Count == 0)
                exIntervention.IsExpanded = false;
        }

        private void cbActionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbActionTypes.SelectedItem == null)
                return;

            ActionType actionT = cbActionTypes.SelectedItem as ActionType;

            string[] synonyms = actionT.Concept.Split('/');

            if (synonyms.Count() > 1)
                acActionTypeSynonyms.ItemsSource = synonyms;
            else
                acActionTypeSynonyms.ItemsSource = null;

        }

       

    }
}
