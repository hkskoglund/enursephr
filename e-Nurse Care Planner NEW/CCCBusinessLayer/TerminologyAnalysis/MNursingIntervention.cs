using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNurseCP.BusinessLayer
{
    public class MNursingIntervention
    {
        public MNursingIntervention(List<ReferenceFrameworkModel.Nursing_Intervention> mnd)
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

        List<ReferenceFrameworkModel.Nursing_Intervention> _missingNursingInterventions;
        public List<ReferenceFrameworkModel.Nursing_Intervention> MissingList
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

