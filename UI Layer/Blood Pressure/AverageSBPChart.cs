using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media;

namespace eNursePHR.userInterfaceLayer
{
    public class AverageChart : GoogleChart
    {
        private int max;

        private double _average;
        public double Average
        {
            get
            {
                return this._average;
            }
            set {
                double avg = (value / max) * 100;
               
                this.ChartData = avg.ToString("00.0",CultureInfo.InvariantCulture);
                this.ChartLabels = Math.Round(value,0).ToString("00", CultureInfo.InvariantCulture);
                this._average = value; 
            }
        }

        public AverageChart(Brush color, string title, uint width, uint height, int max)
            : base(title, width, height)
        {
            this.ChartType = "gom";
            this.ChartColors = "ffffff," + GoogleChart.removeAlphaColor(color.ToString());
            this.max = max;
            
        }
    }
}
