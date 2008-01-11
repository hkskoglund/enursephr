using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
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
using Microsoft.Samples.KMoore.WPFSamples.DateControls;
using System.Windows.Annotations;


namespace CCC.UI
{

    public partial class WindowMain : Window
        {

            myAnnotationService annotationservice;

            char FilterCode;
            string FilterSearch;

            //bool dont = false; // true if selection in listbox, and not update combo
           // Diagnosi changeDiagnosis = null;

            DispatcherTimer timer;
            //DispatcherTimer annotationTimer;

            static List<myImage> coll = new List<myImage>();
            ListCollectionView lcoll = new ListCollectionView(coll);


            ListCollectionView lcollviewDiagnosisEntities;

            public WindowMain()
            {
                InitializeComponent();
            }

            private void updateDatabase(object sender, EventArgs e)
            {
                
                
                tbUserName.Text = DateTime.Now.ToString();
                
                carePlanSubmitHandler();

                //Keep grouping and sorting please...

                System.Collections.ObjectModel.ObservableCollection<GroupDescription> 
                    gruppering = App.carePlan.cvDiagnoses.GroupDescriptions;
                
                SortDescriptionCollection 
                    sortering = App.carePlan.cvDiagnoses.SortDescriptions;
              
                int careplanid = App.carePlan.Id;
                App.carePlan.DB.Dispose();
              
                App.carePlan = new ViewCarePlan(careplanid,App.cccFrameWork.DB);

                
                /*
                // Gjenoppfrisker dataene ved å lage en ny datakontekst
                System.Data.Common.DbConnection conn = App.carePlan.DB.Connection;
               //E App.carePlan.DB.Dispose();
               //E  App.carePlan.DB = new CarePlanDBDataContext(

                App.carePlan.DB = new PleieplanEntitiesv2(conn.ConnectionString);

                App.carePlan.Diagnoses = App.carePlan.DB.Diagnosis.Where(d => d.CarePlan.Id == App.carePlan.Id).ToList();
                */

                tbUserName.Text = "Read "+App.carePlan.DB.Diagnosis.Count().ToString()+" diagnoses"+" at "+DateTime.Now.ToLongTimeString();
                App.carePlan.updateExtendedData(App.cccFrameWork.DB);
             
                lbCarePlanDiagnoses.ItemsSource = null;
             
                  
                //EApp.carePlan.cvDiagnoses = new ListCollectionView(App.carePlan.Diagnoses);
                App.carePlan.cvDiagnoses = new ListCollectionView(App.carePlan._diagnoses);

                foreach (GroupDescription g in gruppering)
                  App.carePlan.cvDiagnoses.GroupDescriptions.Add(g);
                
                foreach (SortDescription s in sortering)
                    App.carePlan.cvDiagnoses.SortDescriptions.Add(s);
               
                App.carePlan.cvDiagnoses.Refresh();
                lbCarePlanDiagnoses.ItemsSource = App.carePlan.cvDiagnoses;
                

                 App.carePlan.PrettyCarePlan_Update();

                 annotationservice.stopAnnotationService();
                 annotationservice.startAnnotationService(fdReaderPrettyCarePlan, lbAnnotations, App.carePlan);
             

                }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {

                fdReaderPrettyCarePlan.Document = App.carePlan.fdPrettyCarePlan;

                lcoll.SortDescriptions.Add(new SortDescription("DateTaken", ListSortDirection.Ascending));
                
                MenuItemOm_Click(this, null);

                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(updateDatabase);
                timer.Interval = new TimeSpan(0,1,0);
                timer.Start();

                annotationservice = new myAnnotationService(fdReaderPrettyCarePlan, App.carePlan, lbAnnotations,timer, myInk);

                tbUserName.Text = "Innlogget som : " + System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName + " Kultur for UI: " + Thread.CurrentThread.CurrentUICulture.DisplayName.ToString();


                lbCarePlanDiagnoses.ItemsSource = App.carePlan.cvDiagnoses;
              
                lbCareComponent.GroupStyle.Add(new GroupStyle());
                lbCareComponent.ItemsSource = App.cccFrameWork.cvComponents;
                lbCareComponent.SelectedIndex = 1; // Default to selfcare

               
                // Setup careplan templates sub-UI

                CarePlanTemplateDBDataContext templateDB = new CarePlanTemplateDBDataContext();

                ListCollectionView cvCarePlanTemplate = new ListCollectionView(templateDB.TemplateCarePlans.ToList());

                cvCarePlanTemplate.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                cvCarePlanTemplate.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));

