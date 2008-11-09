// Revision history :
// 5 nov. 08: Changed XML-format to be compatible with Azure -> allows storage of data on the cloud 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel;
using CCCXMLExport.sdsClient;

namespace CCCXMLExport
{
    class Program


    {

        static XNamespace sNamespace = "http://schemas.microsoft.com/sitka/2008/03/"; // Azure
        static XNamespace xNamespace = "http://www.w3.org/2001/XMLSchema";
        static XNamespace xsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

       
        static void Main(string[] args)
        {
            Console.WriteLine("Export of CCC taxonomy to XML/Azure SQL data services");
           
          
            CCC_FrameworkEntities ctx = new CCC_FrameworkEntities();
          
           
            string cloudSolution = "yourcloud";   // CHANGE BEFORE USE !!!
            string authorityId = "authority";     // CHANGE BEFORE USE !!!

            Console.WriteLine("About to export taxonomy to Azure cloud solution {0}", cloudSolution);
            Console.Write("Password : ");
            string password = Console.ReadLine();

            SitkaSoapServiceClient cloudProxy = null;
            try
            {
                Console.WriteLine("Opening connection to cloud...");
                cloudProxy = new SitkaSoapServiceClient("BasicAuthEndpoint");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }

            // Credentials
            cloudProxy.ClientCredentials.UserName.UserName = cloudSolution;
            cloudProxy.ClientCredentials.UserName.Password = password;

            // Code based on : http://msdn.microsoft.com/nb-no/library/cc512408(en-us).aspx

            Scope containerScope = createContainer(cloudProxy, authorityId, "CCCTaxonomy");

            XDocument xDocument = startV2Export(cloudProxy,containerScope,ctx);


            Console.ReadLine();
//            startV1Export(ctx);

        }

        private static Scope createContainer(SitkaSoapServiceClient proxy, string authorityId, string containerId)
        {
            Scope scope = new Scope();
            scope.AuthorityId = authorityId;

            Container container = new Container();
            container.Id = containerId;
            try
            {
                proxy.Create(scope,container);
                Console.WriteLine("Created container for {0}", containerId);
            }
            catch (FaultException<Error> e)
            {
                if (e.Detail.StatusCode == ErrorCodes.EntityExists)
                {
                    Console.WriteLine("Container for {0} exists, deleting it", containerId);
                    deleteContainer(proxy, authorityId, containerId);
                    createContainer(proxy, authorityId, containerId);
                }
               
            }

            Scope containerScope = new Scope();
            containerScope.AuthorityId = authorityId;
            containerScope.ContainerId = containerId;

            return containerScope;
        }

        private static void deleteContainer(SitkaSoapServiceClient proxy, string authorityId, string containerId)
        {
            Scope scope = new Scope();
            scope.AuthorityId = authorityId;
            scope.ContainerId = containerId;
            try
            {
                proxy.Delete(scope);
            }
            catch (FaultException<Error> e)
            {
            }
        }

        private static XDocument startV2Export(SitkaSoapServiceClient proxy, Scope containerScope,CCC_FrameworkEntities ctx)
        {
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement xRoot =
                new XElement("CCCTaxonomy",
                    new XAttribute("ExportedBy", "CCC XML Export HKS nov. 08 - Azure version"),
                    new XAttribute("ExportedDate", DateTime.Now.ToString()));

            xDocument.Add(xRoot);

            // Meta
            Console.WriteLine("Creating XML for meta information....");
            createMetaInformationv2(xRoot, ctx);
            Console.WriteLine("Writing meta info. to cloud...");
            createMetaInformationv2Cloud(proxy, containerScope, xRoot, ctx);

            // Patterns
            Console.WriteLine("Creating XML for care pattern....");
            createPatternsv2(xRoot, ctx);
            Console.WriteLine("Writing patterns to cloud...");
            createPatternsv2Cloud(proxy, containerScope, xRoot, ctx);


            // Components
            Console.WriteLine("Creating XML for care components...");
            createComponentsv2(xRoot, ctx);
            Console.WriteLine("Writing components to cloud...");
            createComponentsv2Cloud(proxy, containerScope, xRoot, ctx);
            
            // Diagnoses

            Console.WriteLine("Creating XML for nursing diagnoses...");
            createDiagnosesv2(xRoot, ctx);
            Console.WriteLine("Writing diagnoses to cloud...");
            createDiagnosesv2Cloud(proxy, containerScope, xRoot, ctx);

            // Interevntions

            Console.WriteLine("Creating XML for nursing interventions...");
            createInterventionsv2(xRoot, ctx);
            Console.WriteLine("Writing interventions to cloud...");
            createInterventionsv2Cloud(proxy, containerScope, xRoot, ctx);

            // Outcome types
            Console.WriteLine("Creating XML for outcome types...");
            createOutcomeTypesv2(xRoot, ctx);
            Console.WriteLine("Writing to cloud...");
            createOutcomeTypesv2Cloud(proxy, containerScope, xRoot, ctx);

            // Action types
            Console.WriteLine("Creating XML for action types...");
            createActionTypesv2(xRoot, ctx);
            Console.WriteLine("Writing to cloud...");
            createActionTypesv2Cloud(proxy, containerScope, xRoot, ctx);

            Console.WriteLine("Saving to file...");
            xDocument.Save("C:\\CCCTaxonomy.xml");

            return xDocument;
        }

