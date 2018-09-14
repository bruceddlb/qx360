using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{
    public class GivePointBLL
    {
        public static void GivePoint(QX360.Model.Enums.OperationType type, string accountid, double cosMoney, string orderNo)
        {
            //当前的会员对象
            var member = MemberBLL.Instance.GetEntity(accountid);
            //对应规则
            var ruleList = RuleBLL.Instance.GetList(new RuleEntity() { RuleOperation = (int)type });
            if (ruleList != null)
            {
                var rule = ruleList.FirstOrDefault();
                if (rule != null)
                {
                    double point = 0;
                    var flag = false;
                    switch (rule.RuleOperation)
                    {
                        case (int)QX360.Model.Enums.OperationType.登陆操作:
                            if (rule.MemberLevId == "-1")
                            {
                                flag = true;
                            }
                            else
                            {
                                if (rule.MemberLevId == member.LevId)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            var pointlist = PointLogBLL.Instance.GetList(new PointLogEntity() { ObjectId = QX360.Model.Enums.OperationType.登陆操作.ToString(), MemberId = accountid, StartTime = DateTime.Now.ToString("yyyy-MM-dd") });
                            if (pointlist.Count > 0)
                            {
                                flag = false;
                            }
                            point = rule.GivePoint ?? 0;
                            break;
                        default:
                            if (rule.MemberLevId == "-1")
                            {
                                flag = true;
                            }
                            else
                            {
                                if (rule.MemberLevId == member.LevId)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            if (rule.CosMoney != null)
                            {
                                if (rule.Type == 1)//按比例
                                {
                                    // point = cosMoney * double.Parse(rule.CosMoney.ToString());
                                    //flag = true;
                                }
                                else
                                {
                                    ///金额是否满足
                                    if (cosMoney >= double.Parse(rule.CosMoney.ToString()))
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                    point = rule.GivePoint ?? 0;
                                }
                            }
                            break;
                    }
                    if (flag)
                    {
                        PointLogEntity entity = new PointLogEntity();
                        entity.PointLogId = QSDMS.Util.Util.NewUpperGuid();
                        entity.ObjectId = type.ToString();
                        entity.MemberId = accountid;
                        entity.Point = int.Parse(point.ToString());
                        entity.Remark = type.ToString() + orderNo;
                        entity.AddTime = DateTime.Now;
                        entity.Operate = (int)QX360.Model.Enums.PointOperateType.增加;
                        PointLogBLL.Instance.Add(entity);

                        //修改会员积分字段
                        member.Point = member.Point + decimal.Parse(point.ToString());
                        MemberBLL.Instance.Update(member);
                    }
                }
            }
        }
    }
}
