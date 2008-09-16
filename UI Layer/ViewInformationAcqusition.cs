using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Annotations;
using System.Windows.Data;
using System.ComponentModel;
using eNursePHR.BusinessLayer;
using System.Windows.Annotations.Storage;


namespace eNursePHR.userInterfaceLayer
{
    public class ViewInformationAcqusition
    {
        private ObservableCollection<Annotation> _statement = new ObservableCollection<Annotation>();
        public ObservableCollection<Annotation> Statement
        {
            get { return _statement; }
            set { _statement = value; }
        }

        private ListCollectionView _cvStatement;
        public ListCollectionView CvStatement
        {
            get { return _cvStatement; }
            set { _cvStatement = value; }
            
        }

        public ViewInformationAcqusition()
        {
            this.CvStatement = new ListCollectionView(this.Statement);
            this.CvStatement.SortDescriptions.Add(new SortDescription("CreationTime", ListSortDirection.Descending));
        }

        public void Refresh(AnnotationStore aStore)
        {
            _statement.Clear();
            IList<Annotation> aList = aStore.GetAnnotations();
            foreach (Annotation a in aList)
            {
  // 12 september 08 : Added if-statement to only display highlights
                if (a.AnnotationType.Name=="Highlight")
                  _statement.Add(a);
            }
        }
    }
}
