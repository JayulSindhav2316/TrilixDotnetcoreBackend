using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class WebLoginModel
    {
        public int EntityId { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public int AccountLocked { get; set; }
    }
}