                lbCarePlanTemplates.ItemsSource = cvCarePlanTemplate;


                // Test entity framework

                lcollviewDiagnosisEntities = new ListCollectionView(App.carePlan.DB.Diagnosis.ToList());
                lbCarePlanDiagnosesEntities.ItemsSource = lcollviewDiagnosisEntities;
               

                

                //  cvcarePlanInterventions.GroupDescriptions.Add(new PropertyGroupDescription("ComponentName"));
                //    cvcarePlanInterventions.SortDescriptions.Add(new SortDescription("ComponentName", ListSortDirection.Ascending));

                //     lbCarePlanDiagnoses.GroupStyle.Add(new GroupStyle());

                //lbCarePlanDiagnoses.ItemsSource = App.cvcarePlanDiagnoses;
                //          int tall = lbCarePlanDiagnoses.Items.Count;
                //        cvcarePlanDiagnoses.Refresh();

                // Make sure that an AnnotationService isn’t already enabled.


                
                App.carePlan.PrettyCarePlan_Update();

            }

          
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

                Nursing_Diagnosi nd = item as Nursing_Diagnosi;

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

            private void MenuItemOm_Click(object sender, RoutedEventArgs e)
            {
                WindowCopyright wc = new WindowCopyright();
                wc.ShowDialog();
            }

            private void lbCareComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (tbSearch.Text != "")
                    return; // Make sure were not filtering diagnoses while search is active

                ListBox lb = sender as ListBox;

                if (lb.SelectedIndex == -1) // Tom selected item-liste
                    return;

                Care_component cc = lb.SelectedItem as Care_component;
                FilterCode = cc.Code;

                App.cccFrameWork.cvDiagnoses.Filter = new Predicate<object>(FilterOutDiagnoses);
                App.cccFrameWork.cvDiagnoses.Refresh();

                App.cccFrameWork.cvInterventions.Filter = new Predicate<object>(FilterOutInterventions);
                App.cccFrameWork.cvInterventions.Refresh();


                lbNursingDiagnosis.ItemsSource = App.cccFrameWork.cvDiagnoses;
                lbNursingInterventions.ItemsSource = App.cccFrameWork.cvInterventions;


            }

            private bool FilterOutDiagnoses(object item)
            { // Based on example from Beatrize Costa blog, accessed 25 november 2007

                Nursing_Diagnosi nd = item as Nursing_Diagnosi;


                if (nd == null)
                    return false;

                int result = nd.ComponentCode.CompareTo(FilterCode);

                if (result == 0) return true;

                return false;

            }

            private bool FilterOutInterventions(object item)
            {
                // Based on example from Beatrize Costa blog, accessed 25 november 2007

                Nursing_Intervention nd = item as Nursing_Intervention;


                if (nd == null)
                    return false;

                int result = nd.ComponentCode.CompareTo(FilterCode);

                if (result == 0) return true;

                return false;

            }

            private void lbNursingDiagnosis_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                /*
               ListBox lb = sender as ListBox;
               Nursing_Diagnosi nd = lb.SelectedItem as Nursing_Diagnosi;

               ccNursingDiagnosis.Content = nd; FLYTTET TIL PLEIEPLAN */
            }

            private void lbNursingDiagnosis_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

                ListBox lb = sender as ListBox;
                Nursing_Diagnosi nursingDiagnosis = lb.SelectedItem as Nursing_Diagnosi;

                // Create new Diagnosis object

                //E Diagnosi diag = new Diagnosi();

                Diagnosi diag = new Diagnosi();

                //App.carePlan.DB.AddToDiagnosis(diag); // Kaller AddObject på objekt-modellen

               // App.carePlan.DB.AddToDiagnosis(diag);
                // Copy from framework diagnosis to careplan diagnosis
               
                //Ediag.CarePlan.Id = 1; // Må endres etterhvert !!!!

                diag.CarePlan = App.carePlan.DB.CarePlan.First(c => c.Id == 1);
               //E diag.CarePlan.Id = 1;
                diag.cccId = nursingDiagnosis.DiagnosisID;
                diag.Concept = nursingDiagnosis.Concept;
                diag.ComponentName = nursingDiagnosis.Care_component.Component;
                diag.CreationDate = DateTime.Now;
                diag.CreationDateString = diag.CreationDate.Value.ToLongDateString();
                diag.Definition = nursingDiagnosis.Definition;

               
                App.carePlan.InsertDiagnosis(diag);

                App.carePlan.cvDiagnoses.Refresh();


                /*
                diag.ComponentCode = nursingDiagnosis.ComponentCode;
                diag.MajorCode = nursingDiagnosis.MajorCode;
                if (nursingDiagnosis.MinorCode != null)
                    diag.MinorCode = (short)nursingDiagnosis.MinorCode;
                else
                    diag.MinorCode = 0;

               //+reasondiagnosis!!
           
                
                
                
                
            //        myNursingDiagnosis my = new myNursingDiagnosis(nursingDiagnosis.Care_component.Component,
            //        nursingDiagnosis.Concept, cccFrameWork.Outcomes);

                //carePlan.Diagnoses.Add(diag);
            
            
                cvcarePlanDiagnoses.Refresh();


                lbCarePlanDiagnoses.ItemsSource = cvcarePlanDiagnoses; */
                lbCarePlanDiagnoses.Visibility = Visibility.Visible;

                App.carePlan.PrettyCarePlan_Update();


            }

            private void lbCarePlanDiagnoses_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {

              
                App.carePlan.cvDiagnoses.Refresh();

                if (App.carePlan.DB.Diagnosis.Count() == 0)
                    lbCarePlanDiagnoses.Visibility = Visibility.Collapsed;

                App.carePlan.PrettyCarePlan_Update();


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

            private void lbCarePlanInterventions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                /*          carePlan.Interventions.Remove((myNursingIntervention)cvcarePlanInterventions.CurrentItem);
                          cvcarePlanInterventions.Refresh();

                          if (carePlan.Interventions.Count == 0)
                              lbCarePlanInterventions.Visibility = Visibility.Collapsed;
                 */
            }

            private void lbCarePlanDiagnoses_LostFocus(object sender, RoutedEventArgs e)
            {
                //lbCarePlanDiagnoses.SelectedIndex = -1;
            }
           
            private void rbGroupByComponents_Checked(object sender, RoutedEventArgs e)
            {

                if (cbGroupByTime != null)
                cbGroupByTime.IsChecked = false;
                App.carePlan.GroupByComponentName = true;
                //App.carePlan.cvDiagnoses.GroupDescriptions.Clear();
                //App.carePlan.cvDiagnoses.GroupDescriptions.Add(new PropertyGroupDescription("ComponentName"));
                //App.carePlan.cvDiagnoses.SortDescriptions.Add(new SortDescription("ComponentName", ListSortDirection.Descending));
                //App.carePlan.cvDiagnoses.Refresh();

                /*
                cvcarePlanInterventions.GroupDescriptions.Add(new PropertyGroupDescription("ComponentName"));
                cvcarePlanInterventions.Refresh(); */
                //App.carePlan.PrettyCarePlan_Update();


            }

            private void rbGroupByComponents_Unchecked(object sender, RoutedEventArgs e)
            {
                App.carePlan.GroupByComponentName = false;
                ////App.carePlan.cvDiagnoses.GroupDescriptions.Clear();
                ////App.carePlan.cvDiagnoses.Refresh();
                /////*
                //cvcarePlanInterventions.GroupDescriptions.Clear();
                //cvcarePlanInterventions.Refresh(); */
                //App.carePlan.PrettyCarePlan_Update();

            }

            private void menuDeleteDiagnosis_Click(object sender, RoutedEventArgs e)
            {
                Diagnosi deleteDiagnosis = (Diagnosi)App.carePlan.cvDiagnoses.CurrentItem;

                App.carePlan.PrettyCarePlanDeleteDiagnosis(deleteDiagnosis.FlowDiagnosis);
                App.carePlan.DeleteDiagnosis(deleteDiagnosis);
                App.carePlan.cvDiagnoses.Refresh();


                
                lbCarePlanDiagnoses.SelectedIndex = -1; // Unselect diagnoses

                if (App.carePlan.DB.Diagnosis.Count() == 0)
                    lbCarePlanDiagnoses.Visibility = Visibility.Collapsed;

                //App.carePlan.PrettyCarePlan_Update();

            }

            private void menuChangeDiagnosis_Click(object sender, RoutedEventArgs e)
            {
                timer.Stop();
                borderNursingDiagnosisDetailChange.Visibility = Visibility.Visible;
           //     borderNursingDiagnosisDetail.Visibility = Visibility.Collapsed;
            //   cbMoreDiagnosisInformation.IsChecked = false;
                ccNursingDiagnosisChange.Visibility = Visibility.Visible;
                //changeDiagnosis = (Diagnosi)lbCarePlanDiagnoses.SelectedItem;
                //ccNursingDiagnosis.Content = changeDiagnosis;
              

            }

            private void prettyCarePlanPrint_Click(object sender, RoutedEventArgs e)
            {
                
                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                    pd.PrintDocument(((IDocumentPaginatorSource)App.carePlan.fdPrettyCarePlan).DocumentPaginator, "Pretty careplan");

            }

            private void lbCarePlanDiagnoses_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
             //   cbMoreDiagnosisInformation.ToolTip = null;
                  
                ListBox lb = sender as ListBox;
                if (lb.SelectedItem != null)
                {
                    Diagnosi md = lb.SelectedItem as Diagnosi;
                    //ccNursingDiagnosis.Content = md;
                    ccNursingDiagnosisChange.Content = md;
                }
                //else
                //{
                //    ccNursingDiagnosisChange.Visibility = Visibility.Collapsed;
                //    ccNursingDiagnosis.Visibility = Visibility.Collapsed;
                //}
            }

            private void btnStoreNursingDiagnosisDetails_Click(object sender, RoutedEventArgs e)
            {
                carePlanSubmitHandler();
                Section flowDiag = ((Diagnosi)App.carePlan.cvDiagnoses.CurrentItem).FlowDiagnosis;
              
                //ccNursingDiagnosisChange.Visibility = Visibility.Collapsed;
                // lbCarePlanDiagnoses.Visibility = Visibility.Visible;
                App.carePlan.cvDiagnoses.Refresh();
                App.carePlan.PrettyCarePlan_Update();
                timer.Start(); // Stopped in change diagnosis dialog
            }

            private void cbOutCome_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                /*
                if (dont == true)
                {
                    dont = false;
                    return;
                }

                  changeDiagnosis.Outcome = Convert.ToInt16((sender as ComboBox).SelectedValue.ToString());
                cvcarePlanDiagnoses.Refresh();
                PrettyCarePlan_Update();
                 */
            }

            private void dpOutComeEvalDate_ValueChanged(object sender, RoutedEventArgs e)
            {
                /*
                DatePicker dp = sender as DatePicker;
                changeDiagnosis.OutcomeEvalDate = (System.DateTime)dp.Value;
                cvcarePlanDiagnoses.Refresh();
                PrettyCarePlan_Update(); */
            }

            //private void cbMoreDiagnosisInformation_Checked(object sender, RoutedEventArgs e)
            //{
            //    if (lbCarePlanDiagnoses.SelectedItem != null)
            //    {
            //        cbMoreDiagnosisInformation.ToolTip = null;
            //        borderNursingDiagnosisDetail.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        cbMoreDiagnosisInformation.IsChecked = false;
            //        cbMoreDiagnosisInformation.ToolTip = "Ingen diagnose er valgt!";
            //    }
            //}

            //private void cbMoreDiagnosisInformation_Unchecked(object sender, RoutedEventArgs e)
            //{
            //    borderNursingDiagnosisDetail.Visibility = Visibility.Collapsed;
            //}

            private void lbAnnotations_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                Annotation a = (Annotation)(sender as ListBox).SelectedItem;
                if (a != null)
                {
                    IAnchorInfo info;

                    info = AnnotationHelper.GetAnchorInfo(annotationservice.service, a);
                   

                    if (info == null)
                    {
                        MessageBox.Show("Fant ikke anker (tekstområde) informasjon. Ankeret ligger trolig utenfor nåværende pleieplan etter at informasjon er blitt fjernet.", "Annotasjon anker feil");
                        return;
                    }
                    TextAnchor resolvedAnchor = info.ResolvedAnchor as TextAnchor;
                    TextPointer pointerStart = (TextPointer)resolvedAnchor.BoundingStart;
                    TextPointer pointerEnd = (TextPointer)resolvedAnchor.BoundingEnd;
                   
                   

                    pointerStart.Paragraph.BringIntoView();
               //     pointerStart.Paragraph.FontSize = 20;
                  //  pointerStart.GetCharacterRect(LogicalDirection.Backward);
                    ccAnnotations.Text = "Ingenting foreløpig!";
                }
            }

            private void carePlanSubmitHandler()
            {
                try
                {
                    //E ChangeSet cset = App.carePlan.DB.GetChangeSet();

                    int added = App.carePlan.DB.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Count();
                    int deleted = App.carePlan.DB.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted).Count();
                    int updated = App.carePlan.DB.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Modified).Count();

                    //E tbUserName.Text = "Setter inn :"+ cset.Inserts.Count()+" Sletter : "+cset.Deletes.Count()+" Oppdaterer : "+cset.Updates.Count();

                    tbUserName.Text = "Setter inn :" + added + " Sletter : " + deleted + " Oppdaterer : " + updated;

                    //E  App.carePlan.DB.SubmitChanges(ConflictMode.ContinueOnConflict);
                    App.carePlan.DB.SaveChanges();

                }
                catch (System.Data.Linq.ChangeConflictException ee) // Concurrency control for LINQ to SQL
                {
                    /*
                    foreach (ObjectChangeConflict conflict in App.carePlan.DB.ChangeConflicts)
                    {

                        // conflict.Object is null when object could not be converted
                        Annot a = conflict.Object as Annot;
                        Diagnosi d = conflict.Object as Diagnosi;

                        if (conflict.IsDeleted)
                        {

                            if (a != null)
                            {
                                WindowChangeConflict wc = new WindowChangeConflict();
                                wc.ShowException("Noen har slettet denne annotasjonen", ee, conflict);
                                bool result = (bool)wc.ShowDialog();
                                conflict.Resolve(RefreshMode.OverwriteCurrentValues, true);
                                annotationservice.stopAnnotationService();
                                annotationservice.startAnnotationService(fdReaderPrettyCarePlan, lbAnnotations);
                               
                            }

                            if (d != null)
                            {
                                WindowChangeConflict wc = new WindowChangeConflict();
                                wc.ShowException("Noen har slettet denne diagnosen", ee, conflict);
                                bool result = (bool)wc.ShowDialog();
                                conflict.Resolve(RefreshMode.OverwriteCurrentValues, true);
                                App.carePlan.Diagnoses.Remove(d);
                                App.carePlan.cvDiagnoses.Refresh();
                            }
                        }
                        else
                        {
                            WindowChangeConflict wc = new WindowChangeConflict();
                            wc.ShowException("Noen endret dataene", ee, conflict);
                            wc.ShowDialog();
                            
                        } 
                    } */
                    //EApp.carePlan.DB.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    App.carePlan.DB.SaveChanges();
                }
                catch (System.Data.OptimisticConcurrencyException ex) // Concurrency handling for entity framework
                {
                    WindowConcurrencyException wc = new WindowConcurrencyException();
                    wc.ShowException(ex);
                    wc.ShowDialog();




                }
                
            }

            private void Window_Closing(object sender, CancelEventArgs e)
            {
                timer.IsEnabled = false;
                //annotationTimer.IsEnabled = false;
                carePlanSubmitHandler();
                annotationservice.stopAnnotationService();

            }

            private void cbGroupByTime_Checked(object sender, RoutedEventArgs e)
            {
                rbGroupByComponents.IsChecked = false; // Toggle only allow one grouping
                App.carePlan.cvDiagnoses.GroupDescriptions.Clear();
                App.carePlan.cvDiagnoses.GroupDescriptions.Add(new PropertyGroupDescription("CreationDate"));
                App.carePlan.cvDiagnoses.Refresh();

                /*
                cvcarePlanInterventions.GroupDescriptions.Add(new PropertyGroupDescription("ComponentName"));
                cvcarePlanInterventions.Refresh(); */
                App.carePlan.PrettyCarePlan_Update();
            }

            private void cbGroupByTime_Unchecked(object sender, RoutedEventArgs e)
            {
                rbGroupByComponents_Unchecked(sender, e);

            }

            private void lbImage_Drop(object sender, DragEventArgs e)
            {
                IDataObject data = e.Data;

                string[] filenames;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {

                    filenames = (string[])data.GetData(DataFormats.FileDrop);

                    foreach (string filename in filenames)
                    {
                        BitmapImage img = new BitmapImage();
                        BitmapSource imgsource = BitmapFrame.Create(new Uri(filename));
                        BitmapMetadata imgmeta = (BitmapMetadata)imgsource.Metadata;
                           
                        img.BeginInit();
                        img.UriSource = new Uri(filename);
                        img.DecodePixelHeight = 64; // Tip; only choose width or height to conserve aspect ratio
                        img.EndInit();


                        myImage myimg = new myImage();
                        myimg.Img = img;

                        myimg.FileName = filename;
                        myimg.Description = "Beskrivelse";
                        myimg.Meta = imgmeta;
                        
                        if (imgmeta != null)
                        {
                            if (imgmeta.DateTaken != null)
                                myimg.DateTaken = imgmeta.DateTaken;
                            else
                                myimg.DateTaken = "Ukjent dato";

                            if (imgmeta.CameraModel != null && imgmeta.CameraManufacturer != null)
                                myimg.Camera = imgmeta.CameraManufacturer + " " + imgmeta.CameraModel;
                            else
                                myimg.Camera = "Ukjent kamera";
                        }
                        else
                        {
                            myimg.DateTaken = "Ukjent dato";
                            myimg.Camera = "Ukjent kamera";
                        }
                       //BitmapMetadataBlob b = new BitmapMetadataBlob(myimg.Meta.ToArray<Byte>());
                        
                       
                       

                        coll.Add(myimg);


                    }
                    e.Handled = true;
                    lcoll.Refresh();
                    App.carePlan.PrettyCarePlan_Update();
                }

               
                lcoll.Refresh();

                lbImage.ItemsSource = lcoll;

            }


            private void lbImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }


            private void miDelete_Click(object sender, RoutedEventArgs e)
            {
                foreach (myImage img in lbImage.SelectedItems)
                    coll.Remove(img);
                lcoll.Refresh();
                App.carePlan.PrettyCarePlan_Update();
            }


            private void miZoom_Click(object sender, RoutedEventArgs e)
            {
                foreach (myImage img in lbImage.SelectedItems)
                {
                    ImageShow winImageFull = new ImageShow();
                    winImageFull.showImage(img);
                    winImageFull.Show();
                }

            }

            private void lbImage_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true; // Necessary to not unselect item, tip taken from the web
            }

        }
}

   
    

