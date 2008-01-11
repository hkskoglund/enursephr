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
using System.Data.Linq;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for WindowChangeConflict.xaml
    /// </summary>
    public partial class WindowChangeConflict : Window
    {
        private MemberChangeConflict _mcc = null;

        public WindowChangeConflict()
        {
            InitializeComponent();
        }

        public void ShowException(string msg,ChangeConflictException ex, ObjectChangeConflict conflict)
        {
            if (!conflict.IsDeleted)
            {
                Type t = conflict.Object.GetType();
                string objname = t.Name;
                tbCount.Text = "Det er registrert " + conflict.MemberConflicts.Count().ToString() + " oppdateringsproblemer (for typen "+objname+").";
                
                System.Data.Common.DbConnection conn = App.carePlan.DB.Connection;
                tbServer.Text = "Server : " + conn.DataSource +  " Database : " + conn.Database;

                lbMemberConflicts.DataContext = conflict.MemberConflicts;
                lbMemberConflicts.SelectedIndex = 0;
            }
            else tbCount.Text = msg;
         }
        

        private void btnKeepChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_mcc!=null)
            _mcc.Resolve(RefreshMode.KeepChanges);
           
        }

        private void btnOverwriteCurrentValue_Click(object sender, RoutedEventArgs e)
        {
            if (_mcc!=null)
            _mcc.Resolve(RefreshMode.OverwriteCurrentValues);
           
        }

        private void btnKeepCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (_mcc!=null)
            _mcc.Resolve(RefreshMode.KeepCurrentValues);
           

        }

        private void lbMemberConflicts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mcc = lbMemberConflicts.SelectedItem as MemberChangeConflict;

            if (_mcc.CurrentValue != null)
                tbCurrentValue.Text = _mcc.CurrentValue.ToString();
            else
                tbCurrentValue.Text = "";

            if (_mcc.OriginalValue != null)
                tbOriginalValue.Text = _mcc.OriginalValue.ToString();
            else
                tbOriginalValue.Text = "";

            if (_mcc.DatabaseValue != null)
                tbDatabaseValue.Text = _mcc.DatabaseValue.ToString();
            else
                tbDatabaseValue.Text = "";

        }
    }
}
