using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCC.BusinessLayer
{
    public partial class FrameworkDiagnosis : global::System.Data.Objects.DataClasses.EntityObject
    {
        public string Comment { get; set; }
    }

    public partial class FrameworkIntervention : global::System.Data.Objects.DataClasses.EntityObject
    {
        public string Comment { get; set; }
    }
}
