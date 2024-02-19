using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class LineChartModel
    {
        public LineChartModel()
        {
            Datasets = new List<LineChartDataset>();
        }
        public List<string> Labels { get; set; }
        public List<LineChartDataset> Datasets { get; set; }
  
    }
}
