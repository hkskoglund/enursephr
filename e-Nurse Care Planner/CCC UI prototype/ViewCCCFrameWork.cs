using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCC.BusinessLayer;
using System.Windows.Data;
using System.ComponentModel;

namespace CCC.UI
{
    public class ViewCCCFrameWork : CCC_Framework
    {
        private ListCollectionView _cvComponents;
        private ListCollectionView _cvDiagnoses;
        private ListCollectionView _cvInterventions;

        public ViewCCCFrameWork(string languageName)
            : base(languageName)
        {
            this.cvComponents = new ListCollectionView(this.Components);
            this.cvDiagnoses = new ListCollectionView(this.Diagnoses);
            this.cvInterventions = new ListCollectionView(this.Inteventions);

            this.cvComponents.GroupDescriptions.Add(new PropertyGroupDescription("Pattern"));
            this.cvComponents.SortDescriptions.Add(new SortDescription("Component", ListSortDirection.Ascending));
            this.cvComponents.Refresh();

            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));

            this.cvInterventions.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvInterventions.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));

        }

        public ListCollectionView cvComponents
        {
            get { return this._cvComponents; }
            set { this._cvComponents = value; }
        }

        public ListCollectionView cvDiagnoses
        {
            get { return this._cvDiagnoses; }
            set { this._cvDiagnoses = value; }
        }

        public ListCollectionView cvInterventions
        {
            get { return this._cvInterventions; }
            set { this._cvInterventions = value; }
        }
    }

}
