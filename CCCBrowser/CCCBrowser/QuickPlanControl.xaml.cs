#define SKIP_ACTIONTYPE_TRANSLATION // Skip translation of action type assoc. with intervention

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
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Windows.Controls.DataVisualization.Charting;
using DrWPF.Windows.Data; // Thanks for this Dr. WPF, removed serialization
using System.Collections.Specialized;

namespace CCCBrowser
{

    public partial class QuickPlanControl : UserControl
    {
        public ObservableCollectionWithLanguage<QuickPlanDiagnosis> QuickPlanDiagnoses = new ObservableCollectionWithLanguage<QuickPlanDiagnosis>();
        public ObservableCollectionWithLanguage<QuickPlanIntervention> QuickPlanInterventions = new ObservableCollectionWithLanguage<QuickPlanIntervention>();

        private IsolatedStorageFile clientFS = IsolatedStorageFile.GetUserStoreForApplication();

       
        string quickFile = System.IO.Path.Combine("CCCBrowser", "quickplan.xml");

        private CCCXMLAPI xAPI = new CCCXMLAPI();

        // Statistics 

        private ObservableDictionary<string, TaskMinutes> TaskTimePrComponent = new ObservableDictionary<string, TaskMinutes>();
        private ObservableDictionary<string, int> ComponentIntervention = new ObservableDictionary<string, int>();

        private string Language;
        private string Version;

        public QuickPlanControl()
        {
            InitializeComponent();

            dgDiagnoses.ItemsSource = QuickPlanDiagnoses;
            dgInterventions.ItemsSource = QuickPlanInterventions;

            // Statistics
            QuickPlanDiagnoses.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(QuickPlanDiagnoses_CollectionChanged);
            QuickPlanInterventions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(QuickPlanInterventions_CollectionChanged);
           
      
        }

        /// <summary>
        /// Handler for changes to interventions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QuickPlanInterventions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            QuickPlanIntervention qi;

            
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                qi = e.NewItems[0] as QuickPlanIntervention;
                if (qi.Tag.MajorCode == -1)
                    return;

                calculateInterventionTaskTimeVsComponents(qi, qi.Task.Hours * 60 + qi.Task.Minutes);

                if (ComponentIntervention.ContainsKey(qi.Tag.ComponentName))
                    ComponentIntervention[qi.Tag.ComponentName]++;
                else 
                {
                    if (ComponentIntervention.Count == 0)
                        chartInterventionComponents.Visibility = Visibility.Visible;

                    ComponentIntervention.Add(qi.Tag.ComponentName, 1);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                qi = e.OldItems[0] as QuickPlanIntervention;
                
                if (qi.Tag.MajorCode == -1)
                    return;

                calculateInterventionTaskTimeVsComponents(qi, -(qi.Task.Hours * 60 + qi.Task.Minutes));

                
                if (ComponentIntervention.ContainsKey(qi.Tag.ComponentName))
                    ComponentIntervention[qi.Tag.ComponentName]--;

                if (ComponentIntervention[qi.Tag.ComponentName] == 0)
                    ComponentIntervention.Remove(qi.Tag.ComponentName);

                if (ComponentIntervention.Count == 0)
                    chartInterventionComponents.Visibility = Visibility.Collapsed;
               
               
            }
            //calculateInterventionsVsComponents();
          
            //calculateInterventionTaskTimeVsComponents();

        }


        private void calculateInterventionTaskTimeVsComponents(QuickPlanIntervention parentQI,double deltaMinutes)
        {
           
            if (parentQI.Tag.MajorCode == -1) // SKIP free form interventions
                return;

            if (TaskTimePrComponent.ContainsKey(parentQI.Tag.ComponentName))
            {
                TaskTimePrComponent[parentQI.Tag.ComponentName].Minutes += deltaMinutes;
                if (TaskTimePrComponent[parentQI.Tag.ComponentName].Minutes == 0)
                    TaskTimePrComponent.Remove(parentQI.Tag.ComponentName);
            }
            else
                TaskTimePrComponent.Add(parentQI.Tag.ComponentName, new TaskMinutes(deltaMinutes));

          
            if (TaskTimePrComponent.Count == 0)
                chartInterventionComponents.Visibility = Visibility.Collapsed;
            else
                chartInterventionComponents.Visibility = Visibility.Visible;

        }


