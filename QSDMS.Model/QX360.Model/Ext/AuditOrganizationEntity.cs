using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class AuditOrganizationEntity
    {
        public string ImageListStr { get; set; }
        public List<AttachmentPicEntity> AttachmentPic { get; set; }
        public int WeekAuditOrderCount { get; set; }
        public int TotalAuditOrderCount { get; set; }
        public int WeekTakeAuditOrderCount { get; set; }

        public int TotalTakeAuditOrderCount { get; set; }

        public string HowLong { get; set; }

        public string StatusName { get; set; }

        public string IsTakeName { get; set; }
    }
}
