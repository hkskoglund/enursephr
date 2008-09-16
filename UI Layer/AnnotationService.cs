#define SQL_SERVER_COMPACT_SP1_WORKAROUND

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
using System.Xml.Linq;
using System.Xml;

using eNursePHR.BusinessLayer.PHR;

// Code based on info. from Derek Mehlhorn blog http://blogs.msdn.com/mehlhorn/archive/2006/03/11/549132.aspx

namespace eNursePHR.userInterfaceLayer.AnnotationNS
{

    public delegate void hideBtnSaveEventHandler(object sender, EventArgs e);
       
    public class ItemAnnotationStore
    {

        public event hideBtnSaveEventHandler hideBtnSaveEvent;

        private MemoryStream aStream;
        private AnnotationStore _store;
        public AnnotationStore Store
        {
            get { return _store; }
        }

        private Item _CurrentItem;
        public Item CurrentItem
        {
            get { return _CurrentItem; }
        }

        // Text and Ink annotations are saved on explicitt button/save click
        private List<Annotation> _InkAndTextStickNotesToBeSaved;

        public ItemAnnotationStore(Item item)
        {
            aStream = new MemoryStream();
            _store = new XmlStreamStore(aStream);
            this._CurrentItem = item;
            this._InkAndTextStickNotesToBeSaved = new List<Annotation>();
           

            loadAnnotations(item);

            // Subscribe to all events to store annotation data
            _store.StoreContentChanged += new StoreContentChangedEventHandler(annotationStoreChangeHandler);
            _store.CargoChanged += new AnnotationResourceChangedEventHandler(annotationStoreCargoChanged);
            // store.AnchorChanged += new AnnotationResourceChangedEventHandler(store_AnchorChanged);
            // store.AuthorChanged += new AnnotationAuthorChangedEventHandler(store_AuthorChanged);

        }

        /// <summary>
        /// Load all annotations for the current item
        /// </summary>
        /// <param name="item"></param>
        private void loadAnnotations(Item item)
        {
            // If not loaded, then load
            if (!item.Annotation.IsLoaded)
                item.Annotation.Load();
            // Add annotations to Xml store
            foreach (CPAnnotation itemAnnotation in item.Annotation)

                _store.AddAnnotation(deSerializeAnnotation(itemAnnotation.Data));

        }

       /// <summary>
       /// This method handles store changed events like Added and Deleted
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        void annotationStoreChangeHandler(object sender, StoreContentChangedEventArgs e)
        {
            WindowMain wndMain = (WindowMain)App.Current.MainWindow;

            // Handle highlight annotations
            if (e.Annotation.AnnotationType.Name == "Highlight")
            {
                switch (e.Action)
                {
                    case StoreContentAction.Added:
                        wndMain.infoAcq.Statement.Add(e.Annotation); // Add new statement to information acquisition window
                        break;

                    case StoreContentAction.Deleted:

                        wndMain.infoAcq.Statement.Remove(e.Annotation); // Remove statement from information acq. window
                        deleteAnnotation(e.Annotation);
                        break;
                }
            }
            else
            {

                if (e.Action == StoreContentAction.Deleted) // Deletes ink and text notes
                    deleteAnnotation(e.Annotation);
            }

            
        }


        public CPAnnotation annotationInDB(Guid aGuid)
        {

#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            // Sp 1 Beta
            var q = App.carePlan.DB.Annotation.Where(a => a.Id == aGuid);
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)
            // Sp 1 workaround
            var q = App.s_carePlan.DB.CPAnnotation.Where("it.Id = GUID '"+aGuid+"'");
#endif      
            if (q.Count() == 0)
                return null;

            CPAnnotation itemAnnotation = (CPAnnotation)q.First();

            return itemAnnotation;

        }

        public void saveAnnotation(Annotation annotation)
        {
            CPAnnotation itemAnnotation = annotationInDB(annotation.Id);

            // Check to see if annotation is already stored in DB
            if (itemAnnotation == null)
            {
                // If not, create new
                itemAnnotation = CPAnnotation.CreateCPAnnotation(annotation.Id);
                itemAnnotation.Data = serializeAnnotation(annotation);
                itemAnnotation.Item = this._CurrentItem;

                History newHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                itemAnnotation.History = newHistory;

                App.s_carePlan.DB.AddToCPAnnotation(itemAnnotation);
                App.s_carePlan.DB.AddToHistory(newHistory);


            }
            else // else, update
            {
                itemAnnotation.Data = serializeAnnotation(annotation);
                
                // Make sure that the related history reference is loaded
                if (!itemAnnotation.HistoryReference.IsLoaded)
                    itemAnnotation.HistoryReference.Load();

                itemAnnotation.History.UpdatedBy = System.Environment.UserName;
                itemAnnotation.History.UpdatedDate = DateTime.Now;

            }

            App.s_carePlan.DB.SaveChanges();
        }

