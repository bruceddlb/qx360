using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class ApplyOrderEntity
    {
        public string StatusName { get; set; }

        public SchoolEntity School { get; set; }

        public TeacherEntity Teacher { get; set; }

        public MemberEntity Member { get; set; }
    }
}
