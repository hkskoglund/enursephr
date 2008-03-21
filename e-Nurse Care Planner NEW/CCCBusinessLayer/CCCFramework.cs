using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;

namespace CCC.BusinessLayer
{

    public class CCC_Framework : INotifyPropertyChanged 
    {
        private List<CarePattern> _patterns;
        private List<Care_component> _components;
       
        private List<FrameworkDiagnosis> _diagnoses;
        private List<FrameworkIntervention> _interventions;
        private List<FrameworkOutcomeType> _outcomes;
        private List<ActionType> _actionTypes;

        static private CCCFrameworkCompactEntities _db;
        
        
        // Copyright information

        private string _name;
        private string _version;
        private string _authors;
        private DateTime _lastupdate;
        private string _logoURL;


        // Languages

        private List<FrameworkActual> _factual;

        public List<FrameworkActual> FrameworkActual
        {
            get { return this._factual; }
        }

       public CCC_Framework(string languageName, string version)
        {

           
           _db = new CCCFrameworkCompactEntities();

           loadFramework(languageName,version);  
         
 }
       public void loadFramework(string languageName, string version)
       {
                    
           // Get copyright information
           this.Name = _db.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Name;
           this.Authors = _db.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Authors;
           this.Version = _db.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Version;
           this.LastUpdate = (DateTime)_db.Copyright.Where(c => c.Language_Name == languageName && c.Version == "2.0").First().LastUpdate;
           this.LogoURL = _db.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().LogoURL;

           this._patterns = _db.CarePattern.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
           this._components = _db.Care_component.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
           this._diagnoses = _db.Nursing_Diagnosis.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
           this._interventions = _db.Nursing_Intervention.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
           this._outcomes = _db.OutcomeType.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
           this._actionTypes =  _db.ActionType.Where(p => p.Language_Name == languageName && p.Version == version).ToList();

           // Load last languages found in analysis

           loadFrameworkAnalysis();
       }

       public void loadFrameworkAnalysis()
       {
           if (this._factual != null)
               _db.Refresh(System.Data.Objects.RefreshMode.StoreWins, this._factual);
           else
               this._factual = _db.FrameworkActual.ToList();
       }

    public CCCFrameworkCompactEntities DB    
    {
            get { return _db; }
        }

         public List<Care_component> Components
        {
            get { return this._components; }
            
        }

        public List<FrameworkDiagnosis> Diagnoses
        {
            get { return this._diagnoses; }
        }

        public List<FrameworkIntervention> Inteventions
        {
            get { return this._interventions; }
        }

        public List<CarePattern> Patterns 
        {
            get { return this._patterns; }
        }

        public List<FrameworkOutcomeType> Outcomes
        {
            get { return this._outcomes; }
        }


        public List<ActionType> ActionTypes
        {
            get { return this._actionTypes; }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                OnPropertyChanged("Name");
            }
                
        }

        public string Version
        {
            get { return this._version; }
            set
            {
                this._version = value;
                OnPropertyChanged("Version");
            }
        }

        public string Authors
        {
            get { return this._authors; }
            set
            {
                this._authors = value;
                OnPropertyChanged("Authors");
            }
        }

        public DateTime LastUpdate
        {
            get { return this._lastupdate; }
            set
            {
                this._lastupdate = value;
                OnPropertyChanged("LastUpdate");
            }
        }

        public string LogoURL
        {
            get { return this._logoURL; }
            set
            {
                this._logoURL = value;
                OnPropertyChanged("logoURL");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)        
        { 
            if (this.PropertyChanged != null)
                  PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }     
    }
}
