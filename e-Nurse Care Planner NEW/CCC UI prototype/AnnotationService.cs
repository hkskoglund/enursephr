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
using eNurseCP.BusinessLayer;
using System.Xml.Linq;
using System.Xml;

// Code based on info. from Derek Mehlhorn blog http://blogs.msdn.com/mehlhorn/archive/2006/03/11/549132.aspx

namespace eNurseCP.userInterfaceLayer
{

    public class ItemAnnotationStore
    {
        private MemoryStream aStream;
        private AnnotationStore _store;
        public AnnotationStore Store
        {
            get { return _store; }
        }
       
        public ItemAnnotationStore(Item item)
        {
            aStream = new MemoryStream();
            _store = new XmlStreamStore(aStream);
       
            loadAnnotations(item);

            // Subscribe to all events to store annotation data
            _store.StoreContentChanged += new StoreContentChangedEventHandler(annotationStoreChangeHandler);
            // store.CargoChanged += new AnnotationResourceChangedEventHandler(store_CargoChanged);
            // store.AnchorChanged += new AnnotationResourceChangedEventHandler(store_AnchorChanged);
            // store.AuthorChanged += new AnnotationAuthorChangedEventHandler(store_AuthorChanged);

        }

        private void loadAnnotations(Item item)
        {
            if (!item.Annotation.IsLoaded)
                item.Annotation.Load();

            foreach (CPAnnotation itemAnnotation in item.Annotation)

                _store.AddAnnotation(deSerializeAnnotation(itemAnnotation.Data));

        }

       
        void annotationStoreChangeHandler(object sender, StoreContentChangedEventArgs e)
        {
            WindowMain wndMain = (WindowMain)App.Current.MainWindow;

            if (e.Annotation.AnnotationType.Name == "Highlight")
            {
                switch (e.Action)
                {
                    case StoreContentAction.Added:
                        wndMain.infoAcq.Statement.Add(e.Annotation);
                        break;

                    case StoreContentAction.Deleted:

                        wndMain.infoAcq.Statement.Remove(e.Annotation);
                        deleteAnnotation(e.Annotation);
                        break;
                }


            }
        }


        public CPAnnotation annotationInDB(Guid aGuid)
        {
            var q = App.carePlan.DB.Annotation.Where(a => a.Id == aGuid);
            if (q.Count() == 0)
                return null;

            CPAnnotation itemAnnotation = (CPAnnotation)q.First();

            return itemAnnotation;

        }

        public void saveAnnotation(Annotation annotation, Item item)
        {
            CPAnnotation itemAnnotation = annotationInDB(annotation.Id);

            // Check to see if annotation is already stored in DB
            if (itemAnnotation == null)
            {
                // If not, create new
                itemAnnotation = CPAnnotation.CreateCPAnnotation(annotation.Id);
                itemAnnotation.Data = serializeAnnotation(annotation);
                itemAnnotation.Item = item;

                History newHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                itemAnnotation.History = newHistory;

                App.carePlan.DB.AddToAnnotation(itemAnnotation);
                App.carePlan.DB.AddToHistory(newHistory);


            }
            else // else, update
            {
                itemAnnotation.Data = serializeAnnotation(annotation);
                itemAnnotation.History.UpdatedBy = System.Environment.UserName;
                itemAnnotation.History.UpdatedDate = DateTime.Now;

            }

            App.carePlan.DB.SaveChanges();
        }

        private void deleteAnnotation(Annotation annotation)
        {
            var q = App.carePlan.DB.Annotation.Where(a => a.Id == annotation.Id);
            if (q.Count() == 1)
            {
                App.carePlan.DB.DeleteObject((CPAnnotation)q.First());
                App.carePlan.DB.SaveChanges();
            }

        }


        void store_AuthorChanged(object sender, AnnotationAuthorChangedEventArgs e)
        {
            //storeAnnotations();
            //throw new NotImplementedException();

            //     storeAnnotationDetail(e.Annotation, e.Action);
        }

        void store_AnchorChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            //   storeAnnotationDetail(e.Annotation, e.Action);
            // storeAnnotations();
            //throw new NotImplementedException();
        }

