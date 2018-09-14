using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSDMS.Application.Web.Areas.AuthorizeManage.Models
{
    public class DepartmentUserModel
    {
        public int? isdefault { get; set; }
        public int? ischeck { get; set; }
        public string departmentid { get; set; }
        public string departmentname { get; set; }
        public string userid { get; set; }
        public int? gender { get; set; }
        public string account { get; set; }
        public string realname { get; set; }
    }
}