#define SQL_SERVER_COMPACT_SP1_WORKAROUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using eNursePHR.BusinessLayer.CCC_Translations;
using eNursePHR.BusinessLayer.CCC_Terminology;


namespace eNursePHR.BusinessLayer
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
              
        static CCC_FrameworkEntities db = new CCC_FrameworkEntities();

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

            try
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                 this.ExpectedNumberOfCarePatterns = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfCarePatterns;
                 this.ExpectedNumberOfCareComponents = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumerOfCareComponents;
                 this.ExpectedNumberOfDiagnoses = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfDiagnoses;
                 this.ExpectedNumberOfInterventions = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfInterventions;
                 this.ExpectedNumberOfOutcomeTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfOutcomeTypes;
                 this.ExpectedNumberOfActionTypes = db.Framework.Where(v => v.Version.Contains(this.Version)).First().ExpectedNumberOfActionTypes;
                
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)

                // SP 1 code
                this.ExpectedNumberOfCarePatterns = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumberOfCarePatterns;
                this.ExpectedNumberOfCareComponents = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumerOfCareComponents;
                this.ExpectedNumberOfDiagnoses = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumberOfDiagnoses;
                this.ExpectedNumberOfInterventions = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumberOfInterventions;
                this.ExpectedNumberOfOutcomeTypes = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumberOfOutcomeTypes;
                this.ExpectedNumberOfActionTypes = db.Framework.Where("it.Version = '" + this.Version + "'").First().ExpectedNumberOfActionTypes;
#endif
            }
            catch (Exception ex)
            {
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

#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                
                    lf.CarePatternsShallow = db.CarePattern.Where(p => p.Language_Name == language).Count();
                    lf.CareComponentsShallow = db.Care_component.Where(p => p.Language_Name == language).Count();
                    lf.NursingDiagnosesShallow = db.Nursing_Diagnosis.Where(p => p.Language_Name == language).Count();
                    lf.NursingInterventionsShallow = db.Nursing_Intervention.Where(p => p.Language_Name == language).Count();
                    lf.OutcomeTypesShallow = db.OutcomeType.Where(p => p.Language_Name == language).Count();
                    lf.ActionTypesShallow = db.ActionType.Where(p => p.Language_Name == language).Count();
                    
                    lf.MetaInfo = db.Copyright.Where(p => p.Language_Name == language).First().Name +
                        " " + db.Copyright.Where(p => p.Language_Name == language).First().Version +
                        " by " + db.Copyright.Where(p => p.Language_Name == language).First().Authors;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
#endif                    
                    
                lf.CarePatternsShallow = db.CarePattern.Where("it.Language_Name = '"+language+"'").Count();
                lf.CareComponentsShallow = db.Care_component.Where("it.Language_Name = '" + language + "'").Count();
                lf.NursingDiagnosesShallow = db.Nursing_Diagnosis.Where("it.Language_Name = '" + language + "'").Count();
                lf.NursingInterventionsShallow = db.Nursing_Intervention.Where("it.Language_Name = '" + language + "'").Count();
                lf.OutcomeTypesShallow = db.OutcomeType.Where("it.Language_Name = '" + language + "'").Count();
                lf.ActionTypesShallow = db.ActionType.Where("it.Language_Name = '" + language + "'").Count();

                lf.MetaInfo = db.Copyright.Where("it.Language_Name = '" + language + "'").First().Name +
                    " " + db.Copyright.Where("it.Language_Name = '" + language + "'").First().Version +
                    " by " + db.Copyright.Where("it.Language_Name = '" + language + "'").First().Authors;
              
                
                lf.CarePatternsAsExpected = (this.ExpectedNumberOfCarePatterns == lf.CarePatternsShallow) ? true : false;
                lf.CareComponentsAsExpected = (this.ExpectedNumberOfCareComponents == lf.CareComponentsShallow) ? true : false;
                lf.DiagnosesAsExpected = (this.ExpectedNumberOfDiagnoses == lf.NursingDiagnosesShallow) ? true : false;
                lf.InterventionsAsExpected = (this.ExpectedNumberOfInterventions == lf.NursingInterventionsShallow) ? true : false;
                lf.OutcomeTypesAsExpected = (this.ExpectedNumberOfOutcomeTypes == lf.OutcomeTypesShallow) ? true : false;
                lf.ActionTypesAsExpected = (this.ExpectedNumberOfActionTypes == lf.ActionTypesShallow) ? true : false;

                this._langframeworkList.Add(lf);



                FrameworkActual fa;

#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                List<FrameworkActual> qf = (from f in db.FrameworkActual
                        where f.Language_Name == language && f.Version == this.Version
                        select f).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                List<FrameworkActual> qf = db.FrameworkActual.Where("it.Language_Name = '"+language+"' AND it.Version = '"+version+"'").ToList();
                                            
#endif
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
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                FrameworkActual fadelete = db.FrameworkActual.Where(p => p.Language_Name == lang).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                FrameworkActual fadelete = db.FrameworkActual.Where("it.Language_Name = '"+ lang+"'").First();

