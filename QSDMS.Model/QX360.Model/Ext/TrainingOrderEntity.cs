using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class TrainingOrderEntity
    {
        public string StatusName { get; set; }
        public string UserTypeName { get; set; }

        public string CashTypeName { get; set; }
        public List<TrainingOrderDetailEntity> DetailList { get; set; }

        public int? SubrictCount { get; set; }
    }
}
