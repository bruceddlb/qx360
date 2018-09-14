using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    public enum AppStatus
    {
        正常 = 1,
        锁定 = 2,
        删除 = 3,
    }

    public enum IPStatus
    {
        正常 = 1,
        锁定 = 2,
        删除 = 3,
    }

    public enum AccountType
    {
        系统管理员 = 0,
        店铺操作员 = 1,
        会员用户 = 2,
        商城管理员 = 3,
    }

    /// <summary>
    /// 1、入帐  2、购物  3、转帐  4、汇款  5、提款  6、调整
    /// </summary>
    public enum Operate : byte
    {
        入帐 = 0x01,
        购物 = 0x02,
        转帐 = 0x03,
        汇款 = 0x04,
        提款 = 0x05,
        调整 = 0x06
    }

    public enum ShopRemitStatus : byte
    {
        代审核 = 0x01,
        已审核 = 0x02
    }

    public enum RemitType : byte
    {
        现金 = 0x01,
        积分转入 = 0x02
    }

    public enum PayWay : byte
    {
        支付宝 = 0x01,
        银行汇款 = 0x02,
        线下支付 = 0x03,
        微信钱包 = 0x04,
        积分换购 = 0x05
    }

    public enum ProductStatus
    {
        上架 = 1,
        下架 = 2
    }

    public enum AuditStatus
    {
        未审核 = 1,
        已审核 = 2
    }
    public enum OrderStatus : int
    {
        待支付 = 1,
        待发货 = 2,
        待收货 = 3,
        交易成功 = 4,
        交易取消 = 5,
        计划中 = 6
    }

    public enum AddressStatus : int
    {
        正常 = 1,
        已移除 = 2
    }

    public enum IsUploadImage : byte
    {
        No = 0x01,
        Yes = 0x02
    }

    /// <summary>
    /// 订单来源
    /// </summary>
    public enum OrderSource : byte
    {
        商城订单 = 0x01,
        零售配货 = 0x02,
        其他 = 0x03,
        老系统订单 = 0x04

    }

    /// <summary>
    /// 销售单状态
    /// </summary>
    public enum SaleOrderStatus : byte
    {
        计划中 = 0x01,
        订单确认 = 0x02,
        包装出库 = 0x03,
        物流配送 = 0x04
    }

    /// <summary>
    /// 出库类型
    /// </summary>
    public enum ProductOutType : byte
    {
        正常 = 0x01,
        其他 = 0x02
    }
    /// <summary>
    /// 入库类型
    /// </summary>
    public enum ProductInType : byte
    {
        正常 = 0x01,
        其他 = 0x02
    }
    /// <summary>
    /// 出库状态
    /// </summary>
    public enum ProductOutStatus : byte
    {
        制单 = 1,
        已审核 = 2,
        已出货 = 3
    }


    /// <summary>
    /// 入库状态
    /// </summary>
    public enum ProductInStatus : byte
    {
        制单 = 1,
        已审核 = 2,
        已入库 = 3
    }

    /// <summary>
    /// 采购状态
    /// </summary>
    public enum PrchOrderStatus : byte
    {
        制单 = 1,
        已审核 = 2
    }

    /// <summary>
    /// 商铺状态
    /// </summary>
    public enum BusinissStatus : byte
    {
        正常 = 1,
        已删除 = 2
    }

    /// <summary>
    /// 店铺配货订单状态
    /// </summary>
    public enum ShopOrderStatus
    {
        申请 = 0x01,
        已审核发货 = 0x02
    }

    /// <summary>
    ///退货状态
    /// </summary>
    public enum ReturnStatus : byte
    {
        制单 = 0x01,
        已审核 = 0x02
    }

    public enum StockCheckStatus : byte
    {
        制单 = 0x01,
        已审核 = 0x02
    }

    public enum RemitStatus : byte
    {
        待支付 = 0x01,
        待汇款 = 0x02,
        代审核 = 0x03,
        已汇款 = 0x04
    }

    public enum SuitStatus : byte
    {
        正常 = 0x01,
        已删除 = 0x02
    }

    public enum BaseStatus : byte
    {
        正常 = 0x01,
        已删除 = 0x02
    }

    public enum ImageType : byte
    {
        Product = 0x01,
        MemberRemit = 0x02,
        ShopRemit = 0x03,
        Certificate = 0x04,
        Advertising = 0x05,
        HeadPortrait = 0x06,
        AccountCode=0x07,
        Lottery = 0x08,
    }

    public enum RuleField
    {
        现金 = 0x01,
        商城积分 = 0x02,
        奖金积分 = 0x03,
        赠送积分 = 0x04,
        奖金_赠送积分 = 0x05,
        原始积分 = 0x06
    }

    public enum PayStatus
    {
        未扣款 = 0x01,
        已扣款 = 0x02
    }

    public enum ProductSizes
    {
        Size100 = 0x01,
        Size300 = 0x02,
        Size640 = 0x03
    }

    public enum AccountStatus : byte
    {
        锁定 = 0x00,
        正常 = 0x01
    }

    public enum CashPayway
    {
        支付宝 = 0x01,
        银行汇款 = 0x02,
    }

    public enum YesNO
    {
        是 = 1,
        否 = 0
    }

    public enum PurseOperate
    {
        无 = 0x00,
        查看明细 = 0x01,
        提现 = 0x02,
        转账=0x03
    }

    public enum ConsultingType 
    {
        商品咨询=0x01,   
        库存及配送=0x02,
        支付问题=0x03,
        发票及保修=0x04,
        促销及赠品=0x05
    }

    public enum LogType 
    {
        登录=0x01
    }

    public enum ShopType 
    {
        专卖店=0x01,
        专柜=0x02,
    }
}
