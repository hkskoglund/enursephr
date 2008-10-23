#define SQL_SERVER_COMPACT_SP1_WORKAROUND

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
using eNursePHR.BusinessLayer.PHR;
using System.ComponentModel;

namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for TagControl.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
        public TagControl()
        {
            InitializeComponent();
        }

        ListCollectionView cvItemTags;


        public TagLangageConverter tagConverter = new TagLangageConverter();


        public event EventHandler TagCommentChanged;


       
        private void lbTags_Drop(object sender, DragEventArgs e)
        {
            Item selItem = (App.Current.MainWindow as WindowMain).lvCareBlog.SelectedItem as Item;  // This is the currently selected item in the combobox
            if (selItem == null)
            {
                MessageBox.Show("You have not selected an entry to attach this tag to,\nplease create a new entry or select one before retrying this action",
                    "No entry selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }



            IDataObject transfer = e.Data;  // This is the actual drop data

            string taxonomyType = null;
            Guid taxonomyGuid = Guid.Empty;
            Guid taxonomyOutcomeAttachmentGuid = Guid.Empty;
            Guid taxonomyActionTypeAttachmentGuid = Guid.Empty;

            eNursePHR.BusinessLayer.CCC_Translations.OutcomeType fOutcomeType = null;
            eNursePHR.BusinessLayer.CCC_Translations.ActionType fActionType = null;

            string comment = null;

            //getDropCareComponent(transfer, ref taxonomyType, ref taxonomyGuid);

            getDropOutcomeType(transfer, ref taxonomyOutcomeAttachmentGuid, ref fOutcomeType);

            getDropDiagnosis(transfer, ref taxonomyType, ref taxonomyGuid, ref comment);

            getDropActionType(transfer, ref taxonomyType, ref taxonomyActionTypeAttachmentGuid, ref fActionType);

            getDropIntervention(transfer, ref taxonomyType, ref taxonomyGuid, ref comment);



            // Add tag, if found in taxonomy, currently supports only drop of diagnosis and intervention

            if (taxonomyType == "CCC/NursingDiagnosis" || taxonomyType == "CCC/NursingIntervention")
            {
                selItem.Tag.AssociationChanged -= new CollectionChangeEventHandler(this.Tag_AssociationChanged); // Remove event handling now

                Tag newTag = addNewDropTag(selItem, taxonomyType, ref taxonomyGuid, comment);

                taxonomyOutcomeAttachmentGuid = addDropOutcomeToTag(taxonomyOutcomeAttachmentGuid, fOutcomeType, newTag);

                taxonomyActionTypeAttachmentGuid = addDropActionTypeToTag(taxonomyActionTypeAttachmentGuid, fActionType, newTag);

                (App.Current.MainWindow as WindowMain).SaveCarePlan();

                selItem.Tag.AssociationChanged += new CollectionChangeEventHandler(this.Tag_AssociationChanged);
                this.Tag_AssociationChanged(this, null);

                (App.Current.MainWindow as WindowMain).TagOverviewControl.addTag(newTag);
                (App.Current.MainWindow as WindowMain).inferContentFromTags(selItem); // Update content status
                refreshLanguageTranslationForItemTags();
            }
        }

        private void getDropIntervention(IDataObject transfer, ref string taxonomyType, ref Guid taxonomyGuid, ref string comment)
        {
            if (transfer.GetDataPresent("CCC/NursingIntervention"))
            {
                taxonomyType = "CCC/NursingIntervention";
                Nursing_Intervention fInterv = (Nursing_Intervention)transfer.GetData("CCC/NursingIntervention");

                comment = fInterv.Comment;
                // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                taxonomyGuid = tagConverter.getTaxonomyGuidNursingIntervention(fInterv.ComponentCode, fInterv.MajorCode, fInterv.MinorCode, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);


            }
        }

        private void getDropActionType(IDataObject transfer, ref string taxonomyType, ref Guid taxonomyActionTypeAttachmentGuid, ref eNursePHR.BusinessLayer.CCC_Translations.ActionType fActionType)
        {
            if (transfer.GetDataPresent("CCC/ActionType"))
            {
                taxonomyType = "CCC/ActionType";
                fActionType = (ActionType)transfer.GetData("CCC/ActionType");
                taxonomyActionTypeAttachmentGuid = tagConverter.getTaxonomyGuidActionType(fActionType.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);

            }
        }

        private void getDropDiagnosis(IDataObject transfer, ref string taxonomyType, ref Guid taxonomyGuid, ref string comment)
        {
            if (transfer.GetDataPresent("CCC/NursingDiagnosis"))
            {
                taxonomyType = "CCC/NursingDiagnosis";
                eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis fDiag = (eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis)transfer.GetData("CCC/NursingDiagnosis");
                comment = fDiag.Comment;
                // Find taxonomy reference/guid to identify diagnosis (component,major,minor)
                taxonomyGuid = tagConverter.getTaxonomyGuidNursingDiagnosis(fDiag.ComponentCode, fDiag.MajorCode, fDiag.MinorCode, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);

            }
        }

        private void getDropOutcomeType(IDataObject transfer, ref Guid taxonomyOutcomeAttachmentGuid, ref eNursePHR.BusinessLayer.CCC_Translations.OutcomeType fOutcomeType)
        {
            if (transfer.GetDataPresent("CCC/OutcomeType"))
            {
                fOutcomeType = (eNursePHR.BusinessLayer.CCC_Translations.OutcomeType)transfer.GetData("CCC/OutcomeType");
                // Find guid in reference terminology
                taxonomyOutcomeAttachmentGuid = tagConverter.getTaxonomyGuidOutcomeType(fOutcomeType.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);
            }
        }

        private void getDropCareComponent(IDataObject transfer, ref string taxonomyType, ref Guid taxonomyGuid)
        {
            if (transfer.GetDataPresent("CCC/CareComponent"))
            {
                taxonomyType = "CCC/CareComponent";
                Care_component component = (Care_component)transfer.GetData("CCC/CareComponent");
                taxonomyGuid = tagConverter.getTaxonomyGuidCareComponent(component.Code, eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version);

            }
        }

        private Tag addNewDropTag(Item selItem, string taxonomyType, ref Guid taxonomyGuid, string comment)
        {
            Tag newTag = eNursePHR.BusinessLayer.PHR.Tag.CreateTag(Guid.NewGuid(), taxonomyType, taxonomyGuid);

            newTag.Item = selItem;
            newTag.Comment = comment;

            History tagHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
            newTag.History = tagHistory;


            App.s_carePlan.DB.AddToTag(newTag);
            App.s_carePlan.DB.AddToHistory(tagHistory);
            return newTag;
        }

        private Guid addDropActionTypeToTag(Guid taxonomyActionTypeAttachmentGuid, eNursePHR.BusinessLayer.CCC_Translations.ActionType fActionType, Tag newTag)
        {
            if (taxonomyActionTypeAttachmentGuid != Guid.Empty)
            {

                ActionT newActionType = ActionT.CreateActionT(fActionType.Code,
                    (App.Current.MainWindow as WindowMain).CCCTaxonomyControl.tbConceptActionType.Text, taxonomyActionTypeAttachmentGuid, newTag.Id);
                newActionType.Tag = newTag;


                History actionHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                newActionType.History = actionHistory;
                App.s_carePlan.DB.AddToHistory(actionHistory);
                App.s_carePlan.DB.AddToActionT(newActionType);
            }
            return taxonomyActionTypeAttachmentGuid;
        }

        private Guid addDropOutcomeToTag(Guid taxonomyOutcomeAttachmentGuid, eNursePHR.BusinessLayer.CCC_Translations.OutcomeType fOutcomeType, Tag newTag)
        {
            if (taxonomyOutcomeAttachmentGuid != Guid.Empty) // Check if we also have to deal with an outcome
            {

                Outcome newOutcome = Outcome.CreateOutcome(Guid.NewGuid(), fOutcomeType.Code, taxonomyOutcomeAttachmentGuid);
                newOutcome.Tag = newTag;  // Setup referende to tag

                History outcomeHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                newOutcome.History = outcomeHistory;
                App.s_carePlan.DB.AddToHistory(outcomeHistory);
                App.s_carePlan.DB.AddToOutcome(newOutcome);

            }
            return taxonomyOutcomeAttachmentGuid;
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

            MessageBoxResult result = MessageBox.Show("Tag " + selTag.Concept + " will be deleted, are you sure?", "Delete " + selTag.Concept, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            App.s_carePlan.DB.DeleteObject(selTag);
            if ((App.Current.MainWindow as WindowMain).SaveCarePlan() != -1)
                (App.Current.MainWindow as WindowMain).TagOverviewControl.removeTag(selTag);


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

        /// <summary>
        /// Translates all tags for a selected item 
        /// </summary>
        public void refreshLanguageTranslationForItemTags()
        {

            string version = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
            string languageName = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;

            // Check for invalid selection and return if problems
            Item selItem = (App.Current.MainWindow as WindowMain).lvCareBlog.SelectedItem as Item;
            if (selItem == null)
                return;

            // Traverse each tag for the selected item a translate it
            foreach (Tag tag in selItem.Tag)
            {
                refreshOutcomes(tag); // Gets the latest outcome and loads related outcome

                if (!tag.ActionTReference.IsLoaded) // Loads releated actiontype
                    tag.ActionTReference.Load();

                tagConverter.translateTag(App.s_cccFrameWork.DB, tag, languageName, version); // Translate tag to specific language

            }

            Tag_AssociationChanged(this, null); // Call the eventhandler to update the tags, actually no tags have been added or removed for the item.Tags collection therefore a manual-call
        }


        /// <summary>
        /// Loads outcomes for a tag and gets the latest outcome to present
        /// </summary>
        /// <param name="tag"></param>
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
                tag.LatestOutcomeModifier = App.s_cccFrameWork.DB.OutcomeType.Where("it.Code = " + latest.ExpectedOutcome + " AND it.Version = '" +

                eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version + "'AND it.Language_Name = '" + eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName + "'").First().Concept;
#endif
            }

            else
            {
                tag.LatestOutcome = null;
                tag.LatestOutcomeModifier = null;
            }

        }

        /// <summary>
        /// Updates the tags view for a current item and the tags overview. This is an event handler for the
        /// CollectionChangeEvent for item.Tags-collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Tag_AssociationChanged(object sender, CollectionChangeEventArgs e)
        {
            // Check for empty selected item
            Item selItem =  (App.Current.MainWindow as WindowMain).lvCareBlog.SelectedItem as Item;
            if (selItem == null)
                return;

            //App.carePlan.showCareplanItem(fdReaderCareBlog, selItem, tagHandler);

            (App.Current.MainWindow as WindowMain).inferContentFromTags(selItem);

            // Setup tags
            cvItemTags = new ListCollectionView(selItem.Tag.OrderBy(t => t.CareComponentConcept).ThenBy(t => t.Concept).ToList());
            cvItemTags.GroupDescriptions.Add(new PropertyGroupDescription("CareComponentConcept"));
            cvItemTags.Refresh();
            lbTags.ItemsSource = cvItemTags;

            (App.Current.MainWindow as WindowMain).TagOverviewControl.refresh();
        }

        private void tbTagComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            TagCommentChanged(sender, new EventArgs()); // Signal to window main
        }



    }
}
