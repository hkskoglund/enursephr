using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using System.Data.SqlServerCe;
using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.PHR;
using eNursePHR.BusinessLayer.EF_models;


namespace eNursePHR.userInterfaceLayer
{
    
    public class BloodPressureChart : GoogleChart, INotifyPropertyChanged
    {

        const int max = 250;

        const uint highWHOSBP = 140; // WHO limit for systolic high blood pressure 
        const uint highWHODBP = 90; // WHO limit for diastolic high blood pressure

        private MeasurementsEntities ctxBP;
        private PHREntities ctxCarePlan;

        
       
       
         
         
            //double startPointHighSBP = Math.Round((double)highWHOSBP / 250, 2);
            //double startPointHighDBP = Math.Round((double)highWHODBP / 250, 2);

            //string startPointHighSBPString = startPointHighSBP.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
            //string startPointHighDBPString = startPointHighDBP.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
           

            //double lineWidth = 0.01;

            //double endPointHighSBP = startPointHighSBP + lineWidth;
            //double endPointHighDBP = startPointHighDBP + lineWidth;

            //string endPointHighSBPString = endPointHighSBP.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
            //string endPointHighDBPString = endPointHighDBP.ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);

            //bool showWHORangeMarks = false;
            //string chartRangeMark;

            //if (showWHORangeMarks)
            //    chartRangeMark = "chm=r," + colorHighSBPHex + ",0," + startPointHighSBPString + "," + endPointHighSBPString + "|" +
            //        "r," + colorHighDBPHex + ",0," + startPointHighDBPString + "," + endPointHighDBPString;
            //else
            //    chartRangeMark = String.Empty;

            //// Build query string

            //string chartUri = chartBloodPressure.APISecureLocation +
            //    chartType + "&" +
            //    chartData + "&" +
            //    chartBloodPressure.Size + "&" +
            //    chartLabel + "&" +
            //    chartBloodPressure.Title + "&" +
            //    chartGridLines + "&" +
            //    chartColors + "&" +
            //    chartAxisType + "&" +
            //    chartAxisRange + "&" +
            //    chartRangeMark;

        string colorSystolicHex; 
        string colorDiastolicHex; 
        string colorPulseHex; 
        string colorHighSBPHex;
        string colorHighDBPHex;

        private List<BloodPressure> _bpData;
        public List<BloodPressure> BPData
        {
            get { return this._bpData; }
            set { this._bpData = value; }
        }

        private bool _showSystolicBP;
        public bool ShowSystolicBP
        {
            get { return this._showSystolicBP; }
            set
            {
                if (this._showSystolicBP != value)
                {
                    this._showSystolicBP = value;
                    NotifyPropertyChanged("ShowSystolicBP");
                }
            }
        }

        private bool _showDiastolicBP;
        public bool ShowDiastolicBP
        {
            get { return this._showDiastolicBP; }
            set
            {
                if (this._showDiastolicBP != value)
                {
                    this._showDiastolicBP = value;
                    NotifyPropertyChanged("ShowDiastolicBP");
                }

            }
        }

        private bool _showPulseHR;
        public bool ShowPulseHR
        {
            get { return this._showPulseHR; }
            set
            {
                if (this._showPulseHR != value)
                {
                    this._showPulseHR = value;
                    NotifyPropertyChanged("ShowPulseHR");
                }

            }
        }

