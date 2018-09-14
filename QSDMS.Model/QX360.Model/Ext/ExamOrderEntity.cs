using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class ExamOrderEntity
    {
        public string StatusName { get; set; }
        public MemberEntity Member { get; set; }
    }
}
