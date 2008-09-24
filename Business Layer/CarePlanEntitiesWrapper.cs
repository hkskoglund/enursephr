using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using eNursePHR.BusinessLayer.PHR;

namespace eNursePHR.BusinessLayer
{
   
    public class CarePlanEntitesWrapper
    {
        public Item NewestPHRItem
        {
            get { return getNewestItemInPHR(); }
        }

        public Item OldestPHRItem
        {
            get { return getOldestItemInPHR(); }
        }

        public DateTime? OldestPHRViewDate
        {
            get
            {
                return getOldestItemDateInPHR();
            }
            set
            {
                if (OldestPHRViewDate != value)
                    OldestPHRViewDate = value;
            }
        }


        public DateTime? NewestPHRViewDate
        {
            get
            {
                return getNewestItemDateInPHR();
            }
            set
            {
                if (NewestPHRViewDate != value)
                    NewestPHRViewDate = value;
            }
        }

        private PHREntities _db; 

        public PHREntities DB
        {
            get
            {
                if (_db == null)
                    _db = new PHREntities();
                return this._db;
            }
            set { this._db = value; }
        }

       
        public CarePlan ActiveCarePlan
        {
            get ; set ;
        }



       public CarePlanEntitesWrapper()
      {
          this._db = new PHREntities();
        }

       public Item getOldestItemInPHR()
       {
           var orderedPHRItemsByCreatedDate = this.DB.Item.Include("History").OrderByDescending(i => i.History.CreatedDate).ThenBy(i => i.History.UpdatedDate);

           Item oldestPHRItem = orderedPHRItemsByCreatedDate.First();

           return oldestPHRItem;

        
       }

       public Item getNewestItemInPHR()
       {
           var orderedPHRItemsByCreatedDate = this.DB.Item.Include("History").OrderBy(i => i.History.CreatedDate).ThenBy(i => i.History.UpdatedDate);

           Item newestPHRItem = orderedPHRItemsByCreatedDate.First();

           return newestPHRItem;

       }

       public DateTime? getOldestItemDateInPHR()
       {
           Item oldestPHRItem = getOldestItemInPHR();

           if (oldestPHRItem.History.UpdatedDate != null)
               return oldestPHRItem.History.UpdatedDate;
           else
               return oldestPHRItem.History.CreatedDate;
       }

       public DateTime? getNewestItemDateInPHR()
       {

           Item newestPHRItem = getNewestItemInPHR();

           if (newestPHRItem.History.UpdatedDate != null)
               return newestPHRItem.History.UpdatedDate;
           else
               return newestPHRItem.History.CreatedDate;
       }


    
    
    }
}
