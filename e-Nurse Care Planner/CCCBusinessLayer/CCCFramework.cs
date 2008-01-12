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
// NOT implemented yet        private List<ExpectedOutcome> _outcomes;

       //Estatic private CCCDataContext _db;
        static private CCCFrameworkEntities _db;
       public CCC_Framework()
        {

            //E_db = new CCCDataContext();
           
           _db = new CCCFrameworkEntities();

           // _db = new CCCDataContext(Properties.Settings.Default.CCCConnectionString);

            //BUG???_db.ObjectTrackingEnabled = false;

            // Read from framework database and fill up lists

           
            this._patterns = _db.CarePattern.ToList();
            this._components = _db.Care_component.ToList();
            this._diagnoses = _db.Nursing_Diagnosis.ToList();
            this._interventions = _db.Nursing_Intervention.ToList();
            // NOT implemented yet this._outcomes = _db.ExpectedOutcomes.ToList();

            // Does not work in Visual Studio 2008 Beta 2???? db.ObjectTrackingEnabled = false; // Only want read access to framework, tip from Mike Taulty blog video

 }
      //E  public CCCDataContext DB
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

       

    }
}
