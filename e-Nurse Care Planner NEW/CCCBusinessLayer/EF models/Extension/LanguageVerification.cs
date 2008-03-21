using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferenceFrameworkModel
{
    public partial class ActionType : global::System.Data.Objects.DataClasses.EntityObject
    {
        public bool Verified
        {
            get
            {
                return this._Verified;
            }
            set
            {
                this._Verified = value;
               }
        }
        private bool _Verified;
        
    }


    public partial class Nursing_Diagnosis : global::System.Data.Objects.DataClasses.EntityObject
    {
        public bool Verified
        {
            get
            {
                return this._Verified;
            }
            set
            {
                this._Verified = value;
            }
        }
        private bool _Verified;

    }

    public partial class Nursing_Intervention : global::System.Data.Objects.DataClasses.EntityObject
    {
        public bool Verified
        {
            get
            {
                return this._Verified;
            }
            set
            {
                this._Verified = value;
            }
        }
        private bool _Verified;

    }

    public partial class OutcomeType : global::System.Data.Objects.DataClasses.EntityObject
    {
        public bool Verified
        {
            get
            {
                return this._Verified;
            }
            set
            {
                this._Verified = value;
            }
        }
        private bool _Verified;

    }


    public partial class Care_Component : global::System.Data.Objects.DataClasses.EntityObject
    {
        public bool Verified
        {
            get
            {
                return this._Verified;
            }
            set
            {
                this._Verified = value;
            }
        }
        private bool _Verified;

    }



}
