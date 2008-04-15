using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using eNurseCP.BusinessLayer;
using System.Collections.ObjectModel;
using ReferenceFrameworkModel;
using System.Data.EntityClient;

namespace eNurseCP.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for WindowCopyright.xaml
    /// </summary>
    /// 

    
    public partial class WindowCopyright : Window
    {

        
        private ObservableCollection<DatabaseHealth> databaseHealth = new ObservableCollection<DatabaseHealth>();

        private Dictionary<string, bool> healthDB;

        public WindowCopyright()
        {
            InitializeComponent();
            lbDatabaseHealth.ItemsSource = databaseHealth;

        }

        private void hyperCodeplex_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());

            e.Handled = true;
       
        }

        public Dictionary<string,bool> checkDatabasesHealth()
        {

            healthDB = new Dictionary<string, bool>();

            DatabaseHealth carePlanDB = new DatabaseHealth(true,"Careplan","Checking health status...");
            databaseHealth.Add(carePlanDB);
            

            try
            {
                CarePlanEntities ctxCarePlan = new CarePlanEntities();
                ctxCarePlan.Connection.Open();
                healthDB.Add("Careplan", true);
              
                ctxCarePlan.Connection.Close();
                carePlanDB.Message = "OK";
                ctxCarePlan.Dispose();
            }
            catch (Exception ex)
            {
                carePlanDB.OK = false;
                healthDB.Add("Careplan", false);
                carePlanDB.Message = ex.Message;
                if (ex.InnerException != null)
                    carePlanDB.Message += " "+ex.InnerException.Message;
            }

           
            DatabaseHealth cccFrameworkDB = new DatabaseHealth(true, "CCC framework", "Checking health status...");

            databaseHealth.Add(cccFrameworkDB);

            try
            {
                CCCFrameworkCompactEntities ctxCCCFrameworkDB = new CCCFrameworkCompactEntities();
                ctxCCCFrameworkDB.Connection.Open();
                healthDB.Add("CCCFramework", true);
             
                ctxCCCFrameworkDB.Connection.Close();
                cccFrameworkDB.Message = "OK";
                ctxCCCFrameworkDB.Dispose();
            }
            catch (Exception ex)
            {
                cccFrameworkDB.OK = false;
                healthDB.Add("CCCFramework", false);
                cccFrameworkDB.Message = ex.Message;
                if (ex.InnerException != null)
                    cccFrameworkDB.Message += " " + ex.InnerException.Message;
            }

            DatabaseHealth cccReferenceDB = new DatabaseHealth(true, "CCC reference", "Checking health status...");

            databaseHealth.Add(cccReferenceDB);

            try
            {
               ReferenceFrameworkEntities ctxCCCReferenceDB = new ReferenceFrameworkEntities();
               ctxCCCReferenceDB.Connection.Open();
               healthDB.Add("CCCReference", true);
             
               ctxCCCReferenceDB.Connection.Close();
                cccReferenceDB.Message = "OK";
                ctxCCCReferenceDB.Dispose();
            }
            catch (Exception ex)
            {
                cccReferenceDB.OK = false;
                healthDB.Add("CCCReference", false);
                cccReferenceDB.Message = ex.Message;
                if (ex.InnerException != null)
                    cccReferenceDB.Message += " " + ex.InnerException.Message;
            }

            return healthDB;

        }


       
    }
}
