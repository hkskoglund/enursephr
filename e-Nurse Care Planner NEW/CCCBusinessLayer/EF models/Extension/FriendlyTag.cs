using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNurseCP.BusinessLayer
{
    
    public partial class Tag : global::System.Data.Objects.DataClasses.EntityObject
    {
        public string CareComponentConcept
        {
            get;
            set;
        }

        public string Concept
        {
            get;
            set;
        }

        public short? LatestOutcome
        {
            get;
            set;
        }

        public string LatestOutcomeModifier
        {
            get;
            set;
        }

        public string Definition
        {
            get;
            set;

        }
    }

}
