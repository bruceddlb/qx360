using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    public partial class SchoolEntity
    {

        public List<SubjectEntity> SubjectList { get; set; }
        public List<AttachmentPicEntity> AttachmentPicList { get; set; }
        public List<TagEntity> TagList { get; set; }
        public List<TeacherEntity> TeacherList { get; set; }
        public List<TrainingCarEntity> TrainingCarList { get; set; }
        public string SubjectName { get; set; }
        public string TagName { get; set; }
        public int TeacherCount { get; set; }

        public int StudentCount { get; set; }

        public int TrainingCarCount { get; set; }

        public int ApplyOrderCount { get; set; }
        public int TrainingOrderCount { get; set; }

        public string HowLong { get; set; }

        /// <summary>
        /// 距离条件
        /// </summary>
        public QX360.Model.Enums.DistanceRange DistanceRange { get; set; }

        /// <summary>
        /// 实训价格区间
        /// </summary>
        public QX360.Model.Enums.PriceRange TrainingPriceRange { get; set; }

        /// <summary>
        /// 当前经度
        /// </summary>
        public decimal CurentLng { get; set; }

        /// <summary>
        /// 当前纬度
        /// </summary>
        public decimal CurrentLat { get; set; }

        public string StatusName { get; set; }
    }
}
