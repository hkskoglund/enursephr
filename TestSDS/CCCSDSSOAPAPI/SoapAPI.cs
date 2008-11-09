using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ServiceModel;
using CCCBrowser.SoapAPI.sdsClient;
using CCCBrowser.Support;
using System.ComponentModel;

namespace CCCBrowser.SoapAPI
{
    public class CCCSoapAPI
    {
        private string CCCContainer = "CCCTaxonomy";
        private string authorityId = "hks";

        private Scope CCCScope;

        private SitkaSoapServiceClient proxy;


        private ReadOnlyCollection<Entity> Entities { get; set; }

        public  ReadOnlyCollection<Diagnosis> Diagnoses { get; set; }
        public  ReadOnlyCollection<Intervention> Interventions { get; set; }
        public ReadOnlyCollection<ActionType> ActionTypes { get; set; }
        public ReadOnlyCollection<OutcomeType> OutcomeTypes { get; set; }
        public ReadOnlyCollection<CCCBrowser.Support.Component> Components { get; set; }
        public ReadOnlyCollection<Pattern> Patterns { get; set; }
        public MetaInformation MetaInformation { get; set; }


        public CCCSoapAPI(string username, string password)
        {
                try {

                    proxy = new SitkaSoapServiceClient("BasicAuthEndpoint");
                    proxy.ClientCredentials.UserName.UserName = username;
                    proxy.ClientCredentials.UserName.Password = password;
                   
                    CCCScope = new Scope();
                    CCCScope.AuthorityId = authorityId;
                    CCCScope.ContainerId = CCCContainer;

                
            } catch (Exception e)
                {
            }

         
        }

        public void readMetaInformation(string language, string version)
        {
            Entities = getFromCloud<MetaInformation>(language, version);
            ReadOnlyCollection<MetaInformation> metainfo = convert<MetaInformation>(Entities);
            MetaInformation = metainfo[0];

        }


        public void readDiagnoses(string language, string version)
        {
                Entities = getFromCloud<Diagnosis>(language,version);
                Diagnoses = convert<Diagnosis>(Entities);

        }



        public void readCCCFramework(BackgroundWorker bw,string language, string version)
        {
            bw.ReportProgress(10,"Diagnoses"); 

            readDiagnoses(language, version);
            bw.ReportProgress(40, "Interventions");

            readInterventions(language, version);
            bw.ReportProgress(80,"Action types");

            readActionTypes(language, version);
            bw.ReportProgress(85,"Outcome types");

            readOutcomeTypes(language, version);
            bw.ReportProgress(90,"Components");

            readComponents(language, version);
            bw.ReportProgress(95,"Patterns");

            readPatterns(language, version);
            bw.ReportProgress(100,String.Empty);
        }

        public void readPatterns(string language, string version)
        {
            Entities = getFromCloud<Pattern>(language, version);
            Patterns = convert<Pattern>(Entities);

        }

        public void readComponentsForPattern(int patternid, string language, string version)
        {
            Entities = getComponentsForPattern(patternid, language, version);
            Components = convert<CCCBrowser.Support.Component>(Entities);
        }

        public void readComponents(string language, string version)
        {
            Entities = getFromCloud<CCCBrowser.Support.Component>(language, version);
            Components = convert<CCCBrowser.Support.Component>(Entities);
        }

        public void readOutcomeTypes(string language, string version)
        {
            Entities = getFromCloud<OutcomeType>(language, version);
            OutcomeTypes = convert<OutcomeType>(Entities);
        }

        public void readActionTypes(string language, string version)
        {
            Entities = getFromCloud<ActionType>(language, version);
            ActionTypes = convert<ActionType>(Entities);
        }

        public void readInterventions(string language, string version)
        {
            Entities = getFromCloud<Intervention>(language, version);
            Interventions = convert<Intervention>(Entities);
        }

        public void readDiagnosesForComponent(string code,string language, string version)
        {
            Entities = getDiagnosesForComponent(code, language, version);
            Diagnoses = convert<Diagnosis>(Entities);
        }

        public void readInterventionsForComponent(string code, string language, string version)
        {
            Entities = getInterventionsForComponent(code, language, version);
            Interventions = convert<Intervention>(Entities);
        }

