using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportShareModel
    {
        public int ReportShareId { get; set; }
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public int SharedToUserId { get; set; }
        public int tReportId { get; set; }
        //public int ReportStatus { get; set; }

        //public bool IsActive
        //{
        //    get
        //    {
        //        return ReportStatus == 1 ? true : false;
        //    }
        //    set
        //    {
        //        ReportStatus = value ? 1 : 0;
        //    }
        //}
    }
}
