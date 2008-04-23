using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCC.BusinessLayer
{
    
    public class MOutcomeType
    {
        public MOutcomeType(List<ReferenceFrameworkModel.OutcomeType> mot)
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

        List<ReferenceFrameworkModel.OutcomeType> _missingOutcomeTypes;
        public List<ReferenceFrameworkModel.OutcomeType> MissingList
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