        private ReadOnlyCollection<T> convert<T>(ReadOnlyCollection<Entity> entities)
        {
            if (entities.Count == 0)
                return null;
           
            
            if (entities[0].Kind == "Diagnosis")
            {
                List<Diagnosis> ld = new List<Diagnosis>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    Diagnosis diag = new Diagnosis();
                    diag.ComponentCode = entity.Properties["code"] as string;
                    diag.Concept = entity.Properties["concept"] as string;
                    diag.Definition = entity.Properties["definition"] as string;
                    diag.MajorCode = Convert.ToInt32(entity.Properties["majorcode"]);
                    diag.MinorCode = Convert.ToInt32(entity.Properties["minorcode"]);
                    diag.Language = entity.Properties["language"] as string;
                    diag.Version = entity.Properties["version"] as string;
                    ld.Add(diag);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }


            if (entities[0].Kind == "Intervention")
            {
                List<Intervention> ld = new List<Intervention>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    Intervention interv = new Intervention();
                    interv.ComponentCode = entity.Properties["code"] as string;
                    interv.Concept = entity.Properties["concept"] as string;
                    interv.Definition = entity.Properties["definition"] as string;
                    interv.MajorCode = Convert.ToInt32(entity.Properties["majorcode"]);
                    interv.MinorCode = Convert.ToInt32(entity.Properties["minorcode"]);
                    interv.Language = entity.Properties["language"] as string;
                    interv.Version = entity.Properties["version"] as string;
                    ld.Add(interv);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }


            if (entities[0].Kind == "ActionType")
            {
                List<ActionType> ld = new List<ActionType>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    ActionType at = new ActionType();
                    at.Code =  Convert.ToInt32(entity.Properties["code"]);
                    at.Concept = entity.Properties["concept"] as string;
                    at.Definition = entity.Properties["definition"] as string;
                    at.Language = entity.Properties["language"] as string;
                    at.Version = entity.Properties["version"] as string;
                    ld.Add(at);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }


            if (entities[0].Kind == "OutcomeType")
            {
                List<OutcomeType> ld = new List<OutcomeType>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    OutcomeType at = new OutcomeType();
                    at.Code = Convert.ToInt32(entity.Properties["code"]);
                    at.Concept = entity.Properties["concept"] as string;
                    at.Definition = entity.Properties["definition"] as string;
                    at.Language = entity.Properties["language"] as string;
                    at.Version = entity.Properties["version"] as string;
                    ld.Add(at);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }


            if (entities[0].Kind == "Component")
            {
                List<CCCBrowser.Support.Component> ld = new List<CCCBrowser.Support.Component>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    CCCBrowser.Support.Component comp = new CCCBrowser.Support.Component();
                    comp.ComponentCode = entity.Properties["code"] as string;
                    comp.PatternId = Convert.ToInt32(entity.Properties["patternid"]);
                    comp.Concept = entity.Properties["concept"] as string;
                    comp.Definition = entity.Properties["definition"] as string;
                    comp.Language = entity.Properties["language"] as string;
                    comp.Version = entity.Properties["version"] as string;
                    ld.Add(comp);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }

            if (entities[0].Kind == "Pattern")
            {
                List<Pattern> ld = new List<Pattern>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    Pattern pattern = new Pattern();
                    pattern.PatternId = Convert.ToInt32(entity.Properties["patternid"]);
                    pattern.Concept = entity.Properties["concept"] as string;
                    pattern.Definition = entity.Properties["definition"] as string;
                    pattern.Language = entity.Properties["language"] as string;
                    pattern.Version = entity.Properties["version"] as string;
                    ld.Add(pattern);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }

            if (entities[0].Kind == "MetaInformation")
            {
                List<MetaInformation> ld = new List<MetaInformation>();
                // Converter fra entity til diagnosis
                foreach (Entity entity in entities)
                {
                    MetaInformation metaInfo = new MetaInformation();
                    metaInfo.Authors = entity.Properties["authors"] as string;
                    
                    try
                    {
                        metaInfo.LastUpdate = entity.Properties["lastupdate"] as DateTime?;
                    }
                    catch (KeyNotFoundException e)
                    {
                        metaInfo.LastUpdate = null;
                    }
                    
                    metaInfo.LogoURL = entity.Properties["logourl"] as string;
                    metaInfo.Name = entity.Properties["name"] as string;
                    metaInfo.Language = entity.Properties["language"] as string;
                    metaInfo.Version = entity.Properties["version"] as string;
                    ld.Add(metaInfo);
                }
                return (ReadOnlyCollection<T>)ld.AsReadOnly().Cast<T>();
            }

            return null;
        }

        public ReadOnlyCollection<Entity> getComponentsForPattern(int patternid,string language, string version)
        {

            string query = string.Format(@"from e in entities where e.Kind==""Component"" && e[""language""]==""{0}"" && e[""version""]==""{1}"" && e[""patternid""]=={2} select e",  language, version, patternid);

            List<Entity> entities = new List<Entity>();

            try
            {
                entities = proxy.Query(CCCScope, query);

            }
            catch (Exception e)
            {
            }


            return entities.AsReadOnly();

        }

        public ReadOnlyCollection<Entity> getDiagnosesForComponent(string code, string language, string version)
        {

            string query = string.Format(@"from e in entities where e.Kind==""Diagnosis"" && e[""language""]==""{0}"" && e[""version""]==""{1}"" && e[""code""]==""{2}"" select e", language, version, code);

            List<Entity> entities = new List<Entity>();

            try
            {
                entities = proxy.Query(CCCScope, query);

            }
            catch (Exception e)
            {
            }


            return entities.AsReadOnly();

        }

        public ReadOnlyCollection<Entity> getInterventionsForComponent(string code, string language, string version)
        {

            string query = string.Format(@"from e in entities where e.Kind==""Intervention"" && e[""language""]==""{0}"" && e[""version""]==""{1}"" && e[""code""]==""{2}"" select e", language, version, code);

            List<Entity> entities = new List<Entity>();

            try
            {
                entities = proxy.Query(CCCScope, query);

            }
            catch (Exception e)
            {
            }


            return entities.AsReadOnly();

        }


        public ReadOnlyCollection<Entity> getFromCloud<T>(string language, string version)
        {
            string tName = typeof(T).Name;

            string query = string.Format(@"from e in entities where e.Kind==""{0}"" && e[""language""]==""{1}"" && e[""version""]==""{2}"" select e",tName, language, version);
          
            List<Entity> entities = new List<Entity>();

            try
            {
                entities = proxy.Query(CCCScope, query);

            }
            catch (Exception e)
            {
            }


            return entities.AsReadOnly();
                   
        }
    }
}
