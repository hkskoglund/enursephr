using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using CCC.BusinessLayer;
using System.Threading;
using System.IO;


namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
     
       public static ViewCCCFrameWork cccFrameWork; //= new ViewCCCFrameWork();

       public static ViewCarePlan carePlan; // = new ViewCarePlan(1, cccFrameWork.DB);

      
        void AppStartup(object sender, StartupEventArgs args)
        {

            WindowMain mainWindow = new WindowMain();

             mainWindow.Show();
           
            
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            MessageBox.Show("Avslutter på grunn av : " + e.ReasonSessionEnding.ToString());

           
           // App.carePlan.DB.SubmitChanges();
            App.carePlan.DB.SaveChanges();
        }

        


    }
}
