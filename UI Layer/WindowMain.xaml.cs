﻿// Introduced this directive for code targetet at SP 1 release version of SQL COMPACT SERVER
// that unfortunately contains an error in construction of queries with parameters for LINQ
// One way to work around this is to rewrite to queries by using entity SQL instead as suggested
// on request for "query processor error" thread in MSDN forum for SQL Server Compact.

#define  SQL_SERVER_COMPACT_SP1_WORKAROUND


using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using System.Data.Linq;
using System.Data.Objects.DataClasses;
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
using System.Windows.Threading; // DispatcherTimer
using System.ComponentModel;
// Copyright (c) 2007 Kevin Moore j832.com
using Microsoft.Samples.KMoore.WPFSamples.DateControls;
using System.Windows.Annotations;
using System.IO;
using System.Data.Objects;
using System.Xml;
using System.Diagnostics;

using Microsoft.Win32;

using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.CCC_Translations;
using eNursePHR.BusinessLayer.PHR;

// MIT Licence
//
//Copyright (c) 2008 Henning Knut Skoglund

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

// Note: Current application is dependent upon SQL Server Compact


namespace eNursePHR.userInterfaceLayer
{

    public partial class WindowMain : Window
        {

        private string _netVersion = Environment.Version.ToString();
        public string NETVersion 
        {
            get { return _netVersion; }
        }

        public const int EXIT_LOAD_FRAMEWORK = 1;
        public const int EXIT_LOAD_CAREPLAN = 2;

        ObservableCollection<Tag> careplanTags;
        //EntityCollection<Tag> careplanTags = new EntityCollection<Tag>();
        ListCollectionView cvCareplanTags;

      
     //   Dictionary<string, Guid> dictItemBlog;

        public TagHandler tagHandler = new TagHandler();

        ListCollectionView cvItemTags;

        myAnnotationService aService;
        
        public ViewInformationAcqusition infoAcq = new ViewInformationAcqusition();

        public ContentType contentType = new ContentType();

        // Previous width of tag-panel and taxonomy-panel Grid
        GridLength prevTagsWidth = new GridLength(0);
        GridLength prevTaxonomyWidth = new GridLength(0);
        
        // True if item is to be displayed in full screen
        bool FullScreenItem = false;

  
    WindowCopyright wndCopyright;

    Dictionary<string, bool> healthDB; // Holds information about databases and a flag that indicates if problems where detected


    BackgroundWorker bwSave = null;    // Saves BP-data asynchronous/in the background
    BloodPressureChart chartBloodPressure;
    int? systolicBP, diastolicBP, heartRate;
    string comment;
    DateTime time;
    MeasurementUpdateStatus updDB = new MeasurementUpdateStatus();
    Guid cpGuid; // Get test careplan



    private string _SQLCompactVersion;


        /// <summary>
        /// Gets version information from registry for SQL Server Compact
        /// </summary>
        private bool getSQLCompactVersion()
        {
            // Info. resource
            // SQL Server Compact : http://blogs.msdn.com/sqlservercompact/archive/2008/02/08/sql-server-compact-release-versions.aspx
            // Registry editing : http://www.csharphelp.com/archives2/archive430.html
            // Accessed : 10 sept. 2008

            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v3.5\ENU");

            this._SQLCompactVersion = OurKey.GetValue("DesktopRuntimeVersion").ToString();
            
            string spack = OurKey.GetValue("DesktopRuntimeServicePackLevel").ToString();

            // require latest v 3.5 Sp 1
            if (this._SQLCompactVersion == "3.5.5692.0" && spack == "1")
                return true;
            else
                return false;

           
                
        }

        public WindowMain()
    {

        #region Test code


        //CCC_FrameworkEntities ctx = new CCC_FrameworkEntities();
                //string test = ctx.Copyright.Where(c => c.Language_Name == "tr-TR" && c.Version == "2.0").First().Name;

             //   CCC_Framework cf = new CCC_Framework("tr-TR", "2.0");
            
            

#endregion // End test

            // Verify database intallation

        if (!getSQLCompactVersion())
            {
            MessageBox.Show("You have not installed SQL Server Compact V 3.5 SP 1, please install before using application again", "SQL Server Compact v3.5 SP 1 not installed",MessageBoxButton.OK,MessageBoxImage.Stop);
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Current.Shutdown();

            }  else {

                    wndCopyright = new WindowCopyright();
                   healthDB = wndCopyright.checkDatabasesHealth();  // Perform health check on the databases
                    wndCopyright.Show();
                    
                    InitializeComponent();

                    if (healthDB["CCCFramework"])
                    {

                        wndCopyright.tbLoading.Text = "Loading CCC taxonomy...";
                        loadCCCFramework();

                        wndCopyright.pbLoading.Value = 75;
                    }
                    else
                    {
                        exFramework.Visibility = Visibility.Collapsed;
                        exFramework.Header = "Clinical Care Classification";
                        exFramework.IsExpanded = false;
                        exFramework.IsEnabled = false;
                        tbSearch.IsEnabled = false;
                    }
        }
            }

      public void showException(Exception ex)
        {
            WindowDatabaseError wDatabaseError = new WindowDatabaseError();
            //  wDatabaseError.tbDatabaseError.Text = "Cannot access database server : " + conn.DataSource;
            wDatabaseError.tbDatabaseErrorDetail.Text = ex.Source + ": " + ex.Message + "\n";
            if (ex.InnerException != null)
                wDatabaseError.tbDatabaseErrorDetail.Text += ex.InnerException.Source + ": " + ex.InnerException.Message + "\n";
            wDatabaseError.tbDatabaseErrorDetail.Text += ex.StackTrace;

            wDatabaseError.ShowDialog();
          

        }
            
            public void showUpdateException(UpdateException ex)
            {

                WindowDatabaseError wDatabaseError = new WindowDatabaseError();
              //  wDatabaseError.tbDatabaseError.Text = "Cannot access database server : " + conn.DataSource;
                wDatabaseError.tbDatabaseErrorDetail.Text = ex.Source + ": " + ex.Message + "\n";
                if (ex.InnerException != null)
                    wDatabaseError.tbDatabaseErrorDetail.Text += ex.InnerException.Source + ": " + ex.InnerException.Message + "\n";
                wDatabaseError.tbDatabaseErrorDetail.Text += ex.StackTrace;

                wDatabaseError.ShowDialog();

            }

