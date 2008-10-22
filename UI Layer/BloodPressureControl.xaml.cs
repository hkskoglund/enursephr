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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using eNursePHR.BusinessLayer;

namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for BloodPressureControl.xaml
    /// </summary>
    public partial class BloodPressureControl : UserControl
    {

        BackgroundWorker bwSave = null;    // Saves BP-data asynchronous/in the background
        BloodPressureChart chartBloodPressure;
        int? systolicBP, diastolicBP, heartRate;

        Guid cpGuid; // Get test careplan

        string comment;
        DateTime time;

        MeasurementUpdateStatus updDB = new MeasurementUpdateStatus();

        
        public BloodPressureControl()
        {
            InitializeComponent();
        }

        public void setupBloodPressureUI()
        {
            // Setup controls


            tpTime.SelectedTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 00);
            dpDate.Value = DateTime.Now;


            // Save asynchronous
            bwSave = new BackgroundWorker();
            bwSave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwSave_RunWorkerCompleted);
            bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);



            chartBloodPressure = new BloodPressureChart("Blood+pressure (mmHg)", 300, 150);

            cpGuid = chartBloodPressure.getCarePlanGuid(); // Get test careplan

            // Setup display options
            chartBloodPressure.ShowDiastolicBP = true;
            chartBloodPressure.ShowSystolicBP = true;
            chartBloodPressure.ShowPulseHR = true;
            chartBloodPressure.ShowLabels = true;

            gbDisplayOptions.DataContext = chartBloodPressure;


            // Read some test data

            chartBloodPressure.readBPdata(cpGuid, 5);
            double? avgSystolicBP = chartBloodPressure.getAverageSystolicBP();
            double? avgDiastolicBP = chartBloodPressure.getAverageDiastolicBP();
            double? avgPulseHR = chartBloodPressure.getAveragePulseHR();
            AverageChart avgSystolicChart = new AverageChart((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorSystolic"), "Systolic BP Average", 100, 60, 250);
            if (avgSystolicBP.HasValue)
                avgSystolicChart.Average = avgSystolicBP.Value;

            AverageChart avgDiastolicChart = new AverageChart((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorDiastolic"), "Diastolic BP Average", 100, 60, 250);
            if (avgDiastolicBP.HasValue)
                avgDiastolicChart.Average = avgDiastolicBP.Value;

            AverageChart avgPulseChart = new AverageChart((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorPulse"), "Pulse HR Average", 100, 60, 250);
            if (avgPulseHR.HasValue)
                avgPulseChart.Average = avgPulseHR.Value;

            showChart(avgSystolicChart, imgSystolicAverage);
            showChart(avgDiastolicChart, imgDiastolicAverage);
            showChart(avgPulseChart, imgPulseAverage);

            chartBloodPressure.generateChart();

            lvBP.ItemsSource = chartBloodPressure.BPData;



            showChart(chartBloodPressure, img);
        }

        private void cbDisplayOptionsChanged_Click(object sender, RoutedEventArgs e)
        {
            chartBloodPressure.generateChart();
            showChart(chartBloodPressure, img);
        }


        #region Blood pressure UI
        private void showChart(BloodPressureChart bpChart, Image img)
        {
            // Request image from the google chart API

            BitmapImage bimg = new BitmapImage(new Uri(bpChart.getChartURI("s:")));
            loadImage(bpChart, img, bimg);

        }

        private void loadImage(GoogleChart chart, Image img, BitmapImage bImg)
        {
            img.Height = chart.Height;
            img.Width = chart.Width;
            img.BeginInit();
            img.Source = bImg;
            img.EndInit();

        }


        private void showChart(AverageChart avgChart, Image img)
        {
            // Request image from the google chart API
            BitmapImage bimg;
            try
            {
                bimg = new BitmapImage(new Uri(avgChart.getChartURI("t:")));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load image from google chart", "Google chart load fail", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            loadImage(avgChart, img, bimg);

        }


        private void sliderDiastolic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderSystolic == null)
                return;

            // Limit diastolic below systolic pressure
            if (!sliderSystolic.IsEnabled)
                return;

            if (e.NewValue > sliderSystolic.Value)
            {

                sliderDiastolic.Value = sliderSystolic.Value;
                MessageBox.Show("Diastolic pressure cannot exceed systolic pressure", "Limit reached diastolic pressure", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void sliderSystolic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderDiastolic == null)
                return;

            // Limit systolic pressure over diastolic pressure
            if (!sliderDiastolic.IsEnabled)
                return;

            if (e.NewValue < sliderDiastolic.Value)
            {

                sliderDiastolic.Value = sliderSystolic.Value;
                MessageBox.Show("Systolic pressure cannot be below diastolic pressure", "Limit reached systolic pressure", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAddBPReading_Click(object sender, RoutedEventArgs e)
        {
            if (bwSave.IsBusy)
            {
                MessageBox.Show("Waiting for BP data to be saved", "Waiting", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            tbStatusBP.Text = null;



            if (!(bool)cbSystolicBP.IsChecked)
                systolicBP = null;
            else
                systolicBP = Convert.ToInt32(Math.Round(sliderSystolic.Value));


            if (!(bool)cbDiastolicBP.IsChecked)
                diastolicBP = null;
            else
                diastolicBP = Convert.ToInt32(Math.Round(sliderDiastolic.Value));


            if (!(bool)cbPulseHR.IsChecked)
                heartRate = null;
            else
                heartRate = Convert.ToInt32(Math.Round(sliderPulseHR.Value));


            if (!dpDate.Value.HasValue)
            {
                MessageBox.Show("You have not entered a valid date, please specify a date", "Invalid date", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime datePicker = dpDate.Value.Value;
            time = new DateTime(datePicker.Year, datePicker.Month, datePicker.Day, tpTime.SelectedHour, tpTime.SelectedMinute, tpTime.SelectedSecond);
            int result = time.CompareTo(DateTime.Now);

            if (result == 1)
            {
                MessageBox.Show("You have entered a time in the future, please define a new time", "Future time", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (tbComment.Text.Length == 0)
                comment = null;
            else
                comment = tbComment.Text;

            if (systolicBP == null && diastolicBP == null && heartRate == null && comment == null)
            {
                MessageBox.Show("There is no valid data to save, please enter a reading", "No valid data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bwSave.RunWorkerAsync();


        }

        void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {

            updDB = chartBloodPressure.newBPdata(cpGuid, systolicBP, diastolicBP, heartRate, comment, time);

        }

        void bwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (updDB.upd > 0)
            {

                tbStatusBP.Text = "Saved";
                // Remove status text after 0.5 sec.
                BackgroundWorker bw = new BackgroundWorker();
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync();
            }
            else if (updDB.upd == -1)
            {
                tbStatusBP.Text = "Failed to save";
                tbStatusBP.ToolTip = updDB.updMsg;

            }



        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(500);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tbStatusBP.Text = null;
        }




        #endregion

    }
}