#endif
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

        private CCC_Terminology_ReferenceEntities ctx = new CCC_Terminology_ReferenceEntities();

        private CCC_FrameworkEntities ctxverify = new CCC_FrameworkEntities();

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

        /// <summary>
        /// This method is executed in the background and does a "deep level" language analysis
        /// </summary>
        /// <param name="version"></param>
        /// <param name="languageName"></param>
        /// <param name="worker"></param>
        public void doDLAnalysis(string version, string languageName, BackgroundWorker worker)
        {

            this.Version = version;
            this.LanguageName = languageName;

            // Care component
            DLACareComponent(version, languageName);
            worker.ReportProgress(20, FCareComponent);

            // Outcome types
            DLAOutcomeTypes(version, languageName);
            worker.ReportProgress(40, FOutcomeType);

            // Diagnoses
            DLADiagnosis(version, languageName);
            worker.ReportProgress(60, FNursingDiagnosis);
            
            // Action types
            DLAActionTypes(version, languageName);
            worker.ReportProgress(80, FActionType);

            // Interventions
            DLAIntervention(version, languageName);
            worker.ReportProgress(100, FNursingIntervention);

            this.Activity = "Analysis complete";
        }

        private void DLAIntervention(string version, string languageName)
        {
            this.Activity = "Analysing nursing intervention (5 of 5)";
            this.FNursingIntervention.MissingList =
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                deepVerifyNursingIntervention(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
 deepVerifyNursingIntervention(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();

#endif
        }

        private void DLAActionTypes(string version, string languageName)
        {
            this.Activity = "Analysing action types (4 of 5)";
            this.FActionType.MissingList =
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                deepVerifyActionType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                // CHECK THIS
                deepVerifyActionType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();

#endif
        }

        private void DLADiagnosis(string version, string languageName)
        {
            this.Activity = "Analysing nursing diagnoses (3 of 5)";
            this.FNursingDiagnosis.MissingList =
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
              deepVerifyNursingDiagnosis(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                // Check this
                  deepVerifyNursingDiagnosis(version, languageName).OrderBy(c => c.ComponentCode).Where(d => d.Verified == false).ToList();
#endif
        }

        private void DLAOutcomeTypes(string version, string languageName)
        {
            this.Activity = "Analysing outcome types (2 of 5)";
            this.FOutcomeType.MissingList =
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)

                deepVerifyOutcomeType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                // CHECK THIS
 deepVerifyOutcomeType(version, languageName).OrderBy(c => c.Code).Where(d => d.Verified == false).ToList();
#endif
        }

        private void DLACareComponent(string version, string languageName)
        {
            this.Activity = "Analysing care components (1 of 5)";


            this.FCareComponent.MissingList =
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
          deepVerifyCareComponent(version, languageName).OrderBy(c => c.Code).Where(c => c.Verified == false).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                // CHECK THIS
                deepVerifyCareComponent(version, languageName).OrderBy(c => c.Code).Where(c => c.Verified == false).ToList();


#endif
        }

        public List<eNursePHR.BusinessLayer.CCC_Terminology.Care_Component> deepVerifyCareComponent(string verifyVersion, string verifyLanguageName)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            var referenceCareComponent = (from c in ctx.Care_Component
                                          where c.Version == verifyVersion
                                          select c);
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            var referenceCareComponent = ctx.Care_Component.Where("it.Version = '"+verifyVersion+"'");

#endif

            foreach (var verifyc in referenceCareComponent)
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var verifyCareComponent = from frameworkc in ctxverify.Care_component
                                          where frameworkc.Code == verifyc.Code &&
                                             frameworkc.Language_Name == verifyLanguageName &&
                                             frameworkc.Version == verifyVersion
                                          select frameworkc;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var verifyCareComponent = ctxverify.Care_component.Where(
                                          "it.Code = '"+verifyc.Code + 
                                          "' AND it.Language_Name = '"+verifyLanguageName +
                                          "' AND it.Version = '"+verifyVersion+"'");

#endif
                if (verifyCareComponent.Count() == 1)
                    verifyc.Verified = true;
                else
                    verifyc.Verified = false;
            }

            return referenceCareComponent.ToList();
        }

        public List<eNursePHR.BusinessLayer.CCC_Terminology.OutcomeType> deepVerifyOutcomeType(string verifyVersion, string verifyLanguageName)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            var referenceOutcomeType = (from c in ctx.OutcomeType
                                        where c.Version == verifyVersion
                                        select c);

#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            var referenceOutcomeType = ctx.OutcomeType.Where("it.Version = '"+verifyVersion+"'");

#endif
            foreach (var verifyo in referenceOutcomeType)
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)

                var verifyOutcomeType = from frameworko in ctxverify.OutcomeType
                                        where frameworko.Code == verifyo.Code &&
                                           frameworko.Language_Name == verifyLanguageName &&
                                           frameworko.Version == verifyVersion
                                        select frameworko;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var verifyOutcomeType = ctxverify.OutcomeType.Where(
                     "it.Code = "+verifyo.Code + 
                                          "AND it.Language_Name = '"+verifyLanguageName +
                                          "' AND it.Version = '"+verifyVersion+"'");
                                        
