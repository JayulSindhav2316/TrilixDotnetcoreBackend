using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BarChartDataset
    {
        public BarChartDataset()
        {
            Data = new List<int>();
            BackgroundColor = new List<string>();
        }
        public string Label { get; set; }
        public List<string> BackgroundColor { get; set; }
        public List<int> Data { get; set; }
    }
}
