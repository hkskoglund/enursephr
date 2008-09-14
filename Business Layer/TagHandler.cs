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
    /// <summary>
    /// The TagLangageConverter-class handles tag language translation of the tags carecomponent, diagnosis and intervention
    /// </summary>
    public class TagLangageConverter
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


        Care_Component getReferenceCareComponent(Tag tag, string version, CCC_Terminology_ReferenceEntities refDB)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             return ctxRefTerminology.Care_Component.Where(c => c.TagGuid == tag.TaxonomyTagId
                       && c.Version == version).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return ctxRefTerminology.Care_Component.Where("it.TagGuid = GUID '" + tag.TaxonomyTagId + "' AND it.Version = '" + version + "'").First();
#endif
        }

        #endregion CareComponent

        #region Diagnosis
        /// <summary>
        /// This method gets the ComponentCode,MajorCode and MinorCode from the reference terminology based on a
        /// guid foreign reference key in Tag. It will then ask for the language translation.
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="tag"></param>
        /// <param name="languageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis getFrameworkDiagnosis(CCC_FrameworkEntities DB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis rDiag = getReferenceDiagnosis(tag, version, ctxRefTerminology);
            return getFrameworkDiagnosis(DB,rDiag.ComponentCode, rDiag.MajorCode, rDiag.MinorCode, languageName, version);
        }

        /// <summary>
        /// Gives the language translation for a specific ComponentCode,MajorCode,MinorCode
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="componentCode"></param>
        /// <param name="majorCode"></param>
        /// <param name="minorCode"></param>
        /// <param name="languageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the reference diagnosis for a given taxonomyTagId (foreign key)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="version"></param>
        /// <param name="refDB"></param>
        /// <returns></returns>
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

        #endregion

        #region Intervention

        /// <summary>
        /// Finds the componentcode,majorcode and minorcode for a foreign reference key/guid in tag
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="tag"></param>
        /// <param name="languageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention getFrameworkIntervention(CCC_FrameworkEntities DB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention rInterv = getReferenceIntervention(tag, version, ctxRefTerminology);
            return getFrameworkIntervention(DB,rInterv.ComponentCode, rInterv.MajorCode, rInterv.MinorCode, languageName, version);

        }

        eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention getReferenceIntervention(Tag tag, string version, CCC_Terminology_ReferenceEntities refDB)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return refDB.Nursing_Intervention.Where(i => i.TagGuid == tag.TaxonomyTagId
               && i.Version == version).First();
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            return refDB.Nursing_Intervention.Where("it.TagGuid = GUID '" + tag.TaxonomyTagId + "'AND it.Version = '" + version + "'").First();
#endif
        }


        /// <summary>
        /// Gives the langauge translation for an intervention
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="componentCode"></param>
        /// <param name="majorCode"></param>
        /// <param name="minorCode"></param>
        /// <param name="languageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Finds the concept,definition and carecomponent of a given tag that contains foreign key/TaxonomyId that
        /// is a reference to the CCC reference terminology where the componentcode,majorcode, and minorcode is found
        /// Then the language translation is fetched.
        /// </summary>
        /// <param name="cccFrameworkDB"></param>
        /// <param name="tag"></param>
        /// <param name="languageName"></param>
        /// <param name="version"></param>

        public void translateTag(CCC_FrameworkEntities cccFrameworkDB, Tag tag, string languageName, string version)
        {
            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis fDiag;
            eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention fInterv;
            Care_component fComponent;
           
           // Gets language translation of diagnosis, intervention and carecomponent
            switch (tag.TaxonomyType)
            {
                case "CCC/NursingDiagnosis": fDiag = getFrameworkDiagnosis(cccFrameworkDB,tag, languageName, version);
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


                case "CCC/NursingIntervention": fInterv = getFrameworkIntervention(cccFrameworkDB,tag, languageName, version);
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

                case "CCC/CareComponent": fComponent = getFrameworkCareComponent(cccFrameworkDB,tag, languageName, version);
                    tag.Concept = fComponent.Component;
                    tag.Definition = fComponent.Definition;
                    tag.CareComponentConcept = tag.Concept;

                    break;

            }

        }


        #region Get guid in CCC reference terminology for carecomponent, diagnosis, intervention and outcometype
        /// <summary>
        /// Gets a guid in the CCC reference terminology for a given componentCode
        /// </summary>
        /// <param name="componentCode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public Guid getTaxonomyGuidCareComponent(string componentCode, string version)
        {
#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
             Guid taxonomyGuid = ctxRefTerminology.Care_Component.Where(c => c.Code == componentCode && c.Version == version).First().TagGuid;
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)           
            Guid taxonomyGuid = ctxRefTerminology.Care_Component.Where("it.Code = '" + componentCode + "'AND it.Version = '" + version + "'").First().TagGuid;
#endif
            return taxonomyGuid;       
         }

        /// <summary>
        /// Gets a guid in the CCC reference terminologi for a given diagnosis
        /// </summary>
        /// <param name="componentCode"></param>
        /// <param name="majorCode"></param>
        /// <param name="minorCode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a guid in the reference terminology for a given outcometype
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a guid in the CCC reference terminology for a given intervention
        /// </summary>
        /// <param name="componentCode"></param>
        /// <param name="majorCode"></param>
        /// <param name="minorCode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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
        #endregion
    }
}
