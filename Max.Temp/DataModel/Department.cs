using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Department
    {
        public Department()
        {
            Glaccounts = new HashSet<Glaccount>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CostCenterCode { get; set; }
        public int? OrganizationId { get; set; }
        public int? Status { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual ICollection<Glaccount> Glaccounts { get; set; }
    }
}
