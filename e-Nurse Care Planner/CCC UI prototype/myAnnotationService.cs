using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Threading;
using CCC.BusinessLayer;
using System.Xml.Linq;

namespace CCC.UI
{


    public class myAnnotationService
    {
        MemoryStream AnnotationStream;
        AnnotationStore store;
        public AnnotationService service;
       

        ListBox lbAnnot;
        ViewCarePlan cp;

        DispatcherTimer timer;

        private List<CarePlanAnnotation> _annotlist = new List<CarePlanAnnotation>();

        InkCanvas inkyes;
        

        public void startAnnotationService(FlowDocumentReader flowdoc,ListBox lbAnnotations, ViewCarePlan vcp)
        {
           service = AnnotationService.GetService(flowdoc);

            if (service == null)
            {

                // (a) Create a Stream for the annotations to be stored in.

                AnnotationStream = new MemoryStream();
                
                //if (carePlan.Annotation!= null) {
                //     AnnotationStream.Write(carePlan.Annotation, 0, carePlan.Annotation.Count<byte>());

                //AnnotationStream.Position = 0; // Neccessary to add this for correct initialization in XmlStreamStore

                //}

               

                // (b) Create an AnnotationService on our 

                // FlowDocumentPageViewer.

                service = new AnnotationService(flowdoc);

                // (c) Create an AnnotationStore and give it the stream we 

                // created. (Autoflush == false, must do store.Flush manually)
                store = new XmlStreamStore(AnnotationStream);


                // (d) "Turn on annotations". Annotations will be persisted in 

                // the stream created at (a).
       
                service.Enable(store);

                //E var q = cp.DB.Annots.Where(a => a.CarePlanId == cp.Id);

                cp = vcp;
               
                var q = vcp.DB.Annotation.Where(a => a.CarePlan.Id == vcp.Id);


                _annotlist = q.ToList();

                foreach (CarePlanAnnotation a in q)
                {
                   MemoryStream smallStream = new MemoryStream();

                   StreamWriter writer = new StreamWriter(smallStream);
                   writer.Write(a.Data);
                   writer.Flush();
                   smallStream.Position = 0;
                   XmlStreamStore smallstore = new XmlStreamStore(smallStream);
                 
                    
                    //   smallStream.Write(a.Data, 0, a.Data.Length);
                 //   smallStream.Position = 0;
                 //   XmlStreamStore smallstore = new XmlStreamStore(smallStream);
                    
                    // smallstore.Flush();
            
                    Annotation b = smallstore.GetAnnotation(a.Id);

                    store.AddAnnotation(b);
                    smallstore.Dispose();
                    smallStream.Dispose();
                  
                }


                //Annotation a = new Annotation();
                //a.Authors.Add("Henning");
                //a.Anchors.Add(new AnnotationResource("Min annotasjons ressurs"));
                //IAnchorInfo info;
                //info.Anchor.ContentLocators.
                //store.AddAnnotation(a);

                /*  System.Xml.XmlQualifiedName name = new System.Xml.XmlQualifiedName("Highlight","http://schemas.microsoft.com/windows/annotations/2003/11/base");
                  Annotation a = new Annotation(name,Guid.NewGuid(),DateTime.Now,DateTime.Now);
                  a.Authors.Add("Henning");
                  store.GetAnnotations().Add(a);
                  */
                // IAnchorInfo info;


                lbAnnot = lbAnnotations;
                lbAnnot.ItemsSource = store.GetAnnotations();

                // Subscribe to all events to store annotation data
                store.StoreContentChanged += new StoreContentChangedEventHandler(annotationStoreChangeHandler);
                store.CargoChanged += new AnnotationResourceChangedEventHandler(store_CargoChanged);
                store.AnchorChanged += new AnnotationResourceChangedEventHandler(store_AnchorChanged);
                store.AuthorChanged += new AnnotationAuthorChangedEventHandler(store_AuthorChanged);

            }
        }

        public myAnnotationService(FlowDocumentReader flowdoc, ViewCarePlan carePlan, ListBox lbAnnotations, DispatcherTimer t, InkCanvas ink)
        {
            // Code taken from annotation-blog on Internet

            cp = carePlan;
            timer = t;

            inkyes = ink;

            startAnnotationService(flowdoc,lbAnnotations, carePlan);

        
        }


        void store_AuthorChanged(object sender, AnnotationAuthorChangedEventArgs e)
        {
            //storeAnnotations();
            //throw new NotImplementedException();

            storeAnnotationDetail(e.Annotation, e.Action);
        }

        void store_AnchorChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            storeAnnotationDetail(e.Annotation, e.Action);
           // storeAnnotations();
            //throw new NotImplementedException();
        }

