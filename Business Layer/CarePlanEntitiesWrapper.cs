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

          
         
      }

        
    
    }
}
