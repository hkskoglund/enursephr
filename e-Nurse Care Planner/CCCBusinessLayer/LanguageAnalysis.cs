using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReferenceFrameworkModel;
using System.ComponentModel;


namespace CCC.BusinessLayer
{

   

    
    public class CCCFrameworkLanguageAnalysis : INotifyPropertyChanged
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



       
        public CCCFrameworkLanguageAnalysis(string version)
        {

            this.Version = version;
            // Setup what to expect
            
               this.ExpectedNumberOfCarePatterns = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfCarePatterns;
               this.ExpectedNumberOfCareComponents = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumerOfCareComponents;
               this.ExpectedNumberOfDiagnoses = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfDiagnoses;
               this.ExpectedNumberOfInterventions = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfInterventions;
               this.ExpectedNumberOfOutcomeTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfOutcomeTypes;
               this.ExpectedNumberOfActionTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfActionTypes;

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
                
                    lf.CarePatternsShallow = db.CarePattern.Where(p => p.Language_Name == language).Count();
                    lf.CareComponentsShallow = db.Care_component.Where(p => p.Language_Name == language).Count();
                    lf.NursingDiagnosesShallow = db.Nursing_Diagnosis.Where(p => p.Language_Name == language).Count();
                    lf.NursingInterventionsShallow = db.Nursing_Intervention.Where(p => p.Language_Name == language).Count();
                    lf.OutcomeTypesShallow = db.OutcomeType.Where(p => p.Language_Name == language).Count();
                    lf.ActionTypesShallow = db.ActionType.Where(p => p.Language_Name == language).Count();
                    
                    
                    lf.MetaInfo = db.Copyright.Where(p => p.Language_Name == language).First().Name +
                        " " + db.Copyright.Where(p => p.Language_Name == language).First().Version +
                        " by " + db.Copyright.Where(p => p.Language_Name == language).First().Authors;
                    
                    
              
               
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
            
                 qcarepattern = (from cp in db.CarePattern
                                   select cp.Language_Name).ToList();

                 qcarecomponent = (from cc in db.Care_component
                                     select cc.Language_Name).ToList();

                 qdiagnoses = (from nd in db.Nursing_Diagnosis
                                 select nd.Language_Name).ToList();

                 qinterventions = (from ni in db.Nursing_Intervention
                                     select ni.Language_Name).ToList();


                 qoutcomes = (from outcome in db.OutcomeType
                                   select outcome.Language_Name).ToList();

                 qactions = (from action in db.ActionType
                             select action.Language_Name).ToList();
            
          
            return qcarepattern.Union(qcarecomponent).Union(qdiagnoses).Union(qinterventions).Union(qoutcomes).Union(qactions).ToList();
            
        }

        private ReferenceFrameworkEntities ctx = new ReferenceFrameworkEntities();

        private CCCFrameworkEntities ctxverify = new CCCFrameworkEntities();

        private MCareComponent _fCareComponent = new MCareComponent();
        public MCareComponent FCareComponent
        {
            get { return this._fCareComponent; }
            set { this._fCareComponent = value; }
        }

        private MOutcomeType _fOutcomeType = new MOutcomeType();
        public MOutcomeType FOutcomeType
        {
            get { return this._fOutcomeType; }
            set { this._fOutcomeType = value; }
        }


        private MNursingDiagnosis _fNursingDiagnosis = new MNursingDiagnosis();
        public MNursingDiagnosis FNursingDiagnosis
        {
            get { return this._fNursingDiagnosis; }
            set { this._fNursingDiagnosis = value; }
        }

        private MNursingIntervention _fNursingIntervention = new MNursingIntervention();
        public MNursingIntervention FNursingIntervention
        {
            get { return this._fNursingIntervention; }
            set { this._fNursingIntervention = value; }
        }

        private MActionType _fActionType = new MActionType();
        public MActionType FActionType
        {
            get { return this._fActionType; }
            set { this._fActionType = value; }
        }

      
        private string _languageName;

