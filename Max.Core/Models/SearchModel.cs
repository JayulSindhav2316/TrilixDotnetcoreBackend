using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SearchModel
    {
        public SearchModel()
        {
            Data= new List<dynamic>();   
        }
       public dynamic Data { get; set; }
       public bool IsInvoiceModuleInUse { get; set; }

    }
}
