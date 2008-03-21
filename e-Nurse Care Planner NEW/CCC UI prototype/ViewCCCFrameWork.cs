using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCC.BusinessLayer;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;

namespace CCC.UI
{
  
    public class ViewCCCFrameWork : CCC_Framework
    {
        private ListCollectionView _cvComponents;
        private ListCollectionView _cvDiagnoses;
        private ListCollectionView _cvInterventions;
        private ListCollectionView _cvOutcomeTypes;
        private ListCollectionView _cvActionTypes;

        public ViewCCCFrameWork(string languageName, string version)
            : base(languageName, version)
        {
            this.cvComponents = new ListCollectionView(this.Components);
            this.cvDiagnoses = new ListCollectionView(this.Diagnoses);
            this.cvInterventions = new ListCollectionView(this.Inteventions);
            this.cvOutcomeTypes = new ListCollectionView(this.Outcomes);
            this.cvActionTypes = new ListCollectionView(this.ActionTypes);

           
            this.cvComponents.GroupDescriptions.Add(new PropertyGroupDescription("Pattern"));
            this.cvComponents.SortDescriptions.Add(new SortDescription("Component", ListSortDirection.Ascending));
            this.cvComponents.Refresh();

            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));

            this.cvInterventions.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvInterventions.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));

            this.cvComponents.Refresh();
            this.cvDiagnoses.Refresh();
            this.cvInterventions.Refresh();
            this.cvOutcomeTypes.Refresh();
            this.cvActionTypes.Refresh();

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

        public ListCollectionView cvOutcomeTypes
        {
            get { return this._cvOutcomeTypes; }
            set { this._cvOutcomeTypes = value; }
        }

        public ListCollectionView cvActionTypes
        {
            get { return this._cvActionTypes; }
            set { this._cvActionTypes = value; }
        }

    }

}
