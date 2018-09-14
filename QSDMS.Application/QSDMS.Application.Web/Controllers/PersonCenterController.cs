using iFramework.Framework.Security;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Controllers
{

    /// <summary>
    /// 个人中心
    /// </summary>
    public class PersonCenterController : BaseController
    {
        private UserBLL userBLL = new UserBLL();

        #region 视图功能
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.userId = OperatorProvider.Provider.Current().UserId;
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerifyCode()
        {
            VerifyCode vCode = new VerifyCode();
            string code = vCode.CreateVerifyCode(4);
            Session["session_verifycode"] = Md5Helper.MD5(code, 16);
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
            //return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public JsonResult UploadFile()
        {
            ReturnMessage result = new ReturnMessage(false) { Message = "上传图片失败!" };
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                //没有文件上传，直接返回
                if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
                {
                    result.Message = "无效文件";
                    return Json(result);
                    // return HttpNotFound();
                }
                string FileEextension = Path.GetExtension(files[0].FileName);
                string UserId = OperatorProvider.Provider.Current().UserId;
                string virtualPath = string.Format("/Resources/PhotoFile/{0}{1}", UserId, FileEextension);
                string fullFileName = Server.MapPath("~" + virtualPath);
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                files[0].SaveAs(fullFileName);

                UserEntity userEntity = new UserEntity();
                userEntity.UserId = OperatorProvider.Provider.Current().UserId;
                userEntity.HeadIcon = virtualPath;
                userBLL.SaveForm(userEntity.UserId, userEntity);

                result.IsSuccess = true;
                result.Message = "上传成功";
            }
            catch (Exception)
            {
            }
            return Json(result);

        }

        /// <summary>
        /// 验证旧密码
        /// </summary>
        /// <param name="OldPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidationOldPassword(string OldPassword)
        {
            OldPassword = Md5Helper.MD5(DESEncrypt.Encrypt(OldPassword.ToLower(), OperatorProvider.Provider.Current().Secretkey).ToLower(), 32).ToLower();//Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(OldPassword, 32).ToLower(), OperatorProvider.Provider.Current().Secretkey).ToLower(), 32).ToLower();
            if (OldPassword != OperatorProvider.Provider.Current().Password)
            {

                return Error("原密码错误，请重新输入");
            }
            else
            {
                return Success("通过信息验证");
            }
        }

        /// <summary>
        /// 设置系统风格
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateTheme(string theme)
        {
            try
            {
                UserBLL.Instance.UpdateEntity(new UserEntity() { UserId = OperatorProvider.Provider.Current().UserId, Theme = theme });
                return Success("系统风格设置成功,重新登陆生效!");
            }
            catch (Exception ex)
            {
                return Error("系统风格设置失败!");
            }
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateContactInfo(string data)
        {
            try
            {
                UserEntity user = Serializer.DeserializeJson<UserEntity>(data, true); //JsonConvert.DeserializeObject<tbl_product>(json);
                user.UserId = OperatorProvider.Provider.Current().UserId;
                UserBLL.Instance.UpdateEntity(user);
                return Success("修改信息成功!");
            }
            catch (Exception ex)
            {
                return Error("修改信息失败!");
            }
        }
        /// <summary>
        /// 提交修改密码
        /// </summary>
        /// <param name="userId">当前用户Id</param>
        /// <param name="password">新密码</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitResetPassword(string password, string oldPassword, string verifyCode)
        {
            verifyCode = Md5Helper.MD5(verifyCode.ToLower(), 16);
            if (Session["session_verifycode"].IsEmpty() || verifyCode != Session["session_verifycode"].ToString())
            {
                return Error("验证码错误，请重新输入");
            }
            oldPassword = Md5Helper.MD5(DESEncrypt.Encrypt(oldPassword, OperatorProvider.Provider.Current().Secretkey).ToLower(), 32).ToLower();
            if (oldPassword != OperatorProvider.Provider.Current().Password)
            {
                return Error("原密码错误，请重新输入");
            }
            userBLL.RevisePassword(OperatorProvider.Provider.Current().UserId, password.ToLower());
            Session.Abandon(); Session.Clear();
            return Success("密码修改成功，请牢记新密码。\r 将会自动安全退出。");
        }
        #endregion

    }
}
