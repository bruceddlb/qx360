using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class TeacherEntity
    {
        public SchoolEntity School { get; set; }
        public int StudentCount { get; set; }
        public int WeekStudyOrderCount { get; set; }
        public int WeekWithDrivingOrderCount { get; set; }

        public string StatusName { get; set; }

        public string IsWithDrivingName { get; set; }
        public string IsTakeCarName { get; set; }
    }
}
