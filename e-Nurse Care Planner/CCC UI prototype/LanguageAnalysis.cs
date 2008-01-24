using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCC.BusinessLayer;

// First stage: Count carepatterns,carecomponents,diagnoses and interventions for each language and
// compare with expected count

namespace CCC.UI
{

   

    
    public class CCCFrameworkLanguageAnalysis
    {

        private string _version;
        private int _expectedNumberOfCarePatterns;
        private int _expectedNumberOfCareComponents;
        private int _expectedNumberOfDiagnoses;
        private int _expectedNumberOfInterventions;
        private int _expectedNumberOfOutcomeTypes;
        private int _expectedNumberOfActionTypes;
              
        static CCCFrameworkEntities db = new CCCFrameworkEntities();

        private List<langFramework> _langframeworkList = new List<langFramework>();

        public List<langFramework> LanguageFrameworkList
        {
            get { return this._langframeworkList; }
        }

        private string _ExpectedInfo;

        public string ExpectedInfo
        {
            get { return this._ExpectedInfo; }
            set { this._ExpectedInfo = value; }
        }

        public string Version
        {
            get { return this._version; }
            set { this._version = value; }
        }

        public int ExpectedNumberOfCarePatterns
        {
            get { return this._expectedNumberOfCarePatterns; }
            set { this._expectedNumberOfCarePatterns = value; }
        }

        public int ExpectedNumberOfCareComponents
        {
            get { return this._expectedNumberOfCareComponents; }
            set { this._expectedNumberOfCareComponents = value; }
        }

        public int ExpectedNumberOfDiagnoses
        {
            get { return this._expectedNumberOfDiagnoses; }
            set { this._expectedNumberOfDiagnoses = value; }
        }

        public int ExpectedNumberOfInterventions
        {
            get { return this._expectedNumberOfInterventions; }
            set { this._expectedNumberOfInterventions = value; }
        }

        public int ExpectedNumberOfOutcomeTypes
        {
            get { return this._expectedNumberOfOutcomeTypes; }
            set { this._expectedNumberOfOutcomeTypes = value; }
        }

        public int ExpectedNumberOfActionTypes
        {
            get { return this._expectedNumberOfActionTypes; }
            set { this._expectedNumberOfActionTypes = value; }
        }



        private void showDatabaseError(Exception ex)
        {
            WindowDatabaseError wDatabaseError = new WindowDatabaseError();
            wDatabaseError.tbDatabaseError.Text = "Cannot access database server : " + db.Connection.DataSource;
            wDatabaseError.tbDatabaseErrorDetail.Text = ex.Source + ": " + ex.Message + "\n";
            if (ex.InnerException != null)
                wDatabaseError.tbDatabaseErrorDetail.Text += ex.InnerException.Source + ": " + ex.InnerException.Message + "\n";
            wDatabaseError.tbDatabaseErrorDetail.Text += ex.StackTrace;

            wDatabaseError.Show();

        }

