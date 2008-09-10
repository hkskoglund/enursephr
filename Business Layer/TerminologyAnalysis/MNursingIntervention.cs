using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    public class MNursingIntervention
    {
        public MNursingIntervention(List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> mnd)
        {
            this.FrameworkElement = "Nursing Intervention";
            this.MissingList = mnd;
        }

        public MNursingIntervention()
        {
            this.FrameworkElement = "Nursing Intervention";
        }

        private string _frameworkElement;
        public string FrameworkElement
        {
            get { return this._frameworkElement; }
            set { this._frameworkElement = value; }
        }

        List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> _missingNursingInterventions;
        public List<eNursePHR.BusinessLayer.CCC_Terminology.Nursing_Intervention> MissingList
        {
            get { return this._missingNursingInterventions; }
            set { this._missingNursingInterventions = value; }
        }

        public int MissingListCount
        {
            get { return this.MissingList.Count; }
        }
    }

}

