// Introduced this directive for code targetet at SP 1 release version of SQL COMPACT SERVER
// that unfortunately contains an error in construction of queries with parameters for LINQ
// One way to work around this is to rewrite to queries by using entity SQL instead as suggested
// on request for "query processor error" thread in MSDN forum for SQL Server Compact.

#define  SQL_SERVER_COMPACT_SP1_WORKAROUND

#undef VERBOSE_SAVE_CHANGES
// If defined will give added, deleted, modified object entitites status message on an object context

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
using eNursePHR.userInterfaceLayer.AnnotationNS;

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

        private DispatcherTimer dispatcherTimerUI;

        

        private string _netVersion = Environment.Version.ToString();
        public string NETVersion
        {
            get { return _netVersion; }
        }

        public const int EXIT_LOAD_FRAMEWORK = 1;
        public const int EXIT_LOAD_CAREPLAN = 2;



        //   Dictionary<string, Guid> dictItemBlog;


       
        eNAnnotationService annotationService;

        public ViewInformationAcqusition infoAcq = new ViewInformationAcqusition();

        public ContentType contentType = new ContentType();

        // Previous width of tag-panel and taxonomy-panel Grid
        GridLength prevTagsWidth = new GridLength(0);
        GridLength prevTaxonomyWidth = new GridLength(0);

        // True if item is to be displayed in full screen
        bool FullScreenItem = false;


        WindowCopyright wndCopyright;

        Dictionary<string, bool> healthDB; // Holds information about databases and a flag that indicates if problems where detected

        TagLangageConverter tagConverter = new TagLangageConverter();
       
        public delegate void turnOffSaveAnnotationsButton();

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
            if (this._SQLCompactVersion.StartsWith("3.5.") && spack == "1")
                return true;
            else
                return false;



        }

        public void tbTagComment_TextChanged(object sender, EventArgs e)
        {

            btnSaveTagComments.Visibility = Visibility.Visible;  


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
                // TO DO: Consider bundling sql server compact .dll with eNursePHR
                MessageBox.Show("You have not installed SQL Server Compact V 3.5 SP 1, please install before using application again", "SQL Server Compact v3.5 SP 1 not installed", MessageBoxButton.OK, MessageBoxImage.Stop);
                App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                App.Current.Shutdown();

            }
            else
            {

                wndCopyright = new WindowCopyright();
                healthDB = wndCopyright.checkDatabasesHealth();  // Perform health check on the databases
                wndCopyright.Show();

                InitializeComponent();

                
            }
        }

        /// <summary>
        /// Shows System.Exception error in a dialog
        /// </summary>
        /// <param name="ex"></param>
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

        /// <summary>
        /// Shows UpdateException in separate dialog window
        /// </summary>
        /// <param name="ex"></param>
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

        
        /// <summary>
        /// Event handler for Window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CCCTaxonomyControl.CCCLoadFail += new CCCTaxonomyControl.LoadEventHandler(CCCTaxonomyControl_CCCLoadFail);

            BloodPressureControl.setupBloodPressureUI();

            TagControl.TagCommentChanged += new EventHandler(tbTagComment_TextChanged);
            TagOverviewControl.TagCommentChanged += new EventHandler(tbTagComment_TextChanged);

           
            /* LANGUAGE attrib change */
            changeStatus("Logged in as " + System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName);
            //+ " Kultur for UI: " + Thread.CurrentThread.CurrentUICulture.DisplayName.ToString();

            // Enable sharing of properties across layers

            App.Current.Properties["Version"] = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
            App.Current.Properties["LanguageName"] = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;

            // Setup event handling of hyperlinks

            fdReaderCareBlog.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(hyperlink_RequestNavigate));

            // Start annotation service
            annotationService = new eNAnnotationService(fdReaderCareBlog);
            AnnotationControl.lvAnnotation.ItemsSource = infoAcq.CvStatement;


            // If CCC framework is in good condition load it
            if (healthDB["CCCFramework"])
            {

                wndCopyright.tbLoading.Text = "Loading CCC taxonomy...";
                CCCTaxonomyControl.loadCCCFramework();

                wndCopyright.pbLoading.Value = 75;
            }
            else // turn off ui elements for CCC framework
            {
                CCCTaxonomyControl.turnOffUI();   
                CCCTaxonomyControl.tbSearch.IsEnabled = false;
            }

            if (healthDB["PHR"])
            {
                wndCopyright.tbLoading.Text = "Loading test health record....";
                loadCareplan(Guid.NewGuid(), true, updateCCCDBSaveStatus,null,null); // Load a test careplan
                wndCopyright.pbLoading.Value = 100;
                // Show all tags for the current careplan
                TagOverviewControl.buildTagsOverview(App.s_carePlan.ActiveCarePlan);
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

        void TagControl_TagCommentChanged(object sender, EventArgs e)
        {
            
        }

        void CCCTaxonomyControl_CCCLoadFail(object sender, LoadEventArgs e)
        {
            showException(e.Exception);

        }

        private void stopApplication(string errMsg, string errTitle, int exitCode)
        {
            MessageBox.Show(errMsg, errTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
            App.Current.MainWindow.Close();
            System.Environment.Exit(exitCode);

        }

        private void loadCareplan(Guid carePlanID, bool testPlan, EventHandler DB_SavingChanges, DateTime? startDatePHR, DateTime? endDatePHR)
        {

            try
            {
                App.s_carePlan = new ViewCarePlan(DB_SavingChanges);
                if (testPlan)
                    App.s_carePlan.ActiveCarePlan = App.s_carePlan.DB.CarePlan.First();
                else
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                         App.carePlan.ActiveCarePlan = App.carePlan.DB.CarePlan.Where(cp => cp.Id == carePlanID).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    //SP 1 workaround
                    App.s_carePlan.ActiveCarePlan = App.s_carePlan.DB.CarePlan.Where("it.Id = '" + carePlanID + "'").First();
#endif
            }
            catch (Exception exception)
            {
                showException(exception);
                stopApplication("Could not load health record", "Load fail personal health record", WindowMain.EXIT_LOAD_CAREPLAN);

            }

            // Setup care blog items
            if (!App.s_carePlan.ActiveCarePlan.Item.IsLoaded)
                App.s_carePlan.ActiveCarePlan.Item.Load();

            Item lastItem = null;

            if (App.s_carePlan.ActiveCarePlan.Item.Count > 0)
            {

                foreach (Item i in App.s_carePlan.ActiveCarePlan.Item)
                {// Load related history and tags
                    i.HistoryReference.Load();
                    i.Tag.Load();
                    i.Tag.AssociationChanged += new CollectionChangeEventHandler(TagControl.Tag_AssociationChanged);
                    inferContentFromTags(i); // Find content from tags, contains diagnoses, interventions, care component?

                    if (i.Id == eNursePHR.userInterfaceLayer.Properties.Settings.Default.LastItem) // Point to last item selected
                        lastItem = i;
                }


                setupPHRDateControls(startDatePHR, endDatePHR, false);

                // Setup of item collection change handling (updates combobox)

                App.s_carePlan.ActiveCarePlan.Item.AssociationChanged += new CollectionChangeEventHandler(Item_AssociationChanged);
                Item_AssociationChanged(this, null);

                // Start at last selected item from previous session by default

                if (lastItem != null)
                    lvCareBlog.SelectedItem = lastItem;
            }
            //else
            //    MessageBox.Show("This careplan is empty", "Empty careplan", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

        }

        private void dpStartDatePHR_ValueChanged<DateTime>(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {
         
        }


        private void dpEndDatePHR_ValueChanged<DateTime>(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {

        }

        private void setupPHRDateControls(DateTime? startDatePHR, DateTime? endDatePHR, bool MaxMinLimits)
        {
            

            if (MaxMinLimits)
              setupPHRViewDateControlsMaxMinLimits();


            // Setup date control to the first/oldest PHR entry
            if (startDatePHR == null)
                dpStartDatePHR.Value = App.s_carePlan.OldestPHRViewDate;
            else
                dpStartDatePHR.Value = startDatePHR;
            
            // Setup date control to the last/newest PHR entry
            if (endDatePHR == null)

                dpEndDatePHR.Value = App.s_carePlan.NewestPHRViewDate;
            else
                dpEndDatePHR.Value = endDatePHR;
            
          }

        private void setupPHRViewDateControlsMaxMinLimits()
        {
            // Setup max/min limit for start date
            if (App.s_carePlan.OldestPHRViewDate != null)
                dpStartDatePHR.MinDate = (DateTime)App.s_carePlan.OldestPHRViewDate;

            if (App.s_carePlan.NewestPHRViewDate != null)
                dpStartDatePHR.MaxDate = (DateTime)App.s_carePlan.NewestPHRViewDate;

            // Setup max/min limit for end date
            if (App.s_carePlan.NewestPHRViewDate != null)
                dpEndDatePHR.MaxDate = (DateTime)App.s_carePlan.NewestPHRViewDate;

            if (App.s_carePlan.OldestPHRViewDate != null)
                dpEndDatePHR.MinDate = (DateTime)App.s_carePlan.OldestPHRViewDate;
        }


        /// <summary>
        /// Event handler for collectionchangeevent for item-collection in the active careplan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Item_AssociationChanged(object sender, CollectionChangeEventArgs e)
        {
            
            lvCareBlog.ItemsSource = App.s_carePlan.ActiveCarePlan.Item.OrderByDescending(i => i.History.LastUpdate);
        }

        /// <summary>
        /// Travereses all tags for given item and finds out if this item contains diagnosis, intervention, carecomponent
        /// or folksonomy. The green indicator flag in UI besides the item title signals that this item contains diagnosis,
        /// a blue indicator is an intervention.
        /// </summary>
        /// <param name="item"></param>
        public void inferContentFromTags(Item item)
        {
            // Asssume that we have nothing to begin with
            item.ContainsDiagnosis = false;
            item.ContainsIntervention = false;
            item.ContainsCareComponent = false;
            item.ContainsFolksonomy = false;

            // Make sure that all tags for the item is loaded
            if (!item.Tag.IsLoaded)
                item.Tag.Load();

            // No need to traverse is item has no tags, return please
            if (item.Tag.Count == 0)
                return;

            // Give an indication of whether item contains diagnosis, intervention or carecomponent
            foreach (Tag t in item.Tag)
            {
                if (t.TaxonomyType.Contains("CCC/NursingDiagnosis"))
                    item.ContainsDiagnosis = true;
                else if (t.TaxonomyType.Contains("CCC/NursingIntervention"))
                    item.ContainsIntervention = true;
                else if (t.TaxonomyType.Contains("CCC/CareComponent"))
                    item.ContainsCareComponent = true;

            }

        }

       

        
       
        /// <summary>
        /// Shows the about eNursePHR dialog window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // If a new item is created, then update the end time date control
            setupPHRDateControls(App.s_carePlan.CurrentPHRViewFromDate, null,false);

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

            if (MessageBox.Show("Delete \"" + selItem.Title + "\", are you sure ?", "Delete " + selItem.Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {


                try
                {
                    //if (!selItem.Tag.IsLoaded)
                    //    selItem.Tag.Load();

                    selItem.Tag.AssociationChanged -= new CollectionChangeEventHandler(TagControl.Tag_AssociationChanged); // Disable tag association

                    App.s_carePlan.DB.DeleteObject(selItem.History);

                    infoAcq.Statement.Clear(); // Remove annotation-references
                    if (annotationService.Service.IsEnabled)
                        annotationService.Service.Disable();

                    App.s_carePlan.DB.DeleteObject(selItem);

                    SaveCarePlan();


                }
                catch (UpdateException ee)
                {
                    showUpdateException(ee);
                }
                finally
                {
                    TagControl.lbTags.ItemsSource = null;
                    fdReaderCareBlog.Document = null;
                    fdReaderCareBlog.Visibility = Visibility.Collapsed;
                    TagOverviewControl.buildTagsOverview(App.s_carePlan.ActiveCarePlan);

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

            TagControl.refreshLanguageTranslationForItemTags();

            // Build flowdocument

            //Section sItem = new Section();
            //BlockUIContainer bui = new BlockUIContainer();
            //StackPanel spPanel = new StackPanel();
            //Paragraph pTitle = new Paragraph();
            //pTitle.FontSize = 15;
            //pTitle.Inlines.Add(new Bold(new Run(selItem.Title)));
            ////spPanel.Children.Add(pTitle);
            //sItem.Blocks.Add(pTitle);

            annotationService.changeItemStore(selItem,annotationStoreCargoChanged,hideBtnSaveAnnotations);

            infoAcq.Refresh(annotationService.Service.Store);


            App.s_carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagConverter);


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

            infoAcq.Refresh(annotationService.Service.Store); // In case annotation references is changed...

            App.s_carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagConverter);

        }

        #endregion



       
        
       
        

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

        /// <summary>
        /// Generates a blog of all health entries
        /// TO DO : Date filter - start - end data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCarePlanBlog_Click(object sender, RoutedEventArgs e)
        {
            if (App.s_carePlan == null)
            {
                MessageBox.Show("No active personal health record, is database connectivity OK?", "No active personal health record", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (App.s_carePlan.ActiveCarePlan.Item.Count == 0)
            {
                MessageBox.Show("The current health record is empty, please create at least one entry first", "Empty health record", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (annotationService.Service.IsEnabled)
                annotationService.Service.Disable();

            App.s_carePlan.generateCareBlog(fdReaderCareBlog, App.s_carePlan.ActiveCarePlan, tagConverter, true);

            TagControl.exTags.IsExpanded = false;
            TagOverviewControl.exTaxonomy.IsExpanded = true;
        }

        



        



        public int SaveCarePlan()
        {
            try
            {
                int upd = App.s_carePlan.DB.SaveChanges();
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

       

        private void btnSaveTagComments_Click(object sender, RoutedEventArgs e)
        {
            SaveCarePlan();
            // App.carePlan.showCareplanItem(fdReaderCareBlog, (Item)lvCareBlog.SelectedItem, tagHandler);
            btnSaveTagComments.Visibility = Visibility.Collapsed;
            TagOverviewControl.refresh();
        }

        private void turnOnAnnotationService(Item item)
        {
            annotationService.changeItemStore(item,annotationStoreCargoChanged,hideBtnSaveAnnotations);
          
        }

        private void turnOffAnnotationService()
        {
            annotationService.Service.Disable();
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
                App.s_carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagConverter);
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
                //aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);
                annotationService.IAnnotationStore.saveAnnotation(annotation);

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
                //aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);
                annotationService.IAnnotationStore.saveAnnotation(annotation);

            }
            else
                MessageBox.Show("Please select some text as the diagnostic information", "Empty diagnostic information selection", MessageBoxButton.OK, MessageBoxImage.Information);
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
                //aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);
                annotationService.IAnnotationStore.saveAnnotation(annotation);

            }
            else
                MessageBox.Show("Please select some text as the interventional information", "Empty interventional information selection", MessageBoxButton.OK, MessageBoxImage.Information);

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
                //aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);
                annotationService.IAnnotationStore.saveAnnotation(annotation);

            }
            else
                MessageBox.Show("Please select some text as the medicational information", "Empty medicational information selection", MessageBoxButton.OK, MessageBoxImage.Information);

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

        

        #endregion

       
        

        // Info from https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=2574377&SiteID=1
        void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());

            e.Handled = true;
        }

        /// <summary>
        /// Toggles between fullscreen and normal view of an item/heath entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (!FullScreenItem) // In case we are not in full screen, switch to full screen
            {
                prevTagsWidth = gcTags.Width; // Remember last position of the grid-splitter for tags
                gcTags.Width = new GridLength(0);

                prevTaxonomyWidth = gcTaxonomy.Width; // Remember last pos. of grid-splitter for taxonomy
                gcTaxonomy.Width = new GridLength(0);

                FullScreenItem = true; // We are now in full screen yeah
            }
            else
            {
                // Restore last pos. of grid-splitters for tags and taxonomy
                gcTags.Width = prevTagsWidth;
                gcTaxonomy.Width = prevTaxonomyWidth;

                FullScreenItem = false; // Back to normal view mode

            }

            // Use page mode reading when we are not in full screen and scroll mode in full screen
            if (!FullScreenItem)
                fdReaderCareBlog.SwitchViewingMode(FlowDocumentReaderViewingMode.Page);
            else
                fdReaderCareBlog.SwitchViewingMode(FlowDocumentReaderViewingMode.Scroll);

        }



        #region Explorer



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
                
                App.s_cccFrameWork.loadFrameworkLanguageAnalysis();
                CCCTaxonomyControl.refreshFrameworkLanguageAnalysis();
            }
            catch (Exception exception)
            {
                showException(exception);
            }
        }

       
       
        
        #endregion

        
        //private TextBlock getActionTypeCP()
        //{
        //    // Based on info: http://joshsmithonwpf.wordpress.com/2007/06/28/how-to-use-findname-with-a-contentcontrol/
        //    // Accessed : 10 march 2008

        //    ContentPresenter contentPresenter = VisualTreeHelper.GetChild(ccActionType, 0) as ContentPresenter;

        //    TextBlock tb = (TextBlock)ccActionType.ContentTemplate.FindName("tbTemplateConceptActionType", contentPresenter);
        //    FrameworkElement fe = tb.Parent as FrameworkElement;

        //    return tb;
        //}

        
        #endregion


        private void btnTextNote_Click(object sender, RoutedEventArgs e)
        {
            // Check for empty selection
            if (!checkSelection(fdReaderCareBlog))
                return;

            if (!fdReaderCareBlog.Selection.IsEmpty)
            {

                Annotation annotation = AnnotationHelper.CreateTextStickyNoteForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName);

                annotationService.IAnnotationStore.saveAnnotation(annotation);

            }
            else
                MessageBox.Show("Please select some text associated with the text note", "Empty text note association (selection is empty)", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void btnRemoveAnnotations_Click(object sender, RoutedEventArgs e)
        {
            // Check for empty selection
            if (!checkSelection(fdReaderCareBlog))
                return;

            if (!fdReaderCareBlog.Selection.IsEmpty)
            {

                AnnotationHelper.ClearHighlightsForSelection(AnnotationService.GetService(fdReaderCareBlog));

                AnnotationHelper.DeleteTextStickyNotesForSelection(AnnotationService.GetService(fdReaderCareBlog));

                AnnotationHelper.DeleteInkStickyNotesForSelection(AnnotationService.GetService(fdReaderCareBlog));

        
            }
            else
                MessageBox.Show("Empty selection to delete annotations from", "Empty selection", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        private void btnSaveTextInkAnnotation_Click(object sender, RoutedEventArgs e)
        {
            annotationService.IAnnotationStore.saveAnnotationWithCargoChanged();
            btnSaveTextInkAnnotation.Visibility = Visibility.Collapsed;
        }

        private void btnInkNote_Click(object sender, RoutedEventArgs e)
        {
            // Check for empty selection
            if (!checkSelection(fdReaderCareBlog))
                return;

            if (!fdReaderCareBlog.Selection.IsEmpty)
            {

                Annotation annotation = AnnotationHelper.CreateInkStickyNoteForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName);

                annotationService.IAnnotationStore.saveAnnotation(annotation);

            }
            else
                MessageBox.Show("Please select some text associated with the ink note", "Empty ink note association (selection is empty)", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        /// <summary>
        /// Event handler for AnnotationResourceChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void annotationStoreCargoChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            if (e.Annotation.AnnotationType.Name == "TextStickyNote" ||
                 e.Annotation.AnnotationType.Name == "InkStickyNote")
            {
                // Turn on save button
                WindowMain wndMain = App.Current.MainWindow as WindowMain;
                wndMain.btnSaveTextInkAnnotation.Visibility = System.Windows.Visibility.Visible;
            }

        }

        // Event handler for hiding annotations
        public void hideBtnSaveAnnotations(object sender, EventArgs e)
        {
            btnSaveTextInkAnnotation.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for SavingChanges event on object-context, entity framework
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void updateCCCDBSaveStatus(object sender, EventArgs e)
        {
            ObjectContext ctx = sender as ObjectContext;
            string prevStatusMsg = (wndMain.TryFindResource("StatusHandler") as StatusHandler).StatusMsg;

            int added = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Count();
            int deleted = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Count();
            int modified = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Count();

#if (VERBOSE_SAVE_CHANGES)
            changeStatus("Saving changes (" +
                         added.ToString() + " added " +
                         deleted.ToString() + " deleted " +
                         modified.ToString() + " modified )");
#else
            changeStatus("Saving changes...");
#endif

            // Setup timer 
            // The dispatcher timer allows update on the user interface thread, I tried to used
            // timer-class first, but was not allowed to make ui changes from another thread
            // More info: http://blogs.msdn.com/shen/archive/2008/02/28/changing-the-ui-in-a-multi-threaded-wpf-application.aspx
            // Accessed : 16 september 2008

            dispatcherTimerUI = new DispatcherTimer(DispatcherPriority.Background);
            dispatcherTimerUI.Interval = TimeSpan.FromMilliseconds(2000);
            dispatcherTimerUI.Tag = prevStatusMsg;
            dispatcherTimerUI.Tick += new EventHandler(removeDBSaveStatus);
            dispatcherTimerUI.Start();

        }

        /// <summary>
        /// Event handler on the dispatch timer for removal of "Saving changes..." message and insert the previous message available in the Tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeDBSaveStatus(object sender, EventArgs e)
        {
            dispatcherTimerUI.Stop();
            changeStatus((sender as DispatcherTimer).Tag as string);
        }

        /// <summary>
        /// Changes the status message
        /// </summary>
        /// <param name="statusMsg"></param>
        private void changeStatus(string statusMsg)
        {
            StatusHandler handler = wndMain.TryFindResource("StatusHandler") as StatusHandler;
            handler.StatusMsg = statusMsg;
        }
    }
}

   
    

