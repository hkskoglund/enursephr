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
using CCC.BusinessLayer;
using System.IO;
using System.Windows.Shapes;

using System.Windows.Media.Imaging;


namespace CCC.UI
{
    public class ViewCarePlan : CarePlanEntitesWrapper
    {
       
        public ViewCarePlan()
            : base()
        {
                    
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


        public Section generateItem(TagHandler tagHandler, Item item, bool showTags)
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


        public void generateCareBlog(FlowDocumentReader fdReaderCareBlog,CarePlan cp, TagHandler tagHandler, bool hasLineSeparator)
        {
            if (cp.Item.Count == 0)
                return;

            FlowDocument fdCareBlog = new FlowDocument();

            int blogItemNr = 0;
            //dictItemBlog = new Dictionary<string,Guid>();

            Item lastItem = cp.Item.OrderByDescending(d => d.History.LastUpdate).Last();
            foreach (Item item in cp.Item.OrderByDescending(d => d.History.LastUpdate))
            {

                fdCareBlog.Blocks.Add(this.generateItem(tagHandler, item,true));
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


        public void showCareplanItem(FlowDocumentReader fdReaderCareBlog,Item selItem, TagHandler tagHandler)
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
            Section itemSection = this.generateItem(tagHandler, selItem, false);
           // foreach (Paragraph paragraph in itemSection.Blocks)
              fdItem.Blocks.Add(itemSection);

            fdReaderCareBlog.Document = fdItem;
            fdReaderCareBlog.Visibility = Visibility.Visible;
           
            fdReaderCareBlog.ViewingMode = FlowDocumentReaderViewingMode.Page;

        }

        

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

        private void sItem_MouseLeave(object sender, MouseEventArgs e)
        {
            //(sender as Section).Background = new SolidColorBrush(Colors.White);
            (sender as Section).BorderBrush = null;
        }


        private Paragraph generateTags(TagHandler tagHandler,Item item)
        {
            
            bool hasComment;
            bool hasActionType;

            if (item.Tag.Count == 0)
                return null;

            IOrderedEnumerable<Tag> orderedTags = item.Tag.OrderBy(t => t.CareComponentConcept).ThenBy(t => t.Concept);
            Tag lastTag = orderedTags.Last();

            Paragraph pTags = new Paragraph();
            pTags.FontSize = 9;
            pTags.TextAlignment = TextAlignment.Left;
            

            string prevCareComponent = null;
            string currentCareComponent;

           // pTags.Inlines.Add("Tags : ");
            foreach (Tag tag in orderedTags)
            {


                tagHandler.updateTag(App.cccFrameWork.DB, tag, Properties.Settings.Default.LanguageName, Properties.Settings.Default.Version);
                ((WindowMain)App.Current.MainWindow).refreshOutcomes(tag);

                // Care Component Concept
                TextBlock tbCareComponent;
                currentCareComponent = tag.CareComponentConcept.Trim().ToUpperInvariant();
                if (prevCareComponent != currentCareComponent)
                {
                    tbCareComponent = new TextBlock(new Run(currentCareComponent));
                    tbCareComponent.FontSize = 14;
                    tbCareComponent.FontWeight = FontWeights.UltraBold;
                    tbCareComponent.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("CareComponentColor");
                    tbCareComponent.Margin = new Thickness(10, 0, 10, 0);
                    tbCareComponent.VerticalAlignment = VerticalAlignment.Center;
                    pTags.Inlines.Add(tbCareComponent);
                    prevCareComponent = currentCareComponent;
                }

                //string delimiter = ", ";
                //if (tag == lastTag)
                //    delimiter = null;

                
               //string startParentesis = String.Empty;
               //string endParentesis = String.Empty;

                TextBlock tbComment = null;
               hasComment = false;
               if (tag.Comment != null)
                   if (tag.Comment.Length > 0)
                   {
                       hasComment = true;
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
                   }

                
               //if (!hasComment)
               //{

                   InlineUIContainer tagContainer = new InlineUIContainer();
                   StackPanel spTagContainer = new StackPanel();
                   spTagContainer.Width = 220;
                   spTagContainer.Margin = new Thickness(5,5,5,0);
                   

                   StackPanel spTagDiagInterv = new StackPanel();
                   spTagDiagInterv.Orientation = Orientation.Horizontal;

                   // Make sure action type is loaded
                   if (!tag.ActionTReference.IsLoaded)
                       tag.ActionTReference.Load();

                   hasActionType = (tag.ActionT == null) ? false : true;

                   
                   TextBlock tbActionModifier;

                   if (hasActionType)
                   {
                       tbActionModifier = new TextBlock(new Run(/*startParentesis+*/tag.ActionT.SingleConcept));
                       tbActionModifier.Margin = new Thickness(0, 0, 5, 0);
                       //startParentesis = String.Empty;
                       tbActionModifier.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("InterventionColor");
                       tbActionModifier.FontSize = 12;
                       tbActionModifier.VerticalAlignment = VerticalAlignment.Center;
                       string definition = App.cccFrameWork.DB.ActionType.Where(a => a.Code == tag.ActionT.Code && a.Language_Name == Properties.Settings.Default.LanguageName && a.Version == Properties.Settings.Default.Version).First().Definition;
                       if (definition != null)
                           tbActionModifier.ToolTip = definition;
                       else
                           tbActionModifier.ToolTip = "Definition for action type was not found";

                       spTagDiagInterv.Children.Add(tbActionModifier);
                       //pTags.Inlines.Add(tbActionModifier);
                   }


                   //if (hasComment)
                   //    endParentesis = ")";

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
                       case "CCC/CareComponent": tbTag.Foreground = (Brush)((WindowMain)App.Current.MainWindow).TryFindResource("CareComponentColor") ; break;
                   }


                   spTagDiagInterv.Children.Add(tbTag);


                   StackPanel spLatestOutcome = null;

                 bool hasLatestOutcome = (tag.LatestOutcome == null) ? false : true;

                   if (hasLatestOutcome)
                   {
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
                       tbLatestOutcome.Inlines.Add((string)conv.Convert(tag.LatestOutcomeModifier,null,null,null));
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
                   }
               //    pTags.Inlines.Add(tbTag);
               //}

                //if (delimiter != null)
                //    pTags.Inlines.Add(delimiter);
                   spTagContainer.Children.Add(spTagDiagInterv);
                   if (hasLatestOutcome)
                       spTagContainer.Children.Add(spLatestOutcome);

                   if (hasComment)
                      spTagContainer.Children.Add(tbComment);
                  
                   //Border bTag = new Border();
                   //bTag.BorderThickness = new Thickness(1);
                   //bTag.BorderBrush = Brushes.CadetBlue;
                   //bTag.CornerRadius = new CornerRadius(5);
                   //bTag.Padding = new Thickness(5);
                   //bTag.Child = spTagContainer;
                   pTags.Inlines.Add(spTagContainer);
            }

            return pTags;
        }

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

  


    }

}
