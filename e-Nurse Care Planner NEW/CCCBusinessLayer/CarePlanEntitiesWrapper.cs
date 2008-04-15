using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace eNurseCP.BusinessLayer
{
   
    public class CarePlanEntitesWrapper
    {

        private CarePlanEntities _db; 

        public CarePlanEntities DB
        {
            get
            {
                if (_db == null)
                    _db = new CarePlanEntities();
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
