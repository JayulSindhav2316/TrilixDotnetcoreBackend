using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DoughnutChartModel
    {
        public DoughnutChartModel()
        {
            Datasets = new List<DoughnutChartDataSet>();
        }
        public List<string> Labels { get; set; }
        public List<DoughnutChartDataSet> Datasets { get; set; }
    }
}
