using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNurseCP.BusinessLayer
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
