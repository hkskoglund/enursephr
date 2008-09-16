using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;

using eNursePHR.BusinessLayer;

namespace eNursePHR.userInterfaceLayer
{

    public enum CCCFrameworkElements {CareComponentAndPattern};

    public delegate void LoadFrameworkElementEventHandler(object sender, LoadFrameworkElementEventArgs e);

    public class LoadFrameworkElementEventArgs : EventArgs
    {
        public CCCFrameworkElements FrameworkElement
        {
            get;
            set;
        }

        public LoadFrameworkElementEventArgs(CCCFrameworkElements frameworkElement)
        {
            this.FrameworkElement = frameworkElement;
        }
    }

    public class ViewCCCFrameWork : CCC_Framework
    {
        private string languageName;
        private string version;

        private ListCollectionView _cvComponents = new ListCollectionView(new List<eNursePHR.BusinessLayer.CCC_Translations.Care_component>());
        private ListCollectionView _cvDiagnoses = new ListCollectionView(new List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Diagnosis>());
        private ListCollectionView _cvInterventions = new ListCollectionView(new List<eNursePHR.BusinessLayer.CCC_Translations.Nursing_Intervention>());
        private ListCollectionView _cvOutcomeTypes;
        private ListCollectionView _cvActionTypes;

        
        public void loadCareComponentAndPatternView(string version, string languageName)
        {
            base.loadCareComponentAndPattern(version,languageName);
       
            this.cvComponents = new ListCollectionView(this.Components);
            this.cvComponents.GroupDescriptions.Add(new PropertyGroupDescription("Pattern"));
            this.cvComponents.SortDescriptions.Add(new SortDescription("Component", ListSortDirection.Ascending));
            this.cvComponents.Refresh();
        }


        
        public void loadDiagnosesView(string version,string languageName)
        
        {
            base.loadDiagnoses(version, languageName);
            this.cvDiagnoses = new ListCollectionView(this.Diagnoses);
            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvDiagnoses.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));
            this.cvDiagnoses.Refresh();
           
        }


        public void loadInterventionsView(string version, string languageName)
        {
            this.loadInterventions(version, languageName);
        
            this.cvInterventions = new ListCollectionView(this.Inteventions);
            this.cvInterventions.SortDescriptions.Add(new SortDescription("MajorCode", ListSortDirection.Ascending));
            this.cvInterventions.SortDescriptions.Add(new SortDescription("MinorCode", ListSortDirection.Ascending));
            this.cvInterventions.Refresh();
        }


        public void loadOutcomeTypesView(string version, string languageName)
        {
            this.loadOutcomeTypes(version, languageName);
       
            this.cvOutcomeTypes = new ListCollectionView(this.Outcomes);
            this.cvOutcomeTypes.Refresh();
            
             }

        public void loadActionTypesView(string version, string languageName)
        {
            this.loadActionTypes(version, languageName);
        
            this.cvActionTypes = new ListCollectionView(this.ActionTypes);
            this.cvActionTypes.Refresh();
        }

        

        public ViewCCCFrameWork(string languageName, string version)
            : base(languageName, version)
        {
            this.version = version;
            this.languageName = languageName;
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
