using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSDMS.Application.Web.Areas.ReportManage.Models
{
    public class AuditSmsModel
    {
        public string SmsLogId { get; set; }
        public string Mobile { get; set; }

        public string CarNum { get; set; }

        public DateTime? RegisterTime { get; set; }

        public string UseTypeName { get; set; }

        public string CarTypeName { get; set; }

        public DateTime? SendTime { get; set; }

        public string Content { get; set; }
    }
}