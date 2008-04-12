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
using CCC.BusinessLayer;
using System.ComponentModel;
// Copyright (c) 2007 Kevin Moore j832.com
using Microsoft.Samples.KMoore.WPFSamples.DateControls;
using System.Windows.Annotations;
using System.IO;
using System.Data.Objects;
using System.Xml;
using System.Diagnostics;


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

namespace CCC.UI
{

    public partial class WindowMain : Window
        {

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

        public WindowMain()
            {
           
                    InitializeComponent();
                
               
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

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                // Enable sharing of properties across layers

                App.Current.Properties["Version"] = Properties.Settings.Default.Version;
                App.Current.Properties["LanguageName"] = Properties.Settings.Default.LanguageName;

                // Setup event handling of hyperlinks

                fdReaderCareBlog.AddHandler(Hyperlink.RequestNavigateEvent,new RequestNavigateEventHandler(hyperlink_RequestNavigate));

                // Load CCC Framework 

                try
                {
                    App.cccFrameWork = new ViewCCCFrameWork(Properties.Settings.Default.LanguageName,
                        Properties.Settings.Default.Version);
                   
                }
                catch (Exception exception)
                {
                    
                    showException(exception);
                    App.Current.Shutdown(1);
                  
                }

                refreshCCCFramework();

                // Load a test care plan

                try
                {
                    App.carePlan = new ViewCarePlan();
                    App.carePlan.ActiveCarePlan = App.carePlan.DB.CarePlan.First();
                }
                catch (Exception exception)
                {
                    showException(exception);
                    MessageBox.Show("Could not load careplan, application will end now", "Loading of careplan failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    this.Close();
                }
                
                
                // Setup care blog items
                if (!App.carePlan.ActiveCarePlan.Item.IsLoaded)
                    App.carePlan.ActiveCarePlan.Item.Load();

                Item lastItem = null;

               
                foreach (Item i in App.carePlan.ActiveCarePlan.Item)
                {// Load related history and tags
                    i.HistoryReference.Load();
                    i.Tag.Load();
                    i.Tag.AssociationChanged += new CollectionChangeEventHandler(Tag_AssociationChanged);
                    inferContentFromTags(i); // Find content from tags, contains diagnoses, interventions, care component?
                
                    if (i.Id == Properties.Settings.Default.LastItem) // Point to last item selected
                        lastItem = i;
                }


                // Start annotation service

                aService = new myAnnotationService(fdReaderCareBlog);
               
                // Setup of item collection change handling (updates combobox)

                App.carePlan.ActiveCarePlan.Item.AssociationChanged += new CollectionChangeEventHandler(Item_AssociationChanged);
                Item_AssociationChanged(this, null);
               
                // Start at last selected item from previous session by default
              
                if (lastItem != null)
                    lvCareBlog.SelectedItem = lastItem;

                // Show all tags for the current careplan
                buildAllTags(App.carePlan.ActiveCarePlan);

               

                /* LANGUAGE attrib change */
                tbUserName.Text = "Logged in as " + System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName + " Kultur for UI: " + Thread.CurrentThread.CurrentUICulture.DisplayName.ToString();

                lvAnnotation.ItemsSource = infoAcq.CvStatement;


                
                
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


            void buildAllTags(CarePlan cp)
            {
                string version = Properties.Settings.Default.Version;
                string languageName = Properties.Settings.Default.LanguageName;

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
               var qOutcome = tag.Outcome.Where(o => o.ActualDate == null).OrderBy(o => o.ExpectedDate);
               if (qOutcome.Count<Outcome>() > 0)
               {
                   latest = (Outcome)qOutcome.Take(1).First();
                   tag.LatestOutcome = latest.ExpectedOutcome;
                   tag.LatestOutcomeModifier = App.cccFrameWork.DB.OutcomeType.Where(o => o.Code == latest.ExpectedOutcome &&
                       o.Version == Properties.Settings.Default.Version && o.Language_Name == Properties.Settings.Default.LanguageName).First().Concept;
               }

               else
               {
                   tag.LatestOutcome = null;
                   tag.LatestOutcomeModifier = null;
               }

           }

            public void refreshTags()
            {

               string version = Properties.Settings.Default.Version;
               string languageName = Properties.Settings.Default.LanguageName;
   
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
                Properties.Settings.Default.LastItem = selItem.Id;
                Properties.Settings.Default.Save();

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

                FrameworkOutcomeType fOutcomeType = null;
                ActionType fActionType = null;

               string comment = null;


                if (selItem == null)
                {
                    MessageBox.Show("You have not selected an careplan item to attach this tag to", "No careplan item selected");
                    return;
                }

                 
                if (transfer.GetDataPresent("CCC/CareComponent"))
                {
                    taxonomyType = "CCC/CareComponent";
                    Care_component component = (Care_component)transfer.GetData("CCC/CareComponent");
                    taxonomyGuid = tagHandler.getTaxonomyGuidCareComponent(component.Code, Properties.Settings.Default.Version);
                 
                }

                if (transfer.GetDataPresent("CCC/OutcomeType"))
                {
                    fOutcomeType = (FrameworkOutcomeType)transfer.GetData("CCC/OutcomeType");
                    // Find guid in reference terminology
                    taxonomyOutcomeAttachmentGuid = tagHandler.getTaxonomyGuidOutcomeType(fOutcomeType.Code, Properties.Settings.Default.Version);
                }

                if (transfer.GetDataPresent("CCC/NursingDiagnosis"))
                {
                    taxonomyType = "CCC/NursingDiagnosis";
                    FrameworkDiagnosis fDiag = (FrameworkDiagnosis)transfer.GetData("CCC/NursingDiagnosis");
                    comment = fDiag.Comment;
                    // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                    taxonomyGuid = tagHandler.getTaxonomyGuidNursingDiagnosis(fDiag.ComponentCode, fDiag.MajorCode, fDiag.MinorCode, Properties.Settings.Default.Version);

                }

                if (transfer.GetDataPresent("CCC/ActionType"))
                {
                    taxonomyType = "CCC/ActionType";
                    fActionType = (ActionType)transfer.GetData("CCC/ActionType");
                    taxonomyActionTypeAttachmentGuid = tagHandler.getTaxonomyGuidActionType(fActionType.Code, Properties.Settings.Default.Version);
                   
                }

                if (transfer.GetDataPresent("CCC/NursingIntervention"))
                    {
                    taxonomyType = "CCC/NursingIntervention";
                    FrameworkIntervention fInterv = (FrameworkIntervention)transfer.GetData("CCC/NursingIntervention");

                    comment = fInterv.Comment;
                    // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                    taxonomyGuid = tagHandler.getTaxonomyGuidNursingIntervention(fInterv.ComponentCode, fInterv.MajorCode, fInterv.MinorCode, Properties.Settings.Default.Version);
                   
                   
                }


                 
                // Add tag, if found in taxonomy

                if (taxonomyType == "CCC/NursingDiagnosis" || taxonomyType == "CCC/NursingIntervention")
                {
                    selItem.Tag.AssociationChanged -= new CollectionChangeEventHandler(Tag_AssociationChanged); // Remove event handling now
                    Tag newTag = CCC.BusinessLayer.Tag.CreateTag(Guid.NewGuid(), taxonomyType, taxonomyGuid);
                  
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

                        ActionT newActionType = ActionT.CreateActionT(fActionType.Code, tbConceptActionType.Text, taxonomyActionTypeAttachmentGuid, newTag.Id);
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
                    MessageBox.Show("No tag selected to delete", "No tag selected", MessageBoxButton.OK);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Tag " + selTag.Concept + " will be deleted, are you sure?", "Delete "+selTag.Concept, MessageBoxButton.YesNo);
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
                FrameworkDiagnosis selDiag = lbNursingDiagnosis.SelectedItem as FrameworkDiagnosis;
                if (selDiag == null)
                    return;

                selDiag.Comment = tbReasonDiagnosis.Text;

            }

            private void tbReasonIntervention_TextChanged(object sender, TextChangedEventArgs e)
            {
                FrameworkIntervention selInterv = lbNursingInterventions.SelectedItem as FrameworkIntervention;
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
                App.carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagHandler);
                turnOnAnnotationService(selItem);
            }

            #endregion


            #region Annotation toolbar

            private void btnCaptureDiseaseStatement_Click(object sender, RoutedEventArgs e)
            {

                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Disease;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("DiseaseHighlightColor"));

                    this.addContent(annotation, ContentType.Disease);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the disease information", "Empty disease information selection", MessageBoxButton.OK);
            }


            private void btnCaptureDiagnosticStatement_Click(object sender, RoutedEventArgs e)
            {
                
                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Diagnostic;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("DiagnosisHighlightColor"));
                    
                    this.addContent(annotation, ContentType.Diagnostic);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the diagnostic information", "Empty diagnostic information selection", MessageBoxButton.OK);
            }


