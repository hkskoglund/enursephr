using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    public class MCareComponent
    {
        public MCareComponent(List<ReferenceFrameworkModel.Care_Component> mcc)
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

        List<ReferenceFrameworkModel.Care_Component> _missingCareComponents;
        public List<ReferenceFrameworkModel.Care_Component> MissingList
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
