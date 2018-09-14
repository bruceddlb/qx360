using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSDMS.Application.Web.Areas.ReportManage.Models
{
    public class AuditCollectModel
    {

        public string Id { get; set; }

        public string SubTypeName { get; set; }

        public string OrganizationName { get; set; }

        public string ServiceDate { get; set; }

        public string ServiceTime { get; set; }

        public int? SubricCount { get; set; }

        public int? SortNum { get; set; }
    }
}