            private void btnCaptureInterventionalStatement_Click(object sender, RoutedEventArgs e)
            {
                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Interventional;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("InterventionHighlightColor"));
                    
                    this.addContent(annotation, ContentType.Interventional);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the interventional information", "Empty interventional information selection", MessageBoxButton.OK);

            }

            private void btnCaptureMedicationalStatement_Click(object sender, RoutedEventArgs e)
            {
                if (!fdReaderCareBlog.Selection.IsEmpty)
                {
                    contentType = ContentType.Medication;

                    Annotation annotation = AnnotationHelper.CreateHighlightForSelection(AnnotationService.GetService(fdReaderCareBlog), System.Environment.UserName, (Brush)this.TryFindResource("MedicationHighlightColor"));

                    this.addContent(annotation, ContentType.Medication);
                    aService.IAnnotationStore.saveAnnotation(annotation, lvCareBlog.SelectedItem as Item);

                }
                else
                    MessageBox.Show("Please select some text as the medicational information", "Empty medicational information selection", MessageBoxButton.OK);

            }

            /// <summary>
            /// Inserts a XML-tree in the contents section of an annotation to capture information that was annotated
            /// this might be a diagnostics statement or an interventional statement
            /// <eNurseCP>
            ///     <Statement ContentType="Diagnosis" Content="???"
            /// </eNurseCP>
            ///  </summary>
            /// <param name="annotation"></param>
            /// <param name="contentType"></param>
            public void addContent(Annotation annotation, ContentType contentType)
            {
                XmlDocument xDoc = new XmlDocument();
                XmlElement xHeading = xDoc.CreateElement("eNurseCP");
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

                XmlNode xe = xdoc.SelectSingleNode("eNurseCP/Statement[1]");
                return xe.Attributes["Content"].Value;

            }

