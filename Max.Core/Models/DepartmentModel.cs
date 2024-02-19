using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CostCenterCode { get; set; }
        public int? OrganizationId { get; set; }
        public int Status { get; set; }
        public bool Active
        {
            get
            {
                if (Status == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    Status = 1;
                else
                    Status = 0;
            }
        }
    }
}
