using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CCC.BusinessLayer
{
   
    public class myCarePlan
       
    {

      
        private PleieplanEntitiesv2 _db = new PleieplanEntitiesv2();
        
    //E    private CarePlanDBDataContext _db = new CarePlanDBDataContext();

//        private CarePlanDBDataContext _db = new CarePlanDBDataContext(Properties.Settings.Default.PleieplanConnectionString);

        public List<Diagnosi> _diagnoses = null;

        private CarePlan cp;

      
        private int _id;

        
//E        public myCarePlan(int id, CCCDataContext CCCDB)
        public myCarePlan(int id, CCCFrameworkEntities CCCDB)
   
    {
        
           
            _id = id;

            
//E           _diagnoses = _db.Diagnosis.Where(d => d.careplanId == id).ToList();

           _diagnoses = _db.Diagnosis.Where(d => d.CarePlan.Id == id).ToList();

            //E  cp = _db.CarePlans.Single(d => d.Id == id);
           cp = _db.CarePlan.First(fcp => fcp.Id == id);

            

      
           //  cp.Annotation = new byte[]{0,1,2,3};
          // DEBUG  System.Data.Linq.ChangeSet cs = _db.GetChangeSet();
          //  _db.SubmitChanges();

            // Find Concept


         

            updateExtendedData(CCCDB);
        }

            public void updateExtendedData(CCCFrameworkEntities CCCDB)
            {
            foreach (Diagnosi d in _diagnoses)
            {
                // TEMPORARY TEST SOLUTION FOR LANGUAGAGE......
                //Nursing_Diagnosi ndiag =   CCCDB.Nursing_Diagnosis.First(nd => nd.DiagnosisID ==  d.cccId);
                Nursing_Diagnosis ndiag = CCCDB.Nursing_DiagnosisSet.Where(diag => diag.Language_Name == "nb-NO").First(nd => nd.Id == d.cccId);
                d.Concept = ndiag.Concept;
                //E d.ComponentName = ndiag.Care_component.Component;
                d.ComponentName = " To be found.....";
                if (d.CreationDate != null)
                    d.CreationDateString = d.CreationDate.Value.ToLongDateString();
                else d.CreationDateString = null;
                d.Definition = ndiag.Definition;

            }
            }


        
        
        //public List<Diagnosi> Diagnoses
        //{
        //    get { return this._diagnoses; }
           
        //    set { this._diagnoses = value; }
            
        //  }
  
        public void DeleteDiagnosis(Diagnosi d)
        {
            this._diagnoses.Remove(d);
            //E_db.Diagnosis.DeleteOnSubmit(d);
        
            //_db.SubmitChanges();
            _db.DeleteObject(d);
            _db.SaveChanges();
         
        }

        public void InsertDiagnosis(Diagnosi d)
        {
            this._diagnoses.Add(d);
            //E _db.Diagnosis.InsertOnSubmit(d);
            //E _db.SubmitChanges();
            _db.AddToDiagnosis(d);
            _db.SaveChanges();
        }

        public void UpdateDiagnosis(Diagnosi d)
        {
         //E  _db.SubmitChanges();
            _db.SaveChanges();
        }

       // public CarePlanDBDataContext DB
        
    public PleieplanEntitiesv2 DB
    {
            get { return this._db; }
            set { this._db = value; }
        }

        public int Id
        {
            get { return _id; }
        }

    }
}
