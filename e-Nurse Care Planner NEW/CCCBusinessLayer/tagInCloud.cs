using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCC.BusinessLayer


{

    public class KeyTagInCloud
    {
        public string Concept
        {
            get;
            set;
        }

        public string CareComponentConcept
        {
            get;
            set;
        }

    }


    public class TagInCloud : KeyTagInCloud
    {
        
        public long Count
        {
            get;
            set;
        }
    }
}
