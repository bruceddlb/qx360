using QSDMS.Cache.Factory;
using QSDMS.Util;
using System;
using iFramework.Framework.Security;
namespace QSDMS.Model
{ 
    /// <summary>   
    /// 描 述：当前操作者回话
    /// </summary>
    public class OperatorProvider : OperatorIProvider
    {
        #region 静态实例
        /// <summary>
        /// 当前提供者
        /// </summary>
        public static OperatorIProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        /// <summary>
        /// 给app调用
        /// </summary>
        public static string AppUserId
        {
            set;
            get;
        }
        #endregion

        /// <summary>
        /// 秘钥
        /// </summary>
        public static string LoginCookieIdentityUserKey = "QSDMS_LoginUserKey_2016";
        /// <summary>
        /// 登陆提供者模式:Session、Cookie 
        /// </summary>
        private string LoginProvider = Config.GetValue("LoginProvider");
        /// <summary>
        /// 写入登录信息
        /// </summary>
        /// <param name="user">成员信息</param>
        public virtual void AddCurrent(Operator user)
        {
            try
            {
                if (LoginProvider == "Cookie")
                {
                    WebHelper.WriteCookie(LoginCookieIdentityUserKey, DESEncrypt.Encrypt(user.ToJson()));
                }
                else
                {
                    WebHelper.WriteSession(LoginCookieIdentityUserKey, DESEncrypt.Encrypt(user.ToJson()));
                }
                CacheFactory.Cache().WriteCache(user.Token, user.UserId, user.LogTime.AddHours(12));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public virtual Operator Current()
        {
            try
            {
                Operator user = new Operator();
                if (LoginProvider == "Cookie")
                {
                    user = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginCookieIdentityUserKey).ToString()).ToObject<Operator>();
                }
                else if (LoginProvider == "AppClient")
                {
                    user = CacheFactory.Cache().GetCache<Operator>(AppUserId);
                }
                else
                {
                    user = DESEncrypt.Decrypt(WebHelper.GetSession(LoginCookieIdentityUserKey).ToString()).ToObject<Operator>();
                }
                return user;
            }
            catch (Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 删除登录信息
        /// </summary>
        public virtual void EmptyCurrent()
        {
            if (LoginProvider == "Cookie")
            {
                WebHelper.RemoveCookie(LoginCookieIdentityUserKey.Trim());
            }
            else
            {
                WebHelper.RemoveSession(LoginCookieIdentityUserKey.Trim());
            }
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOverdue()
        {
            try
            {
                object str = "";
                if (LoginProvider == "Cookie")
                {
                    str = WebHelper.GetCookie(LoginCookieIdentityUserKey);
                }
                else
                {
                    str = WebHelper.GetSession(LoginCookieIdentityUserKey);
                }
                if (str != null && str.ToString() != "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
        /// <summary>
        /// 是否已登录
        /// </summary>
        /// <returns></returns>
        public virtual int IsOnLine()
        {
            Operator user = new Operator();
            if (LoginProvider == "Cookie")
            {
                user = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginCookieIdentityUserKey).ToString()).ToObject<Operator>();
            }
            else
            {
                user = DESEncrypt.Decrypt(WebHelper.GetSession(LoginCookieIdentityUserKey).ToString()).ToObject<Operator>();
            }
            object token = CacheFactory.Cache().GetCache<string>(user.UserId);
            if (token == null)
            {
                return -1;//过期
            }
            if (user.Token == token.ToString())
            {
                return 1;//正常
            }
            else
            {
                return 0;//已登录
            }
        }
    }
}