            private void setupBloodPressureUI()
            {
                // Setup controls


                tpTime.SelectedTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 00);
                dpDate.Value = DateTime.Now;


                // Save asynchronous
                bwSave = new BackgroundWorker();
                bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);
                bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);



                chartBloodPressure = new BloodPressureChart("Blood+pressure (mmHg)", 300, 150);

                cpGuid = chartBloodPressure.getCarePlanGuid(); // Get test careplan

                // Setup display options
                chartBloodPressure.ShowDiastolicBP = true;
                chartBloodPressure.ShowSystolicBP = true;
                chartBloodPressure.ShowPulseHR = true;
                chartBloodPressure.ShowLabels = true;

                gbDisplayOptions.DataContext = chartBloodPressure;


                // Read some test data

                chartBloodPressure.readBPdata(cpGuid, 5);
                double? avgSystolicBP = chartBloodPressure.getAverageSystolicBP();
                double? avgDiastolicBP = chartBloodPressure.getAverageDiastolicBP();
                double? avgPulseHR = chartBloodPressure.getAveragePulseHR();
                AverageChart avgSystolicChart = new AverageChart((Brush)App.Current.MainWindow.TryFindResource("colorSystolic"), "Systolic BP Average", 100, 60, 250);
                if (avgSystolicBP.HasValue)
                    avgSystolicChart.Average = avgSystolicBP.Value;

                AverageChart avgDiastolicChart = new AverageChart((Brush)App.Current.MainWindow.TryFindResource("colorDiastolic"), "Diastolic BP Average", 100, 60, 250);
                if (avgDiastolicBP.HasValue)
                    avgDiastolicChart.Average = avgDiastolicBP.Value;

                AverageChart avgPulseChart = new AverageChart((Brush)App.Current.MainWindow.TryFindResource("colorPulse"), "Pulse HR Average", 100, 60, 250);
                if (avgPulseHR.HasValue)
                    avgPulseChart.Average = avgPulseHR.Value;

                showChart(avgSystolicChart, imgSystolicAverage);
                showChart(avgDiastolicChart, imgDiastolicAverage);
                showChart(avgPulseChart, imgPulseAverage);

                chartBloodPressure.generateChart();

                lvBP.ItemsSource = chartBloodPressure.BPData;



                showChart(chartBloodPressure, img);
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {

                setupBloodPressureUI();
                
                /* LANGUAGE attrib change */
                tbUserName.Text = "Logged in as " + System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName;
                //+ " Kultur for UI: " + Thread.CurrentThread.CurrentUICulture.DisplayName.ToString();

                // Enable sharing of properties across layers
              
                App.Current.Properties["Version"] = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
                App.Current.Properties["LanguageName"] = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;

                // Setup event handling of hyperlinks

                fdReaderCareBlog.AddHandler(Hyperlink.RequestNavigateEvent,new RequestNavigateEventHandler(hyperlink_RequestNavigate));

              
               
                // Start annotation service
                 aService = new myAnnotationService(fdReaderCareBlog);
                lvAnnotation.ItemsSource = infoAcq.CvStatement;


                if (healthDB["PHR"])
                {
                    wndCopyright.tbLoading.Text = "Loading test health record....";
                    loadCareplan(Guid.NewGuid(), true); // Load a test careplan
                    wndCopyright.pbLoading.Value = 100;
                    // Show all tags for the current careplan
                    buildAllTags(App.carePlan.ActiveCarePlan);
               }



               


                
                
                // Setup careplan templates sub-UI

                //E CarePlanTemplateDBDataContext templateDB = new CarePlanTemplateDBDataContext();

                //CareplanTemplateEntities templateDB = null;
                //try
                //{
                //    templateDB = new CareplanTemplateEntities();
                //}
                //catch (Exception ex)
                //{
                //}

// ERROR                List<TemplateCarePlan> careplanTemplateList = templateDB.TemplateCarePlan.ToList();

                //List<TemplateCarePlan> careplanTemplateList = null;

                //ListCollectionView cvCarePlanTemplate = new ListCollectionView(careplanTemplateList);

                //cvCarePlanTemplate.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                //cvCarePlanTemplate.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));

                //lbCarePlanTemplates.ItemsSource = cvCarePlanTemplate;
                
               if (healthDB["PHR"] && healthDB["CCCFramework"] && healthDB["CCCReference"])
                   wndCopyright.Close();
               
            }

        public void loadCCCFramework()
        {
            bool loadFail = false;
             // Load CCC Framework 

                try
                {
                    App.cccFrameWork = new ViewCCCFrameWork(eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName,
                        eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
                   
                }
                catch (Exception exception)
                {
                    loadFail = true;
                    showException(exception);
                    
                   // stopApplication("Could not load CCC framework", "CCC framework", WindowMain.EXIT_LOAD_FRAMEWORK);
                  
                }

                if (!loadFail)
                    refreshCCCFramework();
                else
                {
                    exFramework.IsExpanded = false;
                    exFramework.IsEnabled = false;
                    tbSearch.IsEnabled = false;
                }
        }

        private void stopApplication(string errMsg, string errTitle, int exitCode)
        {
            MessageBox.Show(errMsg, errTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
            App.Current.MainWindow.Close();
            System.Environment.Exit(exitCode);
               
        }

            private void loadCareplan(Guid carePlanID, bool testPlan)
            {
                
                try
                {
                    App.carePlan = new ViewCarePlan();
                    if (testPlan)
                        App.carePlan.ActiveCarePlan = App.carePlan.DB.CarePlan.First();
                    else
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                         App.carePlan.ActiveCarePlan = App.carePlan.DB.CarePlan.Where(cp => cp.Id == carePlanID).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                        //SP 1 workaround
                        App.carePlan.ActiveCarePlan = App.carePlan.DB.CarePlan.Where("it.Id = '"+ carePlanID+"'").First();
#endif
                    }
                catch (Exception exception)
                {
                    showException(exception);
                    stopApplication("Could not load health record","Load fail personal health record", WindowMain.EXIT_LOAD_CAREPLAN);

                        }

                // Setup care blog items
                if (!App.carePlan.ActiveCarePlan.Item.IsLoaded)
                    App.carePlan.ActiveCarePlan.Item.Load();

                Item lastItem = null;

                if (App.carePlan.ActiveCarePlan.Item.Count > 0)
                {

                    foreach (Item i in App.carePlan.ActiveCarePlan.Item)
                    {// Load related history and tags
                        i.HistoryReference.Load();
                        i.Tag.Load();
                        i.Tag.AssociationChanged += new CollectionChangeEventHandler(Tag_AssociationChanged);
                        inferContentFromTags(i); // Find content from tags, contains diagnoses, interventions, care component?

                        if (i.Id == eNursePHR.userInterfaceLayer.Properties.Settings.Default.LastItem) // Point to last item selected
                            lastItem = i;
                    }

                    // Setup of item collection change handling (updates combobox)

                    App.carePlan.ActiveCarePlan.Item.AssociationChanged += new CollectionChangeEventHandler(Item_AssociationChanged);
                    Item_AssociationChanged(this, null);

                    // Start at last selected item from previous session by default

                    if (lastItem != null)
                        lvCareBlog.SelectedItem = lastItem;
                }
                //else
                //    MessageBox.Show("This careplan is empty", "Empty careplan", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        
            }


            public void Item_AssociationChanged(object sender, CollectionChangeEventArgs e)
            {
              
                lvCareBlog.ItemsSource = App.carePlan.ActiveCarePlan.Item.OrderByDescending(i => i.History.LastUpdate);
            }

            public void inferContentFromTags(Item i)
            {

                i.ContainsDiagnosis = false;
                i.ContainsIntervention = false;
                i.ContainsCareComponent = false;
                i.ContainsFolksonomy = false;
                
                if (!i.Tag.IsLoaded)
                    i.Tag.Load();


                if (i.Tag.Count == 0)
                    return;

                foreach (Tag t in i.Tag)
               {
                   if (t.TaxonomyType.Contains("CCC/NursingDiagnosis"))
                      i.ContainsDiagnosis = true;
                   else if (t.TaxonomyType.Contains("CCC/NursingIntervention"))
                      i.ContainsIntervention = true;
                   else if (t.TaxonomyType.Contains("CCC/CareComponent"))
                      i.ContainsCareComponent = true;
                
               }

               //try
               //{
               //    int upd = App.carePlan.DB.SaveChanges();
               //}
               //catch (UpdateException ex)
               //{
                   
               //    showUpdateException(ex);
               //}

            }

        /// <summary>
        /// Finds concept and care component for a tag and build up a careplan of all tags
        /// </summary>
        /// <param name="cp"></param>
            void buildAllTags(CarePlan cp)
            {
                string version = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
                string languageName = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;

                careplanTags = new ObservableCollection<Tag>();

                foreach (Item item in cp.Item)
                  foreach (Tag tag in item.Tag)
                    {
                        
                       tagHandler.updateTag(App.cccFrameWork.DB,tag, languageName, version); // Find Concept and CareComponent for tag
                       careplanTags.Add(tag);
                    }

                cvCareplanTags = new ListCollectionView(careplanTags);
              
                cvCareplanTags.GroupDescriptions.Add(new PropertyGroupDescription("CareComponentConcept"));
                cvCareplanTags.SortDescriptions.Add(new SortDescription("CareComponentConcept",ListSortDirection.Ascending));
                cvCareplanTags.SortDescriptions.Add(new SortDescription("Concept", ListSortDirection.Ascending));
                lbTaxonomy.ItemsSource = cvCareplanTags; 
               
            }

            
           
           public void refreshOutcomes(Tag tag)
           {
               Outcome latest;

               if (!tag.Outcome.IsLoaded)
                   tag.Outcome.Load();

               // Find latest outcome that is not evaluated yet
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
              var qOutcome = tag.Outcome.Where(o => o.ActualDate == null).OrderBy(o => o.ExpectedDate);
               // Sp 1 workaround
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
               // CHECK THIS LATER.....
               var qOutcome = tag.Outcome.Where(a => a.ActualDate == null).OrderBy(o => o.ExpectedDate);
#endif
               
               if (qOutcome.Count<Outcome>() > 0)
               {
                   latest = (Outcome)qOutcome.Take(1).First();
                   tag.LatestOutcome = latest.ExpectedOutcome;
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    tag.LatestOutcomeModifier = App.cccFrameWork.DB.OutcomeType.Where(o => o.Code == latest.ExpectedOutcome &&
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                   tag.LatestOutcomeModifier = App.cccFrameWork.DB.OutcomeType.Where("it.Code = "+ latest.ExpectedOutcome + " AND it.Version = '"+

                   eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version + "'AND it.Language_Name = '"+ eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName+"'").First().Concept;
#endif
               }

               else
               {
                   tag.LatestOutcome = null;
                   tag.LatestOutcomeModifier = null;
               }

           }

            public void refreshTags()
            {

                string version = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
                string languageName = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;
   
                Item selItem = lvCareBlog.SelectedItem as Item;
                if (selItem == null)
                    return;

                foreach (Tag tag in selItem.Tag)
                {
                    refreshOutcomes(tag);

                    if (!tag.ActionTReference.IsLoaded)
                        tag.ActionTReference.Load();

                    tagHandler.updateTag(App.cccFrameWork.DB,tag,languageName,version);
                    
                }

               Tag_AssociationChanged(this, null);
            }

            public void  Tag_AssociationChanged(object sender, CollectionChangeEventArgs e)
            {
                Item selItem = lvCareBlog.SelectedItem as Item;
                if (selItem == null)
                    return;
               
                //App.carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagHandler);
                 
                inferContentFromTags(selItem);

                cvItemTags = new ListCollectionView(selItem.Tag.OrderBy(t => t.CareComponentConcept).ThenBy(t => t.Concept).ToList());
                cvItemTags.GroupDescriptions.Add(new PropertyGroupDescription("CareComponentConcept"));
                cvItemTags.Refresh();
                lbTags.ItemsSource = cvItemTags;

                if (cvCareplanTags != null)
                    cvCareplanTags.Refresh();                
            }
           
        
            private void MenuItemOm_Click(object sender, RoutedEventArgs e)
            {
                WindowCopyright wc = new WindowCopyright();
                wc.spLoading.Visibility = Visibility.Collapsed;
                wc.Height = 450;
                wc.ShowDialog();
            }

           
           
            private void Window_Closing(object sender, CancelEventArgs e)
            {
               // timer.IsEnabled = false;
                //annotationTimer.IsEnabled = false;
                //carePlanSubmitHandler();
                //annotationservice.stopAnnotationService();
                foreach (Window w in App.Current.MainWindow.OwnedWindows) // Close explorer window if open
                    w.Close();


            }

            #region Careplan handling

            private void btnNewItem_Click(object sender, RoutedEventArgs e)
            {

                if (!healthDB["PHR"])
                {
                    MessageBox.Show("No database access to personal health record, please check database connectivity", "No database access to personal health record", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
               
                WindowNewItem wndNewItem = new WindowNewItem();
                wndNewItem.ShowDialog();

                // Assume user want to work with the newly created item

                lvCareBlog.SelectedItem = wndNewItem.CurrentItem;
               
             }

           private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
           {
               
               Item selItem = (Item)lvCareBlog.SelectedItem;
               if (selItem == null)
               {
                   MessageBox.Show("No item is selected to delete, please select an item first", "No item selected", MessageBoxButton.OK, MessageBoxImage.Information);
                   return;
               }

               if (MessageBox.Show("Delete \""+selItem.Title+"\", are you sure ?", "Delete "+selItem.Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
               {
                  
                 
                   try
                   {
                       //if (!selItem.Tag.IsLoaded)
                       //    selItem.Tag.Load();
                      
                       selItem.Tag.AssociationChanged -= new CollectionChangeEventHandler(Tag_AssociationChanged); // Disable tag association
                      
                       App.carePlan.DB.DeleteObject(selItem.History);

                       infoAcq.Statement.Clear(); // Remove annotation-references
                       if (aService.Service.IsEnabled)
                           aService.Service.Disable();
                      
                       App.carePlan.DB.DeleteObject(selItem);

                       SaveCarePlan();
                       
                      
                   }
                   catch (UpdateException ee)
                   {
                     showUpdateException(ee);
                   }
                   finally
                   {
                       lbTags.ItemsSource = null;
                       fdReaderCareBlog.Document = null;
                       fdReaderCareBlog.Visibility = Visibility.Collapsed;
                       buildAllTags(App.carePlan.ActiveCarePlan);
                    
                       lvCareBlog_SelectionChanged(lvCareBlog, null); // Fake event to update control 

                   }

               }
           }

           
            private void lvCareBlog_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
              //  ListView lv = sender as ListView;
                ComboBox lv = sender as ComboBox;

                Item selItem = (Item)lv.SelectedItem;
                if (selItem == null)
                    return; // Empty selection

                // Keep record of latest item selected
                eNursePHR.userInterfaceLayer.Properties.Settings.Default.LastItem = selItem.Id;
                eNursePHR.userInterfaceLayer.Properties.Settings.Default.Save();

                refreshTags();

                // Build flowdocument

                //Section sItem = new Section();
                //BlockUIContainer bui = new BlockUIContainer();
                //StackPanel spPanel = new StackPanel();
                //Paragraph pTitle = new Paragraph();
                //pTitle.FontSize = 15;
                //pTitle.Inlines.Add(new Bold(new Run(selItem.Title)));
                ////spPanel.Children.Add(pTitle);
                //sItem.Blocks.Add(pTitle);

                aService.changeItemStore(selItem);

                infoAcq.Refresh(aService.Service.Store);
               

                App.carePlan.showCareplanItem(fdReaderCareBlog,selItem,tagHandler);


               

                //spPanel.Children.Add(fdview);

                //bui.Child = spPanel;
                //sItem.Blocks.Add(bui);
                //fdCareBlog.Blocks.Add(sItem);

                //lvCareBlog.ScrollIntoView(selItem);
            }

               
           
           
            private void lvCareBlog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                Item selItem = (Item)(sender as ComboBox).SelectedItem;
                
                if (selItem == null)
                    return;

                WindowNewItem wndUpdateItem = new WindowNewItem();
                wndUpdateItem.UpdateItem = true;
                wndUpdateItem.CurrentItem = selItem;
                wndUpdateItem.ShowDialog();

                infoAcq.Refresh(aService.Service.Store); // In case annotation references is changed...

                App.carePlan.showCareplanItem(fdReaderCareBlog,selItem,tagHandler);
                
            }

            #endregion


            
            private void lbTags_Drop(object sender, DragEventArgs e)
            {
                IDataObject transfer = e.Data;
                Item selItem = lvCareBlog.SelectedItem as Item;
                
                string taxonomyType = null;
                Guid taxonomyGuid = Guid.Empty;
                Guid taxonomyOutcomeAttachmentGuid = Guid.Empty;
                Guid taxonomyActionTypeAttachmentGuid = Guid.Empty;

                eNursePHR.BusinessLayer.CCC_Translations.OutcomeType fOutcomeType = null;
                eNursePHR.BusinessLayer.CCC_Translations.ActionType fActionType = null;

               string comment = null;


                if (selItem == null)
                {
                    MessageBox.Show("You have not selected an entry to attach this tag to,\nplease create a new entry or select one before retrying this action", 
                        "No entry selected",MessageBoxButton.OK,MessageBoxImage.Information);
                    return;
                }

                 
                if (transfer.GetDataPresent("CCC/CareComponent"))
                {
                    taxonomyType = "CCC/CareComponent";
                    Care_component component = (Care_component)transfer.GetData("CCC/CareComponent");
                    taxonomyGuid = tagHandler.getTaxonomyGuidCareComponent(component.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
                 
                }

                if (transfer.GetDataPresent("CCC/OutcomeType"))
                {
                    fOutcomeType = (eNursePHR.BusinessLayer.CCC_Translations.OutcomeType)transfer.GetData("CCC/OutcomeType");
                    // Find guid in reference terminology
                    taxonomyOutcomeAttachmentGuid = tagHandler.getTaxonomyGuidOutcomeType(fOutcomeType.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
                }

                if (transfer.GetDataPresent("CCC/NursingDiagnosis"))
                {
                    taxonomyType = "CCC/NursingDiagnosis";
                    eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis fDiag = (eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis)transfer.GetData("CCC/NursingDiagnosis");
                    comment = fDiag.Comment;
                    // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                    taxonomyGuid = tagHandler.getTaxonomyGuidNursingDiagnosis(fDiag.ComponentCode, fDiag.MajorCode, fDiag.MinorCode, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);

                }

                if (transfer.GetDataPresent("CCC/ActionType"))
                {
                    taxonomyType = "CCC/ActionType";
                    fActionType = (ActionType)transfer.GetData("CCC/ActionType");
                    taxonomyActionTypeAttachmentGuid = tagHandler.getTaxonomyGuidActionType(fActionType.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
                   
                }

                if (transfer.GetDataPresent("CCC/NursingIntervention"))
                    {
                    taxonomyType = "CCC/NursingIntervention";
                    Nursing_Intervention fInterv = (Nursing_Intervention)transfer.GetData("CCC/NursingIntervention");

                    comment = fInterv.Comment;
                    // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                    taxonomyGuid = tagHandler.getTaxonomyGuidNursingIntervention(fInterv.ComponentCode, fInterv.MajorCode, fInterv.MinorCode, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
                   
                   
                }


                 
                // Add tag, if found in taxonomy

                if (taxonomyType == "CCC/NursingDiagnosis" || taxonomyType == "CCC/NursingIntervention")
                {
                    selItem.Tag.AssociationChanged -= new CollectionChangeEventHandler(Tag_AssociationChanged); // Remove event handling now
                    Tag newTag = eNursePHR.BusinessLayer.PHR.Tag.CreateTag(Guid.NewGuid(), taxonomyType, taxonomyGuid);
                  
                    newTag.Item = selItem;
                    newTag.Comment = comment;

                    History tagHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                    newTag.History = tagHistory;
                   

                    App.carePlan.DB.AddToTag(newTag);
                    App.carePlan.DB.AddToHistory(tagHistory);
                    
                    if (taxonomyOutcomeAttachmentGuid != Guid.Empty) // Check if we also have to deal with an outcome
                    {
                       
                        Outcome newOutcome = Outcome.CreateOutcome(Guid.NewGuid(), fOutcomeType.Code, taxonomyOutcomeAttachmentGuid);
                        newOutcome.Tag = newTag;  // Setup referende to tag

                        History outcomeHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                        newOutcome.History = outcomeHistory;
                        App.carePlan.DB.AddToHistory(outcomeHistory);
                        App.carePlan.DB.AddToOutcome(newOutcome);

                    }

                    if (taxonomyActionTypeAttachmentGuid != Guid.Empty)
                    {

                        ActionT newActionType = ActionT.CreateActionT( fActionType.Code,
                            tbConceptActionType.Text, taxonomyActionTypeAttachmentGuid, newTag.Id);
                        newActionType.Tag = newTag;

                   
                        History actionHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                        newActionType.History = actionHistory;
                        App.carePlan.DB.AddToHistory(actionHistory);
                        App.carePlan.DB.AddToActionT(newActionType);
                    }

                    SaveCarePlan();

                    selItem.Tag.AssociationChanged +=new CollectionChangeEventHandler(Tag_AssociationChanged);
                    Tag_AssociationChanged(this, null);
                    careplanTags.Add(newTag);
                    inferContentFromTags(selItem); // Update content status
                    refreshTags();
                }
            }

            private void lbTaxonomy_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                Tag selTag = (Tag)(sender as ListBox).SelectedItem;
                if (selTag == null)
                    return;
              
                lvCareBlog.SelectedItem = selTag.Item; // Keep care plan blog synchronized with tag from taxonomy/CCC
                
            }

            private void lvCareBlog_Drop(object sender, DragEventArgs e)
            {
                IDataObject transfer = e.Data;
                // Add diagnosis concept as title and automatic tag
                Item i = new Item();
                i.Id = Guid.NewGuid();
                i.Title = "Not implemented yet!";
                History h = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                i.History = h;
                
                //items.Add(i);


            }

            private void miCarePlanBlog_Click(object sender, RoutedEventArgs e)
            {
                if (App.carePlan == null)
                {
                    MessageBox.Show("No active personal health record, is database connectivity OK?", "No active personal health record", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (App.carePlan.ActiveCarePlan.Item.Count  == 0)
                {
                    MessageBox.Show("The current health record is empty, please create at least one entry first", "Empty health record", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (aService.Service.IsEnabled)
                    aService.Service.Disable();
                
                App.carePlan.generateCareBlog(fdReaderCareBlog, App.carePlan.ActiveCarePlan,tagHandler,true);
                
                exTags.IsExpanded = false;
                exTaxonomy.IsExpanded = true;
            }

            private void lbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                Tag selTag = (Tag)lbTags.SelectedItem;
                if (selTag == null)
                    return;
                
                if (!selTag.Outcome.IsLoaded)
                    selTag.Outcome.Load();

                
            }

       

            private void miDeleteTag_Click(object sender, RoutedEventArgs e)
            {
                Tag selTag = (Tag)lbTags.SelectedItem;
                if (selTag == null)
                {
                    MessageBox.Show("No tag selected to delete", "No tag selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Tag " + selTag.Concept + " will be deleted, are you sure?", "Delete "+selTag.Concept, MessageBoxButton.YesNo,MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;

                 App.carePlan.DB.DeleteObject(selTag);
                 if (SaveCarePlan() != -1)
                    careplanTags.Remove(selTag);
                

            }

            private void miManageOutcomes_Click(object sender, RoutedEventArgs e)
            {
                // Load outcome history
                Tag selTag = (Tag)lbTags.SelectedItem;
                foreach (Outcome o in selTag.Outcome)
                    if (!o.HistoryReference.IsLoaded)
                        o.HistoryReference.Load();

                WindowOutcomes wndOutcomes = new WindowOutcomes();
                wndOutcomes.DataContext = selTag;
                wndOutcomes.lvOutcome.ItemsSource = selTag.Outcome;
                wndOutcomes.ccOutcome.Content = selTag.Outcome;
                wndOutcomes.ShowDialog();
                refreshOutcomes(selTag);
                Tag_AssociationChanged(this, null);
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


            public int SaveCarePlan()
            {
                try
                {
                    int upd = App.carePlan.DB.SaveChanges();
                }
                catch (UpdateException uex)
                {
                    showUpdateException(uex);
                    return -1;
                }
                catch (Exception ex)
                {
                    showException(ex);
                    return -1;
                }

                return 0;


            }

            private void tbTagComment_TextChanged(object sender, TextChangedEventArgs e)
            {
                btnSaveTagComments.Visibility = Visibility.Visible;
                
                
            }

            private void btnSaveTagComments_Click(object sender, RoutedEventArgs e)
            {
                SaveCarePlan();
               // App.carePlan.showCareplanItem(fdReaderCareBlog, (Item)lvCareBlog.SelectedItem, tagHandler);
                btnSaveTagComments.Visibility = Visibility.Collapsed;
                cvCareplanTags.Refresh();
                
            }

            private void turnOnAnnotationService(Item item)
            {
                aService.changeItemStore(item);
            }

            private void turnOffAnnotationService()
            {
                aService.Service.Disable();
            }

            #region Item navigation for a specific careplan

            private void btnBack_Click(object sender, RoutedEventArgs e)
            {

                int selIndex = lvCareBlog.SelectedIndex;

                if (selIndex > 0)
                    selIndex--;
                else
                    selIndex = lvCareBlog.Items.Count - 1;

                lvCareBlog.SelectedIndex = selIndex;

               
            }

            private void btnForward_Click(object sender, RoutedEventArgs e)
            {
                int selIndex = lvCareBlog.SelectedIndex;

                if (selIndex < (lvCareBlog.Items.Count - 1))
                    selIndex++;
                else
                    selIndex = 0;

                lvCareBlog.SelectedIndex = selIndex;
            }

            private void btnHome_Click(object sender, RoutedEventArgs e)
            {
                Item selItem = lvCareBlog.SelectedItem as Item;
                if (selItem != null)
                {
                    App.carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagHandler);
                    turnOnAnnotationService(selItem);
                }
                else
                {
                    MessageBox.Show("No entry selected, probably a empty health record?", "No entry selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            #endregion


            #region Annotation toolbar

            private void btnCaptureDiseaseStatement_Click(object sender, RoutedEventArgs e)
            {

                if (!checkSelection(fdReaderCareBlog))
                    return;

                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Disease;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("DiseaseHighlightColor"));

                    this.addContent(annotation, ContentType.Disease);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the disease information", "Empty disease information selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }


            private void btnCaptureDiagnosticStatement_Click(object sender, RoutedEventArgs e)
            {

                if (!checkSelection(fdReaderCareBlog))
                    return;

                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Diagnostic;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("DiagnosisHighlightColor"));
                    
                    this.addContent(annotation, ContentType.Diagnostic);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the diagnostic information", "Empty diagnostic information selection", MessageBoxButton.OK,MessageBoxImage.Information);
            }


            private void btnCaptureInterventionalStatement_Click(object sender, RoutedEventArgs e)
            {
                if (!checkSelection(fdReaderCareBlog))
                    return;

                
                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Interventional;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("InterventionHighlightColor"));
                    
                    this.addContent(annotation, ContentType.Interventional);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the interventional information", "Empty interventional information selection", MessageBoxButton.OK,MessageBoxImage.Information);

            }

            /// <summary>
            /// Checks if a flowdocument reader has empty selection
            /// </summary>
            /// <param name="fdReader"></param>
            /// <returns></returns>
            private bool checkSelection(FlowDocumentReader fdReader)
            {

                if (fdReader.Selection == null)
                {
                    MessageBox.Show("You have not selected any text to highlight", "No text selected to highlight", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                else
                    return true;

            }

            private void btnCaptureMedicationalStatement_Click(object sender, RoutedEventArgs e)
            {

                if (!checkSelection(fdReaderCareBlog))
                    return;

                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Medication;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("MedicationHighlightColor"));

                    this.addContent(annotation, ContentType.Medication);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the medicational information", "Empty medicational information selection", MessageBoxButton.OK,MessageBoxImage.Information);

            }

            /// <summary>
            /// Inserts a XML-tree in the contents section of an annotation to capture information that was annotated
            /// this might be a diagnostics statement or an interventional statement
            /// <eNursePHR>
            ///     <Statement ContentType="Diagnosis" Content="???"
            /// </eNursePHR>
            ///  </summary>
            /// <param name="annotation"></param>
            /// <param name="contentType"></param>
            public void addContent(Annotation annotation, ContentType contentType)
            {
                XmlDocument xDoc = new XmlDocument();
                XmlElement xHeading = xDoc.CreateElement("eNursePHR");
                XmlElement xStatement = xDoc.CreateElement("Statement");

                // Content type
                if (contentType == ContentType.Diagnostic)
                    xStatement.SetAttribute("ContentType", "Diagnostic");
                else if (contentType == ContentType.Interventional)
                    xStatement.SetAttribute("ContentType", "Interventional");
                else if (contentType == ContentType.Disease)
                    xStatement.SetAttribute("ContentType", "Disease");
                else if (contentType == ContentType.Medication)
                    xStatement.SetAttribute("ContentType", "Medication");
                
                // Content

                xStatement.SetAttribute("Content", getText(annotation));
                xHeading.AppendChild(xStatement);

                annotation.Anchors.First().Contents.Add(xHeading);

            }


            /// <summary>
            /// Get text pointed to by BoundingStart and BoundingEnd-anchors (highlighted area) in the flowdocument 
            /// Partly dependent on information avalable on MSDN-library
            /// </summary>
            /// <param name="annotation"></param>
            /// <returns></returns>
            public string getText(Annotation annotation)
            {
                AnnotationService service = AnnotationService.GetService(fdReaderCareBlog);

                IAnchorInfo info = AnnotationHelper.GetAnchorInfo(service, annotation);
                if (info == null)
                    return null;

                TextAnchor tAnchor = info.ResolvedAnchor as TextAnchor;

                TextRange range = new TextRange((TextPointer)tAnchor.BoundingStart, (TextPointer)tAnchor.BoundingEnd);

                return range.Text;

            }

            public string getAnnotationContent(Annotation annotation)
            {
                if (annotation.Anchors[0].Contents.Count == 0)
                    return String.Empty;

                
                XmlDocument xdoc = new XmlDocument();

                xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

                XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]");
                return xe.Attributes["Content"].Value;

            }

            public ContentType getContentType(Annotation annotation)
            {

                if (annotation.Anchors[0].Contents.Count == 0)
                    return ContentType.Null;

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

                XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]"); // XPath-query
                string contentType = xe.Attributes["ContentType"].Value;

                ContentType cType = new ContentType();
                switch (contentType)
                {
                    case "Diagnostic": cType = ContentType.Diagnostic; break;
                    case "Interventional": cType = ContentType.Interventional; break;
                    case "Disease": cType = ContentType.Disease; break;
                    case "Medication": cType = ContentType.Medication; break;
                }

                return cType;
            }

           
            #endregion

            private void lvAnnotation_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                

                //Annotation selAnnotation = ((sender as ListBox).SelectedItem) as Annotation;
                //if (selAnnotation == null)
                //    return;

                //// Suggest information/reason for code in CCC framework
                //if (this.getContentType(selAnnotation) == ContentType.Diagnostic)
                //    tbReasonDiagnosis.Text = this.getText(selAnnotation);
                //else
                    
                //    if (this.getContentType(selAnnotation) == ContentType.Interventional)
                //    tbReasonIntervention.Text = this.getText(selAnnotation);
                
               
            }

            /// <summary>
            /// Convert the first character to uppercase
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            private string firstCharToUpper(string content)
            {
                string firstChar = content.Substring(0, 1).ToUpperInvariant();
                return content.Remove(0, 1).Insert(0, firstChar);
            }

            
            private void btnAcquireDiagnosticInformation_Click(object sender, RoutedEventArgs e)
            {
                Annotation selAnnotation = lvAnnotation.SelectedItem as Annotation;
                if (selAnnotation == null)
                {
                    MessageBox.Show("No aquired diagnostic information selected, try selecting a diagnostic statement", "No acquired diagnostic information", MessageBoxButton.OK,MessageBoxImage.Information);
                    return;
                }

                if (getContentType(selAnnotation) == ContentType.Diagnostic)
                    tbReasonDiagnosis.Text += firstCharToUpper(getAnnotationContent(selAnnotation));
                
                else
                    MessageBox.Show("Please select diagnostic information", "Select diagnostic information", MessageBoxButton.OK,MessageBoxImage.Information);
            }

            private void btnAcquireInterventionalInformation_Click(object sender, RoutedEventArgs e)
            {
                Annotation selAnnotation = lvAnnotation.SelectedItem as Annotation;
                if (selAnnotation == null)
                {
                    MessageBox.Show("No aquired interventional or medicational information selected", "No acquired interventional or medicational information", MessageBoxButton.OK,MessageBoxImage.Information);
                    return;
                }

                ContentType contentType = getContentType(selAnnotation);
                if (contentType == ContentType.Interventional || contentType == ContentType.Medication)
                    tbReasonIntervention.Text += firstCharToUpper(getAnnotationContent(selAnnotation));
                else
                    MessageBox.Show("Please select interventional or medicational information", "Select interventional or medicational information", MessageBoxButton.OK, MessageBoxImage.Information);
            
            }


            // Info from https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=2574377&SiteID=1
            void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
            {
                Process.Start(e.Uri.ToString());

                e.Handled = true;
            }

            private void btnFullScreen_Click(object sender, RoutedEventArgs e)
            {
                if (!FullScreenItem)
                {
                    prevTagsWidth = gcTags.Width;
                    gcTags.Width = new GridLength(0);

                    prevTaxonomyWidth = gcTaxonomy.Width;
                    gcTaxonomy.Width = new GridLength(0);
                
                    FullScreenItem = true;
                }
                else
                {
                    gcTags.Width = prevTagsWidth;
                    gcTaxonomy.Width = prevTaxonomyWidth;

                    FullScreenItem = false;

                }

               
                if (!FullScreenItem)
                    fdReaderCareBlog.SwitchViewingMode(FlowDocumentReaderViewingMode.Page);
                else
                    fdReaderCareBlog.SwitchViewingMode(FlowDocumentReaderViewingMode.Scroll);

            }



            #region Explorer

            private char FilterCode;
            private string FilterSearch;


            #region Language handling

            private void MenuItemLanguageIntegrity_Click(object sender, RoutedEventArgs e)
            {

                if (!healthDB["CCCReference"] || !healthDB["CCCFramework"])
                {
                    MessageBox.Show("Attempt to run language integrity check, cannot proceed because something is wrong with database access", "Language integrity cannot proceed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    WindowMultiLanguageIntegrity winMultiLangIntegrity = new WindowMultiLanguageIntegrity();
                    winMultiLangIntegrity.ShowDialog();
                    App.cccFrameWork.loadFrameworkAnalysis();
                    refreshCCCFramework();
                }
                catch (Exception exception)
                {
                    showException(exception);
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
                    if (App.cccFrameWork.FrameworkActual[i].Language_Name == eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName)
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

                        refreshTags();

                        buildAllTags(App.carePlan.ActiveCarePlan);
                    }

                }

            }

            #endregion

            #region Explorer search

            private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
            {
                TextBox tb = sender as TextBox;

                if (App.cccFrameWork == null)
                {
                    MessageBox.Show("Cannot search in the clinical care classification, because database is not loaded", "Cannot search", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                FilterSearch = tb.Text;
                if (FilterSearch == "")
                {
                    lblNoMatchCareComponent.Visibility = Visibility.Collapsed;
                    lblNoMatchNursingDiagnoses.Visibility = Visibility.Collapsed;
                    lblNoMatchNursingInterventions.Visibility = Visibility.Collapsed;

                    spCareComponent.Visibility = Visibility.Visible;
                    App.cccFrameWork.cvComponents.Filter = null;

                    lbCareComponent.SelectedIndex = 1;
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


            private bool FilterOutDiagnoses(object item)
            { // Based on example from Beatrize Costa blog, accessed 25 november 2007

                Nursing_Diagnosis nd = item as Nursing_Diagnosis;


                if (nd == null)
                    return false;


                int result = nd.ComponentCode.CompareTo(FilterCode.ToString());

                if (result == 0) return true;

                return false;


            }

            private bool FilterOutInterventions(object item)
            {
                // Based on example from Beatrize Costa blog, accessed 25 november 2007

                Nursing_Intervention nd = item as Nursing_Intervention;


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



            }


            #endregion

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


            private void lbNursingInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                ListBox lb = sender as ListBox;

                Nursing_Intervention fi = (Nursing_Intervention)lb.SelectedItem;
                if (fi == null)
                    return;

                if ((bool)cbAttachToIntervention.IsChecked)
                {
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

                spShowOutcome.Visibility = Visibility.Visible;
                //tbShowOutcomeConcept.DataContext = lbOutcomeType.SelectedItem as FrameworkOutcomeType;

            }

            private void cbAttachToDiagnosis_UnChecked(object sender, RoutedEventArgs e)
            {
                spShowOutcome.Visibility = Visibility.Collapsed;
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


            private void cbAttachToIntervention_Checked(object sender, RoutedEventArgs e)
            {
                gcActionType.Width = new GridLength(75);
                tbNursingInterventionActionModifier.Visibility = Visibility.Visible;
                tbConceptActionType.Text = cbActionType.Text;
                tbNursingInterventionActionModifier.Text = cbActionType.Text;
                tbNursingInterventionConcept.Text = ((Nursing_Intervention)(lbNursingInterventions.SelectedItem)).Concept;
            }

            //private TextBlock getActionTypeCP()
            //{
            //    // Based on info: http://joshsmithonwpf.wordpress.com/2007/06/28/how-to-use-findname-with-a-contentcontrol/
            //    // Accessed : 10 march 2008

            //    ContentPresenter contentPresenter = VisualTreeHelper.GetChild(ccActionType, 0) as ContentPresenter;

            //    TextBlock tb = (TextBlock)ccActionType.ContentTemplate.FindName("tbTemplateConceptActionType", contentPresenter);
            //    FrameworkElement fe = tb.Parent as FrameworkElement;

            //    return tb;
            //}

            private void cbAttachToIntervention_UnChecked(object sender, RoutedEventArgs e)
            {
                gcActionType.Width = new GridLength(0);
                tbNursingInterventionActionModifier.Visibility = Visibility.Collapsed;
                tbConceptActionType.Text = ((ActionType)(lbActionType.SelectedItem)).Concept;
                tbNursingInterventionActionModifier.Text = null;
                tbNursingInterventionConcept.Text = ((Nursing_Intervention)lbNursingInterventions.SelectedItem).Concept;
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



            #endregion


            #region Blood pressure UI
            private void showChart(BloodPressureChart bpChart, Image img)
            {
                // Request image from the google chart API

                BitmapImage bimg = new BitmapImage(new Uri(bpChart.getChartURI("s:")));
                loadImage(bpChart, img, bimg);

            }

            private void loadImage(GoogleChart chart, Image img, BitmapImage bImg)
            {
                img.Height = chart.Height;
                img.Width = chart.Width;
                img.BeginInit();
                img.Source = bImg;
                img.EndInit();

            }


            private void showChart(AverageChart avgChart, Image img)
            {
                // Request image from the google chart API
                BitmapImage bimg;
                try
                {
                    bimg = new BitmapImage(new Uri(avgChart.getChartURI("t:")));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load image from google chart", "Google chart load fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                loadImage(avgChart, img, bimg);

            }


            private void sliderDiastolic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                if (sliderSystolic == null)
                    return;

                // Limit diastolic below systolic pressure
                if (!sliderSystolic.IsEnabled)
                    return;

                if (e.NewValue > sliderSystolic.Value)
                {

                    sliderDiastolic.Value = sliderSystolic.Value;
                    MessageBox.Show("Diastolic pressure cannot exceed systolic pressure", "Limit reached diastolic pressure", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            private void sliderSystolic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                if (sliderDiastolic == null)
                    return;

                // Limit systolic pressure over diastolic pressure
                if (!sliderDiastolic.IsEnabled)
                    return;

                if (e.NewValue < sliderDiastolic.Value)
                {

                    sliderDiastolic.Value = sliderSystolic.Value;
                    MessageBox.Show("Systolic pressure cannot be below diastolic pressure", "Limit reached systolic pressure", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            private void btnAddBPReading_Click(object sender, RoutedEventArgs e)
            {
                if (bwSave.IsBusy)
                {
                    MessageBox.Show("Waiting for BP data to be saved", "Waiting", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                tbStatusBP.Text = null;



                if (!(bool)cbSystolicBP.IsChecked)
                    systolicBP = null;
                else
                    systolicBP = Convert.ToInt32(Math.Round(sliderSystolic.Value));


                if (!(bool)cbDiastolicBP.IsChecked)
                    diastolicBP = null;
                else
                    diastolicBP = Convert.ToInt32(Math.Round(sliderDiastolic.Value));


                if (!(bool)cbPulseHR.IsChecked)
                    heartRate = null;
                else
                    heartRate = Convert.ToInt32(Math.Round(sliderPulseHR.Value));


                if (!dpDate.Value.HasValue)
                {
                    MessageBox.Show("You have not entered a valid date, please specify a date", "Invalid date", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime datePicker = dpDate.Value.Value;
                time = new DateTime(datePicker.Year, datePicker.Month, datePicker.Day, tpTime.SelectedHour, tpTime.SelectedMinute, tpTime.SelectedSecond);
                int result = time.CompareTo(DateTime.Now);

                if (result == 1)
                {
                    MessageBox.Show("You have entered a time in the future, please define a new time", "Future time", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if (tbComment.Text.Length == 0)
                    comment = null;
                else
                    comment = tbComment.Text;

                if (systolicBP == null && diastolicBP == null && heartRate == null && comment == null)
                {
                    MessageBox.Show("There is no valid data to save, please enter a reading", "No valid data", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bwSave.RunWorkerAsync();


            }

            void bwSave_DoWork(object sender, DoWorkEventArgs e)
            {

                updDB = chartBloodPressure.newBPdata(cpGuid, systolicBP, diastolicBP, heartRate, comment, time);

            }

            void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (updDB.upd > 0)
                {

                    tbStatusBP.Text = "Saved";
                    // Remove status text after 0.5 sec.
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bw.RunWorkerAsync();
                }
                else if (updDB.upd == -1)
                {
                    tbStatusBP.Text = "Failed to save";
                    tbStatusBP.ToolTip = updDB.updMsg;

                }



            }

            void bw_DoWork(object sender, DoWorkEventArgs e)
            {
                Thread.Sleep(500);
            }

            void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                tbStatusBP.Text = null;
            }

            private void cbDisplayOptionsChanged_Click(object sender, RoutedEventArgs e)
            {
                chartBloodPressure.generateChart();
                showChart(chartBloodPressure, img);
            }


            #endregion



        }
}

   
    
