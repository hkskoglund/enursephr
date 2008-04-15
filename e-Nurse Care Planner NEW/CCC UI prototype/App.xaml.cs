using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.IO;
using eNurseCP.BusinessLayer;

namespace eNurseCP.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
     
       public static ViewCCCFrameWork cccFrameWork; //= new ViewCCCFrameWork();
       public static ViewCarePlan carePlan; 

      
       public static WindowMain mainWindow;
     
      
        void AppStartup(object sender, StartupEventArgs args)
        {

            mainWindow = new WindowMain();
            App.Current.MainWindow = mainWindow;
            mainWindow.Show();
           
            
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            MessageBox.Show("Application session ending because: " + e.ReasonSessionEnding.ToString());

           
           // App.carePlan.DB.SubmitChanges();
           // App.carePlan.DB.SaveChanges();
        }

        


    }
}
