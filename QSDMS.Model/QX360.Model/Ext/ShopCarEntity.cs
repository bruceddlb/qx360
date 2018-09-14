using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class ShopCarEntity
    {
        public List<AttachmentPicEntity> AttachmentPicList { get; set; }
        public ShopEntity Shop { get; set; }
        public int WeekSeeCarOrderCount { get; set; }
        public int TotalSeeCarOrderCount { get; set; }
        public string StatusName { get; set; }
    }
}
