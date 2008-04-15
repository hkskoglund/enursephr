using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNurseCP.BusinessLayer
{
    public partial class History: global::System.Data.Objects.DataClasses.EntityObject
    {
        
        public DateTime LastUpdate
        {
            get
            {
                if (this.UpdatedDate.HasValue)
                    return this.UpdatedDate.Value;
                else
                    return this.CreatedDate;
            }
        }

        public string LastUpdater
        {
            get
            {
                if (this.UpdatedDate.HasValue)
                    return this.UpdatedBy;
                else
                    return this.CreatedBy;
            }
        }
    }
}
