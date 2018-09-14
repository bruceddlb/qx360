using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class ShopEntity
    {
        public List<ShopCarEntity> ShopCarList { get; set; }
        public int CarCount { get; set; }
        public int WeekSeeCarOrderCount { get; set; }
        public int TotalSeeCarOrderCount { get; set; }
        public string HowLong { get; set; }

        public string StatusName { get; set; }
        /// <summary>
        /// 距离条件
        /// </summary>
        public QX360.Model.Enums.DistanceRange DistanceRange { get; set; }

        public string PriceRange { get; set; }

        public string BrandRange { get; set; }

    }
}