        private void deleteAnnotation(Annotation annotation)
        {
            CPAnnotation deleteAnnotation;

#if (!SQL_SERVER_COMPACT_SP1_WORKAROUND)
            // Sp 1 BETA
            var q = App.carePlan.DB.Annotation.Where(a => a.Id == annotation.Id);
#elif (SQL_SERVER_COMPACT_SP1_WORKAROUND)           
            // SP 1 workaround
            var q = App.s_carePlan.DB.CPAnnotation.Where("it.Id = GUID '"+ annotation.Id+"'");
#endif
            if (q.Count() == 1)
            {
                deleteAnnotation = q.First() as CPAnnotation;
                
                // Check if this annotation has been changed recently
                if (this._InkAndTextStickNotesToBeSaved.Contains(annotation))
                {
                    this._InkAndTextStickNotesToBeSaved.Remove(annotation);
                    if (this._InkAndTextStickNotesToBeSaved.Count == 0)
                    {
                        // A little dirty trick here to call user interface code....
                        //WindowMain wndMain = App.Current.MainWindow as WindowMain;
                        //wndMain.btnSaveTextInkAnnotation.Visibility = System.Windows.Visibility.Collapsed;
                        if (hideBtnSaveEvent != null)
                            hideBtnSaveEvent(this, new EventArgs());

                    }
                       
                }
                App.s_carePlan.DB.DeleteObject((CPAnnotation)q.First());
                App.s_carePlan.DB.SaveChanges();
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

        public void saveAnnotationWithCargoChanged()
        {
            foreach (Annotation a in this._InkAndTextStickNotesToBeSaved)
                saveAnnotation(a);
        }

        void annotationStoreCargoChanged(object sender, AnnotationResourceChangedEventArgs e)
        {
            
             if (e.Annotation.AnnotationType.Name == "TextStickyNote" ||
                 e.Annotation.AnnotationType.Name == "InkStickyNote")
            {
                 // Turn on save button
                 WindowMain wndMain = App.Current.MainWindow as WindowMain;
                wndMain.btnSaveTextInkAnnotation.Visibility = System.Windows.Visibility.Visible;

                 // Add annotation to the save list...
                 if (!this._InkAndTextStickNotesToBeSaved.Contains(e.Annotation))
                   this._InkAndTextStickNotesToBeSaved.Add(e.Annotation);
            
            }

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

    public class eNAnnotationService
    {
        // This is the annotation store that is associated to the annotation service
        private ItemAnnotationStore _iAnnotationStore;
        public ItemAnnotationStore IAnnotationStore
        {
            get { return _iAnnotationStore; }
        }

        // Property for annotation service
        private AnnotationService _service;
        public AnnotationService Service
        {
            get { return _service; }
        }

        /// <summary>
        /// This method creates/reloads a new annotation store for a specified item
        /// It is associated with two eventhandlers that shows/hides a save button for annotations
        /// Decided to not use automatic save or timer based same to keep things simple.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="uiUpdSave"></param>
        /// <param name="uiHideSave"></param>
        public void changeItemStore(Item item, 
            AnnotationResourceChangedEventHandler uiUpdSave, 
            hideBtnSaveEventHandler uiHideSave)
        {
            if (_service.IsEnabled)
              _service.Disable();
             _iAnnotationStore = new ItemAnnotationStore(item);
            // Add CargoChanged event handling for the user interface -> updates the save button visibility when cargo changes
            _iAnnotationStore.Store.CargoChanged +=new AnnotationResourceChangedEventHandler(uiUpdSave);
            _iAnnotationStore.hideBtnSaveEvent += new hideBtnSaveEventHandler(uiHideSave);
            _service.Enable(_iAnnotationStore.Store);
        }

        /// <summary>
        /// Creates a new annotation service
        /// </summary>
        /// <param name="flowDocReader"></param>
        public eNAnnotationService(FlowDocumentReader flowDocReader)
        {
            _service = new AnnotationService(flowDocReader);
           

        }

    }
}
