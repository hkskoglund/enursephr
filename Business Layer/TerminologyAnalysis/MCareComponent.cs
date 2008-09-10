using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    public class MCareComponent
    {
        public MCareComponent(List<eNursePHR.BusinessLayer.CCC_Terminology.Care_Component> mcc)
        {
            this.FrameworkElement = "Care Component";
            this.MissingList = mcc;
        }

        public MCareComponent()
        {
            this.FrameworkElement = "Care Component";
        }

        private string _frameworkElement;
        public string FrameworkElement
        {
            get { return this._frameworkElement; }
            set { this._frameworkElement = value; }
        }

        List<eNursePHR.BusinessLayer.CCC_Terminology.Care_Component> _missingCareComponents;
        public List<eNursePHR.BusinessLayer.CCC_Terminology.Care_Component> MissingList
        {
            get { return this._missingCareComponents; }
            set { this._missingCareComponents = value; }
        }

        public int MissingListCount
        {
            get { return this.MissingList.Count; }
        }
    }

    
}