        private static void startV1Export(CCC_FrameworkEntities ctx)
        {
            XmlWriter xw = XmlWriter.Create("C:\\CCCTaxonomy.xml");
            xw.WriteStartDocument();
            xw.WriteWhitespace("\r\n");
            xw.WriteStartElement("CCCTaxonomy");
            xw.WriteAttributeString("ExportedBy", "CCC XML Export HKS nov. 08");
            xw.WriteAttributeString("ExportedDate", DateTime.Now.ToString());

            xw.WriteWhitespace("\r\n");


            createMetaInformationv1(ctx, xw);

            createPatternsv1(ctx, xw);

            createComponentsv1(ctx, xw);


            createDiagnosesv1(ctx, xw);

            createInterventionsv1(ctx, xw);


            createActionTypesv1(ctx, xw);

            createOutcomeTypesv1(ctx, xw);

            ctx.SaveChanges();
            xw.WriteEndElement();
            xw.Close();
        }

        private static void createOutcomeTypesv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (OutcomeType ot in ctx.OutcomeType.OrderBy(d => d.Version).ThenBy(o => o.Language_Name).ThenBy(o => o.Code))
            {
                xw.WriteStartElement("OutcomeType");
                xw.WriteAttributeString("Code", ot.Code.ToString());
                xw.WriteAttributeString("Concept", ot.Concept.Trim());
                if (ot.Definition == null || ot.Definition == String.Empty)
                    xw.WriteAttributeString("Definition", "");
                else
                    xw.WriteAttributeString("Definition", ot.Definition.Trim());
                xw.WriteAttributeString("Version", ot.Version);
                xw.WriteAttributeString("Language", ot.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");

            }
        }

        private static bool createEntity(SitkaSoapServiceClient proxy, Scope containerScope, Entity entity)
        {
            try
            {
                proxy.Create(containerScope,entity);
                return true;
            }
            catch (FaultException<Error> e)
            {
                Console.WriteLine("Failed to create ", e.Detail.Message);
                return false;
            }
        }


        private static void createOutcomeTypesv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope, XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (OutcomeType ot in ctx.OutcomeType.OrderBy(d => d.Version).ThenBy(a => a.Language_Name).ThenBy(a => a.Code))
            {

                
                string id = "OutcomeType!" + ot.Code.ToString() + "!" + ot.Language_Name + "!" + ot.Version;
                
                Entity eOutcome = new Entity();

                eOutcome.Id = id;
                eOutcome.Kind = "OutcomeType";
                eOutcome.Properties = new Dictionary<string, object>();
                eOutcome.Properties["code"] = (decimal)ot.Code;
                eOutcome.Properties["concept"] = ot.Concept;
                eOutcome.Properties["definition"] = ot.Definition;
                if (eOutcome.Properties["definition"] == null) // SDS Does not allow nulls
                    eOutcome.Properties["definition"] = String.Empty;
                eOutcome.Properties["version"] = ot.Version;
                eOutcome.Properties["language"] = ot.Language_Name;

                createEntity(proxy, containerScope, eOutcome);

            }
        }

