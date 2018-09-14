using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Model
{
    /// <summary>
    /// 系统枚举类型值定义
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 支付状态
        /// </summary>
        public enum PaySatus
        {
            待支付 = 1,
            已支付 = 2,
            已完成 = 3,
            已取消 = 4,
            已评价 = 5
        }

        /// <summary>
        ///实训订单状态
        /// </summary>
        public enum TrainingStatus
        {
            待审核 = 1,
            待支付 = 2,
            已支付 = 3,
            已完成 = 4,
            已取消 = 5
        }

        /// <summary>
        /// 实训订单预约类型
        /// </summary>
        public enum TrainingUserType
        {
            学员 = 1,
            教练 = 2
        }

        /// <summary>
        /// 学车预约状态
        /// </summary>
        public enum StudySubscribeStatus
        {
            预约成功 = 1,
            取消 = 2,
            预约完成 = 3,
            学员评价 = 4,
            教练评价 = 5
        }

        /// <summary>
        /// 预约状态,看车 保险 年检
        /// </summary>
        public enum SubscribeStatus
        {
            预约成功 = 1,
            已完成 = 2,
            已取消 = 3
        }

        /// <summary>
        /// 考试状态
        /// </summary>
        public enum ExamStatus
        {
            考试成功 = 1,
            考试失败 = 2
        }

        /// <summary>
        /// 报名订单状态
        /// </summary>
        public enum ApplyStatus
        {
            待支付 = 1,
            已支付 = 2,
            已分配 = 3,
            已取消 = 4
        }
        /// <summary>
        /// 启用状态
        /// </summary>
        public enum UseStatus
        {
            启用 = 1,
            禁用 = 2,
        }

        /// <summary>
        /// 文章状态
        /// </summary>
        public enum ArticleStatus
        {
            已发送 = 1,
            草稿 = 2

        }
        /// <summary>
        /// 空闲状态
        /// </summary>
        public enum FreeTimeStatus
        {
            空闲 = 1,
            已预约 = 2,
            锁定 = 3

        }
        /// <summary>
        /// 用户操作类型
        /// </summary>
        public enum OperationType
        {
            预约驾校缴费 = 1,
            预约驾校成功 = 2,
            预约学车完成 = 3,
            预约陪驾缴费 = 4,
            预约陪驾完成 = 5,
            预约代审缴费 = 6,
            预约代审完成 = 7,
            预约年检缴费 = 7,
            预约年检完成 = 8,
            登陆操作 = 9
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public enum UserType
        {
            预约记时会员 = 1,
            VIP会员 = 2
        }

        /// <summary>
        /// 赠送积分方式
        /// </summary>
        public enum GiveType
        {
            与费用按比例赠送 = 1,
            一次性赠送 = 2
        }
        public enum YesOrNo
        {
            是 = 1,
            否 = 0
        }

        /// <summary>
        /// 图片附件业务类型
        /// </summary>
        public enum AttachmentPicType
        {
            驾校 = 1,
            商城汽车 = 2,
            年检机构 = 3,
            投诉建议 = 4,
            考场 = 5,
            保险机构 = 6

        }

        /// <summary>
        /// 系统机构类型
        /// </summary>
        public enum OrganizeType
        {
            驾校机构 = 1,
            年检机构 = 2,
            保险机构 = 3,
            店铺机构 = 4,
            管理机构 = 5,
            考场机构 = 6
        }

        /// <summary>
        /// 文章适用人群类型
        /// </summary>
        public enum ToGroupType
        {
            驾校机构 = 1,
            年检机构 = 2,
            保险机构 = 3,
            店铺机构 = 4,
            管理机构 = 5,
            考场机构 = 6,

            普通会员 = 7,
            VIP会员 = 8,
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public enum PayType
        {
            微信支付 = 1,
            线下支付 = 2
        }

        /// <summary>
        /// 报名班级类型
        /// </summary>
        public enum StudyType
        {
            普通班 = 1,
            VIP班 = 2,
        }

        /// <summary>
        /// 是否带车
        /// </summary>
        public enum IsBandType
        {
            带车 = 1,
            不带车 = 0
        }

        /// <summary>
        /// 距离
        /// </summary>
        public enum DistanceRange
        {

            [Description("一千米以内")]
            一千米内 = 1,
            [Description("三千米以内")]
            一千米至两千米 = 2,
            [Description("五千米以内")]
            二千米至五千米 = 3,
            [Description("五千米以上")]
            五千米以上 = 4,
        }

        /// <summary>
        /// 价格区间
        /// </summary>
        public enum PriceRange
        {
            [Description("3000以内")]
            三千以内 = 1,
            [Description("3000-4000")]
            三千到四千 = 2,
            [Description("4000-5000")]
            四千到五千 = 3,
            [Description("5000-6000")]
            五千到六千 = 4,
            [Description("6000以上")]
            六千以上 = 5,
        }

        public enum CarPriceRange
        {
            [Description("10W以内")]
            十万以内 = 1,
            [Description("10W-20W")]
            十万到二十万 = 2,
            [Description("20W-30W")]
            二五到三十万 = 3,
            [Description("30W-50W")]
            三十到五十 = 4,
            [Description("50W以上")]
            五十万以上 = 5,
        }

        /// <summary>
        /// 消息提醒类型
        /// </summary>
        public enum MessageAlterType
        {
            学车预约提示 = 1,
            实训预约提示 = 2,
            看车预约提示 = 3,
            年检预约提示 = 4,
            代审预约提示 = 5,
            陪驾预约提示 = 6,
            保险预约提示 = 7

        }


        /// <summary>
        /// 消息通知提醒类型
        /// </summary>
        public enum NoticeType
        {
            预约提醒 = 1,
            取消提醒 = 2,
            更改提醒 = 3,
            完成通知 = 4
        }
        /// <summary>
        /// 积分操作类型
        /// </summary>
        public enum PointOperateType
        {
            增加 = 1,
            减少 = 2
        }

        /// <summary>
        /// 财务类型
        /// </summary>
        public enum FinaceOperateType
        {
            增加 = 1,
            减少 = 2
        }

        /// <summary>
        /// 财务类型
        /// </summary>
        public enum FinaceSourceType
        {
            驾校报名 = 1,
            陪驾报名 = 2,
            实训报名 = 3,
            年检预约 = 4,
            代审预约 = 5
        }

        /// <summary>
        /// 上午下午
        /// </summary>
        public enum TimeSpaceType
        {
            上午 = 1,
            下午 = 2
        }
        /// <summary>
        /// 工作时间非工作时间
        /// </summary>
        public enum TimeType
        {
            工作时间 = 1,
            休息时间 = 0
        }
        /// <summary>
        /// 上午下午晚上
        /// </summary>
        public enum WithDrivingTimeSpaceType
        {
            上午 = 1,
            下午 = 2,
            晚上 = 3
        }

        /// <summary>
        /// 教练班次类型 白班，夜班
        /// </summary>
        public enum WorkTimeType
        {
            白班 = 1,
            夜班 = 2
        }

        /// <summary>
        /// 工作日休息日
        /// </summary>
        public enum DateType
        {
            工作日 = 1,
            休息日 = 0
        }

        /// <summary>
        /// 汽车使用性质
        /// </summary>
        public enum UseType
        {
            [Description("非营运")]
            非营运 = 1,
            [Description("警车、救护")]
            警车_救护 = 2,
            [Description("营运")]
            营运 = 3,
            [Description("校车")]
            校车 = 4,
        }

        /// <summary>
        /// 汽车类型
        /// </summary>
        public enum CarType
        {
            [Description("微/小型客车")]
            微_小型客车 = 1,
            [Description("中/大型客车")]
            中_大型客车 = 2,
            [Description("轻型货车")]
            货车1 = 3,
            [Description("中/重型货车、牵引车、挂车、专项作业车")]
            货车2 = 4,
            [Description("低速车")]
            低速车 = 5,
            [Description("校车")]
            校车 = 6,
        }

        /// <summary>
        /// 实训类型
        /// </summary>
        public enum TrainingType
        {
            科目二 = 1,
            科目三 = 2
        }

        public enum IsWithDrivingType
        {
            是 = 1,
            否 = 0
        }

        /// <summary>
        /// 支付方式类型
        /// </summary>
        public enum CashType
        {
            现金 = 1,
            银行卡 = 2,
            微信 = 3,
            支付宝 = 4
        }

        /// <summary>
        /// 预约时间状态
        /// </summary>
        public enum SubritFreeTimeStatus
        {
            正常 = 1,
            自定义 = 2
        }

        /// <summary>
        /// 短信类型
        /// </summary>
        public enum SMNoticeType
        {
            年审短信 = 0,
            提示短信 = 1
        }

        /// <summary>
        /// 年检类型
        /// </summary>
        public enum AuditType
        {
            个人年检 = 0,
            集团年检 = 1,
            代检 = 2
        }
    }
}