#endif

                if (verifyOutcomeType.Count() == 1)
                    verifyo.Verified = true;
                else
                    verifyo.Verified = false;
            }

            return referenceOutcomeType.ToList();
        }

        public List<eNursePHR.BusinessLayer.CCC_Terminology.ActionType> deepVerifyActionType(string verifyVersion, string verifyLanguageName)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)

            var referenceActionType = (from c in ctx.ActionType
                                       where c.Version == verifyVersion
                                       select c);

#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                 var referenceActionType = ctx.ActionType.Where(
                     "it.Version = '"+verifyVersion+"'");
#endif
            foreach (var verifya in referenceActionType)
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)

                
                var verifyActionType = from frameworka in ctxverify.ActionType
                                       where frameworka.Code == verifya.Code &&
                                          frameworka.Language_Name == verifyLanguageName &&
                                          frameworka.Version == verifyVersion
                                       select frameworka;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var verifyActionType = ctxverify.ActionType.Where(
                    "it.Code = "+verifya.Code + 
                                          " AND it.Language_Name = '"+verifyLanguageName +
                                          "' AND it.Version = '"+verifyVersion+"'");
                    
#endif

                if (verifyActionType.Count() == 1)
                    verifya.Verified = true;
                else
                    verifya.Verified = false;
            }

            return referenceActionType.ToList();
        }

        /// <summary>
        /// This method will query the database / CCC framework for a specific diagnosis
        /// Pseudo :
        ///    1. Query the reference framework for all diagnosis (componentcode,major,minor) - set RF
        ///    2. For each diagnosis in RF query CCC framework for the diagnosis
        /// </summary>
        /// <param name="verifyVersion"></param>
        /// <param name="verifyLanguageName"></param>
        /// <returns></returns>
        /// 
        public List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis> deepVerifyNursingDiagnosis(string verifyVersion, string verifyLanguageName)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis> 
                referenceNursingDiagnosis =
                (from diag in ctx.Nursing_Diagnosis
                 where diag.Version == verifyVersion
                 select diag).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis>
    referenceNursingDiagnosis = ctx.Nursing_Diagnosis.Where(
                "it.Version = '"+verifyVersion+"'").ToList();
             
#endif

            foreach (eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis verifyDiag in referenceNursingDiagnosis)
            {

                List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis> verifyNursingDiagnosis = new List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis>();

                if (verifyDiag.MinorCode != null)
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)

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
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    verifyNursingDiagnosis = ctxverify.Nursing_Diagnosis.Where(
                        "it.ComponentCode = '"+ verifyDiag.ComponentCode
                        + "' AND it.MajorCode = "+ verifyDiag.MajorCode +
                          " AND it.MinorCode = "+ verifyDiag.MinorCode+
                          " AND it.Language_Name = '"+ verifyLanguageName +
                          "' AND it.Version = '"+ verifyVersion + "'").ToList();

#endif
                    else
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
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
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    verifyNursingDiagnosis = ctxverify.Nursing_Diagnosis.Where(
                                           "it.ComponentCode = '" + verifyDiag.ComponentCode
                                           + "' AND it.MajorCode = " + verifyDiag.MajorCode +
                                             " AND it.MinorCode IS NULL " +
                                             " AND it.Language_Name = '" + verifyLanguageName +
                                             "' AND it.Version = '" + verifyVersion + "'").ToList();
#endif
                int diagFound = verifyNursingDiagnosis.Count();



                verifyDiag.Verified = (diagFound == 1) ? true : false;

            }

            return referenceNursingDiagnosis;
        }

        public List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> deepVerifyNursingIntervention(string verifyVersion, string verifyLanguageName)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> referenceNursingIntervention =
                (from interv in ctx.Nursing_Intervention
                 where interv.Version == verifyVersion
                 select interv).ToList();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> referenceNursingIntervention =
                ctx.Nursing_Intervention.Where(
                "it.Version = '"+verifyVersion+"'").ToList();

#endif

            foreach (eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention verifyInterv in referenceNursingIntervention)
            {

                List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention> verifyNursingIntervention = new List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention>();

                if (verifyInterv.MinorCode != null)
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
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
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    verifyNursingIntervention = ctxverify.Nursing_Intervention.Where(
                        "it.ComponentCode = '"+ verifyInterv.ComponentCode
                        + "' AND it.MajorCode = "+ verifyInterv.MajorCode +
                          " AND it.MinorCode = "+ verifyInterv.MinorCode+
                          " AND it.Language_Name = '"+ verifyLanguageName +
                          "' AND it.Version = '"+ verifyVersion + "'").ToList();

#endif
                    else
                    
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
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
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                    verifyNursingIntervention = ctxverify.Nursing_Intervention.Where(
                        "it.ComponentCode = '" + verifyInterv.ComponentCode
                        + "' AND it.MajorCode = " + verifyInterv.MajorCode +
                          " AND it.MinorCode IS NULL "  +
                          " AND it.Language_Name = '" + verifyLanguageName +
                          "' AND it.Version = '" + verifyVersion + "'").ToList();

#endif
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