        void store_CargoChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            //storeAnnotations();
            storeAnnotationDetail(e.Annotation, e.Action);

        }

        private void storeAnnotationDetail(Annotation a, AnnotationAction action)
        {
            MemoryStream stream = new MemoryStream();
           // XmlStreamStore smallstore = new XmlStreamStore(stream);

            StreamReader reader = new StreamReader(stream);
            XmlStreamStore smallstore = new XmlStreamStore(stream);

            if (a.Cargos.Count >= 2) {
                if (a.Cargos[1].Name == "Ink Data")
                {
             
                    string data = a.Cargos[1].Contents[0].InnerText;
                    //System.Xml.XmlNode checkExp =a.Cargos[0].Contents[0].Attributes.GetNamedItem("IsExpanded"); 
                    //string val;
                    //if (checkExp != null)
                    //{

                    //    val = checkExp.Value;

                    //    if (val == "False")
                    //        checkExp.Value = "True";

                    //}
                    byte[] inkdata = System.Convert.FromBase64String(data);
                    MemoryStream inkstream = new MemoryStream(inkdata);
                    // InkCanvas ink = new InkCanvas();
                    System.Windows.Ink.StrokeCollection coll = new System.Windows.Ink.StrokeCollection(inkstream);
                    inkyes.RenderTransform = new ScaleTransform(0.5, 0.5);
                    inkyes.Strokes.Clear();
                    inkyes.Strokes.Add(coll);
                }
            }

            // Create memorystream of annoation
           
            smallstore.AddAnnotation(a);
            smallstore.Flush();
            

           
            CarePlanAnnotation myAnnot = null;
                foreach (CarePlanAnnotation ann in _annotlist)
                    if (ann.Id == a.Id)
                        myAnnot = ann;
               // myAnnot = cp.DB.Annots.Single(an => an.Id == a.Id);
                if (myAnnot == null)
                {
                    myAnnot = new CarePlanAnnotation();
                    myAnnot.Id = a.Id;
                   //E myAnnot.CarePlanId = cp.Id;
                    myAnnot.CarePlan = App.carePlan.DB.CarePlan.First(c => c.Id == cp.Id);
                    //E myAnnot.CarePlan.Id = cp.Id;
                    stream.Position = 0;
                    myAnnot.Data = reader.ReadToEnd();
                    //Ecp.DB.Annots.InsertOnSubmit(myAnnot);
                    //Ecp.DB.SubmitChanges();
                    cp.DB.AddToAnnotation(myAnnot);
                    cp.DB.SaveChanges();
                    
                    _annotlist.Add(myAnnot);
                } else
                {
                    byte[] buffer = stream.ToArray();
                    stream.Position = 0;
                  myAnnot.Data = reader.ReadToEnd(); // Update annotation data, submit in database timer
                 //E int c = cp.DB.GetChangeSet().Updates.Count();
                   
            }

                smallstore.Dispose();
          

        }
       
        private void storeAnnotation(Annotation a, StoreContentAction action)
        {
            CarePlanAnnotation targetAnnot;


            if (action == StoreContentAction.Deleted)
            {
                 //E   targetAnnot = cp.DB.Annotation.Single(an => an.Id == a.Id);
                targetAnnot = cp.DB.Annotation.First(an => an.Id == a.Id);
                 
                //E cp.DB.Annotation.DeleteOnSubmit(targetAnnot);
                    cp.DB.DeleteObject(targetAnnot);
            }


            
        }

       
        private void annotationStoreChangeHandler(Object sender, StoreContentChangedEventArgs e)
        {
            // Next line added because WPF does not add usernames 
            e.Annotation.Authors.Add(System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName);
           
            switch (e.Action) 
            {

                case StoreContentAction.Added: storeAnnotation(e.Annotation, e.Action); break;
                case StoreContentAction.Deleted: storeAnnotation(e.Annotation, e.Action); break;
        }


            lbAnnot.ItemsSource = store.GetAnnotations();
           // storeAnnotations();


        }

        public void stopAnnotationService()
        {
            // Code taken from Derek Mehlhorn blog http://blogs.msdn.com/mehlhorn/archive/2006/03/11/549132.aspx

            // (a) Check that an AnnotationService actually 

            // existed and was Enabled.

            //AnnotationService service =

            //AnnotationService.GetService(fdReaderPrettyCarePlan);
            
            if (service != null && service.IsEnabled)
            {

                // (b) Flush changes to annotations to our stream.

                service.Store.Flush();

                // (c) Turn off annotations.

               
                service.Disable();

                // (d) Close our stream.



                AnnotationStream.Close();

            }
        }
    }
}
