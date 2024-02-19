using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PieChartModel
    {
        public PieChartModel()
        {
            Datasets = new List<PieChartData>();
        }
        public List<string> Labels { get; set; }
        public List<PieChartData> Datasets { get; set; }

    }
}
