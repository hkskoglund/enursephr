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
using System.Collections.ObjectModel;
using eNursePHR.BusinessLayer.PHR;
using eNursePHR.BusinessLayer;
using System.ComponentModel;

namespace eNursePHR.userInterfaceLayer
    
{
    /// <summary>
    /// Interaction logic for TagOverviewControl.xaml
    /// </summary>
    public partial class TagOverviewControl : UserControl
    {

        ObservableCollection<Tag> careplanTags;
        //EntityCollection<Tag> careplanTags = new EntityCollection<Tag>();
        ListCollectionView cvTagsOverview;

        TagLangageConverter tagConverter = new TagLangageConverter();

        public event EventHandler TagCommentChanged;

        public TagOverviewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Finds concept and care component for a tag and build up a careplan of all tags
        /// </summary>
        /// <param name="cp"></param>
        public void buildTagsOverview(CarePlan cp)
        {
            string version = eNursePHR.userInterfaceLayer.Properties.Settings.Default.Version;
            string languageName = eNursePHR.userInterfaceLayer.Properties.Settings.Default.LanguageName;

            careplanTags = new ObservableCollection<Tag>();

            foreach (Item item in cp.Item)
                foreach (Tag tag in item.Tag)
                {

                    tagConverter.translateTag(App.s_cccFrameWork.DB, tag, languageName, version); // Find Concept and CareComponent for tag
                    careplanTags.Add(tag);
                }

            cvTagsOverview = new ListCollectionView(careplanTags);
           
            cvTagsOverview.GroupDescriptions.Add(new PropertyGroupDescription("CareComponentConcept"));
            cvTagsOverview.SortDescriptions.Add(new SortDescription("CareComponentConcept", ListSortDirection.Ascending));
            cvTagsOverview.SortDescriptions.Add(new SortDescription("Concept", ListSortDirection.Ascending));
            lbTaxonomy.ItemsSource = cvTagsOverview;

        }

        public void refresh()
        {
            // Also do a refresh of the tags overview
            if (cvTagsOverview != null)
                cvTagsOverview.Refresh();
       
        }

        public void addTag(Tag newTag)
        {
            careplanTags.Add(newTag);
        }

        public void removeTag(Tag removeTag)
        {
            careplanTags.Remove(removeTag);
        }

        private void lbTaxonomy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tag selTag = (Tag)(sender as ListBox).SelectedItem;
            if (selTag == null)
                return;

           (App.Current.MainWindow as WindowMain).lvCareBlog.SelectedItem = selTag.Item; // Keep care plan blog synchronized with tag from taxonomy/CCC

        }

        private void tbTagComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            TagCommentChanged(sender, new EventArgs()); // Signal to window main
        }

    }
}
