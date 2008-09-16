#define SQL_SERVER_COMPACT_SP1_WORKAROUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;
using System.Data.Objects;

using eNursePHR.BusinessLayer.CCC_Translations;

namespace eNursePHR.BusinessLayer
{
    /// <summary>
    /// This class load all Clinical Care Classification data and provides properties to bind UI to
    /// </summary>
    public class CCC_Framework : INotifyPropertyChanged
    {

        #region CCC Framework
        private List<CarePattern> _patterns;
        public List<CarePattern> Patterns
        {
            get { return this._patterns; }
        }
        
        private List<Care_component> _components;
        public List<Care_component> Components
        {
            get { return this._components; }

        }
        
        private List<Nursing_Diagnosis> _diagnoses;
        public List<Nursing_Diagnosis> Diagnoses
        {
            get { return this._diagnoses; }
        }

        
        private List<Nursing_Intervention> _interventions;
        public List<Nursing_Intervention> Inteventions
        {
            get { return this._interventions; }
        }

        
        private List<OutcomeType> _outcomes;
        public List<OutcomeType> Outcomes
        {
            get { return this._outcomes; }
        }

        private List<ActionType> _actionTypes;
        public List<ActionType> ActionTypes
        {
            get { return this._actionTypes; }
        }

        #endregion

        #region Copyright information
        // Copyright information

        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                OnPropertyChanged("Name");
            }

        }

        
        private string _version;
        public string Version
        {
            get { return this._version; }
            set
            {
                this._version = value;
                OnPropertyChanged("Version");
            }
        }

        
        private string _authors;
        public string Authors
        {
            get { return this._authors; }
            set
            {
                this._authors = value;
                OnPropertyChanged("Authors");
            }
        }

        
        private DateTime _lastupdate;
        public DateTime LastUpdate
        {
            get { return this._lastupdate; }
            set
            {
                this._lastupdate = value;
                OnPropertyChanged("LastUpdate");
            }
        }

        private string _logoURL;
        public string LogoURL
        {
            get { return this._logoURL; }
            set
            {
                this._logoURL = value;
                OnPropertyChanged("logoURL");
            }
        }
        #endregion

        #region Language statictics
        
        private List<FrameworkActual> _actualLanguageAnalysisResult;

        public List<FrameworkActual> ActualLanguageAnalysisResult
        {
            get { return this._actualLanguageAnalysisResult; }
        }

        #endregion

        private CCC_FrameworkEntities _db;
        public CCC_FrameworkEntities DB
        {
            get
            {
                if (this._db == null)
                    this._db = new CCC_FrameworkEntities();
                return this._db;
            }
        }

        
       public CCC_Framework(string languageName, string version)
        {
            
           //loadFramework(languageName,version);  
         
 }
        /// <summary>
        /// Loads meta information of CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
       public void loadMetaInformation(string version, string languageName)
       {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
        
           this.Name = this.DB.Copyright.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").First().Name;
           this.Authors = this.DB.Copyright.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").First().Authors;
           this.Version = this.DB.Copyright.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").First().Version;
           this.LastUpdate = (DateTime)this.DB.Copyright.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").First().LastUpdate;
           this.LogoURL = this.DB.Copyright.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").First().LogoURL;
#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
              this.Name = this.DB.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Name;
              this.Authors = this.DB.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Authors;
            this.Version = this.DB.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().Version;
            this.LogoURL = this.DB.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().LogoURL;
               this.LastUpdate = (DateTime)this.DB.Copyright.Where(c => c.Language_Name == languageName && c.Version == version).First().LastUpdate;

#endif
       }

        /// <summary>
        /// Loads carecomponents and patterns from CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
       protected void loadCareComponentAndPattern(string version, string languageName)
       {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
               // SP 1 work-around

           this._patterns = this.DB.CarePattern.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();
           this._components = this.DB.Care_component.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();
#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             this._patterns = this.DB.CarePattern.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
             this._components = this.DB.Care_component.Where(p => p.Language_Name == languageName && p.Version == version).ToList();

#endif
       }

        /// <summary>
        /// Loads diagnoses from CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
       protected void loadDiagnoses(string version, string languageName)
       {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
           this._diagnoses = this.DB.Nursing_Diagnosis.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();

#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             this._diagnoses = this.DB.Nursing_Diagnosis.Where(p => p.Language_Name == languageName && p.Version == version).ToList();

#endif

       }

        /// <summary>
        /// Loads interventions from CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
       protected void loadInterventions(string version, string languageName)
       {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
           this._interventions = this.DB.Nursing_Intervention.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();

#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
              this._interventions = this.DB.Nursing_Intervention.Where(p => p.Language_Name == languageName && p.Version == version).ToList();

#endif

       }
       
        /// <summary>
        /// Loads outcome types from CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
        protected void loadOutcomeTypes(string version, string languageName)
       {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
           this._outcomes = this.DB.OutcomeType.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();
        
#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             this._outcomes = this.DB.OutcomeType.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
        
#endif

       }


        /// <summary>
        /// Loads action types from CCC framework
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
        protected void loadActionTypes(string version, string languageName)
        {
#if (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            this._actionTypes = this.DB.ActionType.Where("it.Version = '" + version + "' AND it.Language_Name ='" + languageName + "'").ToList();

#elif (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
           this._actionTypes = this.DB.ActionType.Where(p => p.Language_Name == languageName && p.Version == version).ToList();
        
#endif

        }

       /// <summary>
       /// Load language analysis data that shows language integrity
       /// </summary>
       public void loadFrameworkLanguageAnalysis()
       {
           // If framework language analysis contains data, then do a refresh from the database
           if (this._actualLanguageAnalysisResult != null)
               this.DB.Refresh(System.Data.Objects.RefreshMode.StoreWins, this._actualLanguageAnalysisResult);
           else
               this._actualLanguageAnalysisResult = this.DB.FrameworkActual.ToList();  // Try do load language analysis data
       }

        // Used in update of meta data information
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)        
        { 
            if (this.PropertyChanged != null)
                  PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }     
    }
}
