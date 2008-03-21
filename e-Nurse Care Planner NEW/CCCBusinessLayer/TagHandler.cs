using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReferenceFrameworkModel;

namespace CCC.BusinessLayer
{
    public class TagHandler
    {
        ReferenceFrameworkEntities ctxRefTerminology = new ReferenceFrameworkEntities();

        #region CareComponent
        Care_component getFrameworkCareComponent(CCCFrameworkCompactEntities DB, string componentCode, string languageName, string version)
        {
            Care_component fComponent;
            fComponent = (Care_component)DB.Care_component.Where(c =>
                                                c.Code == componentCode &&
                                                c.Version == version &&
                                                c.Language_Name == languageName).First();
            return fComponent;

        }

        Care_component getFrameworkCareComponent(CCCFrameworkCompactEntities DB,Tag tag, string languageName, string version)
        {
            Care_Component rCareComponent = getReferenceCareComponent(tag, version, ctxRefTerminology);
            return getFrameworkCareComponent(DB,rCareComponent.Code, languageName, version);
        }
        #endregion CareComponent

        #region Diagnosis
        FrameworkDiagnosis getFrameworkDiagnosis(CCCFrameworkCompactEntities DB, Tag tag, string languageName, string version)
        {
            Nursing_Diagnosis rDiag = getReferenceDiagnosis(tag, version, ctxRefTerminology);
            return getFrameworkDiagnosis(DB,rDiag.ComponentCode, rDiag.MajorCode, rDiag.MinorCode, languageName, version);
        }

        FrameworkDiagnosis getFrameworkDiagnosis(CCCFrameworkCompactEntities DB, string componentCode, short majorCode, short? minorCode, string languageName, string version)
        {
            FrameworkDiagnosis fDiag;

            if (minorCode == null)

                fDiag = (FrameworkDiagnosis)(from fd in DB.Nursing_Diagnosis
                                             where fd.ComponentCode == componentCode &&
                                             fd.MajorCode == majorCode &&
                                             fd.MinorCode == null &&
                                             fd.Version == version &&
                                             fd.Language_Name == languageName
                                             select fd).First();


            else

                fDiag = (FrameworkDiagnosis)(from fd in DB.Nursing_Diagnosis
                                             where fd.ComponentCode == componentCode &&
                                             fd.MajorCode == majorCode &&
                                             fd.MinorCode == minorCode &&
                                             fd.Version == version &&
                                             fd.Language_Name == languageName
                                             select fd).First();

            return fDiag;

        }

        #endregion

        #region Intervention

        FrameworkIntervention getFrameworkIntervention(CCCFrameworkCompactEntities DB, Tag tag, string languageName, string version)
        {
            Nursing_Intervention rInterv = getReferenceIntervention(tag, version, ctxRefTerminology);
            return getFrameworkIntervention(DB,rInterv.ComponentCode, rInterv.MajorCode, rInterv.MinorCode, languageName, version);

        }


        FrameworkIntervention getFrameworkIntervention(CCCFrameworkCompactEntities DB, string componentCode, short majorCode, short? minorCode, string languageName, string version)
        {

            FrameworkIntervention fInterv;

            if (minorCode == null)
                fInterv = (FrameworkIntervention)(from fi in DB.Nursing_Intervention
                                                  where fi.ComponentCode == componentCode &&
                                                  fi.MajorCode == majorCode &&
                                                  fi.MinorCode == null &&
                                                  fi.Version == version &&
                                                  fi.Language_Name == languageName
                                                  select fi).First();
            else
                fInterv = (FrameworkIntervention)(from fi in DB.Nursing_Intervention
                                                  where fi.ComponentCode == componentCode &&
                                                  fi.MajorCode == majorCode &&
                                                  fi.MinorCode == minorCode &&
                                                  fi.Version == version &&
                                                  fi.Language_Name == languageName
                                                  select fi).First();
            return fInterv;

        }