        private void calculateInterventionTaskTimeVsComponents()
        {
            TaskTimePrComponent.Clear();

            foreach (QuickPlanIntervention qi in QuickPlanInterventions.Where(i => i.Tag.MajorCode != -1).OrderBy(i => i.Tag.ComponentCode))
            {
                if (!TaskTimePrComponent.ContainsKey(qi.Tag.ComponentName))
                    TaskTimePrComponent.Add(qi.Tag.ComponentName, new TaskMinutes(qi.Task.Hours * 60 + qi.Task.Minutes));
                else
                {
                    double prevTaskTime = TaskTimePrComponent[qi.Tag.ComponentName].Minutes;
                    TaskTimePrComponent[qi.Tag.ComponentName].Minutes = prevTaskTime + qi.Task.Hours * 60 + qi.Task.Minutes;
                }
            }
            
            
            if (TaskTimePrComponent.Count == 0)
                    chartInterventionComponents.Visibility = Visibility.Collapsed;
                else
                    chartInterventionComponents.Visibility = Visibility.Visible;
      
            
        }


        
        /// <summary>
        /// Calculate number of interventions vs. components
        /// </summary>
        private void calculateInterventionsVsComponents()
        {
            ComponentIntervention.Clear();

            foreach (QuickPlanIntervention qi in QuickPlanInterventions.Where(i=>i.Tag.MajorCode != -1).OrderBy(i => i.Tag.ComponentCode))

                if (!ComponentIntervention.ContainsKey(qi.Tag.ComponentName))
                    ComponentIntervention.Add(qi.Tag.ComponentName, 1);
                else
                    ComponentIntervention[qi.Tag.ComponentName] = ++ComponentIntervention[qi.Tag.ComponentName];

            if (ComponentIntervention.Count == 0)
                chartInterventionComponents.Visibility = Visibility.Collapsed;
            else
                chartInterventionComponents.Visibility = Visibility.Visible;
        }