            public ContentType getContentType(Annotation annotation)
            {

                if (annotation.Anchors[0].Contents.Count == 0)
                    return ContentType.Null;

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

                XmlNode xe = xdoc.SelectSingleNode("eNurseCP/Statement[1]"); // XPath-query
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
                    MessageBox.Show("No aquired diagnostic information selected", "No acquired diagnostic information", MessageBoxButton.OK);
                    return;
                }

                if (getContentType(selAnnotation) == ContentType.Diagnostic)
                    tbReasonDiagnosis.Text += firstCharToUpper(getAnnotationContent(selAnnotation));
                
                else
                    MessageBox.Show("Please select diagnostic information", "Select diagnostic information", MessageBoxButton.OK);
            }

            private void btnAcquireInterventionalInformation_Click(object sender, RoutedEventArgs e)
            {
                Annotation selAnnotation = lvAnnotation.SelectedItem as Annotation;
                if (selAnnotation == null)
                {
                    MessageBox.Show("No aquired interventional or medicational information selected", "No acquired interventional or medicational information", MessageBoxButton.OK);
                    return;
                }

                ContentType contentType = getContentType(selAnnotation);
                if (contentType == ContentType.Interventional || contentType == ContentType.Medication)
                    tbReasonIntervention.Text += firstCharToUpper(getAnnotationContent(selAnnotation));
                else
                    MessageBox.Show("Please select interventional or medicational information", "Select interventional or medicational information", MessageBoxButton.OK);
            
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
           








        }
}

   
    

