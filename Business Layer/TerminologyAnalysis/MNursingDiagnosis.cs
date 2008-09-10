using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    public class MNursingDiagnosis
    {
        public MNursingDiagnosis(List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis> mnd)
        {
            this.FrameworkElement = "Nursing Diagnosis";
            this.MissingList = mnd;
        }

        public MNursingDiagnosis()
        {
            this.FrameworkElement = "Nursing Diagnosis";
        }

        private string _frameworkElement;
        public string FrameworkElement
        {
            get { return this._frameworkElement; }
            set { this._frameworkElement = value; }
        }

        List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis> _missingNursingDiagnoses;
        public List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Diagnosis> MissingList
        {
            get { return this._missingNursingDiagnoses; }
            set { this._missingNursingDiagnoses = value; }
        }

        public int MissingListCount
        {
            get { return this.MissingList.Count; }
        }
    }

}
