using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class InsuranceCommpayEntity
    {
        public int WeekInsuranceOrderCount { get; set; }
        public int TotalInsuranceOrderCount { get; set; }
        public string ImageListStr { get; set; }
        public List<AttachmentPicEntity> AttachmentPic { get; set; }
        public string StatusName { get; set; }

        public string HowLong { get; set; }
    }
}
