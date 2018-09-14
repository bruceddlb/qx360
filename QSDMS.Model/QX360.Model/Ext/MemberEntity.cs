
using System;
namespace QX360.Model
{

    public partial class MemberEntity
    {

        public string MemberLevelName { get; set; }

        public string sms_verify_code { get; set; }

        public OwnerEntity Owner { get; set; }
        public int StudyOrderCount { get; set; }

        public int WithDrivingOrderCount { get; set; }

        public int AuditOrderCount { get; set; }
        public int TakeAuditOrderCount { get; set; }
        public int SeeCarOrderCount { get; set; }

        public string WxHeadIcon { get; set; }
        public string HaveStudyHours { get; set; }

    }
}
