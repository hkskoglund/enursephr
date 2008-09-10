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
using System.Collections.ObjectModel;
using System.Data.EntityClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Globalization;

using eNursePHR.BusinessLayer.CCC_Terminology;
using eNursePHR.BusinessLayer.CCC_Translations;
using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.PHR;

namespace eNursePHR.userInterfaceLayer
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
            Process.Start(e.Uri.ToString());  // Sends the request to the default installed web-browser

            e.Handled = true;
       
        }

        /// <summary>
        /// Performs a health check on the databases. It does this by trying to create and open a connection to the database and calculating checksum/verification
        /// </summary>
        /// <returns>Database + flag that indicates if problems where detected , TRUE = ok</returns>
        public Dictionary<string,bool> checkDatabasesHealth()
        {
            double GB4;
            unchecked
            {
                GB4 = 1024 * 1024 * 1024 * 4; // OVERFLOW......??
            }
                healthDB = new Dictionary<string, bool>();

            DatabaseHealth carePlanDB = new DatabaseHealth(true,"Personal health record",String.Empty);
            databaseHealth.Add(carePlanDB);
            

            try
            {
                string conn;
                PHREntities ctxCarePlan = new PHREntities();
                ctxCarePlan.Connection.Open();
                //healthDB.Add("PHR", true);
                
                conn = "DataSource=" + "\"" + ctxCarePlan.Connection.DataSource + "\"" + ";Password='eNurseCP2008!'";
                FileInfo fi = new FileInfo(ctxCarePlan.Connection.DataSource);
                carePlanDB.Message = "Used " + Math.Round((double)((double)fi.Length / GB4 * 100), 1).ToString() + "%";
                //carePlanDB.Message = "OK";
                ctxCarePlan.Dispose();

                #region SQL Compact specific code

                // Checksum verification
                SqlCeEngine sqlEngine = new SqlCeEngine(conn);
                if (sqlEngine.Verify())
                    healthDB.Add("PHR", true);
                else
                {
                    carePlanDB.OK = false;
                    healthDB.Add("PHR", false);
                    carePlanDB.Message = "Checksum failed on database, recovering will be tried";
                    sqlEngine.Repair(sqlEngine.LocalConnectionString, RepairOption.RecoverCorruptedRows);
                    // To DO: Add new verification and repair with new RepairOptions DeleteCorruptedRows if thats oK with user
                }
                sqlEngine.Dispose();

                #endregion


            }
            catch (Exception ex)
            {
                carePlanDB.OK = false;
                healthDB.Add("PHR", false); 
                carePlanDB.Message = ex.Message;
                if (ex.InnerException != null)
                    carePlanDB.Message += " "+ex.InnerException.Message;
            }

           
            DatabaseHealth cccFrameworkDB = new DatabaseHealth(true, "CCC framework", String.Empty);

            databaseHealth.Add(cccFrameworkDB);

            try
            {
                string conn;
                // Open database
                CCC_FrameworkEntities ctxCCCFrameworkDB = new CCC_FrameworkEntities();
                ctxCCCFrameworkDB.Connection.Open();
                //healthDB.Add("CCCFramework", true);
                conn = "DataSource=" + "\"" + ctxCCCFrameworkDB.Connection.DataSource + "\"";

                ctxCCCFrameworkDB.Connection.Close();
                //cccFrameworkDB.Message = "OK";
                ctxCCCFrameworkDB.Dispose();

                #region SQL Server Compact specific code
                // Checksum verification
                SqlCeEngine sqlEngine = new SqlCeEngine(conn);
                if (sqlEngine.Verify())
                    healthDB.Add("CCCFramework", true);
                else
                {
                    cccFrameworkDB.OK = false;
                    healthDB.Add("CCCFramework", false);
                    cccFrameworkDB.Message = "Checksum failed on database, recovering will be tried";
                    sqlEngine.Repair(sqlEngine.LocalConnectionString, RepairOption.RecoverCorruptedRows);
                }
                sqlEngine.Dispose();
                #endregion

            }
            catch (Exception ex)
            {
                cccFrameworkDB.OK = false;
                healthDB.Add("CCCFramework", false);
                cccFrameworkDB.Message = ex.Message;
                if (ex.InnerException != null)
                    cccFrameworkDB.Message += " " + ex.InnerException.Message;
            }

            DatabaseHealth cccReferenceDB = new DatabaseHealth(true, "CCC reference", String.Empty);

            databaseHealth.Add(cccReferenceDB);

            try
            {
                string conn;
               CCC_Terminology_ReferenceEntities ctxCCCReferenceDB = new CCC_Terminology_ReferenceEntities();
               ctxCCCReferenceDB.Connection.Open();
               //healthDB.Add("CCCReference", true);
               conn = "DataSource=" + "\"" + ctxCCCReferenceDB.Connection.DataSource + "\"";
                ctxCCCReferenceDB.Connection.Close();
               //cccReferenceDB.Message = "OK";
               ctxCCCReferenceDB.Dispose();

               #region SQL Server Compact specific code
               // Checksum verification
                SqlCeEngine sqlEngine = new SqlCeEngine(conn);
               if (sqlEngine.Verify())
                   healthDB.Add("CCCReference", true);
               else
               {
                  cccReferenceDB.OK = false;
                  healthDB.Add("CCCReference", false);
                  cccReferenceDB.Message = "Checksum failed on database, recovering will be tried";
                  sqlEngine.Repair(sqlEngine.LocalConnectionString, RepairOption.RecoverCorruptedRows);

               }
               sqlEngine.Dispose();
               #endregion


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