        private static void createOutcomeTypesv2(XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (OutcomeType ot in ctx.OutcomeType.OrderBy(d => d.Version).ThenBy(a => a.Language_Name).ThenBy(a => a.Code))
            {

                XElement xOutcomeType = new XElement("OutcomeType"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );

                string id = "OutcomeType!"+ot.Code.ToString() + "!" + ot.Language_Name + "!" + ot.Version;
                xOutcomeType.Add(
                    new XElement(sNamespace + "Id", id),
                    new XElement("code", new XAttribute(xsiNamespace + "type", "x:decimal"), ot.Code),
                    new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), ot.Concept),
                    new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), ot.Definition),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), ot.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), ot.Language_Name));

                root.Add(xOutcomeType);

                
            }
        }


        private static void createActionTypesv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (ActionType at in ctx.ActionType.OrderBy(d => d.Version).ThenBy(a => a.Language_Name).ThenBy(a => a.Code))
            {
                xw.WriteStartElement("ActionType");
                xw.WriteAttributeString("Code", at.Code.ToString());
                xw.WriteAttributeString("Concept", at.Concept.Trim());
                if (at.Definition != null)
                    xw.WriteAttributeString("Definition", at.Definition.Trim());
                else
                    xw.WriteAttributeString("Definition", "");
                xw.WriteAttributeString("Version", at.Version);
                xw.WriteAttributeString("Language", at.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");

            }
        }

        private static void createActionTypesv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope, XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (ActionType at in ctx.ActionType.OrderBy(d => d.Version).ThenBy(a => a.Language_Name).ThenBy(a => a.Code))
            {

                
                string id = "ActionType!" + at.Code.ToString() + "!" + at.Language_Name + "!" + at.Version;

                
                // Prepare cloud entity
                Entity eActionType = new Entity();

                eActionType.Id = id;
                eActionType.Kind = "ActionType";
                eActionType.Properties = new Dictionary<string, object>();
                eActionType.Properties["code"] = (decimal)at.Code;
                eActionType.Properties["concept"] = at.Concept;
                eActionType.Properties["definition"] = at.Definition;
                if (eActionType.Properties["definition"] == null) // SDS Does not allow nulls
                    eActionType.Properties["definition"] = String.Empty;

                eActionType.Properties["version"] = at.Version;
                eActionType.Properties["language"] = at.Language_Name;

                createEntity(proxy, containerScope, eActionType);

            }
        }


        private static void createActionTypesv2( XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (ActionType at in ctx.ActionType.OrderBy(d => d.Version).ThenBy(a => a.Language_Name).ThenBy(a => a.Code))
            {

                XElement xActionType = new XElement("ActionType"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );

                string id = "ActionType!"+at.Code.ToString() + "!" + at.Language_Name + "!" + at.Version;

                xActionType.Add(
                    new XElement(sNamespace + "Id",id),
                    new XElement("code", new XAttribute(xsiNamespace + "type", "x:decimal"), at.Code),
                    new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), at.Concept),
                    new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), at.Definition),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), at.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), at.Language_Name));
              
                root.Add(xActionType);
                
            }
        }

        private static void createInterventionsv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (Nursing_Intervention interv in ctx.Nursing_Intervention.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode))
            {

                xw.WriteStartElement("Intervention");
                xw.WriteAttributeString("ComponentCode", interv.ComponentCode);
                xw.WriteAttributeString("MajorCode", interv.MajorCode.ToString());
                xw.WriteAttributeString("MinorCode", interv.MinorCode.ToString());
                xw.WriteAttributeString("Concept", interv.Concept.Trim());
                xw.WriteAttributeString("Definition", interv.Definition.Trim());
                xw.WriteAttributeString("Version", interv.Version);
                xw.WriteAttributeString("Language", interv.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");
            }
        }

        private static void createInterventionsv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope, XElement root, CCC_FrameworkEntities ctx)
        {
            var q = ctx.Nursing_Intervention.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode);
            Console.WriteLine("Processing {0} interventions", q.Count());

            int count = 0;
            foreach (Nursing_Intervention interv in q)
            {
             
                string id;

                if (!interv.MinorCode.HasValue)
                    id = "Intervention!" + interv.ComponentCode + "."
                         + string.Format("{0:00}", interv.MajorCode) + "." + "0" + "!" + interv.Language_Name + "!" + interv.Version;

                else
                
                    id = "Intervention!" + interv.ComponentCode + "."
                        + string.Format("{0:00}", interv.MajorCode) + "." + interv.MinorCode + "!" + interv.Language_Name + "!" + interv.Version;


                Console.Write("Nr. {0} - {1}\r", ++count, id);

                    // Prepare cloud entity
                    Entity eIntervention = new Entity();

                    eIntervention.Id = id;
                    eIntervention.Kind = "Intervention";
                    eIntervention.Properties = new Dictionary<string, object>();
                    eIntervention.Properties["code"] = interv.ComponentCode;
                    eIntervention.Properties["concept"] = interv.Concept;
                    eIntervention.Properties["definition"] = interv.Definition;
                    if (eIntervention.Properties["definition"] == null) // SDS Does not allow nulls
                        eIntervention.Properties["definition"] = String.Empty;
                    eIntervention.Properties["majorcode"] = (decimal)interv.MajorCode;
                    if (interv.MinorCode.HasValue)
                        eIntervention.Properties["minorcode"] = (decimal)interv.MinorCode;
                    else
                        eIntervention.Properties["minorcode"] = (decimal)0;

                    eIntervention.Properties["version"] = interv.Version;
                    eIntervention.Properties["language"] = interv.Language_Name;

                    createEntity(proxy, containerScope, eIntervention);

                
            }
        }


        private static void createDiagnosesv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope, XElement root, CCC_FrameworkEntities ctx)
        {
            var q = ctx.Nursing_Diagnosis.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode);
            Console.WriteLine("Processing {0} diagnoses", q.Count());

            int count = 0;
            foreach (Nursing_Diagnosis diag in q)
            {

                string id;

                if (!diag.MinorCode.HasValue)
                    id = "Diagnosis!" + diag.ComponentCode + "."
                         + string.Format("{0:00}", diag.MajorCode) + "." + "0" + "!" + diag.Language_Name + "!" + diag.Version;

                else
                
                    id = "Diagnosis!" + diag.ComponentCode + "."
                        + string.Format("{0:00}", diag.MajorCode) + "." + diag.MinorCode + "!" + diag.Language_Name + "!" + diag.Version;

                Console.Write("Nr. {0} - {1}\r", ++count, id);
              
                    // Prepare cloud entity
                    Entity eIntervention = new Entity();

                    eIntervention.Id = id;
                    eIntervention.Kind = "Diagnosis";
                    eIntervention.Properties = new Dictionary<string, object>();
                    eIntervention.Properties["code"] = diag.ComponentCode;
                    eIntervention.Properties["concept"] = diag.Concept;
                    eIntervention.Properties["definition"] = diag.Definition;
                    if (eIntervention.Properties["definition"] == null) // SDS Does not allow nulls
                        eIntervention.Properties["definition"] = String.Empty;
                    eIntervention.Properties["majorcode"] = (decimal)diag.MajorCode;
                    if (diag.MinorCode.HasValue)
                        eIntervention.Properties["minorcode"] = (decimal)diag.MinorCode;
                    else
                        eIntervention.Properties["minorcode"] = (decimal)0;

                    eIntervention.Properties["version"] = diag.Version;
                    eIntervention.Properties["language"] = diag.Language_Name;

                    createEntity(proxy, containerScope, eIntervention);

                
            }
        }
        

        private static void createInterventionsv2(XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (Nursing_Intervention interv in ctx.Nursing_Intervention.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode))
            {

                XElement xIntervention = new XElement("Intervention"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );

                string id;

                if (!interv.MinorCode.HasValue)
                {
                    id = "Intervention!" + interv.ComponentCode + "."
                         + string.Format("{0:00}", interv.MajorCode) + "." + "0" + "!" + interv.Language_Name + "!" + interv.Version;
                    xIntervention.Add(
                      new XElement(sNamespace + "Id", id));
                }

                else
                {
                    id = "Intervention!" + interv.ComponentCode + "."
                        + string.Format("{0:00}", interv.MajorCode) + "." + interv.MinorCode + "!" + interv.Language_Name + "!" + interv.Version;
                    xIntervention.Add(
                     new XElement(sNamespace + "Id", id));
                }
                
                if (!interv.MinorCode.HasValue)
                
                    xIntervention.Add(
                        new XElement("code", new XAttribute(xsiNamespace + "type", "x:string"), interv.ComponentCode),
                        new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), interv.Concept),
                        new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), interv.Definition),
                        new XElement("majorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), interv.MajorCode),
                        new XElement("minorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), 0),
                        new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), interv.Version),
                        new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), interv.Language_Name));
                 else

                   xIntervention.Add(
                    new XElement("code", new XAttribute(xsiNamespace + "type", "x:string"), interv.ComponentCode),
                    new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), interv.Concept),
                    new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), interv.Definition),
                    new XElement("majorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), interv.MajorCode),
                    new XElement("minorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), interv.MinorCode),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), interv.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), interv.Language_Name));
                
                root.Add(xIntervention);

                
            }
        }



        private static void createDiagnosesv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (Nursing_Diagnosis diag in ctx.Nursing_Diagnosis.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode))
            {

                xw.WriteStartElement("Diagnosis");
                xw.WriteAttributeString("ComponentCode", diag.ComponentCode);
                xw.WriteAttributeString("MajorCode", diag.MajorCode.ToString());
                xw.WriteAttributeString("MinorCode", diag.MinorCode.ToString());
                xw.WriteAttributeString("Concept", diag.Concept.Trim());
                xw.WriteAttributeString("Definition", diag.Definition.Trim());
                xw.WriteAttributeString("Version", diag.Version);
                xw.WriteAttributeString("Language", diag.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");
            }
        }

        private static void createDiagnosesv2(XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (Nursing_Diagnosis diag in ctx.Nursing_Diagnosis.OrderBy(d => d.Version).ThenBy(d => d.Language_Name).ThenBy(d => d.ComponentCode).ThenBy(d => d.MajorCode))
          
            {

                XElement xDiagnosis = new XElement("Diagnosis"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );

                if (!diag.MinorCode.HasValue)
                  xDiagnosis.Add(
                    new XElement(sNamespace + "Id", diag.ComponentCode + "."
                       +string.Format("{0:00}",diag.MajorCode) + "."+"0"+"!"+ diag.Language_Name + "!" + diag.Version));
                else
                   xDiagnosis.Add(
                    new XElement(sNamespace + "Id", diag.ComponentCode + "."
                       +string.Format("{0:00}",diag.MajorCode) + "."+diag.MinorCode+"!"+ diag.Language_Name + "!" + diag.Version));

                if (!diag.MinorCode.HasValue)
                {

                    xDiagnosis.Add(
                        new XElement("code", new XAttribute(xsiNamespace + "type", "x:string"), diag.ComponentCode),
                        new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), diag.Concept),
                        new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), diag.Definition),
                        new XElement("majorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), diag.MajorCode.ToString()),
                        new XElement("minorcode", new XAttribute(xsiNamespace + "type", "x:decimal"),0),
                        new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), diag.Version),
                        new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), diag.Language_Name));
                }
                else
                {

                    xDiagnosis.Add(
                        new XElement("code", new XAttribute(xsiNamespace + "type", "x:string"), diag.ComponentCode),
                        new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), diag.Concept),
                        new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), diag.Definition),
                        new XElement("majorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), diag.MajorCode),
                        new XElement("minorcode", new XAttribute(xsiNamespace + "type", "x:decimal"), diag.MinorCode),
                        new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), diag.Version),
                        new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), diag.Language_Name));
               
                }

                root.Add(xDiagnosis);

            }
        }


        private static void createComponentsv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (Care_component comp in ctx.Care_component.Include("CarePattern").OrderBy(d => d.Version).ThenBy(c => c.Language_Name).ThenBy(c => c.Code))
            {
                xw.WriteStartElement("CareComponent");
                xw.WriteAttributeString("ComponentCode", comp.Code);
                xw.WriteAttributeString("Concept", comp.Component.Trim());
                xw.WriteAttributeString("Definition", comp.Definition.Trim());
                xw.WriteAttributeString("PatternId", comp.CarePattern.Id.ToString());
                xw.WriteAttributeString("Version", comp.Version);
                xw.WriteAttributeString("Language", comp.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");
            }
        }

        private static void createComponentsv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope,XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (Care_component comp in ctx.Care_component.Include("CarePattern").OrderBy(d => d.Version).ThenBy(c => c.Language_Name).ThenBy(c => c.Code))
            {
                
                  string id = "Component!"+comp.Code.ToString() + "!" + comp.Language_Name + "!" + comp.Version;
                  Console.Write(id+"\r");

                // Prepare cloud entity
                Entity eComponent = new Entity();

                eComponent.Id = id;
                eComponent.Kind = "Component"; // Creates Component XML element instead of Entity-element/by default
                eComponent.Properties = new Dictionary<string, object>();
                eComponent.Properties["code"] = comp.Code;
                eComponent.Properties["concept"] = comp.Component;
                eComponent.Properties["definition"] = comp.Definition;
                if (eComponent.Properties["definition"] == null) // SDS Does not allow nulls
                    eComponent.Properties["definition"] = String.Empty;
                eComponent.Properties["version"] =comp.Version;
                eComponent.Properties["language"] = comp.Language_Name;
                eComponent.Properties["patternid"] = (decimal)comp.PatternId;

                createEntity(proxy, containerScope, eComponent);

            }
        }


        private static void createComponentsv2(XElement root, CCC_FrameworkEntities ctx)
        {
            foreach (Care_component comp in ctx.Care_component.Include("CarePattern").OrderBy(d => d.Version).ThenBy(c => c.Language_Name).ThenBy(c => c.Code))
            {

                XElement xComponent = new XElement("Component"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );


                xComponent.Add(
                    new XElement(sNamespace + "Id", comp.Code.ToString() + "!" + comp.Language_Name + "!" + comp.Version),

                    new XElement("code", new XAttribute(xsiNamespace + "type", "x:string"), comp.Code),
                    new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), comp.Component),
                    new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), comp.Definition),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), comp.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), comp.Language_Name),
                    new XElement("patternid", new XAttribute(xsiNamespace + "type", "x:decimal"), comp.PatternId));
                  
                root.Add(xComponent);

            }
        }

        private static void createPatternsv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            foreach (CarePattern cp in ctx.CarePattern.OrderBy(d => d.Version).ThenBy(i => i.Language_Name).ThenBy(i => i.Id))
            {
                xw.WriteStartElement("CarePattern");
                xw.WriteAttributeString("PatternId", cp.Id.ToString());
                xw.WriteAttributeString("Concept", cp.Pattern);
                xw.WriteAttributeString("Definition", cp.Definition);
                xw.WriteAttributeString("Version", cp.Version);
                xw.WriteAttributeString("Language", cp.Language_Name);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");
            }
        }


        private static void createPatternsv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope,  XElement root, CCC_FrameworkEntities ctx)
        {

            foreach (CarePattern cp in ctx.CarePattern.OrderBy(d => d.Version).ThenBy(i => i.Language_Name).ThenBy(i => i.Id))
            {

                string id = "Pattern!" + cp.Id.ToString() + "!" + cp.Language_Name + "!" + cp.Version;

                Entity ePattern = new Entity();

                ePattern.Id = id;
                ePattern.Kind = "Pattern"; // Creates Component XML element instead of Entity-element/by default
                ePattern.Properties = new Dictionary<string, object>();
                ePattern.Properties["patternid"] = (decimal)cp.Id;
                ePattern.Properties["concept"] = cp.Pattern;
                ePattern.Properties["definition"] = cp.Definition;
                if (ePattern.Properties["definition"] == null) // SDS Does not allow nulls
                    ePattern.Properties["definition"] = String.Empty;
                ePattern.Properties["version"] = cp.Version;
                ePattern.Properties["language"] = cp.Language_Name;

                createEntity(proxy, containerScope, ePattern);


            }
        }
        
        private static void createPatternsv2(XElement root, CCC_FrameworkEntities ctx)
        {

                foreach (CarePattern cp in ctx.CarePattern.OrderBy(d => d.Version).ThenBy(i => i.Language_Name).ThenBy(i => i.Id))
            {

                XElement xPattern = new XElement("Pattern"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );


                xPattern.Add(
                    new XElement(sNamespace + "Id",cp.Id.ToString()+"!"+ cp.Language_Name + "!" + cp.Version),
                
                    new XElement("patternid", new XAttribute(xsiNamespace + "type", "x:decimal"), cp.Id),
                    new XElement("concept", new XAttribute(xsiNamespace + "type", "x:string"), cp.Pattern),
                    new XElement("definition", new XAttribute(xsiNamespace + "type", "x:string"), cp.Definition),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), cp.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), cp.Language_Name));

                root.Add(xPattern);

            }
        }
        
        
        private static void createMetaInformationv1(CCC_FrameworkEntities ctx, XmlWriter xw)
        {
            
            foreach (Copyright mi in ctx.Copyright.OrderBy(d => d.Version).ThenBy(i => i.Language_Name))
            {
                xw.WriteStartElement("MetaInformation");
                xw.WriteAttributeString("Name", mi.Name);
                xw.WriteAttributeString("Authors", mi.Authors);
                xw.WriteAttributeString("LastUpdate", mi.LastUpdate.ToString());
                xw.WriteAttributeString("LogoURL", mi.LogoURL);
                xw.WriteAttributeString("Version", mi.Version.Trim());
                xw.WriteAttributeString("Language", mi.Language_Name);
                Console.WriteLine("Language {0} Version {1}", mi.Language_Name, mi.Version);
                xw.WriteEndElement();
                xw.WriteWhitespace("\r\n");

              
            }
        }

        private static void createMetaInformationv2Cloud(SitkaSoapServiceClient proxy, Scope containerScope, XElement root, CCC_FrameworkEntities ctx)
        {

            foreach (Copyright mi in ctx.Copyright.OrderBy(d => d.Version).ThenBy(i => i.Language_Name))
            {

                string id = "MetaInformation!" + mi.Language_Name + "!" + mi.Version;

                Entity eMeta = new Entity();
                eMeta.Id = id;
                eMeta.Kind = "MetaInformation";
                eMeta.Properties = new Dictionary<string, object>();
                eMeta.Properties["name"] = mi.Name;
                eMeta.Properties["authors"] = mi.Authors;
                if (mi.LastUpdate != null)
                  eMeta.Properties["lastupdate"] = (DateTime)mi.LastUpdate; // Skips if null

                if (mi.LogoURL != null)
                    eMeta.Properties["logourl"] = mi.LogoURL;
                else
                    eMeta.Properties["logourl"] = String.Empty;

                eMeta.Properties["version"] = mi.Version;
                eMeta.Properties["language"] = mi.Language_Name;

                createEntity(proxy, containerScope, eMeta);


            }
        }

        private static void createMetaInformationv2(XElement root,CCC_FrameworkEntities ctx)
        {
           
            foreach (Copyright mi in ctx.Copyright.OrderBy(d => d.Version).ThenBy(i => i.Language_Name))
            {

                XElement xMetaInformation = new XElement("MetaInformation"
                    ,
    new XAttribute(XNamespace.Xmlns + "s", sNamespace),
    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespace),
   new XAttribute(XNamespace.Xmlns + "x", xNamespace)
    );


                xMetaInformation.Add(
                    new XElement(sNamespace + "Id", mi.Language_Name + "!" + mi.Version),
                    new XElement("name", new XAttribute(xsiNamespace + "type", "x:string"), mi.Name),
                    new XElement("authors", new XAttribute(xsiNamespace + "type", "x:string"), mi.Authors),
                    new XElement("lastupdate", new XAttribute(xsiNamespace + "type", "x:dateTime"), mi.LastUpdate),
                    new XElement("logourl", new XAttribute(xsiNamespace + "type", "x:string"), mi.LogoURL),
                    new XElement("version", new XAttribute(xsiNamespace + "type", "x:string"), mi.Version),
                    new XElement("language", new XAttribute(xsiNamespace + "type", "x:string"), mi.Language_Name));

                root.Add(xMetaInformation);

            }
        }
    }
}