        private bool _showLabels;
        public bool ShowLabels
        {
            get { return this._showLabels; }
            set
            {
                if (this._showLabels != value)
                {
                    this._showLabels = value;
                    NotifyPropertyChanged("ShowLabels");
                }

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }




        public BloodPressureChart() : base()
        {
            this.createContext();
            this.ChartTitle = "Blood+pressure (mmHg)";
            setupChart();
        }

        public BloodPressureChart(string Title) : base(Title)
        {
            createContext();
            setupChart();
        }

        public BloodPressureChart(string title, uint width, uint height) : base(title,width,height)
        {
            createContext();
            //SqlCeEngine sqlEngine = new SqlCeEngine("DataSource="+"\""+ctxBP.Connection.DataSource+"\"");
           
            //bool verify = sqlEngine.Verify();
            
            setupChart();
            
        }

        private void setupChart()
        {

            // Colors
            colorSystolicHex = GoogleChart.removeAlphaColor(((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorSystolic")).ToString()); 
            colorDiastolicHex = GoogleChart.removeAlphaColor(((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorDiastolic")).ToString());
            colorPulseHex = GoogleChart.removeAlphaColor(((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorPulse")).ToString());
            colorHighSBPHex = GoogleChart.removeAlphaColor(((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorHighSBP")).ToString());
            colorHighDBPHex = GoogleChart.removeAlphaColor(((Brush)(App.Current.MainWindow as WindowMain).BloodPressureControl.TryFindResource("colorHighDBP")).ToString());

           // this.ChartColors = colorSystolicHex+ "," + colorDiastolicHex+","+colorPulseHex;
      
            // Type

          this.ChartType = "lc";
          
            // Labels
          //  this.ChartLabels = "Systolic SBP|Diastolic DBP|Pulse HR";

            // Grid lines

            this.ChartGridLines = "10,25";
           
            // Axis type

            this.ChartAxisType =  "y";


            // Axis range
            
            this.ChartAxisRange = "0,0,250";
           
           
        }

       
        /// <summary>
        /// Generates a google chart API simple encoding of bloodpressure values
        /// </summary>
          public void generateChart()
        {
            int?[] systolicBP = null;
            int?[] diastolicBP = null;
            int?[] pulseHR = null;
 
            // Get data from database

            this.ChartData = String.Empty;
            this.ChartColors = String.Empty;
            this.ChartLabels = String.Empty;

            if (this.ShowSystolicBP)
            {
                if (this.BPData != null)
                {
                    systolicBP = (int?[])this.BPData.Select(bp => bp.SystolicBP).ToArray();

                    this.ChartData += this.simpleEncode(systolicBP, max);
                }
                this.ChartColors += colorSystolicHex;

                this.ChartLabels += "Systolic SBP";
            
            }

            if (this.ShowDiastolicBP)
            {
                if (this.BPData != null)
                {
                    diastolicBP = (int?[])this.BPData.Select(bp => bp.DiastolicBP).ToArray();
                    if (this.ShowSystolicBP)
                    {
                        this.ChartData += ",";
                        this.ChartColors += ",";
                        this.ChartLabels += "|";
                    }
                    this.ChartData += this.simpleEncode(diastolicBP, max);
                }
                    this.ChartColors += colorDiastolicHex;
                this.ChartLabels += "Diastolic DBP";
           
            }

            if (this.ShowPulseHR)
            {
                if (this.BPData != null)
                {
                    pulseHR = (int?[])this.BPData.Select(bp => bp.Pulse).ToArray();
                    if (this.ShowSystolicBP || this.ShowDiastolicBP)
                    {
                        this.ChartData += ",";
                        this.ChartColors += ",";
                        this.ChartLabels += "|";
                    }
                    this.ChartData += this.simpleEncode(pulseHR, max);
                }
                    this.ChartColors += colorPulseHex;
                this.ChartLabels += "Pulse HR";
            
            }

            if (!this.ShowLabels)
                this.ChartLabels = String.Empty;
        }

        public Guid getCarePlanGuid()
        {
            return ctxCarePlan.CarePlan.First().Id;
        }

        private void createContext()
        {
            try
            {
                ctxBP = new MeasurementsEntities();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new NotImplementedException("Failed to create datacontext to Measurement database");
            }

            try
            {
                ctxCarePlan = new PHREntities();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("Failed to create datacontext to personal health record (PHR) database");
            }
        }

        public double? getAverageSystolicBP()
        {
            if (this.BPData == null)
                return null;
            double? avgSBP = this.BPData.Average(bp => bp.SystolicBP);
            return avgSBP;

        }

        public double? getAverageDiastolicBP()
        {
            if (this.BPData == null)
                return null;
            double? avgDBP = this.BPData.Average(bp => bp.DiastolicBP);
            return avgDBP;

        }

        public double? getAveragePulseHR()
        {
            if (this.BPData == null)
                return null;
            double? avgHR = this.BPData.Average(bp => bp.Pulse);
            return avgHR;

        }

        public void readBPdata(Guid carePlanId,int lastReadings)
        {
            var q = ctxBP.BloodPressure.OrderByDescending(bp => bp.Time);

            // Quit if empty
            if (q.Count() == 0)
            {

                this.BPData = null;
                return;
            }

            // Adjust number of lastreadings if query shows that there is less number of readings available
            if (q.Count() < lastReadings)
                lastReadings = q.Count();

            this.BPData =  q.Take(lastReadings).OrderBy(bp=> bp.Time).ToList();
        
        
        }

        public void readBPdata(Guid carePlanId, DateTime startDate, DateTime stopDate)
        {
        }

        public MeasurementUpdateStatus newBPdata(Guid carePlanId, int? systolicBP, int? diastolicBP, int? HeartRate, string comment, DateTime Time)
        {
            BloodPressure measurementBP = BloodPressure.CreateBloodPressure(Time, carePlanId);

            measurementBP.Id = Guid.NewGuid();
            measurementBP.SystolicBP = systolicBP;
            measurementBP.DiastolicBP = diastolicBP;
            measurementBP.Pulse = HeartRate;
            measurementBP.Comment = comment;

            ctxBP.AddToBloodPressure(measurementBP);

            MeasurementUpdateStatus updStatus = saveBPdata();

            if (updStatus.upd == -1)
                ctxBP.DeleteObject(measurementBP); // Remove object that causes problems....

            return updStatus;
            
                


           }

        public MeasurementUpdateStatus saveBPdata()
        {
            int updates;
            MeasurementUpdateStatus updStatus = new MeasurementUpdateStatus();

            try
            {
                updates = ctxBP.SaveChanges();
                updStatus.upd = updates;
            }
            catch (System.Data.UpdateException ex)
            {
                updStatus = new MeasurementUpdateStatus();
                updStatus.upd = -1;
                updStatus.updMsg = ex.Message;
                if (ex.InnerException != null)
                    updStatus.updMsg += " "+ ex.InnerException.Message;
               
                   }

            return updStatus;

        }


    }
}
