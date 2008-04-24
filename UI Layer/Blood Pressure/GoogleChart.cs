using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.userInterfaceLayer
{
    public class GoogleChart
    {

        // Javascript-code
        //        var simpleEncoding = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

        //function simpleEncode(valueArray,maxValue) {

        //var chartData = ['s:'];
        //  for (var i = 0; i < valueArray.length; i++) {
        //    var currentValue = valueArray[i];
        //    if (!isNaN(currentValue) && currentValue >= 0) {
        //    chartData.push(simpleEncoding.charAt(Math.round((simpleEncoding.length-1) * currentValue / maxValue)));
        //    }
        //      else {
        //      chartData.push('_');
        //      }
        //  }
        //return chartData.join('');
        //}


        public GoogleChart()
        {
        }


        public GoogleChart(string Title)
        {
            this.ChartTitle = Title;
        }

        public GoogleChart(string title, uint width, uint height)
        {
            this.ChartTitle = title;
            this.Width = width;
            this.Height = height;
        }

        private string _title;
        public string ChartTitle
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public string APILocation
        {
            get { return "http://chart.apis.google.com/chart?"; }

        }

        public string APISecureLocation // Not recommended
        {
            get { return "https://www.google.com/chart?"; }
        }

        public  uint Width { get; set; }
        public uint Height { get; set; }

        public string ChartSize
        {
            get { return "chs="+Width.ToString()+"x"+Height.ToString(); }
        }

        private string _chartType;
        public string ChartType
        {
            get { return this._chartType; }  
            set
            {
                if (this._chartType != value)
                    this._chartType = value;
            }

        }

        private string _chartData;
        public string ChartData
        {
            get { return  this._chartData; }  // Simple encoding
            set
            {
                if (this._chartData != value)
                    this._chartData = value;
            }

        }


        private string _chartGridLines;
        public string ChartGridLines


        {
           
           
            get { return  this._chartGridLines; }  
            set
            {
                if (this._chartGridLines!= value)
                    this._chartGridLines = value;
            }

        }

        private string _chartLabels;
        public string ChartLabels
        {
            get { return this._chartLabels; }  
            set
            {
                if (this._chartLabels != value)
                    this._chartLabels = value;
            }

        }


        private string _chartColors;
        public string ChartColors
        {
            get { return  this._chartColors; }  
            set
            {
                if (this._chartColors != value)
                    this._chartColors = value;
            }

        }


        private string _chartAxisType;
        public string ChartAxisType
        {
            get { return  this._chartAxisType; }
            set
            {
                if (this._chartAxisType != value)
                    this._chartAxisType = value;
            }

        }

        private string _chartAxisRange;
        public string ChartAxisRange
        {

            get { return  this._chartAxisRange; }
            set
            {
                if (this._chartAxisRange != value)
                    this._chartAxisRange = value;
            }

        }
        
           


        public string simpleEncode(int? [] valueArray, int maxValue)
        {
            string simpleEncoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder chartData = new StringBuilder();

           
            for (uint i = 0; i < valueArray.Length; i++)
            {
                int? currentValue = valueArray[i];

                if (currentValue == null)
                    chartData.Append("_");
                else
                    if (currentValue >= 0)
                        chartData.Append(simpleEncoding.Substring((int)Math.Round((double)(simpleEncoding.Length - 1) * (int)currentValue / maxValue), 1));
                    else
                        chartData.Append("_");
            }

            return chartData.ToString();

        }

        public static string removeAlphaColor(string color)
        {
            return color.Remove(0, 3);
        }

        public string getChartURI(string encoding)
        {

            // Build query string

            string chartUri = this.APILocation +
                this.ChartSize + "&" +
                "cht="+ this.ChartType + "&" +
                "chd=" + encoding + this.ChartData;

            if (this.ChartLabels != null && this.ChartLabels.Length > 0)
            {
                string token;
                if (this.ChartType.Contains("gom"))
                    token = "chl=";
                else
                    token = "chdl=";
                chartUri += "&" + token + this.ChartLabels;
            }

            if (this.ChartTitle != null && this.ChartTitle.Length > 0)
                chartUri += "&" + "chtt="+this.ChartTitle;
    
            if (this.ChartGridLines != null && this.ChartGridLines.Length > 0)
                chartUri += "&" + "chg=" + this.ChartGridLines;

            if (this.ChartColors != null && this.ChartColors.Length > 0)
                chartUri += "&" + "chco=" + this.ChartColors;


            if (this.ChartAxisType != null && this.ChartAxisType.Length > 0)
                chartUri += "&" + "chxt=" + this.ChartAxisType;

            if (this.ChartAxisRange != null && this.ChartAxisRange.Length > 0)
                chartUri += "&" + "chxr=" + this.ChartAxisRange;

            
            //this.ChartRangeMark;


            return chartUri; 


        }



    }
}
