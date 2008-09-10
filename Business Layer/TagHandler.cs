#define SQL_SERVER_COMPACT_SP1_WORKAROUND

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eNursePHR.BusinessLayer.CCC_Translations;
using eNursePHR.BusinessLayer.CCC_Terminology;
using eNursePHR.BusinessLayer.PHR;

namespace eNursePHR.BusinessLayer
{
    public class TagHandler
    {
        CCC_Terminology_ReferenceEntities ctxRefTerminology = new CCC_Terminology_ReferenceEntities();

        #region CareComponent
        Care_component getFrameworkCareComponent(CCC_FrameworkEntities DB, string componentCode, string languageName, string version)
        {
            Care_component fComponent;
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            fComponent = (Care_component)DB.Care_component.Where(c =>
                                                c.Code == componentCode &&
                                                c.Version == version &&
                                                c.Language_Name == languageName).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            fComponent = (Care_component)DB.Care_component.Where(
                "it.Code = '" + componentCode + "'" +
                "AND it.Version = '" + version + "'" +
                "AND it.Language_Name = '" + languageName + "'").First();
#endif
            return fComponent;

        }

        Care_component getFrameworkCareComponent(CCC_FrameworkEntities DB,Tag tag, string languageName, string version)
        {
            Care_Component rCareComponent = getReferenceCareComponent(tag, version, ctxRefTerminology);
            return getFrameworkCareComponent(DB,rCareComponent.Code, languageName, version);
        }
        #endregion CareComponent

        #region Diagnosis
        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis getFrameworkDiagnosis(CCC_FrameworkEntities DB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis rDiag = getReferenceDiagnosis(tag, version, ctxRefTerminology);
            return getFrameworkDiagnosis(DB,rDiag.ComponentCode, rDiag.MajorCode, rDiag.MinorCode, languageName, version);
        }

        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis getFrameworkDiagnosis(CCC_FrameworkEntities DB, string componentCode, short majorCode, short? minorCode, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis fDiag;

            if (minorCode == null)
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qDiag = from fd in DB.Nursing_Diagnosis
                            where fd.ComponentCode == componentCode &&
                            fd.MajorCode == majorCode &&
                            fd.MinorCode == null &&
                            fd.Version == version &&
                            fd.Language_Name == languageName
                            select fd;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qDiag = DB.Nursing_Diagnosis.Where(
                    "it.ComponentCode = '" + componentCode + "' AND it.MajorCode =" + majorCode +
                    "AND it.MinorCode IS NULL" + " AND it.Version = '" + version + "'" +
                    " AND it.Language_Name = '" + languageName + "'");
#endif
                if (qDiag.Count() == 0)
                    fDiag = null;
                else
                 fDiag = qDiag.First();

            }
            else
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qDiag = from fd in DB.Nursing_Diagnosis
                            where fd.ComponentCode == componentCode &&
                            fd.MajorCode == majorCode &&
                            fd.MinorCode == minorCode &&
                            fd.Version == version &&
                            fd.Language_Name == languageName
                            select fd;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qDiag = DB.Nursing_Diagnosis.Where(
                   "it.ComponentCode = '" + componentCode + "' AND it.MajorCode =" + majorCode +
                   "AND it.MinorCode =" +minorCode + " AND it.Version = '" + version + "'" +
                   " AND it.Language_Name = '" + languageName + "'");
#endif
                
                if (qDiag.Count() == 0)
                    fDiag = null;
                else
                    fDiag = qDiag.First();
            }

