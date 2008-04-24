using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{


    public partial class Care_component : global::System.Data.Objects.DataClasses.EntityObject
    {
        public string Pattern
        {
            get
            {
                if (!this.CarePatternReference.IsLoaded)
                    this.CarePatternReference.Load();
                return this.CarePattern.Pattern;
            }
         }
    }

}
