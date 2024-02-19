using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DoughnutChartDataSet
    {
        public DoughnutChartDataSet()
        {
            Data = new List<int>();
        }
        public string Label { get; set; }
        public List<string> BackgroundColor { get; set; }
        public List<string> HoverBackgroundColor { get; set; }
        public List<int> Data { get; set; }
    }
}
