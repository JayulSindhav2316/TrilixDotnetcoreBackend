using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GlAccountModel
    {
        public int GlAccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public string GlAccountTypeName { get; set; }
        public string DetailType { get; set; }
        public int Status { get; set; }
        public int CostCenter { get; set; }
        public string CostCenterName { get; set; }
    }
}
