using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace CCC.BusinessLayer
{

    public class CCC_Framework : INotifyPropertyChanged 
    {
        private List<CarePattern> _patterns;
        private List<Care_component> _components;
       
        private List<Nursing_Diagnosi> _diagnoses;
        private List<Nursing_Intervention> _interventions;
// NOT implemented yet        private List<ExpectedOutcome> _outcomes;

       //Estatic private CCCDataContext _db;
        static private CCCFrameworkEntities _db;


        // Copyright information

        private string _name;
        private string _version;
        private string _authors;

       public CCC_Framework(string languageName)
        {

           
           _db = new CCCFrameworkEntities();

           loadFramework(languageName);  
         
 }
       public void loadFramework(string languageName)
       {
           this.Name = _db.CopyrightSet.Where(c => c.Language_Name == languageName).First().Name;
           this.Authors = _db.CopyrightSet.Where(c => c.Language_Name == languageName).First().Authors;
           this.Version = _db.CopyrightSet.Where(c => c.Language_Name == languageName).First().Version;


           this._patterns = _db.CarePattern.Where(p => p.Language_Name == languageName).ToList();
           this._components = _db.Care_component.Where(p => p.Language_Name == languageName).ToList();
           this._diagnoses = _db.Nursing_Diagnosis.Where(p => p.Language_Name == languageName).ToList();
           this._interventions = _db.Nursing_Intervention.Where(p => p.Language_Name == languageName).ToList();
           
       }

    public CCCFrameworkEntities DB    
    {
            get { return _db; }
        }

         public List<Care_component> Components
        {
            get { return this._components; }
            
        }

        public List<Nursing_Diagnosi> Diagnoses
        {
            get { return this._diagnoses; }
        }

        public List<Nursing_Intervention> Inteventions
        {
            get { return this._interventions; }
        }

        public List<CarePattern> Patterns 
        {
            get { return this._patterns; }
        }

        //public List<ExpectedOutcome> Outcomes
        //{
        //    get { return this._outcomes; }
        //}


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

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)        
        { 
            if (this.PropertyChanged != null)
                  PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }     
    }
}
