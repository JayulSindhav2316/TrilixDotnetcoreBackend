using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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