        #endregion
        //Finds tag Concept+CareComponent for CCC framework
        public void updateTag(CCCFrameworkCompactEntities DB, Tag tag, string languageName, string version)
        {
            FrameworkDiagnosis fDiag;
            FrameworkIntervention fInterv;
            Care_component fComponent;
           

            switch (tag.TaxonomyType)
            {
                case "CCC/NursingDiagnosis": fDiag = getFrameworkDiagnosis(DB,tag, languageName, version);
                    tag.Concept = fDiag.Concept;
                    tag.Definition = fDiag.Definition;
                    if (!fDiag.Care_componentReference.IsLoaded)
                        fDiag.Care_componentReference.Load();
                    tag.CareComponentConcept = fDiag.Care_component.Component;
                    break;


                case "CCC/NursingIntervention": fInterv = getFrameworkIntervention(DB,tag, languageName, version);
                    tag.Concept = fInterv.Concept;
                    tag.Definition = fInterv.Definition;
                    if (!fInterv.Care_componentReference.IsLoaded)
                        fInterv.Care_componentReference.Load();
                    tag.CareComponentConcept = fInterv.Care_component.Component;

                    break;

                case "CCC/CareComponent": fComponent = getFrameworkCareComponent(DB,tag, languageName, version);
                    tag.Concept = fComponent.Component;
                    tag.Definition = fComponent.Definition;
                    tag.CareComponentConcept = tag.Concept;

                    break;

            }

        }



        Nursing_Diagnosis getReferenceDiagnosis(Tag tag, string version, ReferenceFrameworkEntities refDB)
        {
            return refDB.Nursing_Diagnosis.Where(d =>
                        d.TagGuid == tag.TaxonomyTagId &&
                        d.Version == version).First();
        }

        Care_Component getReferenceCareComponent(Tag tag, string version, ReferenceFrameworkEntities refDB)
        {
            return ctxRefTerminology.Care_Component.Where(c => c.TagGuid == tag.TaxonomyTagId
                        && c.Version == version).First();
        }

        Nursing_Intervention getReferenceIntervention(Tag tag, string version, ReferenceFrameworkEntities refDB)
        {
            return refDB.Nursing_Intervention.Where(i => i.TagGuid == tag.TaxonomyTagId
                         && i.Version == version).First();

        }

       
       

        
        public Guid getTaxonomyGuidCareComponent(string componentCode, string version)
        {
            Guid taxonomyGuid = ctxRefTerminology.Care_Component.Where(c => c.Code == componentCode && c.Version == version).First().TagGuid;
            return taxonomyGuid;       
         }

        public Guid getTaxonomyGuidNursingDiagnosis(string componentCode, decimal majorCode, short? minorCode, string version)
        {
            Guid taxonomyGuid;
      
            if (minorCode != null)
                taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(d => d.ComponentCode == componentCode
                  && d.MajorCode == majorCode && d.MinorCode == minorCode && d.Version == version).First().TagGuid;
            else
                taxonomyGuid = ctxRefTerminology.Nursing_Diagnosis.Where(d => d.ComponentCode == componentCode
                    && d.MajorCode == majorCode && d.MinorCode == null && d.Version == version).First().TagGuid;
            
            return taxonomyGuid;
        }

        public Guid getTaxonomyGuidOutcomeType(short code,string version)
        {
            Guid taxonomyAttachmentGuid;
            taxonomyAttachmentGuid = ctxRefTerminology.OutcomeType.Where(ot => 
                        ot.Code == code && 
                        ot.Version == version).First().TagGuid;
            return taxonomyAttachmentGuid;
        }

        public Guid getTaxonomyGuidActionType(short code, string version)
        {
            Guid taxonomyAttachmentGuid;

            taxonomyAttachmentGuid = ctxRefTerminology.ActionType.Where(at => at.Code == code && at.Version == version).First().TagGuid;
            return taxonomyAttachmentGuid;
        }

        public Guid getTaxonomyGuidNursingIntervention(string componentCode, decimal majorCode, short? minorCode, string version)
        {
            Guid taxonomyGuid;
            if (minorCode != null)
                taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(d => d.ComponentCode == componentCode
                  && d.MajorCode == majorCode && d.MinorCode == minorCode && d.Version == version).First().TagGuid;
            else
                taxonomyGuid = ctxRefTerminology.Nursing_Intervention.Where(d => d.ComponentCode == componentCode
                    && d.MajorCode == majorCode && d.MinorCode == null && d.Version == version).First().TagGuid;
            return taxonomyGuid;
        }
    }
}
