using iFramework.Framework;
using iFramework.Framework.Security;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Attributes;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Controllers
{
    [AuthorizeFilter(PermissionMode.Ignore)]//设置忽略验证
    public class LoginController : BaseController
    {
        public ActionResult Default()
        {

            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index1()
        {
            return View();
        }
        //
        // GET: /Account/

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult LoginFirst()
        {
            return View();
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetValidateCode()
        {
            VerifyCode vCode = new VerifyCode();
            string code = vCode.CreateVerifyCode(4);
            Session["session_verifycode"] = Md5Helper.MD5(code, 16);
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult OutLogin()
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 1;
            logEntity.OperateTypeId = ((int)OperationType.Exit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Exit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "退出系统";
            logEntity.Module = Config.GetValue("SoftName");
            LogBLL.Instance.WriteLog(logEntity);
            Session.Abandon();                                          //清除当前会话
            Session.Clear();                                            //清除当前浏览器所有Session
            WebHelper.RemoveCookie("dms_autologin");                  //清除自动登录
            //return Content(new AjaxResult { type = ResultType.success, message = "退出系统" }.ToJson());
            return Success("退出系统");
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="verifycode">验证码</param>
        /// <param name="autologin">下次自动登录</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckLogin(string username, string password, string verifycode, int autologin)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = (int)QSDMS.Model.Enums.LogCategoryEnum.登陆;
            logEntity.OperateTypeId = ((int)OperationType.Login).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Login);
            logEntity.OperateAccount = username;
            logEntity.OperateUserId = username;
            logEntity.Module = Config.GetValue("SoftName");

            try
            {
                #region 验证码验证
                if (autologin == 0)
                {
                    verifycode = Md5Helper.MD5(verifycode.ToLower(), 16);
                    if (Session["session_verifycode"].IsEmpty() || verifycode != Session["session_verifycode"].ToString())
                    {
                        throw new Exception("验证码错误，请重新输入");
                    }
                }
                #endregion

                #region 第三方账户验证
                //AccountEntity accountEntity = accountBLL.CheckLogin(username, password);
                //if (accountEntity != null)
                //{
                //    Operator operators = new Operator();
                //    operators.UserId = accountEntity.AccountId;
                //    operators.Code = accountEntity.MobileCode;
                //    operators.Account = accountEntity.MobileCode;
                //    operators.UserName = accountEntity.FullName;
                //    operators.Password = accountEntity.Password;
                //    operators.IPAddress = Net.Ip;
                //    operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                //    operators.LogTime = DateTime.Now;
                //    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                //    operators.IsSystem = true;
                //    OperatorProvider.Provider.AddCurrent(operators);
                //    //登录限制
                //    LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                //    return Success("登录成功。");
                //}
                #endregion

                #region 内部账户验证
                UserEntity userEntity = UserBLL.Instance.CheckLogin(username, password);
                if (userEntity != null)
                {
                    AuthorizeBLL authorizeBLL = new AuthorizeBLL();
                    Operator operators = new Operator();
                    operators.UserId = userEntity.UserId;
                    operators.Code = userEntity.EnCode;
                    operators.Account = userEntity.Account;
                    operators.UserName = userEntity.RealName;
                    operators.Password = userEntity.Password;
                    operators.Secretkey = userEntity.Secretkey;
                    operators.CompanyId = userEntity.OrganizeId;
                    operators.DepartmentId = userEntity.DepartmentId;
                    operators.IPAddress = Net.Ip;
                    operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                    operators.ObjectId = PermissionBLL.Instance.GetObjectStr(userEntity.UserId);
                    operators.LogTime = DateTime.Now;
                    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                    operators.Theme = userEntity.Theme == null ? "default" : userEntity.Theme;
                    operators.HeadIcon = userEntity.HeadIcon == null ? "/Content/default/img/avatar.png" : userEntity.HeadIcon;
                    operators.UserDataAuthorize = UserAuthorizeBLL.Instance.GetUserAuthorizeListStr(userEntity.UserId);
                    //写入当前用户数据权限
                    //AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
                    //dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators);
                    //dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators);
                    //dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators, true);
                    //dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators, true);
                    //operators.DataAuthorize = dataAuthorize;
                    //判断是否系统管理员
                    if (userEntity.Account == "System")
                    {
                        operators.IsSystem = true;
                    }
                    else
                    {
                        operators.IsSystem = false;
                    }
                    OperatorProvider.Provider.AddCurrent(operators);
                    //登录限制
                    //LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                    //写入日志
                    logEntity.ExecuteResult = 1;
                    logEntity.ExecuteResultJson = "登录成功";
                    LogBLL.Instance.WriteLog(logEntity);
                    //logEntity.WriteLog();
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic["theme"] = OperatorProvider.Provider.Current().Theme;
                return Success("登录成功", dic);
                #endregion
            }
            catch (Exception ex)
            {
                WebHelper.RemoveCookie("dms_autologin");                  //清除自动登录
                logEntity.ExecuteResult = -1;
                logEntity.ExecuteResultJson = ex.Message;
                LogBLL.Instance.WriteLog(logEntity);
                //Logger.Error(ex);
                return Error(ex.Message);
            }
        }

    }
}
