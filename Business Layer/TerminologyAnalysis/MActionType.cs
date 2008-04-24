using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace eNursePHR.BusinessLayer
{
    public class MActionType
    {
        public MActionType(List<ReferenceFrameworkModel.ActionType> mat)
        {
            this.FrameworkElement = "Action Type";
            this.MissingList = mat;
        }

        public MActionType()
        {
            this.FrameworkElement = "Action Type";
        }

        private string _frameworkElement;
        public string FrameworkElement
        {
            get { return this._frameworkElement; }
            set { this._frameworkElement = value; }
        }

        List<ReferenceFrameworkModel.ActionType> _missingActionTypes;
        public List<ReferenceFrameworkModel.ActionType> MissingList
        {
            get { return this._missingActionTypes; }
            set { this._missingActionTypes = value; }
        }

        public int MissingListCount
        {
            get { return this.MissingList.Count; }
        }
    }

}
