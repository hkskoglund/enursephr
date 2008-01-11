using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace CCC.BusinessLayer
{
   
     public partial class Diagnosi : global::System.Data.Objects.DataClasses.EntityObject
  
  //  public partial class Diagnosi
    {
        private string _Concept;

        private string _ComponentName;

        private string _CreationDateString;

        private string _Definition;

        private Section _FlowDiagnosis = new Section();

        private Paragraph _pConcept = new Paragraph();
        private Paragraph _pReasonForDiagnosis = new Paragraph();

        public string Definition
        {
            get
            {
                return this._Definition;
            }
           set
            {
                if ((this._Definition != value))
                {
                  
                  //E  this.SendPropertyChanging();
                  //ERR  this.ReportPropertyChanging("Defintion");
                    this._Definition = value;
                  //E  this.SendPropertyChanged("Definition");
                  //ERR  this.ReportPropertyChanged("Definition");
                }
            }
        }


        //partial void OnCreated()
        //{
        //    this._pConcept = new Paragraph();
        //    this._pReasonForDiagnosis = new Paragraph();
        //    this._FlowDiagnosis = new Section();
        //}
        //partial void OnLoaded()
        //{
        //    this._pConcept = new Paragraph();
            
        //    this._pReasonForDiagnosis = new Paragraph();
           
        //    if (this._ReasonForDiagnosis != null)
        //      this._pReasonForDiagnosis.Inlines.Add(this._ReasonForDiagnosis);
        //}

        public Section FlowDiagnosis
        {
            get
            {
                this._FlowDiagnosis.Blocks.Clear();
               
               // Paragraph pConcept = new Paragraph();
                this._pConcept.Margin = new Thickness(0, 0, 0, 0);
                this._pConcept.Foreground = Brushes.Black;
               // pConcept.Inlines.Add(this._Concept);
                
                this._FlowDiagnosis.Blocks.Add(this._pConcept);

                 if (this._ReasonForDiagnosis != null)
                 {
                     //Paragraph pReason = new Paragraph();
                     this._pReasonForDiagnosis.FontSize = 9;
                     this._pReasonForDiagnosis.Margin = new Thickness(5, 0, 0, 0);
                    // this._pReasonForDiagnosis.Inlines.Add(this._ReasonForDiagnosis);
                        this._FlowDiagnosis.Blocks.Add(this._pReasonForDiagnosis);
                 }

                 return this._FlowDiagnosis;
            }
        }


        public string CreationDateString
        {
            get
            {
                return this._CreationDateString;
            }
            set
            {
                if ((this._CreationDateString != value))
                {
                    //Ethis.SendPropertyChanging();
                    //ERRthis.ReportPropertyChanging("CreationDateString");
                    this._CreationDateString = value;
                    //Ethis.SendPropertyChanged("CreationDateString");
                    //ERRthis.ReportPropertyChanged("CreationDateString");
                }
            }
        }

        public string Concept
        {
            get
            {
                return this._Concept;
            }
            set
            {
                if ((this._Concept != value))
                {
                    //Ethis.SendPropertyChanging();
                  //  this.ReportPropertyChanging("Concept");
                    this._pConcept.Inlines.Clear();
                    this._Concept = value;
                    this._pConcept.Inlines.Add(this._Concept);
                    
                  //E  this.SendPropertyChanged("Concept");
                  //  this.ReportPropertyChanged("Concept");   
                }
            }
        }

       
        partial void OnReasonForDiagnosisChanged()
        {
            if (this._ReasonForDiagnosis == null)
            {
                Section d = this.FlowDiagnosis; // Update diagnosis
            }
            this._pReasonForDiagnosis.Inlines.Clear();
            if (this._ReasonForDiagnosis != null)
              this._pReasonForDiagnosis.Inlines.Add(this._ReasonForDiagnosis);
       
        }
  
        public string ComponentName
        {
            get
            {
                return this._ComponentName;
            }
            set
            {
                if ((this._ComponentName != value))
                {
                    //Ethis.SendPropertyChanging();
                    //this.ReportPropertyChanging("ComponentName");
                    this._ComponentName = value;
                    //Ethis.SendPropertyChanged("ComponentName");
                   // this.ReportPropertyChanging("ComponentName");
                }
            }
        }

        
    }
}
