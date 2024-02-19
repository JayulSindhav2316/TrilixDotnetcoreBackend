using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class BarChartModel
    {
        public BarChartModel()
        {
            Datasets = new List<BarChartDataset>();
            Labels = new List<string>();
        }
        public List<string> Labels { get; set; }
        public List<BarChartDataset> Datasets { get; set; }
    }
}