            return fDiag;

        }

        #endregion

        #region Intervention

        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention getFrameworkIntervention(CCC_FrameworkEntities DB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention rInterv = getReferenceIntervention(tag, version, ctxRefTerminology);
            return getFrameworkIntervention(DB,rInterv.ComponentCode, rInterv.MajorCode, rInterv.MinorCode, languageName, version);

        }


        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention getFrameworkIntervention(CCC_FrameworkEntities DB, string componentCode, short majorCode, short? minorCode, string languageName, string version)
        {

            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention fInterv;

            if (minorCode == null)
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                  var qInterv = from fi in DB.Nursing_Intervention
                              where fi.ComponentCode == componentCode &&
                              fi.MajorCode == majorCode &&
                              fi.MinorCode == null &&
                              fi.Version == version &&
                              fi.Language_Name == languageName
                              select fi;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qInterv = DB.Nursing_Intervention.Where(
                   "it.ComponentCode = '" + componentCode + "' AND it.MajorCode =" + majorCode +
                   "AND it.MinorCode IS NULL" + " AND it.Version = '" + version + "'" +
                   " AND it.Language_Name = '" + languageName + "'");
#endif

                if (qInterv.Count() == 0)
                    fInterv = null;
                else
                    fInterv = qInterv.First();
            }
            else
            {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qInterv = from fi in DB.Nursing_Intervention
                              where fi.ComponentCode == componentCode &&
                              fi.MajorCode == majorCode &&
                              fi.MinorCode == minorCode &&
                              fi.Version == version &&
                              fi.Language_Name == languageName
                              select fi;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                var qInterv = DB.Nursing_Intervention.Where(
                   "it.ComponentCode = '" + componentCode + "' AND it.MajorCode =" + majorCode +
                   "AND it.MinorCode =" +minorCode + " AND it.Version = '" + version + "'" +
                   " AND it.Language_Name = '" + languageName + "'");
#endif

                if (qInterv.Count() == 0)
                    fInterv = null;
                else
                    fInterv = qInterv.First();
            }

            return fInterv;

        }

        #endregion
        //Finds tag Concept+CareComponent for CCC framework
        public void updateTag(CCC_FrameworkEntities DB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis fDiag;
            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention fInterv;
            Care_component fComponent;
           

            switch (tag.TaxonomyType)
            {
                case "CCC/NursingDiagnosis": fDiag = getFrameworkDiagnosis(DB,tag, languageName, version);
                    if (fDiag == null)
                    {
                        tag.Concept = "Not found, check language integrity";
                        tag.Definition = String.Empty;
                        tag.CareComponentConcept = "Unknown";
                    }
                    else
                    {

                        tag.Concept = fDiag.Concept;
                        tag.Definition = fDiag.Definition;
                        if (!fDiag.Care_componentReference.IsLoaded)
                            fDiag.Care_componentReference.Load();
                        tag.CareComponentConcept = fDiag.Care_component.Component;
                    }
                        break;


                case "CCC/NursingIntervention": fInterv = getFrameworkIntervention(DB,tag, languageName, version);
                        if (fInterv == null)
                        {
                           
                            tag.Concept = "Not found, check language integrity";
                            tag.Definition = String.Empty;
                            tag.CareComponentConcept = "Unknown";
                        }
                        else
                        {

                            tag.Concept = fInterv.Concept;
                            tag.Definition = fInterv.Definition;
                            if (!fInterv.Care_componentReference.IsLoaded)
                                fInterv.Care_componentReference.Load();
                            tag.CareComponentConcept = fInterv.Care_component.Component;
                        }
                    break;

                case "CCC/CareComponent": fComponent = getFrameworkCareComponent(DB,tag, languageName, version);
                    tag.Concept = fComponent.Component;
                    tag.Definition = fComponent.Definition;
                    tag.CareComponentConcept = tag.Concept;

                    break;

            }

        }



        eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis getReferenceDiagnosis(Tag tag, string version, CCC_Terminology_ReferenceEntities refDB)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
              return refDB.Nursing_Diagnosis.Where(d =>
                        d.TagGuid == tag.TaxonomyTagId &&
                        d.Version == version).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return refDB.Nursing_Diagnosis.Where("it.TagGuid = GUID '" + tag.TaxonomyTagId + "' AND it.Version = '" + version + "'").First();
#endif               
        
        }

        Care_Component getReferenceCareComponent(Tag tag, string version, CCC_Terminology_ReferenceEntities refDB)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             return ctxRefTerminology.Care_Component.Where(c => c.TagGuid == tag.TaxonomyTagId
                       && c.Version == version).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return ctxRefTerminology.Care_Component.Where("it.TagGuid = GUID '"+ tag.TaxonomyTagId + "' AND it.Version = '"+version+"'").First();