        public string LanguageName
        {
            get { return this._languageName; }
            set { this._languageName = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private string _activity;

        public string Activity
        {
            get { return this._activity; }

            set
            {
                if (value != this._activity)
                {
                    this._activity = value;
                    OnPropertyChanged("Activity");
                }
            }
        }


        public void doDLAnalysis(string version, string languageName, BackgroundWorker worker)
        {


            this.Version = version;
            this.LanguageName = languageName;

            this.Activity = "Analysing care components (1 of 5)";


            this.FCareComponent.MissingList =
          deepVerifyCareComponent(version, languageName).OrderBy(c => c.Code).Where(c => c.Verified == false).ToList();


            worker.ReportProgress(20, FCareComponent);

            this.Activity = "Analysing outcome types (2 of 5)";
            this.FOutcomeType.MissingList =
             deepVerifyOutcomeType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();
            worker.ReportProgress(40, FOutcomeType);

            this.Activity = "Analysing nursing diagnoses (3 of 5)";
            this.FNursingDiagnosis.MissingList =
              deepVerifyNursingDiagnosis(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();
            worker.ReportProgress(60, FNursingDiagnosis);

            this.Activity = "Analysing action types (4 of 5)";
            this.FActionType.MissingList =
                deepVerifyActionType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();

            worker.ReportProgress(80, FActionType);

            this.Activity = "Analysing nursing intervention (5 of 5)";
            this.FNursingIntervention.MissingList =
            deepVerifyNursingIntervention(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();
            worker.ReportProgress(100, FNursingIntervention);

            this.Activity = "Analysis complete";
        }

        public List<ReferenceFrameworkModel.Care_Component> deepVerifyCareComponent(string verifyVersion, string verifyLanguageName)
        {
            var referenceCareComponent = (from c in ctx.Care_Component
                                          where c.Version == verifyVersion
                                          select c);


            foreach (var verifyc in referenceCareComponent)
            {
                var verifyCareComponent = from frameworkc in ctxverify.Care_component
                                          where frameworkc.Code == verifyc.Code &&
                                             frameworkc.Language_Name == verifyLanguageName &&
                                             frameworkc.Version == verifyVersion
                                          select frameworkc;

                if (verifyCareComponent.Count() == 1)
                    verifyc.Verified = true;
                else
                    verifyc.Verified = false;
            }

            return referenceCareComponent.ToList();
        }

        public List<ReferenceFrameworkModel.OutcomeType> deepVerifyOutcomeType(string verifyVersion, string verifyLanguageName)
        {
            var referenceOutcomeType = (from c in ctx.OutcomeType
                                        where c.Version == verifyVersion
                                        select c);


            foreach (var verifyo in referenceOutcomeType)
            {
                var verifyOutcomeType = from frameworko in ctxverify.OutcomeType
                                        where frameworko.Code == verifyo.Code &&
                                           frameworko.Language_Name == verifyLanguageName &&
                                           frameworko.Version == verifyVersion
                                        select frameworko;

                if (verifyOutcomeType.Count() == 1)
                    verifyo.Verified = true;
                else
                    verifyo.Verified = false;
            }

            return referenceOutcomeType.ToList();
        }

        public List<ReferenceFrameworkModel.ActionType> deepVerifyActionType(string verifyVersion, string verifyLanguageName)
        {
            var referenceActionType = (from c in ctx.ActionType
                                       where c.Version == verifyVersion
                                       select c);


            foreach (var verifya in referenceActionType)
            {
                var verifyActionType = from frameworka in ctxverify.ActionType
                                       where frameworka.Code == verifya.Code &&
                                          frameworka.Language_Name == verifyLanguageName &&
                                          frameworka.Version == verifyVersion
                                       select frameworka;

                if (verifyActionType.Count() == 1)
                    verifya.Verified = true;
                else
                    verifya.Verified = false;
            }

            return referenceActionType.ToList();
        }

        public List<ReferenceFrameworkModel.Nursing_Diagnosis> deepVerifyNursingDiagnosis(string verifyVersion, string verifyLanguageName)
        {
            List<ReferenceFrameworkModel.Nursing_Diagnosis> referenceNursingDiagnosis =
                (from diag in ctx.Nursing_Diagnosis
                 where diag.Version == verifyVersion
                 select diag).ToList();


            foreach (ReferenceFrameworkModel.Nursing_Diagnosis verifyDiag in referenceNursingDiagnosis)
            {

                List<CCC.BusinessLayer.Nursing_Diagnosis> verifyNursingDiagnosis = new List<CCC.BusinessLayer.Nursing_Diagnosis>();

                if (verifyDiag.MinorCode != null)
                    verifyNursingDiagnosis = (from
                                                   frameworkd in ctxverify.Nursing_Diagnosis
                                              where
                                                 frameworkd.ComponentCode == verifyDiag.ComponentCode &&
                                                 frameworkd.MajorCode == verifyDiag.MajorCode &&
                                                 frameworkd.MinorCode == verifyDiag.MinorCode &&
                                                 frameworkd.Language_Name == verifyLanguageName &&
                                                 frameworkd.Version == verifyVersion
                                              select
                                                 frameworkd).ToList();
                else
                    verifyNursingDiagnosis = (from
                                               frameworkd in ctxverify.Nursing_Diagnosis
                                              where
                                            frameworkd.ComponentCode == verifyDiag.ComponentCode &&
                                            frameworkd.MajorCode == verifyDiag.MajorCode &&
                                            frameworkd.MinorCode == null &&
                                            frameworkd.Language_Name == verifyLanguageName &&
                                            frameworkd.Version == verifyVersion
                                              select
                                                 frameworkd).ToList();

                int diagFound = verifyNursingDiagnosis.Count();

                if (verifyNursingDiagnosis.Count() == 1)
                    verifyDiag.Verified = true;
                else
                    verifyDiag.Verified = false;
            }

            return referenceNursingDiagnosis;
        }

        public List<ReferenceFrameworkModel.Nursing_Intervention> deepVerifyNursingIntervention(string verifyVersion, string verifyLanguageName)
        {
            List<ReferenceFrameworkModel.Nursing_Intervention> referenceNursingIntervention =
                (from interv in ctx.Nursing_Intervention
                 where interv.Version == verifyVersion
                 select interv).ToList();


            foreach (ReferenceFrameworkModel.Nursing_Intervention verifyInterv in referenceNursingIntervention)
            {

                List<CCC.BusinessLayer.Nursing_Intervention> verifyNursingIntervention = new List<CCC.BusinessLayer.Nursing_Intervention>();

                if (verifyInterv.MinorCode != null)
                    verifyNursingIntervention = (from
                                                   frameworki in ctxverify.Nursing_Intervention
                                                 where
                                                    frameworki.ComponentCode == verifyInterv.ComponentCode &&
                                                    frameworki.MajorCode == verifyInterv.MajorCode &&
                                                    frameworki.MinorCode == verifyInterv.MinorCode &&
                                                    frameworki.Language_Name == verifyLanguageName &&
                                                    frameworki.Version == verifyVersion
                                                 select
                                                    frameworki).ToList();
                else
                    verifyNursingIntervention = (from
                                               frameworki in ctxverify.Nursing_Intervention
                                                 where
                                               frameworki.ComponentCode == verifyInterv.ComponentCode &&
                                               frameworki.MajorCode == verifyInterv.MajorCode &&
                                               frameworki.MinorCode == null &&
                                               frameworki.Language_Name == verifyLanguageName &&
                                               frameworki.Version == verifyVersion
                                                 select
                                                    frameworki).ToList();

                int intervFound = verifyNursingIntervention.Count();

                if (intervFound == 1)
                    verifyInterv.Verified = true;
                else
                    verifyInterv.Verified = false;
            }

            return referenceNursingIntervention.ToList();
        }

    }
}