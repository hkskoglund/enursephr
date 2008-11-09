using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CCCBrowser
{
    public class CCCXMLAPI
    {
        private XDocument xCCC = null; // XML document containing CCC 

        public CCCXMLAPI()
        {
            xCCC = XDocument.Load("Taxonomy/CCCTaxonomy.xml",LoadOptions.None);
        }

        public string getComponentNameA(string componentCode, string language, string version)
        {
            return  (from diag in xCCC.Descendants("CareComponent")
                                           where diag.Attribute("ComponentCode").Value == componentCode &&
                                           diag.Attribute("Language").Value == language &&
                                           diag.Attribute("Version").Value == version
                                           select diag.Attribute("Concept").Value).First();
     
        }

        public string getComponentNameV2(string componentCode, string language, string version)
        {
            return (from diag in xCCC.Descendants("Component")
                    where diag.Element("code").Value == componentCode &&
                    diag.Element("language").Value == language &&
                    diag.Element("version").Value == version
                    select diag.Element("concept").Value).First();

        }


       // public Diagnosis getDiagnosisV1(string componentCode, string majorCode, string minorCode, string language, string version)
       // {
       //     IEnumerable<Diagnosis> diags = from diag in xCCC.Descendants("Diagnosis")
       //                                                where diag.Attribute("ComponentCode").Value == componentCode &&
       //                                                diag.Attribute("MajorCode").Value == majorCode &&
       //                                                diag.Attribute("MinorCode").Value == minorCode  &&
       //                                                diag.Attribute("Language").Value == language &&
       //                                                diag.Attribute("Version").Value == version
       //                                                select new Diagnosis
       //                                                {
       //                                                    Concept = diag.Attribute("Concept").Value,
       //                                                    Definition = diag.Attribute("Definition").Value,
       //                                                    ComponentName = getComponentNameV2(componentCode,language,version),
       //                                                     ComponentCode = componentCode,
       //                                                     Language = language,
       //                                                     Version = version,
       //                                                      MajorCode = diag.Attribute("MajorCode").Value,
       //                                                      MinorCode = diag.Attribute("MinorCode").Value

       //                                                };
       //     if (diags == null || diags.Count() == 0)
       //         return null;
       //     else 
       //         return diags.First();
       //}

        public Diagnosis getDiagnosisV2(string componentCode, int majorCode, int minorCode, string language, string version)
        {
            IEnumerable<Diagnosis> diags = from diag in xCCC.Descendants("Diagnosis")
                                           where diag.Element("code").Value == componentCode &&
                                           Convert.ToInt32(diag.Element("majorcode").Value) == majorCode &&
                                           Convert.ToInt32(diag.Element("minorcode").Value) == minorCode &&
                                           diag.Element("language").Value == language &&
                                           diag.Element("version").Value == version
                                           select new Diagnosis
                                           {
                                               Concept = diag.Element("concept").Value,
                                               Definition = diag.Element("definition").Value,
                                               ComponentName = getComponentNameV2(componentCode, language, version),
                                               ComponentCode = componentCode,
                                               Language = language,
                                               Version = version,
                                               MajorCode = Convert.ToInt32(diag.Element("majorcode").Value),
                                               MinorCode = Convert.ToInt32(diag.Element("minorcode").Value)

                                           };
            if (diags == null || diags.Count() == 0)
                return null;
            else
                return diags.First();
        }

        //public Intervention getInterventionV1(string componentCode, int majorCode, int minorCode, string language, string version)
        //{
        //    IEnumerable<Intervention> intervs = from interv in xCCC.Descendants("Intervention")
        //                                   where interv.Attribute("ComponentCode").Value == componentCode &&
        //                                   interv.Attribute("MajorCode").Value == majorCode &&
        //                                   interv.Attribute("MinorCode").Value == minorCode &&
        //                                   interv.Attribute("Language").Value == language &&
        //                                   interv.Attribute("Version").Value == version
        //                                   select new Intervention
        //                                   {
        //                                       Concept = interv.Attribute("Concept").Value,
        //                                       Definition = interv.Attribute("Definition").Value,
        //                                       ComponentName = getComponentNameV2(componentCode, language, version),
        //                                       ComponentCode = componentCode,
        //                                       Language = language,
        //                                       Version = version,
        //                                       MajorCode = majorCode,
        //                                       MinorCode = minorCode
        //                                   };
        //    if (intervs == null || intervs.Count() == 0)
        //        return null;
        //    else
        //        return intervs.First();
        //}


        public Intervention getInterventionV2(string componentCode, int majorCode, int minorCode, string language, string version)
        {
            IEnumerable<Intervention> intervs = from interv in xCCC.Descendants("Intervention")
                                                where interv.Element("code").Value == componentCode &&
                                                Convert.ToInt32(interv.Element("majorcode").Value) == majorCode &&
                                                Convert.ToInt32(interv.Element("minorcode").Value) == minorCode &&
                                                interv.Element("language").Value == language &&
                                                interv.Element("version").Value == version
                                                select new Intervention
                                                {
                                                    Concept = interv.Element("concept").Value,
                                                    Definition = interv.Element("definition").Value,
                                                    ComponentName = getComponentNameV2(componentCode, language, version),
                                                    ComponentCode = componentCode,
                                                    Language = language,
                                                    Version = version,
                                                    MajorCode = majorCode,
                                                    MinorCode = minorCode
                                                };
            if (intervs == null || intervs.Count() == 0)
                return null;
            else
                return intervs.First();
        }

        public MetaInformation getMetaInformationV1(string language, string version)
        {
            
            IEnumerable<MetaInformation> allMeta =  from   mi in xCCC.Descendants("MetaInformation")
                     where
                        mi.Attribute("Version").Value == version &&
                        mi.Attribute("Language").Value == language

                     select
                        new MetaInformation 
                              {
                                 Authors = mi.Attribute("Authors").Value,
                                LogoURL = mi.Attribute("LogoURL").Value,
                                LastUpdateString = mi.Attribute("LastUpdate").Value,
                                Version = version,
                                Language = language,
                                Name = mi.Attribute("Name").Value
                            };
            
            if (allMeta.Count() == 1)
                return allMeta.First() as MetaInformation;
            else
                return null;
        }


        public MetaInformation getMetaInformationV2(string language, string version)
        {

            IEnumerable<MetaInformation> allMeta = from mi in xCCC.Descendants("MetaInformation")
                                                   where
                                                      mi.Element("version").Value == version &&
                                                      mi.Element("language").Value == language

                                                   select
                                                      new MetaInformation
                                                      {
                                                          Authors = mi.Element("authors").Value,
                                                          LogoURL = mi.Element("logourl").Value,
                                                          LastUpdateString = mi.Element("lastupdate").Value,
                                                          Version = version,
                                                          Language = language,
                                                          Name = mi.Element("name").Value
                                                      };

            if (allMeta.Count() == 1)
                return allMeta.First() as MetaInformation;
            else
                return null;
        }

        public IEnumerable<CarePattern> getCarePatternsV1(string language, string version)
        {
            IEnumerable<CarePattern> q = from pattern in xCCC.Descendants("CarePattern")
                                       
                                       where
                                       
                                      pattern.Attribute("Language").Value == language &&
                                      pattern.Attribute("Version").Value == version
                                       
                                       select new CarePattern

                                       {
                                           Concept = pattern.Attribute("Concept").Value,
                                           Definition = pattern.Attribute("Definition").Value,
                                           Language = language,
                                           Version = version
                                       };
       
                     
            return q;
        }


        public IEnumerable<CarePattern> getCarePatternsV2(string language, string version)
        {
            IEnumerable<CarePattern> q = from pattern in xCCC.Descendants("Pattern")

                                         where

                                        pattern.Element("language").Value == language &&
                                        pattern.Element("version").Value == version

                                         select new CarePattern

                                         {
                                             Concept = pattern.Element("concept").Value,
                                             Definition = pattern.Element("definition").Value,
                                             Language = language,
                                             Version = version
                                         };


            return q;
        }


        public OutcomeType getOutcomeTypeV1(string code, string language, string version)
        {
            IEnumerable<OutcomeType> q = from ot in xCCC.Descendants("OutcomeType")

                                         where

                                         ot.Attribute("Code").Value == code &&
                                        ot.Attribute("Language").Value == language &&
                                        ot.Attribute("Version").Value == version

                                         select new OutcomeType

                                         {
                                             Concept = ot.Attribute("Concept").Value,
                                             Definition = ot.Attribute("Definition").Value,
                                             Code = code,
                                             Language = language,
                                             Version = version
                                         };

           if (q == null || q.Count() == 0)
                return null;
            else
                return q.First();
       
        }


        public OutcomeType getOutcomeTypeV2(string code, string language, string version)
        {
            IEnumerable<OutcomeType> q = from ot in xCCC.Descendants("OutcomeType")

                                         where

                                         ot.Element("code").Value == code &&
                                        ot.Element("language").Value == language &&
                                        ot.Element("version").Value == version

                                         select new OutcomeType

                                         {
                                             Concept = ot.Element("concept").Value,
                                             Definition = ot.Element("definition").Value,
                                             Code = code,
                                             Language = language,
                                             Version = version
                                         };

            if (q == null || q.Count() == 0)
                return null;
            else
                return q.First();

        }


        public IEnumerable<OutcomeType> getOutcomeTypesV1(string language, string version)
        {
            IEnumerable<OutcomeType> q = from ot in xCCC.Descendants("OutcomeType")

                                         where

                                        ot.Attribute("Language").Value == language &&
                                        ot.Attribute("Version").Value == version

                                         select new OutcomeType

                                         {
                                             Concept = ot.Attribute("Concept").Value,
                                             Definition = ot.Attribute("Definition").Value,
                                             Code = ot.Attribute("Code").Value,
                                             Language = language,
                                             Version = version
                                         };


            return q;
        
        }

        public IEnumerable<OutcomeType> getOutcomeTypesV2(string language, string version)
        {
            IEnumerable<OutcomeType> q = from ot in xCCC.Descendants("OutcomeType")

                                         where

                                        ot.Element("language").Value == language &&
                                        ot.Element("version").Value == version

                                         select new OutcomeType

                                         {
                                             Concept = ot.Element("concept").Value,
                                             Definition = ot.Element("definition").Value,
                                             Code = ot.Element("code").Value,
                                             Language = language,
                                             Version = version
                                         };


            return q;

        }



        public ActionType getActionTypeV1(string code,string language, string version)
        {
            IEnumerable<ActionType> q = from ot in xCCC.Descendants("ActionType")

                                        where

                                       ot.Attribute("Language").Value == language &&
                                       ot.Attribute("Version").Value == version &&
                                       ot.Attribute("Code").Value == code

                                        select new ActionType

                                        {
                                            Concept = ot.Attribute("Concept").Value,
                                            Definition = ot.Attribute("Definition").Value,
                                            Code = ot.Attribute("Code").Value,
                                            Language = language,
                                            Version = version
                                        };


            if (q == null || q.Count() == 0)
                return null;
            else
                return q.First();
       

        }

        public ActionType getActionTypeV2(string code, string language, string version)
        {
            IEnumerable<ActionType> q = from ot in xCCC.Descendants("ActionType")

                                        where

                                       ot.Element("language").Value == language &&
                                       ot.Element("version").Value == version &&
                                       ot.Element("code").Value == code

                                        select new ActionType

                                        {
                                            Concept = ot.Element("concept").Value,
                                            Definition = ot.Element("definition").Value,
                                            Code = ot.Element("code").Value,
                                            Language = language,
                                            Version = version
                                        };


            if (q == null || q.Count() == 0)
                return null;
            else
                return q.First();


        }

        public IEnumerable<ActionType> getActionTypesV1(string language, string version)
        {
            IEnumerable<ActionType> q = from ot in xCCC.Descendants("ActionType")

                                         where

                                        ot.Attribute("Language").Value == language &&
                                        ot.Attribute("Version").Value == version

                                         select new ActionType

                                         {
                                             Concept = ot.Attribute("Concept").Value,
                                             Definition = ot.Attribute("Definition").Value,
                                             Code = ot.Attribute("Code").Value,
                                             Language = language,
                                             Version = version
                                         };


            return q;

        }

        public IEnumerable<ActionType> getActionTypesV2(string language, string version)
        {
            IEnumerable<ActionType> q = from ot in xCCC.Descendants("ActionType")

                                        where

                                       ot.Element("language").Value == language &&
                                       ot.Element("version").Value == version

                                        select new ActionType

                                        {
                                            Concept = ot.Element("concept").Value,
                                            Definition = ot.Element("definition").Value,
                                            Code = ot.Element("code").Value,
                                            Language = language,
                                            Version = version
                                        };


            return q;

        }

        //public IEnumerable<Diagnosis> getDiagnosesV1(string cCode, string languageName, string version)
        //{
        //    IEnumerable<Diagnosis> q = (from diag in xCCC.Descendants("Diagnosis")
                                       
        //                               where
                                       
        //                              diag.Attribute("ComponentCode").Value == cCode &&
        //                               diag.Attribute("Language").Value == languageName &&
        //                               diag.Attribute("Version").Value == version
                                       
        //                               select new Diagnosis

        //                               {
        //                                   ComponentCode = diag.Attribute("ComponentCode").Value,
        //                                   Concept = diag.Attribute("Concept").Value,
        //                                   Definition = diag.Attribute("Definition").Value,
        //                                   MajorCode = diag.Attribute("MajorCode").Value,
        //                                   MinorCode = diag.Attribute("MinorCode").Value,
        //                                   Language = languageName,
        //                                   Version = version,
        //                                   ComponentName = getComponentNameV2(cCode, languageName, version)
 
        //                                  // PatternId = diag.Attribute("PatternId").Value
        //                               }).OrderBy(d => d.MajorCode).ThenBy(d => d.MinorCode);
       
                     
        //    return q;
        //}

        public IEnumerable<Diagnosis> getDiagnosesV2(string cCode, string languageName, string version)
        {
            IEnumerable<Diagnosis> q = (from diag in xCCC.Descendants("Diagnosis")

                                        where

                                       diag.Element("code").Value == cCode &&
                                        diag.Element("language").Value == languageName &&
                                        diag.Element("version").Value == version

                                        select new Diagnosis

                                        {
                                            ComponentCode = diag.Element("code").Value,
                                            Concept = diag.Element("concept").Value,
                                            Definition = diag.Element("definition").Value,
                                            MajorCode = Convert.ToInt32(diag.Element("majorcode").Value),
                                            MinorCode = Convert.ToInt32(diag.Element("minorcode").Value),
                                            Language = languageName,
                                            Version = version,
                                            ComponentName = getComponentNameV2(cCode, languageName, version)

                                            // PatternId = diag.Attribute("PatternId").Value
                                        }).OrderBy(d => d.MajorCode).ThenBy(d => d.MinorCode);


            return q;
        }


        //public IEnumerable<Intervention> getInterventionsV1(string cCode, string languageName, string version)
        //{
        //    IEnumerable<Intervention> q = (from interv in xCCC.Descendants("Intervention")

        //                                where

        //                               interv.Attribute("ComponentCode").Value == cCode &&
        //                                interv.Attribute("Language").Value == languageName &&
        //                                interv.Attribute("Version").Value == version

        //                                select new Intervention

        //                                {
        //                                    ComponentCode = interv.Attribute("ComponentCode").Value,
        //                                    Concept = interv.Attribute("Concept").Value,
        //                                    Definition = interv.Attribute("Definition").Value,
        //                                    MajorCode = interv.Attribute("MajorCode").Value,
        //                                    MinorCode = interv.Attribute("MinorCode").Value,
        //                                    Language = languageName,
        //                                    Version = version,
        //                                    ComponentName = getComponentNameV2(cCode, languageName, version)

        //                                    // PatternId = diag.Attribute("PatternId").Value
        //                                }).OrderBy(d => d.MajorCode).ThenBy(d => d.MinorCode);


        //    return q;
        //}


        public IEnumerable<Intervention> getInterventionsV2(string cCode, string languageName, string version)
        {
            IEnumerable<Intervention> q = (from interv in xCCC.Descendants("Intervention")

                                           where

                                          interv.Element("code").Value == cCode &&
                                           interv.Element("language").Value == languageName &&
                                           interv.Element("version").Value == version

                                           select new Intervention

                                           {
                                               ComponentCode = interv.Element("code").Value,
                                               Concept = interv.Element("concept").Value,
                                               Definition = interv.Element("definition").Value,
                                               MajorCode = Convert.ToInt32(interv.Element("majorcode").Value),
                                               MinorCode = Convert.ToInt32(interv.Element("minorcode").Value),
                                               Language = languageName,
                                               Version = version,
                                               ComponentName = getComponentNameV2(cCode, languageName, version)

                                               // PatternId = diag.Attribute("PatternId").Value
                                           }).OrderBy(d => d.MajorCode).ThenBy(d => d.MinorCode);


            return q;
        }

  
        //public IEnumerable<CareComponent> getCareComponentsV1(int patternId,string languageName, string version)
        //{
        //    IEnumerable<CareComponent> q = (from comp in xCCC.Descendants("CareComponent")

        //                                  where

        //                                  comp.Attribute("PatternId").Value == patternId &&
        //                                  comp.Attribute("Language").Value == languageName &&
        //                                  comp.Attribute("Version").Value == version

        //                                  select new CareComponent
        //                                  {
        //                                      ComponentCode = comp.Attribute("ComponentCode").Value,
        //                                      Concept = comp.Attribute("Concept").Value,
        //                                      Definition = comp.Attribute("Definition").Value,
        //                                      Language = languageName,
        //                                      Version = version,
        //                                      PatternId = patternId

        //                                  }).OrderBy(cc => cc.Concept);


        //    return q;
        //}

        public IEnumerable<CareComponent> getCareComponentsV2(int patternId, string languageName, string version)
        {
            IEnumerable<CareComponent> q = (from comp in xCCC.Descendants("Component")

                                            where

                                            Convert.ToInt32(comp.Element("patternid").Value) == patternId &&
                                            comp.Element("language").Value == languageName &&
                                            comp.Element("version").Value == version

                                            select new CareComponent
                                            {
                                                ComponentCode = comp.Element("code").Value,
                                                Concept = comp.Element("concept").Value,
                                                Definition = comp.Element("definition").Value,
                                                Language = languageName,
                                                Version = version,
                                                PatternId = patternId

                                            }).OrderBy(cc => cc.Concept);


            return q;
        }
    }
}