#endif
        }

        eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention getReferenceIntervention(Tag tag, string version, CCC_Terminology_ReferenceEntities refDB)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return refDB.Nursing_Intervention.Where(i => i.TagGuid == tag.TaxonomyTagId
               && i.Version == version).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return refDB.Nursing_Intervention.Where("it.TagGuid = GUID '"+ tag.TaxonomyTagId +"'AND it.Version = '"+version+"'").First();
#endif
        }

       
       

        
        public Guid getTaxonomyGuidCareComponent(string componentCode, string version)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             Guid taxonomyGuid = ctxRefTerminology.Care_Component.Where(c => c.Code == componentCode && c.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)           
            Guid taxonomyGuid = ctxRefTerminology.Care_Component.Where("it.Code = '" + componentCode + "'AND it.Version = '" + version + "'").First().TagGuid;
#endif
            return taxonomyGuid;       
         }

        public Guid getTaxonomyGuidNursingDiagnosis(string componentCode, decimal majorCode, short? minorCode, string version)
        {
            Guid taxonomyGuid;
      
            if (minorCode != null)
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                 taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(d => d.ComponentCode == componentCode
                 && d.MajorCode == majorCode && d.MinorCode == minorCode && d.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(
                "it.ComponentCode = '"+componentCode+"' AND it.MajorCode ="+

                 majorCode + "AND it.MinorCode ="+minorCode+"AND it.Version = '"+ version+"'").First().TagGuid;
#endif
            else
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(d => d.ComponentCode == componentCode
                    && d.MajorCode == majorCode && d.MinorCode == null && d.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(
                        "it.ComponentCode = '" + componentCode + "' AND it.MajorCode =" +

                 majorCode + "AND it.MinorCode IS NULL AND it.Version = '"+ version+"'").First().TagGuid;
#endif
            return taxonomyGuid;
        }

        public Guid getTaxonomyGuidOutcomeType(short code,string version)
        {
            Guid taxonomyAttachmentGuid;
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             taxonomyAttachmentGuid = ctxRefTerminology.OutcomeType.Where(ot => 
                        ot.Code == code && 
                        ot.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            taxonomyAttachmentGuid = ctxRefTerminology.OutcomeType.Where(
                "it.Code = "+code +" AND it.Version = '"+version+"'").First().TagGuid;
#endif
            return taxonomyAttachmentGuid;
        }

        public Guid getTaxonomyGuidActionType(short code, string version)
        {
            Guid taxonomyAttachmentGuid;
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             taxonomyAttachmentGuid = ctxRefTerminology.ActionType.Where(at => at.Code == code && at.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)           
            taxonomyAttachmentGuid = ctxRefTerminology.ActionType.Where("it.Code = "+code+"it.Version = '"+ version+"'").First().TagGuid;
#endif
            return taxonomyAttachmentGuid;
        }

        public Guid getTaxonomyGuidNursingIntervention(string componentCode, decimal majorCode, short? minorCode, string version)
        {
            Guid taxonomyGuid;
            if (minorCode != null)
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
               taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(d => d.ComponentCode == componentCode
                      && d.MajorCode == majorCode && d.MinorCode == minorCode && d.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(
                    "it.ComponentCode = '" + componentCode + "'" +
                    "AND it.MajorCode = " + majorCode +
                    " AND it.MinorCode = " + minorCode +
                    " AND it.Version = '" + version + "'").First().TagGuid;
#endif
            else
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
                 taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(d => d.ComponentCode == componentCode
                    && d.MajorCode == majorCode && d.MinorCode == null && d.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
                taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(
                    "it.ComponentCode = '" + componentCode + "'" +
                    "AND it.MajorCode = " + majorCode +
                    " AND it.MinorCode IS NULL"+
                    " AND it.Version = '" + version + "'").First().TagGuid;
#endif
                return taxonomyGuid;
        }
    }
}