        public CCCFrameworkLanguageAnalysis(string version)
        {

            this.Version = version;
            // Setup what to expect
            
            try
            {
               this.ExpectedNumberOfCarePatterns = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfCarePatterns;
               this.ExpectedNumberOfCareComponents = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumerOfCareComponents;
               this.ExpectedNumberOfDiagnoses = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfDiagnoses;
               this.ExpectedNumberOfInterventions = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfInterventions;
               this.ExpectedNumberOfOutcomeTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfOutcomeTypes;
               this.ExpectedNumberOfActionTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfActionTypes;

               
            }
            catch (Exception ex)
            {
                showDatabaseError(ex);
                
                return;
            }

            this.ExpectedInfo = "Analysis on version " + this.Version +
                    " - expected care patterns: " + this.ExpectedNumberOfCarePatterns +
                    " care components: " + this.ExpectedNumberOfCareComponents +
                    " diagnoses: " + this.ExpectedNumberOfDiagnoses +
                    " interventions: " + this.ExpectedNumberOfInterventions+
                    " outcomes: "+this.ExpectedNumberOfOutcomeTypes+
                    " actions: "+this.ExpectedNumberOfActionTypes;
               
            // Find unique languages...

            List<string> uniqueLanguages = new List<string>(findUniqueLanguagesNew());

            foreach (string language in uniqueLanguages)
            {
                langFramework lf = new langFramework();
                lf.LanguageName = language;
                
                try
                {
                    lf.CarePatternsShallow = db.CarePattern.Where(p => p.Language_Name == language).Count();
                    lf.CareComponentsShallow = db.Care_component.Where(p => p.Language_Name == language).Count();
                    lf.NursingDiagnosesShallow = db.Nursing_DiagnosisSet.Where(p => p.Language_Name == language).Count();
                    lf.NursingInterventionsShallow = db.Nursing_InterventionSet.Where(p => p.Language_Name == language).Count();
                    lf.OutcomeTypesShallow = db.OutcomeType.Where(p => p.Language_Name == language).Count();
                    lf.ActionTypesShallow = db.ActionType.Where(p => p.Language_Name == language).Count();
                    
                    
                    lf.MetaInfo = db.Copyright.Where(p => p.Language_Name == language).First().Name +
                        " " + db.Copyright.Where(p => p.Language_Name == language).First().Version +
                        " by " + db.Copyright.Where(p => p.Language_Name == language).First().Authors;
                    
                    
                }
                catch (Exception ex)
                {
                    showDatabaseError(ex);
                    return;
                }

               
                if (this.ExpectedNumberOfCarePatterns == lf.CarePatternsShallow)
                    lf.CarePatternsAsExpected = true;
                else
                    lf.CarePatternsAsExpected = false;

                if (this.ExpectedNumberOfCareComponents == lf.CareComponentsShallow)
                    lf.CareComponentsAsExpected = true;
                else
                    lf.CareComponentsAsExpected = false;

                if (this.ExpectedNumberOfDiagnoses == lf.NursingDiagnosesShallow)
                    lf.DiagnosesAsExpected = true;
                else
                    lf.DiagnosesAsExpected = false;

                if (this.ExpectedNumberOfInterventions == lf.NursingInterventionsShallow)
                    lf.InterventionsAsExpected = true;
                else
                    lf.InterventionsAsExpected = false;

                if (this.ExpectedNumberOfOutcomeTypes == lf.OutcomeTypesShallow)
                    lf.OutcomeTypesAsExpected= true;
                else
                    lf.OutcomeTypesAsExpected = false;


                if (this.ExpectedNumberOfActionTypes == lf.ActionTypesShallow)
                    lf.ActionTypesAsExpected = true;
                else
                    lf.ActionTypesAsExpected = false;

                this._langframeworkList.Add(lf);



                FrameworkActual fa;

                List<FrameworkActual> qf = (from f in db.FrameworkActual
                        where f.Language_Name == language && f.Version == this.Version
                        select f).ToList();

                if (qf.Count == 1) // Language already in database
                    fa = qf[0];
                else
                {
                    fa = new FrameworkActual();
                    fa.Version = this.Version;
                    fa.Language_Name = language;
                }

                fa.CarePattern = lf.CarePatternsShallow;
                fa.CareComponent = lf.CareComponentsShallow;
                fa.NursingDiagnosis = lf.NursingDiagnosesShallow;
                fa.NursingIntervention = lf.NursingInterventionsShallow;
                fa.OutcomeTypes = lf.OutcomeTypesShallow;
                fa.ActionTypes = lf.ActionTypesShallow;
                fa.Date = DateTime.Now;
              
                if (lf.CarePatternsAsExpected &&
                    lf.CareComponentsAsExpected &&
                    lf.DiagnosesAsExpected &&
                    lf.InterventionsAsExpected &&
                    lf.OutcomeTypesAsExpected &&
                    lf.ActionTypesAsExpected)
                    fa.ValidShallow = true;
                else
                    fa.ValidShallow = false;

               
                if (qf.Count == 0) // In case language is not found
                    db.AddToFrameworkActual(fa);
    
            }
            
            // Clean up language table in case analysis shows that some languages have been removed

            List<string> qFoundLanguages = (from l in db.FrameworkActual
                                            select l.Language_Name).ToList();

            List<string> cleanupLanguages = qFoundLanguages.Except(uniqueLanguages).ToList();

            foreach (string lang in cleanupLanguages)
            {
                FrameworkActual fadelete = db.FrameworkActual.Where(p => p.Language_Name == lang).First();
                db.DeleteObject(fadelete);
            }
           db.SaveChanges();
          
        }


        public List<string> findUniqueLanguagesNew()
        {
            List<string> qcarepattern;
            List<string> qcarecomponent;
            List<string> qdiagnoses;
            List<string> qinterventions;
            List<string> qoutcomes;
            List<string> qactions;
            
            try
            {
                 qcarepattern = (from cp in db.CarePattern
                                   select cp.Language_Name).ToList();

                 qcarecomponent = (from cc in db.Care_component
                                     select cc.Language_Name).ToList();

                 qdiagnoses = (from nd in db.Nursing_DiagnosisSet
                                 select nd.Language_Name).ToList();

                 qinterventions = (from ni in db.Nursing_InterventionSet
                                     select ni.Language_Name).ToList();


                 qoutcomes = (from outcome in db.OutcomeType
                                   select outcome.Language_Name).ToList();

                 qactions = (from action in db.ActionType
                             select action.Language_Name).ToList();
            
            }
           catch (Exception ex)
            {
                showDatabaseError(ex);
                return null;
            }

            return qcarepattern.Union(qcarecomponent).Union(qdiagnoses).Union(qinterventions).Union(qoutcomes).Union(qactions).ToList();
            
        }
    }
}