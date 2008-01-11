using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CCC.BusinessLayer
{

    public class CCC_Framework
    {
        private List<CarePattern> _patterns;
        private List<Care_component> _components;
       
        private List<Nursing_Diagnosi> _diagnoses;
        private List<Nursing_Intervention> _interventions;
        private List<ExpectedOutcome> _outcomes;

       static private CCCDataContext _db;

       public CCC_Framework()
        {
            _db = new CCCDataContext();
           // _db = new CCCDataContext(Properties.Settings.Default.CCCConnectionString);

            //BUG???_db.ObjectTrackingEnabled = false;

            // Read from framework database and fill up lists

            this._patterns = _db.CarePatterns.ToList();
            this._components = _db.Care_components.ToList();
            this._diagnoses = _db.Nursing_Diagnosis.ToList();
            this._interventions = _db.Nursing_Interventions.ToList();
            this._outcomes = _db.ExpectedOutcomes.ToList();

            // Does not work in Visual Studio 2008 Beta 2???? db.ObjectTrackingEnabled = false; // Only want read access to framework, tip from Mike Taulty blog video

 }
        public CCCDataContext DB
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

        public List<ExpectedOutcome> Outcomes
        {
            get { return this._outcomes; }
        }

       

    }
}
