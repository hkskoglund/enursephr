// Allows compilation targeted for SQL Server Compact V3.5 Sp 1

#define SQL_SERVER_COMPACT_SPECIFIC_CODE



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

        
        private ObservableCollection<DatabaseHealth> dbHealthStatus = new ObservableCollection<DatabaseHealth>();

        private Dictionary<string, bool> healthDB;

        public WindowCopyright()
        {
            InitializeComponent();
            lbDatabaseHealth.ItemsSource = dbHealthStatus;

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
            
            healthDB = new Dictionary<string, bool>();

            // Check health of personal health record
            DatabaseHealth PHRDB = new DatabaseHealth(true,"Personal health record",String.Empty);
            dbHealthStatus.Add(PHRDB);
            checkPHRDB(PHRDB,"eNurseCP2008!");
           
            // Check health of ccc framework that contains all language translations
            DatabaseHealth cccFrameworkDB = new DatabaseHealth(true, "CCC framework", String.Empty);
            dbHealthStatus.Add(cccFrameworkDB);
            checkCCCFrameworkDB(cccFrameworkDB);

            // Check health of ccc reference terminology
            DatabaseHealth cccReferenceDB = new DatabaseHealth(true, "CCC reference", String.Empty);
            dbHealthStatus.Add(cccReferenceDB);
            checkCCCReferenceDB(cccReferenceDB);

            return healthDB;

        }

        /// <summary>
        /// Tries to connection to the CCC reference database, and peforms sql server compact verification
        /// </summary>
        /// <param name="cccReferenceDB"></param>
        private void checkCCCReferenceDB(DatabaseHealth cccReferenceDB)
        {
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

                checkSQLServerCompactDB(cccReferenceDB, conn, "CCCReference");

            }
            catch (Exception ex)
            {
                cccReferenceDB.OK = false;
                healthDB.Add("CCCReference", false);
                cccReferenceDB.Message = ex.Message;
                if (ex.InnerException != null)
                    cccReferenceDB.Message += " " + ex.InnerException.Message;
            }
        }

        /// <summary>
        /// Tries to connect to CCC framework database, and performs sql server compact db. verification
        /// </summary>
        /// <param name="cccFrameworkDB"></param>
        private void checkCCCFrameworkDB(DatabaseHealth cccFrameworkDB)
        {
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

                checkSQLServerCompactDB(cccFrameworkDB, conn, "CCCFramework");
                
            }
            catch (Exception ex)
            {
                cccFrameworkDB.OK = false;
                healthDB.Add("CCCFramework", false);
                cccFrameworkDB.Message = ex.Message;
                if (ex.InnerException != null)
                    cccFrameworkDB.Message += " " + ex.InnerException.Message;
            }
        }


        private double getFilesizeUsedInPercent(string fileName, double maxFileSizeLimit)
        {
            FileInfo fi = new FileInfo(fileName);
            return Math.Round((double)fi.Length / maxFileSizeLimit * 100, 1);
               
        }

        /// <summary>
        /// Tries to connect to personal health record PHR database, and performs sql server compact db. verification
        /// </summary>
        /// <param name="GB4"></param>
        /// <param name="carePlanDB"></param>
        /// <param name="password"></param>
        private void checkPHRDB(DatabaseHealth carePlanDB, string password)
        {
            long maxSQLServerCompactDBSize = 4294967296; // 4 GB 1024*1024*1024*4

            try
            {
                string conn;
                PHREntities ctxCarePlan = new PHREntities();
                ctxCarePlan.Connection.Open();
                //healthDB.Add("PHR", true);

                conn = "DataSource=" + "\"" + ctxCarePlan.Connection.DataSource + "\"" + ";Password='"+password+"'";
                FileInfo fi = new FileInfo(ctxCarePlan.Connection.DataSource);
                carePlanDB.Message = "Used " + getFilesizeUsedInPercent(ctxCarePlan.Connection.DataSource,Convert.ToDouble(maxSQLServerCompactDBSize)).ToString() + "%";
                //PHRDB.Message = "OK";
                ctxCarePlan.Dispose();

               checkSQLServerCompactDB(carePlanDB, conn, "PHR");


            }
            catch (Exception ex)
            {
                carePlanDB.OK = false;
                healthDB.Add("PHR", false);
                carePlanDB.Message = ex.Message;
                if (ex.InnerException != null)
                    carePlanDB.Message += " " + ex.InnerException.Message;
            }
        }

        /// <summary>
        /// Verifies and repairs a sql server compact database
        /// </summary>
        /// <param name="DBHealth"></param>
        /// <param name="conn"></param>
        /// <param name="DBKey"></param>
private void checkSQLServerCompactDB(DatabaseHealth DBHealth, string conn, string DBKey)
{
        #if (SQL_SERVER_COMPACT_SPECIFIC_CODE)
                // Checksum verification
                SqlCeEngine sqlEngine = new SqlCeEngine(conn);

  
             
                if (sqlEngine.Verify())
                    healthDB.Add(DBKey, true);
                else
                {
                    DBHealth.OK = false;
                    healthDB.Add(DBKey, false);
                    DBHealth.Message = "Checksum failed on database, recovering will be tried";
                    sqlEngine.Repair(sqlEngine.LocalConnectionString, RepairOption.RecoverAllPossibleRows);
                    // To DO: Add new verification and repair with new RepairOptions DeleteCorruptedRows if thats oK with user
                }
                sqlEngine.Dispose();
#endif
        
}


    }
}
