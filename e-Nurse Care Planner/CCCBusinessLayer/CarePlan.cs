using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace CCC.BusinessLayer
{
   
    public class myCarePlan
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

      
       
      public myCarePlan()
      {

          ActiveCarePlan = DB.CarePlan.First();
         
      }

        
    
    }
}
