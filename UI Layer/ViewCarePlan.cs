#define SQL_SERVER_COMPACT_SP1_WORKAROUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Threading;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Shapes;


using System.Windows.Media.Imaging;

using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.PHR;


namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// This class generates a flowdocument for a careplan (a collection of items). Each item/health entry also
    /// has a set of associated tags. Contains methods to "pretty format" careplan/item/tags.
    /// </summary>
    public class ViewCarePlan : CarePlanEntitesWrapper
    {

        public ViewCarePlan(EventHandler DB_SavingChanges)
            : base()
        {
            base.DB.SavingChanges += new EventHandler(DB_SavingChanges);
                    
        }


        private Paragraph generateTitleParagraph(Item item)
        {
            Paragraph pTitle = new Paragraph();
            pTitle.FontSize = 15;
            pTitle.FontWeight = FontWeights.UltraBold;
            pTitle.Inlines.Add(item.Title);
            return pTitle;

        }

        private Paragraph generateLastUpdate(History history)
        {
            // Creation and update times and authors
            Paragraph pUpdate = new Paragraph();
            pUpdate.FontSize = 9;
            string upd = "Created " + history.CreatedDate.ToLongDateString() +
                         " at " + history.CreatedDate.ToShortTimeString() + " by " + history.CreatedBy;

            if (history.UpdatedDate.HasValue)
                upd += "\nLast update " + history.UpdatedDate.Value.ToLongDateString() + " at " + history.UpdatedDate.Value.ToShortTimeString() + " by " + history.UpdatedBy;
            pUpdate.Inlines.Add(upd);
            return pUpdate;

        }

        public Section generateItem(TagLangageConverter tagHandler, Item item, bool showTags)
        {
            if (item == null)
                return null;

            FlowDocument fdItemDescription = new FlowDocument();
            this.loadItemDescription(fdItemDescription, item);

            Section sItem = new Section();


            sItem.MouseEnter += new MouseEventHandler(this.sItem_MouseEnter);
            sItem.MouseLeave += new MouseEventHandler(this.sItem_MouseLeave);

            // Give a friendly name to blog item section and keep Guid to item in dictionary
            //string blogItemName = "blogItem" + (blogItemNr++).ToString();
            //dictItemBlog.Add(blogItemName, item.Id);
            //sItem.Name = blogItemName;

            // Title
            Paragraph pTitle = this.generateTitleParagraph(item);
            pTitle.Background = (Brush)App.Current.MainWindow.TryFindResource("ItemTitleBackground");
            sItem.Blocks.Add(pTitle);

            // Creation and update times and authors
            Paragraph pUpdate = this.generateLastUpdate(item.History);
            sItem.Blocks.Add(pUpdate);

            // Description
            BlockCollection bItem = fdItemDescription.Blocks;

            // I first used bItem.ElementAt[bNr] and "bNr < bItem.Count" in for-loop here
            // but debugging shows that when I add items to sItem they are removed from bItem
            // thus not all description information would be displayed
            // Fixed : 31 march at 16:03 2008
            
            int numParagraphs = bItem.Count;
            for (int bNr = 0; bNr < numParagraphs; bNr++)
                sItem.Blocks.Add(bItem.FirstBlock);

            // Tags

            if (showTags)
            {
                Paragraph pTags = generateTags(tagHandler, item);
                if (pTags != null)
                    sItem.Blocks.Add(pTags);
            }

            return sItem;

        }

        public void generateCareBlog(FlowDocumentReader fdReaderCareBlog,CarePlan cp, TagLangageConverter tagConverter, bool hasLineSeparator)
        {
            if (cp.Item.Count == 0) // Check for empty careplan
                return;

            FlowDocument fdCareBlog = new FlowDocument();

            //int blogItemNr = 0;
            //dictItemBlog = new Dictionary<string,Guid>();

            Item lastItem = cp.Item.OrderByDescending(d => d.History.LastUpdate).Last();
            foreach (Item item in cp.Item.OrderByDescending(d => d.History.LastUpdate))
            {

                fdCareBlog.Blocks.Add(this.generateItem(tagConverter, item,true));
                if (item != lastItem && hasLineSeparator)
                {
                    Line lineSeparator = new Line();
                    lineSeparator.X1 = 0;
                    lineSeparator.X2 = 200;
                    lineSeparator.Y1 = 0;
                    lineSeparator.Y2 = 0;
                    lineSeparator.Stroke = Brushes.LightGray;
                    lineSeparator.StrokeThickness = 1;
                    Paragraph pLine = new Paragraph();
                    pLine.Inlines.Add(lineSeparator);
                    pLine.TextAlignment = TextAlignment.Center;
                    fdCareBlog.Blocks.Add(pLine);
                }

            }

            fdReaderCareBlog.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            fdReaderCareBlog.Document = fdCareBlog;

        }

        public void showCareplanItem(FlowDocumentReader fdReaderCareBlog,Item selItem, TagLangageConverter tagConverter)
        {
            if (selItem == null)
                return;

            if (selItem.Description == null)
            {
                fdReaderCareBlog.Visibility = Visibility.Hidden;
                fdReaderCareBlog.Document = null;
                return;

            }

            FlowDocument fdItem = new FlowDocument();
            Section itemSection = this.generateItem(tagConverter, selItem, false);
           // foreach (Paragraph paragraph in itemSection.Blocks)
              fdItem.Blocks.Add(itemSection);

            fdReaderCareBlog.Document = fdItem;
            fdReaderCareBlog.Visibility = Visibility.Visible;
           
            fdReaderCareBlog.ViewingMode = FlowDocumentReaderViewingMode.Page;

        }
        
        /// <summary>
        /// Loads the item description for the item into a flowdocument
        /// </summary>
        /// <param name="fdLoadItemDescription"></param>
        /// <param name="item"></param>
        private void loadItemDescription(FlowDocument fdLoadItemDescription, Item item)
        {
            TextRange range = new TextRange(fdLoadItemDescription.ContentStart, fdLoadItemDescription.ContentEnd);
            //MemoryStream stream = new MemoryStream(ConvertHelper.convertToByteArray(item.Description));

            MemoryStream stream = new MemoryStream(item.Description);
            
            //range.Load(stream, DataFormats.XamlPackage); // XAML-package is a .ZIP container -> its possibile to get it parts like images
            try
            {
                range.Load(stream, DataFormats.XamlPackage);
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
               
                MessageBox.Show(e.Message, "Parsing error of item description content", MessageBoxButton.OK);
            }
            catch (ArgumentException arg)
            {

                MessageBox.Show(arg.Message, "Argument exception", MessageBoxButton.OK);
            }
        }

        #region Item grayline-outside mouse enter/leave handling
        /// <summary>
        /// If mouse enters shows a gray line around the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Section sItem = sender as Section;
            //(sender as Section).Background = new SolidColorBrush(Colors.Blue);
            sItem.BorderBrush = new SolidColorBrush(Colors.LightGray);
            sItem.BorderThickness = new Thickness(1);

            //    // Keep view in lvCareBlog synchronized
            //    IQueryable<Item> q = cvItems.SourceCollection.AsQueryable().Cast<Item>();
            //    Item mItem = q.First(i => i.Id == dictItemBlog[sItem.Name]);

            //    cvItems.MoveCurrentTo(mItem);
            //    cvItems.Refresh();
        }

        /// <summary>
        /// Turns off border around item when mouse leaves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sItem_MouseLeave(object sender, MouseEventArgs e)
        {
            //(sender as Section).Background = new SolidColorBrush(Colors.White);
            (sender as Section).BorderBrush = null;
        }

        #endregion

        /// <summary>
        /// This method generate a textblock for a care component, it will not generate the care component
        /// more than once if the next care component is the same (its necessary to have an ordered list by care component)
        /// </summary>
        /// <param name="prevCareComponent"></param>
        /// <param name="tag"></param>
        /// <param name="pTags"></param>
        private TextBlock generateCareComponent(ref string prevCareComponent, Tag tag, ref Paragraph pTags)
        {
            // Care Component Concept
            TextBlock tbCareComponent = null;
            string currentCareComponent = tag.CareComponentConcept.Trim().ToUpperInvariant();
            if (prevCareComponent != currentCareComponent)
            {
                tbCareComponent = new TextBlock(new Run(currentCareComponent));
                tbCareComponent.FontSize = 14;
                tbCareComponent.FontWeight = FontWeights.UltraBold;
                tbCareComponent.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("CareComponentColor");
                tbCareComponent.Margin = new Thickness(10, 0, 10, 0);
                tbCareComponent.VerticalAlignment = VerticalAlignment.Center;
                prevCareComponent = currentCareComponent;
            }

            return tbCareComponent;

        }

        /// <summary>
        /// Generates a comment for a tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private TextBlock generateComment(Tag tag)
        {
            TextBlock tbComment = null;

    
            // Check for a tag with a comment (reason for diagnosis, etc.)
            if (tag.Comment == null)
                if (tag.Comment.Length == 0)
                    return null;


            tbComment = new TextBlock(new Run(tag.Comment));
            tbComment.ToolTip = tag.Concept;
            tbComment.Width = 200;
            tbComment.TextWrapping = TextWrapping.Wrap;
            tbComment.VerticalAlignment = VerticalAlignment.Center;
                    //switch (tag.TaxonomyType)
                    //{
                    //    case "CCC/NursingDiagnosis": tbComment.Foreground = Brushes.Green; break;
                    //    case "CCC/NursingIntervention": tbComment.Foreground = Brushes.Blue; break;
                    //}

                    // pTags.Inlines.Add(tbComment);
                    // startParentesis = "(";

            return tbComment;

               
        }

        /// <summary>
        /// Generates actiontype for a tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private TextBlock generateActionType(Tag tag)
        {
             TextBlock tbActionModifier = null;
            
                // Make sure action type is loaded
                if (!tag.ActionTReference.IsLoaded)
                    tag.ActionTReference.Load();


                bool hasActionType = (tag.ActionT == null) ? false : true;


                if (hasActionType)
                {
                    tbActionModifier = new TextBlock(new Run(/*startParentesis+*/tag.ActionT.SingleConcept));
                    tbActionModifier.Margin = new Thickness(0, 0, 5, 0);
                    //startParentesis = String.Empty;
                    tbActionModifier.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("InterventionColor");
                    tbActionModifier.FontSize = 12;
                    tbActionModifier.VerticalAlignment = VerticalAlignment.Center;

                    // SP 1 Beta

#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                       string definition = App.cccFrameWork.DB.ActionType.Where(a => a.Code == tag.ActionT.Code && a.Language_Name == Properties.Settings.Default.LanguageName && a.Version == Properties.Settings.Default.Version).First().Definition;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    // SP 1 workaround
                    string definition = App.s_cccFrameWork.DB.ActionType.Where("it.Code = '" + tag.ActionT.Code + "' AND it.Language_Name ='" + Properties.Settings.Default.LanguageName + "' AND it.Version = '" + Properties.Settings.Default.Version + "'").First().Definition;
#endif
                    if (definition != null)
                        tbActionModifier.ToolTip = definition;
                    else
                        tbActionModifier.ToolTip = "Definition for action type was not found";

                }

            return tbActionModifier;

        }

        /// <summary>
        /// Generate tags will build a Paragraph of tags for an item -> used in report/blog-mode
        /// </summary>
        /// <param name="tagConverter"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Paragraph generateTags(TagLangageConverter tagHandler,Item item)
        {
          
            // Check for a journal entry that has no associated tags --> unnecessary to generate paragraph for tags
            if (item.Tag.Count == 0)
                return null;

            // Order tags by care component (important) then by concept
            IOrderedEnumerable<Tag> orderedByCareComponentTags = item.Tag.OrderBy(t => t.CareComponentConcept).ThenBy(t => t.Concept);
            Tag lastTag = orderedByCareComponentTags.Last();

            // Setup paragraph layout
            Paragraph pTags = new Paragraph();
            pTags.FontSize = 9;
            pTags.TextAlignment = TextAlignment.Left;
            

            string prevCareComponent = null;
        
           // pTags.Inlines.Add("Tags : ");

            foreach (Tag tag in orderedByCareComponentTags)
            {


                tagHandler.translateTag(App.s_cccFrameWork.DB, tag, Properties.Settings.Default.LanguageName, 
                    Properties.Settings.Default.Version);

                ((WindowMain)App.Current.MainWindow).refreshOutcomes(tag);

                // Generate care component textblock an add it to the paragraph
                TextBlock tbCareComponent = generateCareComponent(ref prevCareComponent, tag, ref pTags); // Refactored 12 september 08
                if (tbCareComponent != null)
                    pTags.Inlines.Add(tbCareComponent);

                TextBlock tbComment = generateComment(tag);

                StackPanel spTagContainer = generateTagContainer(tag, tbComment); 
                pTags.Inlines.Add(spTagContainer);

            }
            return pTags;
        }

        /// <summary>
        /// Generates a stackpanel that contains a tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="tbComment"></param>
        /// <returns></returns>
        private StackPanel generateTagContainer(Tag tag, TextBlock tbComment)
        {
            StackPanel spTagContainer = new StackPanel();
            spTagContainer.Width = 220;
            spTagContainer.Margin = new Thickness(5, 5, 5, 0);


            StackPanel spTagDiagInterv = new StackPanel();
            spTagDiagInterv.Orientation = Orientation.Horizontal;


            TextBlock tbTag = generateTag(tag, ref spTagDiagInterv);
            if (tbTag != null)
                spTagDiagInterv.Children.Add(tbTag);


            spTagContainer.Children.Add(spTagDiagInterv);

            StackPanel spLatestOutcome = null;

            bool hasLatestOutcome = generateLatestOutcome(tag, ref spLatestOutcome);

            if (hasLatestOutcome)
                spTagContainer.Children.Add(spLatestOutcome);

            if (tbComment != null)
                spTagContainer.Children.Add(tbComment);

            //Border bTag = new Border();
            //bTag.BorderThickness = new Thickness(1);
            //bTag.BorderBrush = Brushes.CadetBlue;
            //bTag.CornerRadius = new CornerRadius(5);
            //bTag.Padding = new Thickness(5);
            //bTag.Child = spTagContainer;

            return spTagContainer;
        }

        /// <summary>
        /// Generates the latest outcome for a tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="spLatestOutcome"></param>
        /// <returns></returns>
        private bool generateLatestOutcome(Tag tag, ref StackPanel spLatestOutcome)
        {
            bool hasLatestOutcome = (tag.LatestOutcome == null) ? false : true;

            if (!hasLatestOutcome)
                return hasLatestOutcome;

                spLatestOutcome = new StackPanel();
                spLatestOutcome.Orientation = Orientation.Horizontal;
                TextBlock tbWill = new TextBlock(new Run("will "));
                tbWill.FontSize = 12;
                spLatestOutcome.Children.Add(tbWill);
            
                TextBlock tbLatestOutcome = new TextBlock();
                tbLatestOutcome.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("DiagnosisColor");
                tbLatestOutcome.FontSize = 12;
                tbLatestOutcome.Margin = new Thickness(0, 0, 5, 0);
                RemoveParentesisOutcomeConceptConverter conv = new RemoveParentesisOutcomeConceptConverter();
                tbLatestOutcome.Inlines.Add((string)conv.Convert(tag.LatestOutcomeModifier, null, null, null));
                spLatestOutcome.Children.Add(tbLatestOutcome);

                BitmapImage bImg = new BitmapImage();
                bImg.DecodePixelHeight = 15;
                Image imgOutcome = new Image();
                bImg.BeginInit();
      
               switch (tag.LatestOutcome)
                {
                    case 1: bImg.UriSource = new Uri("pack://application:,,/Outcome Types/Improved.png"); break;
                    case 2: bImg.UriSource = new Uri("pack://application:,,/Outcome Types/Stabilized.png"); break;
                    case 3: bImg.UriSource = new Uri("pack://application:,,/Outcome Types/Worsened.png"); break;
                }

                bImg.EndInit();
                imgOutcome.Source = bImg;
                imgOutcome.Height = 15;
                imgOutcome.Width = 15;

                spLatestOutcome.Children.Add(imgOutcome);
      
            return hasLatestOutcome;
        }

        private TextBlock generateTag(Tag tag, ref StackPanel spTagDiagInterv)
        {

            // Add if neccessary action modifier for intervention
            TextBlock tbActionModifier = null;
            if (tag.TaxonomyType == "CCC/NursingIntervention")
            {

                tbActionModifier = generateActionType(tag);

                if (tbActionModifier != null)
                    spTagDiagInterv.Children.Add(tbActionModifier);
            }


            TextBlock tbTag = new TextBlock(new Run(/*startParentesis+*/tag.Concept.Trim()/*+endParentesis*/));
            tbTag.FontWeight = FontWeights.UltraBold;
            tbTag.FontSize = 12;
            tbTag.VerticalAlignment = VerticalAlignment.Center;
            tbTag.TextWrapping = TextWrapping.Wrap;
            tbTag.Width = 190;
            tbTag.ToolTip = tag.Definition; // Allow definition to pop up as tooltip
            tbTag.MouseEnter += new MouseEventHandler(tbTag_MouseEnter);
            tbTag.MouseLeave += new MouseEventHandler(tbTag_MouseLeave);



            switch (tag.TaxonomyType)
            {
                case "CCC/NursingDiagnosis": tbTag.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("DiagnosisColor"); break;
                case "CCC/NursingIntervention": tbTag.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("InterventionColor"); break;
                case "CCC/CareComponent": tbTag.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("CareComponentColor"); break;
            }


            return tbTag;
        }

        #region Tag mouse enter/leave event handling
        void tbTag_MouseEnter(object sender, MouseEventArgs e)
        {
            //TextBlock tag = sender as TextBlock;

            //TransformGroup tGroup = new TransformGroup();

            //tGroup.Children.Add(new TranslateTransform(-10, -10));
            //tGroup.Children.Add(new ScaleTransform(1.3, 1.3));


            //tag.RenderTransform = tGroup;
        }

        void tbTag_MouseLeave(object sender, MouseEventArgs e)
        {
            //TextBlock tag = sender as TextBlock;
            //tag.RenderTransform = null;
        }
        #endregion

    }

}
