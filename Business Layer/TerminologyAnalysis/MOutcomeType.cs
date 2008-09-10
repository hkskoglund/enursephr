using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    
    public class MOutcomeType
    {
        public MOutcomeType(List<eNursePHR.BusinessLayer.CCC_Terminology.OutcomeType> mot)
        {
            this.FrameworkElement = "Outcome Type";
            this.MissingList = mot;
        }

        public MOutcomeType()
        {
            this.FrameworkElement = "Outcome Type";
        }

        private string _frameworkElement;
        public string FrameworkElement
        {
            get { return this._frameworkElement; }
            set { this._frameworkElement = value; }
        }

        List<eNursePHR.BusinessLayer.CCC_Terminology.OutcomeType> _missingOutcomeTypes;
        public List<eNursePHR.BusinessLayer.CCC_Terminology.OutcomeType> MissingList
        {
            get { return this._missingOutcomeTypes; }
            set { this._missingOutcomeTypes = value; }
        }

        public int MissingListCount
        {
            get { return this.MissingList.Count; }
        }
    }
}