        void store_CargoChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            //storeAnnotations();
            // storeAnnotationDetail(e.Annotation, e.Action);

        }
        /// <summary>
        /// Serialize an annotation to a byte stream
        /// Last update: 30 march 2008
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        private byte[] serializeAnnotation(Annotation annotation)
        {
            MemoryStream stream = new MemoryStream();
            XmlStreamStore annotationStore = new XmlStreamStore(stream);


            annotationStore.AddAnnotation(annotation);
            annotationStore.Flush();

            
            byte[] serializedAnnotation = stream.ToArray();
         
            return serializedAnnotation;
        }

        private Annotation deSerializeAnnotation(byte[] data)
        {
            MemoryStream mStream = new MemoryStream(data);

            XmlStreamStore store = new XmlStreamStore(mStream);

            return store.GetAnnotations().First();

        }



        
    }

    public class myAnnotationService
    {
        private ItemAnnotationStore _iAnnotationStore;
        public ItemAnnotationStore IAnnotationStore
        {
            get { return _iAnnotationStore; }
        }

        private AnnotationService _service;
        public AnnotationService Service
        {
            get { return _service; }
        }

        public void changeItemStore(Item item)
        {
            if (_service.IsEnabled)
              _service.Disable();
             _iAnnotationStore = new ItemAnnotationStore(item);
            _service.Enable(_iAnnotationStore.Store);
        }

     
        ///// <summary>
        ///// Code taken from Derek Mehlhorn blog http://blogs.msdn.com/mehlhorn/archive/2006/03/11/549132.aspx
        ///// Stops annotation service 
        ///// </summary>
        //public void stopAnnotationService()
        //{

        //    //AnnotationService service =

        //    //AnnotationService.GetService(fdReaderPrettyCarePlan);

        //    if (_service != null && _service.IsEnabled) // (a) Check that an AnnotationService actually existed and was Enabled.
        //    {
        //        _service.Store.Flush();  // (b) Flush changes to annotations to our stream.
        //        _service.Disable();      // (c) Turn off annotations.
        //        aStream.Close();        // (d) Close annotation stream
        //    }
        //}

        public myAnnotationService(FlowDocumentReader flowDocReader)
        {
            _service = new AnnotationService(flowDocReader); 
           

        }


        
        
        private void storeAnnotationDetail(Annotation a, AnnotationAction action)
        {
            MemoryStream ms = new MemoryStream();
            
          
            MemoryStream stream = new MemoryStream();
            // XmlStreamStore smallstore = new XmlStreamStore(stream);

            StreamReader reader = new StreamReader(stream);
            XmlStreamStore smallstore = new XmlStreamStore(stream);

            //if (a.Cargos.Count >= 2)
            //{
            //    if (a.Cargos[1].Name == "Ink Data")
            //    {

            //        string data = a.Cargos[1].Contents[0].InnerText;
            //        //System.Xml.XmlNode checkExp =a.Cargos[0].Contents[0].Attributes.GetNamedItem("IsExpanded"); 
            //        //string val;
            //        //if (checkExp != null)
            //        //{

            //        //    val = checkExp.Value;

            //        //    if (val == "False")
            //        //        checkExp.Value = "True";

            //        //}
            //        byte[] inkdata = System.Convert.FromBase64String(data);
            //        MemoryStream inkstream = new MemoryStream(inkdata);
            //        // InkCanvas ink = new InkCanvas();
            //        System.Windows.Ink.StrokeCollection coll = new System.Windows.Ink.StrokeCollection(inkstream);
            //        inkyes.RenderTransform = new ScaleTransform(0.5, 0.5);
            //        inkyes.Strokes.Clear();
            //        inkyes.Strokes.Add(coll);
            //    }
            //}

            // Create memorystream of annoation

            smallstore.AddAnnotation(a);
            smallstore.Flush();



            //CarePlanAnnotation myAnnot = null;
            //foreach (CarePlanAnnotation ann in _annotlist)
            //    if (ann.Id == a.Id)
            //        myAnnot = ann;
            //// myAnnot = cp.DB.Annots.Single(an => an.Id == a.Id);
            //if (myAnnot == null)
            //{
            //    myAnnot = new CarePlanAnnotation();
            //    myAnnot.Id = a.Id;
            //    //E myAnnot.CarePlanId = cp.Id;
            //    myAnnot.CarePlan = App.carePlan.DB.CarePlan.First(c => c.Id == cp.Id);
            //    //E myAnnot.CarePlan.Id = cp.Id;
            //    stream.Position = 0;
            //    myAnnot.Data = reader.ReadToEnd();
            //    //Ecp.DB.Annots.InsertOnSubmit(myAnnot);
            //    //Ecp.DB.SubmitChanges();
            //    cp.DB.AddToAnnotation(myAnnot);
            //    cp.DB.SaveChanges();

            //    _annotlist.Add(myAnnot);
            //}
            //else
            //{
            //    byte[] buffer = stream.ToArray();
            //    stream.Position = 0;
            //    myAnnot.Data = reader.ReadToEnd(); // Update annotation data, submit in database timer
            //    //E int c = cp.DB.GetChangeSet().Updates.Count();

            //}

            smallstore.Dispose();


        }

       


        //private void annotationStoreChangeHandler(Object sender, StoreContentChangedEventArgs e)
        //{
        //    // Next line added because WPF does not add usernames 
        //    e.Annotation.Authors.Add(System.Environment.MachineName.ToString() + "\\" + System.Environment.UserName);

        //    switch (e.Action)
        //    {

        //        case StoreContentAction.Added: storeAnnotation(e.Annotation, e.Action); break;
        //        case StoreContentAction.Deleted: storeAnnotation(e.Annotation, e.Action); break;
        //    }


        //    //lbAnnot.ItemsSource = store.GetAnnotations();
        //    //// storeAnnotations();


        //}



        

        
        

    }
}
