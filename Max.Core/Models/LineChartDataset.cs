using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class LineChartDataset
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public bool Fill { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
    }
}