        void QuickPlanDiagnoses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            calculateDiagnosesVsComponents();
        }

        /// <summary>
        /// Calculate number of diagnoses vs. components
        /// </summary>
        private void calculateDiagnosesVsComponents()
        {
            Dictionary<string, int> ComponentDiagnosis = new Dictionary<string, int>();

            foreach (QuickPlanDiagnosis qd in QuickPlanDiagnoses.Where(d=> d.Tag.MajorCode != -1).OrderBy(i => i.Tag.ComponentCode))
                if (!ComponentDiagnosis.ContainsKey(qd.Tag.ComponentName))
                    ComponentDiagnosis.Add(qd.Tag.ComponentName, 1);
                else
                    ComponentDiagnosis[qd.Tag.ComponentName] = ++ComponentDiagnosis[qd.Tag.ComponentName];


            chartDiagnosisComponents.DataContext = ComponentDiagnosis;
            if (ComponentDiagnosis.Count == 0)
                chartDiagnosisComponents.Visibility = Visibility.Collapsed;
            else
                chartDiagnosisComponents.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Handle language change, update date columns with correct format for language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LanguageChange(object sender, LanguageChangeArgs e)
        {
            Version = e.Version;
            Language = e.Language;
            
            dgDiagnoses.Language = System.Windows.Markup.XmlLanguage.GetLanguage(e.Language);
            dgInterventions.Language = System.Windows.Markup.XmlLanguage.GetLanguage(e.Language);

            // Give message to databinding engine to update Date-columns with new language format
            foreach (QuickPlanDiagnosis d in QuickPlanDiagnoses)
                d.NotifyPropertyChanged("Date");

            foreach (QuickPlanIntervention i in QuickPlanInterventions)
                i.NotifyPropertyChanged("Date");

            calculateDiagnosesVsComponents();
            calculateInterventionsVsComponents();
            calculateInterventionTaskTimeVsComponents();
        }

        /// <summary>
        /// Remove diagnosis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            Dispatcher.BeginInvoke(delegate()
            {
                QuickPlanDiagnoses.Remove(dgDiagnoses.SelectedItem as QuickPlanDiagnosis);
            });
        }

        /// <summary>
        /// Remove intervention 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteInterv_Click(object sender, RoutedEventArgs e)
        {

            Dispatcher.BeginInvoke(delegate()
            {
                QuickPlanInterventions.Remove(dgInterventions.SelectedItem as QuickPlanIntervention);
            });
        }

        // Reference document : http://msdn.microsoft.com/en-us/library/system.io.isolatedstorage.isolatedstoragefile(VS.95).aspx

        /// <summary>
        /// Save plan as xml to isolated storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            
            string[] filenames = clientFS.GetFileNames();
            string[] dir = clientFS.GetDirectoryNames();


            XDocument xDocument = new XDocument();
            XElement xRoot = new XElement(XName.Get("{classification.edu}Plan"));

            // Add XML plan

            QuickPlanDiagnoses.AddXml(ref xRoot);
            QuickPlanInterventions.AddXml(ref xRoot);

            xDocument.Add(xRoot);

            // Store plan to isolated storage
            long free = clientFS.AvailableFreeSpace;
            
            // Check for CCCBrowser directory

            string dName = System.IO.Path.GetDirectoryName(quickFile);

            if (!clientFS.DirectoryExists(dName))
                clientFS.CreateDirectory(dName);

            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(quickFile, FileMode.Create, clientFS);

            xDocument.Save(stream);
            stream.Flush();
            stream.Close();
            stream.Dispose();

         
            //try {
            //    isf.DeleteFile(quickFile);
            //}
            //catch (System.IO.IsolatedStorage.IsolatedStorageException isException)
            //{
            //}
            //finally {
            //    isf.CreateFile(quickFile);
            //}

        }

        /// <summary>
        /// Loads xml plan from isolated storage
        /// </summary>
        public void loadPlanA()
        {
            // load plan for current language and version

            IsolatedStorageFileStream stream;


            try
            {
                stream = new IsolatedStorageFileStream(quickFile, FileMode.Open, clientFS);
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException ex)
            {
                return;
            }
            
            XDocument xDocument = XDocument.Load(stream);

            
            // Process diagnoses
            if (QuickPlanDiagnoses.Count > 0)
             QuickPlanDiagnoses.Clear();

            var xDiagnosesInPlan = xDocument.Descendants("{classification.edu}Diagnosis");

            foreach (XElement diag in xDiagnosesInPlan)
            {
                Diagnosis newTag;

                string component = diag.Attribute("ComponentCode").Value;
                int mc = Convert.ToInt32(diag.Attribute("MajorCode").Value);
               int mi = Convert.ToInt32(diag.Attribute("MinorCode").Value);
                string v = diag.Attribute("Version").Value;
                string l = diag.Attribute("Language").Value;
              
                
                if (mc == -1) // Support free form diagnosis
                {
                    newTag = new Diagnosis();
              
                    newTag.Version = v;
                    newTag.Language = l;
                    newTag.ComponentCode = component;
                    newTag.ComponentName = xAPI.getComponentNameV2(component, l, v);
                    newTag.MajorCode = mc;
                    newTag.MinorCode = mi;
                    newTag.Concept = diag.Attribute("Concept").Value;
                } else
                    newTag = xAPI.getDiagnosisV2(component, mc, mi, l, v);

               
                OutcomeType outcome = null;
                XElement xOutcome = diag.Element(XName.Get("{classification.edu}OutcomeType"));
               
                if (xOutcome != null) // Load outcome
                {
                    string code = xOutcome.Attribute("Code").Value;
                    string concept = xOutcome.Attribute("Concept").Value;
                    string language = xOutcome.Attribute("Language").Value;
                    string version = xOutcome.Attribute("Version").Value;

                    outcome = new OutcomeType(code,concept,language,version);
                }

                newTag.Outcome = outcome;

                QuickPlanDiagnosis qpd =
                    new QuickPlanDiagnosis(DateTime.Parse(diag.Attribute("Date").Value), newTag);
                
                // Take care of description of diagnosis
                if (diag.Value != null)
                    qpd.Description = diag.Value;

                QuickPlanDiagnoses.Add(qpd);
            }
                // Process interventions
            
                if (QuickPlanInterventions.Count > 0)
                 QuickPlanInterventions.Clear();


            var xInterventionsInPlan = xDocument.Descendants("{classification.edu}Intervention");

            foreach (XElement interv in xInterventionsInPlan)
            {

                Intervention newTag = new Intervention();
                   
                string component = interv.Attribute("ComponentCode").Value;
               int mc = Convert.ToInt32(interv.Attribute("MajorCode").Value);
                int mi = Convert.ToInt32(interv.Attribute("MinorCode").Value);
                string l = interv.Attribute("Language").Value;
                string v = interv.Attribute("Version").Value;
                string concept = interv.Attribute("Concept").Value;

                    newTag.Version = v;
                    newTag.Language = l;
                    newTag.ComponentCode = component;
                    newTag.ComponentName = xAPI.getComponentNameV2(component, l, v);
                    newTag.Concept = concept;
                    newTag.MajorCode = mc;
                    newTag.MinorCode = mi;
                

                ActionType actionT = null;
                XElement xActionType = interv.Element(XName.Get("{classification.edu}ActionType"));

                if (xActionType != null) // Load action type
                {
                    string code = xActionType.Attribute("Code").Value;
                    string conceptA = xActionType.Attribute("Concept").Value;
                    string language = xActionType.Attribute("Language").Value;
                    string version = xActionType.Attribute("Version").Value;

                    actionT = new ActionType(code, conceptA, language, version);
                }

                newTag.ActionType = actionT;

               

                QuickPlanIntervention qpi = null;

                qpi = new QuickPlanIntervention(DateTime.Parse(interv.Attribute("Date").Value), newTag);

                qpi.Task.CalculateTaskTime += new Task.CalculateTaskTimeHandler(Task_CalculateTaskTime);
                if (interv.Value != null)
                    qpi.Description = interv.Value;

                QuickPlanInterventions.Add(qpi);

            
            }



        }

        void Task_CalculateTaskTime(QuickPlanIntervention parent, double minutesDelta)
        {
            calculateInterventionTaskTimeVsComponents(parent,minutesDelta);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            (App.Current.RootVisual as Page).CCCBrowserControl.LanguageChange += new CCCBrowserControl.LanguageChangeHandler(this.QuickPlanDiagnoses.LanguageChange);
            (App.Current.RootVisual as Page).CCCBrowserControl.LanguageChange += new CCCBrowserControl.LanguageChangeHandler(this.QuickPlanInterventions.LanguageChange);
            (App.Current.RootVisual as Page).CCCBrowserControl.LanguageChange += new CCCBrowserControl.LanguageChangeHandler(this.LanguageChange);
            
            
            
            (App.Current.RootVisual as Page).CCCBrowserControl.OnDiagnosisCreated += new CCCBrowserControl.DiagnosisCreatedHandler(CCCBrowserControl_DiagnosisCreated);
            (App.Current.RootVisual as Page).CCCBrowserControl.OnInterventionCreated += new CCCBrowserControl.InterventionCreatedHandler(CCCBrowserControl_InterventionCreated);

            // Statistics
            chartTaskTimeVsComponents.DataContext = TaskTimePrComponent;
            chartInterventionComponents.DataContext = ComponentIntervention;
            
            
            //loadPlan();
        }

        void CCCBrowserControl_InterventionCreated(object sender, Intervention intervention)
        {

            QuickPlanIntervention qpi = new QuickPlanIntervention(DateTime.Now, intervention);
            qpi.Task.CalculateTaskTime += new Task.CalculateTaskTimeHandler(Task_CalculateTaskTime);

            QuickPlanInterventions.Add(qpi);
            dgInterventions.ScrollIntoView(qpi, columnIntervention);
            tbiInterventions.IsSelected = true;
        
        }

        void CCCBrowserControl_DiagnosisCreated(object sender, Diagnosis diagnosis)
        {

            QuickPlanDiagnosis qpd = new QuickPlanDiagnosis(DateTime.Now, diagnosis);
            
            QuickPlanDiagnoses.Add(qpd);
            dgDiagnoses.ScrollIntoView(qpd, columnDiagnosis);

            tbiDiagnoses.IsSelected = true;

      
        }

        private void btnDiagnosis_Click(object sender, RoutedEventArgs e)
        {
          
            Diagnosis newTag = new Diagnosis();
         
            newTag.ComponentCode = "*";
            newTag.ComponentName = "User defined";

            newTag.MajorCode = -1;
            newTag.MinorCode = -1;
            newTag.Language = Language;
            newTag.Version = Version;
            newTag.Concept = String.Empty;

            QuickPlanDiagnosis qpd = new QuickPlanDiagnosis(DateTime.Now, newTag);

            QuickPlanDiagnoses.Add(qpd);

            tbiDiagnoses.IsSelected = true;

            dgDiagnoses.ScrollIntoView(qpd, this.columnDiagnosis);

        }

        private void btnIntervention_Click(object sender, RoutedEventArgs e)
        {
            Intervention newTag = new Intervention();
            newTag.ComponentCode = "*";
            newTag.ComponentName = "User Defined";

            newTag.MajorCode = -1;
            newTag.MinorCode = -1;
            newTag.Language = Language;
            newTag.Version = Version;
            newTag.Concept = String.Empty;

            QuickPlanIntervention qi = new QuickPlanIntervention(DateTime.Now, newTag);

            QuickPlanInterventions.Add(qi);

            tbiInterventions.IsSelected = true;

            dgInterventions.ScrollIntoView(qi, this.columnIntervention);
        }

    }

    public class ObservableCollectionWithLanguage<T> : ObservableCollection<T>
    {

        private CCCXMLAPI xAPI = new CCCXMLAPI();

        /// <summary>
        /// Generates XML for diagnoses and interventions
        /// </summary>
        /// <param name="xRoot"></param>
        public void AddXml(ref XElement xRoot)
        {
          
            foreach (T d in this.Items)
            {
                string tName = d.GetType().Name;
                switch (tName)
                {
                    case "QuickPlanDiagnosis": XElement xDiagnosis = new XElement(XName.Get("{classification.edu}Diagnosis"),
                    new XAttribute("Date", (d as QuickPlanDiagnosis).Date.ToString()),
                    new XAttribute("ComponentCode", (d as QuickPlanDiagnosis).Tag.ComponentCode),
                    new XAttribute("MajorCode", (d as QuickPlanDiagnosis).Tag.MajorCode),
                    new XAttribute("MinorCode", (d as QuickPlanDiagnosis).Tag.MinorCode),
                    new XAttribute("Version", (d as QuickPlanDiagnosis).Tag.Version),
                    new XAttribute("Language", (d as QuickPlanDiagnosis).Tag.Language),
                        new XAttribute("Concept", (d as QuickPlanDiagnosis).Tag.Concept));
                        if ((d as QuickPlanDiagnosis).Description != null)
                            xDiagnosis.SetValue((d as QuickPlanDiagnosis).Description);


                        OutcomeType outcome = (d as QuickPlanDiagnosis).Tag.Outcome;

                        if (outcome != null)
                        {
                            // Add Outcome Xml
                            XElement xOutcome = new XElement(XName.Get("{classification.edu}OutcomeType"),
                                 new XAttribute("Code", (d as QuickPlanDiagnosis).Tag.Outcome.Code),
                                 new XAttribute("Concept", (d as QuickPlanDiagnosis).Tag.Outcome.Concept),
                                 new XAttribute("Language",(d as QuickPlanDiagnosis).Tag.Outcome.Language),
                                 new XAttribute("Version", (d as QuickPlanDiagnosis).Tag.Outcome.Version));

                            xDiagnosis.Add(xOutcome);
                        }

                        xRoot.Add(xDiagnosis);
                        break;

                    case "QuickPlanIntervention": XElement xIntervention = new XElement(XName.Get("{classification.edu}Intervention"),
                    new XAttribute("Date", (d as QuickPlanIntervention).Date.ToString()),
                    new XAttribute("ComponentCode", (d as QuickPlanIntervention).Tag.ComponentCode),
                    new XAttribute("MajorCode", (d as QuickPlanIntervention).Tag.MajorCode),
                    new XAttribute("MinorCode", (d as QuickPlanIntervention).Tag.MinorCode),
                    new XAttribute("Version", (d as QuickPlanIntervention).Tag.Version),
                    new XAttribute("Language", (d as QuickPlanIntervention).Tag.Language),
                    new XAttribute("Concept", (d as QuickPlanIntervention).Tag.Concept));
                        if ((d as QuickPlanIntervention).Description != null)
                            xIntervention.SetValue((d as QuickPlanIntervention).Description);

                      

                        ActionType actionT = (d as QuickPlanIntervention).Tag.ActionType;

                        if (actionT != null)
                        {
                            // Add action type Xml
                            XElement xActionType = new XElement(XName.Get("{classification.edu}ActionType"),
                                 new XAttribute("Code", (d as QuickPlanIntervention).Tag.ActionType.Code),
                                 new XAttribute("Concept", (d as QuickPlanIntervention).Tag.ActionType.Concept),
                                 new XAttribute("Language", (d as QuickPlanIntervention).Tag.ActionType.Language),
                                 new XAttribute("Version", (d as QuickPlanIntervention).Tag.ActionType.Version));

                            xIntervention.Add(xActionType);
                        }

                        xRoot.Add(xIntervention);
                        break;

                }
            }
        }

        /// <summary>
        /// Event handler for language change event from CCC browser
        /// Goes through the whole list of codes and translates them into the new language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LanguageChange(object sender, LanguageChangeArgs e)
        {
            foreach (T t in Items)
            {
                string tName = t.GetType().Name;

                
                switch (tName)
                {

                    case "QuickPlanDiagnosis": QuickPlanDiagnosis current = t as QuickPlanDiagnosis;
                        if (current.Tag.MajorCode == -1) // Skip free form diagnosis
                            break;
                        Diagnosis translated = xAPI.getDiagnosisV2(current.Tag.ComponentCode, current.Tag.MajorCode, current.Tag.MinorCode, e.Language, e.Version);

                        OutcomeType translatedOutcome = null;

                        if (current.Tag.Outcome != null && current.Tag.Outcome.Code != "*")
                            translatedOutcome = xAPI.getOutcomeTypeV2(current.Tag.Outcome.Code, e.Language, e.Version);
                        

                        // TO DO : give error if translated outcome not found

                        if (translated == null) // Give user information if translation fails (one cause is version incompatibility of code/terminology)
                        {
                        //  Run silent here....problem with executionEngineException some times from inside... MessageBox.Show   
                        //    try
                        //    {
                        //        MessageBox.Show("Translation of " + current.Tag.Concept + " (v." + current.Tag.Version + ") to " + e.Language + " v" + e.Version + " failed!",
                        //         "Translation fail", MessageBoxButton.OK);
                        //    }
                        //    catch (System.ExecutionEngineException e2) // Have trouble some times with this here....
                        //    {
                        //    }
                        }
                        else
                        {
                            current.Tag.Concept = translated.Concept;
                            current.Tag.Definition = translated.Definition;
                            string name = xAPI.getComponentNameV2(current.Tag.ComponentCode, e.Language, e.Version);

                            current.Tag.ComponentName = name;
                            if (translatedOutcome != null) {
                                current.Tag.Outcome.Concept = translatedOutcome.Concept;
                                current.Tag.Outcome.Definition = translatedOutcome.Definition;
                                current.Tag.Outcome.Language = translatedOutcome.Language;
                                current.Tag.Outcome.Version = translatedOutcome.Version;
                                }
                         }
                        break;


                    case "QuickPlanIntervention": QuickPlanIntervention currentInterv = t as QuickPlanIntervention;
                        if (currentInterv.Tag.MajorCode == -1) // Skip free form intervention
                            break;
                        Intervention translatedInterv = xAPI.getInterventionV2(currentInterv.Tag.ComponentCode, currentInterv.Tag.MajorCode, currentInterv.Tag.MinorCode, e.Language, e.Version);

                        ActionType translatedActionType = null;

                        if (currentInterv.Tag.ActionType != null && currentInterv.Tag.ActionType.Code != "*")
                            translatedActionType = xAPI.getActionTypeV2(currentInterv.Tag.ActionType.Code, e.Language, e.Version);
                        
                        if (translatedInterv == null) // Give user information if translation fails (one cause is version incompatibility of code/terminology)
                        {
                            //MessageBox.Show("Translation of " + currentInterv.Tag.Concept + " (v." + currentInterv.Tag.Version + ") to " + e.Language + " v" + e.Version + " failed!",
                            // "Translation fail", MessageBoxButton.OK);
                            // Have trouble here sometimes with executionengine stop...it seems to occur at quite random times...
                            // I believe it is inside the MessageBox that error is....
                            break;
                        }
                        currentInterv.Tag.Concept = translatedInterv.Concept;
                        currentInterv.Tag.Definition = translatedInterv.Definition;
                        string nameIC = xAPI.getComponentNameV2(currentInterv.Tag.ComponentCode, e.Language, e.Version);

                        currentInterv.Tag.ComponentName = nameIC;

#if !SKIP_ACTIONTYPE_TRANSLATION

                        if (translatedActionType != null)
                        {
                            currentInterv.ActionType.Concept = translatedActionType.Concept;
                            currentInterv.ActionType.Definition = translatedActionType.Definition;
                            currentInterv.ActionType.Language = translatedActionType.Language;
                            currentInterv.ActionType.Version = translatedActionType.Version;
                        }
#endif

                        break;
                }
            }

        }
    }

}
