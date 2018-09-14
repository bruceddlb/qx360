using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class FreeDateEntity
    {
        public string WeekName { get; set; }

        public bool IsCurrentDay { get; set; }

        /// <summary>
        /// 时间集合
        /// </summary>
        public List<FreeTimeEntity> FreeTimeList { get; set; }
    }
}
