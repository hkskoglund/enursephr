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

namespace CCC.UI
{
    public class ViewCarePlan : myCarePlan
    {
        private ListCollectionView _cvDiagnoses;

        private FlowDocument _fdPrettyCarePlan;
        private bool _groupByComponentName = false;

        private PropertyGroupDescription groupComponentName;
        private SortDescription sortbyComponentName;

        public bool GroupByComponentName
        {
            get { return this._groupByComponentName; }
            set
            {
                if (value == false)
                
                    this._cvDiagnoses.GroupDescriptions.Remove(groupComponentName);
                 
                else if (value == true)
                {
                    groupComponentName = new PropertyGroupDescription("ComponentName");
                    if (this._cvDiagnoses.GroupDescriptions.Contains(groupComponentName) == false)
                      this._cvDiagnoses.GroupDescriptions.Add(groupComponentName);

                    sortbyComponentName = new SortDescription("ComponentName",ListSortDirection.Ascending);
                    if (this._cvDiagnoses.SortDescriptions.Contains(sortbyComponentName) == false)
                      this._cvDiagnoses.SortDescriptions.Add(new SortDescription("ComponentName", ListSortDirection.Ascending));
                  

                }

                this._groupByComponentName = value;  
                this._cvDiagnoses.Refresh();
             //TESTING MUST FIX THIS   this.PrettyCarePlan_Update();
            }
        }
       
        public ViewCarePlan(int careplanid, CCCFrameworkEntities DB)
            : base(careplanid, DB)
        {
          //E this._cvDiagnoses = new ListCollectionView(this.Diagnoses);

            // Get diagnoses from diagnoses entities
            //System.Data.Objects.ObjectQuery<Diagnosi> qdiag = (System.Data.Objects.ObjectQuery<Diagnosi>) base.DB.Diagnosis.Where(d => d.CarePlan.Id == careplanid);
          
            this._cvDiagnoses = new ListCollectionView(this._diagnoses);
            
            this.GroupByComponentName = true;


            _fdPrettyCarePlan = new FlowDocument();
            _fdPrettyCarePlan.Background = Brushes.White;

            ContextMenu ctx = new ContextMenu();
            _fdPrettyCarePlan.ContextMenu = ctx;
            
            MenuItem ctxmiCopy = new MenuItem();
            ctxmiCopy.Command = ApplicationCommands.Copy;
            /* LANGUAGE attrib
                                Norwegian : "Kopier"
                                English : "Copy"
                                */ 
                               
            ctxmiCopy.Header = "Copy";
            ctx.Items.Add(ctxmiCopy);

            MenuItem ctxmiSelectAll = new MenuItem();
            ctxmiSelectAll.Command = ApplicationCommands.SelectAll;
            /* LANGUAGE attrib
                                Norwegian : "Velg alt"
                                English : "Select all"
                                */ 
                               
            ctxmiSelectAll.Header = "Select all";
            ctx.Items.Add(ctxmiSelectAll);

            ctx.Items.Add(new Separator());

            MenuItem ctxmiInkStickyNote = new MenuItem();
            ctxmiInkStickyNote.Command = System.Windows.Annotations.AnnotationService.CreateInkStickyNoteCommand;
            /* LANGUAGE attrib
                                Norwegian : "Ny blekk beskjed"
                                English : "New ink message"
                                */ 
                               
            ctxmiInkStickyNote.Header = "New ink message";
            ctx.Items.Add(ctxmiInkStickyNote);

            MenuItem ctxmiTextStickyNote = new MenuItem();
            ctxmiTextStickyNote.Command = System.Windows.Annotations.AnnotationService.CreateTextStickyNoteCommand;
            /* LANGUAGE attrib
                                Norwegian : "Ny tekst beskjed"
                                English : "New text message"
                                */ 
                               
            ctxmiTextStickyNote.Header = "New text message";
            ctx.Items.Add(ctxmiTextStickyNote);

            MenuItem ctxmiHighlight = new MenuItem();
            ctxmiHighlight.Command = System.Windows.Annotations.AnnotationService.CreateHighlightCommand;
            /* LANGUAGE attrib
                                Norwegian : "Ny markering"
                                English : "New highlight"
                                */ 
                               
            ctxmiHighlight.Header = "New highlight";
            ctx.Items.Add(ctxmiHighlight);

            ctx.Items.Add(new Separator());

            MenuItem ctxmiClearHighlight = new MenuItem();
            ctxmiClearHighlight.Command = System.Windows.Annotations.AnnotationService.ClearHighlightsCommand;
            /* LANGUAGE attrib
                                Norwegian : "Slett markeringer"
                                English : "Delete highlights"
                                */ 
                               
            ctxmiClearHighlight.Header = "Delete highlights";
            ctx.Items.Add(ctxmiClearHighlight);

            MenuItem ctxmiDeleteStickyNotes = new MenuItem();
            ctxmiDeleteStickyNotes.Command = System.Windows.Annotations.AnnotationService.DeleteStickyNotesCommand;
            /* LANGUAGE attrib
                                Norwegian : "Slett tekst og blekk beskjeder"
                                English : "Delete text and ink messages"
                                */ 
                               
            ctxmiDeleteStickyNotes.Header = "Delete text and ink messages";
            ctx.Items.Add(ctxmiDeleteStickyNotes);


            MenuItem ctxmiDeleteAnnotations = new MenuItem();
            ctxmiDeleteAnnotations.Command = System.Windows.Annotations.AnnotationService.DeleteAnnotationsCommand;
            /* LANGUAGE attrib
                                Norwegian : "Slett alle annotasjoner"
                                English : "Delete all annotations"
                                */ 
                               
            ctxmiDeleteAnnotations.Header = "Delete all annotations";
            ctx.Items.Add(ctxmiDeleteAnnotations);


            Paragraph pheading = new Paragraph();
            pheading.FontSize = 22;
            pheading.TextAlignment = TextAlignment.Center;

            /* LANGUAGE attrib
                                Norwegian : "Pleieplan"
                                English : "Care plan"
                                */ 
                               
            pheading.Inlines.Add(new Bold(new Run("Care plan")));
            _fdPrettyCarePlan.Blocks.Add(pheading);
        
        
        }

