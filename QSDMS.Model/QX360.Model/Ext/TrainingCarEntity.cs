using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class TrainingCarEntity
    {
        public SchoolEntity School { get; set; }
        public int WeekApplayCount { get; set; }
        public int TotalApplayCount { get; set; }
        public string StatusName { get; set; }

        /// <summary>
        /// 预约时段总数
        /// </summary>
        public int TotalSubricCount { get; set; }
        public int TotalNoSubricCount { get; set; }
        public string SubricInfo { get; set; }
        /// <summary>
        /// 未预约时段总数
        /// </summary>
        public string NoSubricInfo { get; set; }
    }
}