        public ListCollectionView cvDiagnoses
        {
            get { return this._cvDiagnoses; }
            set { this._cvDiagnoses = value; }
        }


        public FlowDocument fdPrettyCarePlan
        {
            
            get { return this._fdPrettyCarePlan; }
        }

        public void PrettyCarePlanDeleteDiagnosis(Section diag)
        {
            this._fdPrettyCarePlan.Blocks.Remove(diag);
        }

        public void PrettyCarePlan_Update(ListCollectionView lcoll)
        {
           
            if (this.fdPrettyCarePlan != null)
                this.fdPrettyCarePlan.Blocks.Clear();
            else return;

            if (this.cvDiagnoses.Count == 0) // No diagnoses for this patient
                return;

           
            //List diagnoses = new List();
            //diagnoses.MarkerStyle = TextMarkerStyle.None;



            Paragraph pheading = new Paragraph();
            pheading.FontSize = 22;
            pheading.TextAlignment = TextAlignment.Center;
            /* LANGUAGE attrib
                                Norwegian : "Pleieplan"
                                English : "Care plan"
                                */ 
                               
            pheading.Inlines.Add(new Bold(new Run("Care plan")));
            this.fdPrettyCarePlan.Blocks.Add(pheading);

            
            Paragraph p = new Paragraph();

            /* LANGUAGE attrib
                                Norwegian : "Brukeren har"
                                English : "Patient have"
                                */ 
                               
            p.Inlines.Add(new Bold(new Run("Patient has")));
            this.fdPrettyCarePlan.Blocks.Add(p);

            string previousComponentName = null;
            string previousDate = null;

            foreach (Diagnosi d in this.cvDiagnoses)
            {

                if (this.GroupByComponentName == true)
                {
                    if (previousComponentName != d.ComponentName)
                    {
                        previousComponentName = d.ComponentName;
                        Paragraph pcomp = new Paragraph();
                        /// Some lines taken from MSDN-library....

                        // Create an underline text decoration. Default is underline.
                        TextDecoration myUnderline = new TextDecoration();
                        //  TextDecoration myOverLine = new TextDecoration();
                        //  myOverLine.Location = TextDecorationLocation.OverLine;

                        // Create a solid color brush pen for the text decoration.
                        myUnderline.Pen = new Pen(Brushes.Black, 1);
                        myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                        // myOverLine.Pen = new Pen(Brushes.Black, 1);
                        // myOverLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;

                        // Set the underline decoration to a TextDecorationCollection and add it to the text block.
                        TextDecorationCollection myCollection = new TextDecorationCollection();
                        myCollection.Add(myUnderline);
                        //  myCollection.Add(myOverLine);
                        pcomp.TextDecorations = myCollection;

                        pcomp.Inlines.Add(d.ComponentName);
                        this.fdPrettyCarePlan.Blocks.Add(pcomp);
                    }
                }

                /*
                if ((bool)(cbGroupByTime.IsChecked) == true)
                {
                    if (previousDate != d.CreationDateString)
                    {
                        previousDate = d.CreationDateString;
                        Paragraph pcomp = new Paragraph();
                        /// Some lines taken from MSDN-library....

                        // Create an underline text decoration. Default is underline.
                        TextDecoration myUnderline = new TextDecoration();
                        //TextDecoration myOverLine = new TextDecoration();
                        //myOverLine.Location = TextDecorationLocation.OverLine;

                        // Create a solid color brush pen for the text decoration.
                        myUnderline.Pen = new Pen(Brushes.Black, 1);
                        myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
                        //myOverLine.Pen = new Pen(Brushes.Black, 1);
                        //myOverLine.PenThicknessUnit = TextDecorationUnit.FontRecommended;

                        // Set the underline decoration to a TextDecorationCollection and add it to the text block.
                        TextDecorationCollection myCollection = new TextDecorationCollection();
                        myCollection.Add(myUnderline);
                        //myCollection.Add(myOverLine);
                        pcomp.TextDecorations = myCollection;

                        pcomp.Inlines.Add(d.CreationDateString);
                        this.fdPrettyCarePlan.Blocks.Add(pcomp);
                    } 
                }*/

                //Paragraph pa = new Paragraph();
                //pa.Margin = new Thickness(0, 0, 0, 0);
                //pa.Foreground = Brushes.Black;

                // Outcome

                /*
                
                BitmapImage bOutCome = new BitmapImage();
                Image iOutCome = new Image();

                bOutCome.BeginInit();

                // Problem: får ikke til å sette relativ Uri....

                switch (d.Outcome)
                {
                    case 3: bOutCome.UriSource = new Uri(@"F:\Documents and Settings\hks\Mine dokumenter\Visual Studio 2008\Projects\CCC UI prototype\CCC UI prototype\Bilder\arrow-down-red_benji_par_01.png", UriKind.Absolute); break;
                    case 2: bOutCome.UriSource = null; break;
                    case 1: bOutCome.UriSource = new Uri(@"F:\Documents and Settings\hks\Mine dokumenter\Visual Studio 2008\Projects\CCC UI prototype\CCC UI prototype\Bilder\arrow-up-green_benji_par_01.png", UriKind.Absolute); break;
                }

                if (d.Outcome != 2)
                {
                    bOutCome.EndInit();

                    iOutCome.Source = bOutCome;
                    iOutCome.Height = 15;

                    pa.Inlines.Add(iOutCome);
                }
                */


                // Add concept

                //  pa.Inlines.Add(d.Concept);
                this.fdPrettyCarePlan.Blocks.Add(d.FlowDiagnosis);


                //if (cbGroupByTime.IsChecked==false)
                //{
                //    Paragraph pr = new Paragraph();
                //    pr.FontSize = 9;
                //    pr.Margin = new Thickness(5, 0, 0, 0);
                //    pr.Inlines.Add("Opprettet : "+d.CreationDateString);
                //    fdPrettyCarePlan.Blocks.Add(pr);
                //}

                //if (d.ReasonForDiagnosis != null)
                //{
                //    Paragraph pr = new Paragraph();
                //    pr.FontSize = 9;
                //    pr.Margin = new Thickness(5, 0, 0, 0);
                //    pr.Inlines.Add(d.ReasonForDiagnosis);
                //    App.carePlan.fdPrettyCarePlan.Blocks.Add(pr);
                //}

                // Show images if diagnose is skin-related
                
                Paragraph prImages = new Paragraph();


                BlockUIContainer blui = new BlockUIContainer();
                WrapPanel spallImg = new WrapPanel();
                blui.Child = spallImg;
                // spallImg.Orientation = Orientation.Horizontal;

                lcoll.Refresh();

                foreach (myImage img in lcoll)
                {

                    DockPanel spMeta = new DockPanel();

                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(2);

                    Image imgUI = new Image();
                    imgUI.Source = img.Img;
                    imgUI.Height = 64;


                    //    Figure f = new Figure(blui);

                    //    f.BorderBrush = Brushes.Black;
                    //    f.BorderThickness = new Thickness(1);
                    //   // f.HorizontalAnchor = FigureHorizontalAnchor.ContentLeft;

                    //    f.Height = new FigureLength(64,FigureUnitType.Pixel);
                    //    prImages.Inlines.Add(f);   
                    //

                   
                    TextBlock tbMeta = new TextBlock(new Run(img.DateTaken));
                    tbMeta.HorizontalAlignment = HorizontalAlignment.Left;
                    tbMeta.FontSize = 9;

                    DockPanel.SetDock(imgUI, Dock.Top);
                    DockPanel.SetDock(tbMeta, Dock.Bottom);

                    spMeta.Children.Add(imgUI);
                    spMeta.Children.Add(tbMeta);
                    border.Child = spMeta;

                    spallImg.Children.Add(border);

                }


                if (d.Concept.Contains("Svekket hudoverflate"))
                {

                    this.fdPrettyCarePlan.Blocks.Add(blui);
                } 

                /*
                if (d.OutcomeEvalDate.Year != 1)
                {
                    Paragraph peval = new Paragraph();
                    peval.FontSize = 9;
                    peval.Margin = new Thickness(5, 0, 0, 0);
                    peval.Inlines.Add("Evalueres innen : " + d.OutcomeEvalDate.ToLongDateString());
                    fdPrettyCarePlan.Blocks.Add(peval);
                } */
            }

            //diagnoses.ListItems.Add(new ListItem(pa));
            /* List li = new List();
             foreach (myNursingIntervention i in d.Intervention)
             {
                 Paragraph pi = new Paragraph();
                 pi.Inlines.Add(i.Concept);
                 li.ListItems.Add(new ListItem(pi));
             }
                 diagnoses.ListItems.Add(); 
        }
        fdPrettyCarePlan.Blocks.Add(diagnoses);


        if (cvcarePlanInterventions.Count == 0) // No interventions for patient
            return;

        Paragraph pi = new Paragraph();
        pi.Inlines.Add(new Bold(new Run("Vi hjelper til med :")));
        fdPrettyCarePlan.Blocks.Add(pi);

        List interventions = new List();

        foreach (myNursingIntervention i in lbCarePlanInterventions.ItemsSource)
        {
            Paragraph pa = new Paragraph();
            pa.Inlines.Add(i.Concept);
            interventions.ListItems.Add(new ListItem(pa));
        }
        fdPrettyCarePlan.Blocks.Add(interventions);

*/

        }

    }

}
