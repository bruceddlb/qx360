using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using iFramework.Framework.Log;

namespace iFramework.Framework
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public class ExceptionHelper
    {
        #region 私有变量

        /// <summary>
        /// 异常内容
        /// </summary>
        private string m_exceptionContent = string.Empty;

        /// <summary>
        /// 异常类型
        /// </summary>
        private string m_exceptionType = string.Empty;

        /// <summary>
        /// 用于获取配置信息的名值集合
        /// </summary>
        private NameValueCollection m_resultCollection = new NameValueCollection();

        /// <summary>
        /// ViewState
        /// </summary>
        private string m_viewState = string.Empty;

        /// <summary>
        /// ViewStateKey
        /// </summary>
        private const string m_viewStateKey = "__VIEWSTATE";

        /// <summary>
        /// 异常基类
        /// </summary>
        private const string m_rootException = "System.Web.HttpUnhandledException";

        /// <summary>
        /// 是否记录cache
        /// </summary>
        private bool m_iscahce = false;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iscache"></param>
        public ExceptionHelper(bool iscache = false)
        {
            m_iscahce = iscache;
        }

        #region 异常处理
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">要处理的异常</param>
        public void LogException(Exception e)
        {
            try
            {
                //取出对应的异常内容
                m_exceptionContent = ExceptionToString(e);

                //异常类型
                m_exceptionType = e.GetType().FullName;

                if (m_exceptionType == m_rootException)
                {
                    if (!(e.InnerException == null))
                    {
                        m_exceptionType = e.InnerException.GetType().FullName;
                    }
                }
            }
            catch (Exception ee)
            {
                m_exceptionContent = "Error '" + ee.Message + "' while generating exception string";
            }

            try
            {
                //记录日志到Log文件
                ExceptionToFile();
            }
            catch
            {
            }
        }

        #endregion

        #region 获取堆栈信息
        /// <summary>
        /// 获取堆栈信息
        /// </summary>
        /// <param name="sfInfo">堆栈框架</param>
        /// <returns>数据</returns>
        private string StackFrameToString(StackFrame sfInfo)
        {
            StringBuilder sb = new StringBuilder();
            int intParam;
            MemberInfo mi = sfInfo.GetMethod();

            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);

            //build method params
            sb.Append("(");
            intParam = 0;
            foreach (ParameterInfo param in sfInfo.GetMethod().GetParameters())
            {
                intParam += 1;
                if (intParam > 1)
                {
                    sb.Append(", ");
                }
                sb.Append(param.ParameterType.FullName);
                sb.Append(" ");
                sb.Append(param.Name);
            }
            sb.Append(")");
            sb.Append(Environment.NewLine);

            //-- if source code is available, append location info
            sb.Append("       ");
            if ((sfInfo.GetFileName() == null) || (sfInfo.GetFileName().Length == 0))
            {
                sb.Append("(unknown file)");
                //-- native code offset is always available
                sb.Append(": N ");
                sb.Append(String.Format("{0:#00000}", sfInfo.GetNativeOffset()));
            }
            else
            {
                sb.Append(Path.GetFileName(sfInfo.GetFileName()));
                sb.Append(": line ");
                sb.Append(String.Format("{0:#0000}", sfInfo.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(String.Format("{0:#00}", sfInfo.GetFileColumnNumber()));
                //-- if IL is available, append IL location info
                if (sfInfo.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(String.Format("{0:#0000}", sfInfo.GetILOffset()));
                }
            }

            sb.Append(Environment.NewLine);

            return sb.ToString();
        }
        #endregion

        #region 获取堆栈信息
        /// <summary>
        /// 获取Aspnet程序未处理异常的堆栈信息
        /// </summary>
        /// <returns>返回值</returns>
        private string EnhancedStackTrace()
        {
            //返回当前
            return EnhancedStackTrace(new StackTrace(true), "ASPUnhandledException");
        }
        #endregion

        #region 获取堆内容
        /// <summary>
        /// 获取堆内容
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns>属性</returns>
        private string EnhancedStackTrace(Exception e)
        {
            //调用其重载方法
            return EnhancedStackTrace(new StackTrace(e, true), string.Empty);
        }
        #endregion

        #region 取堆内容
        /// <summary>
        /// 取堆内容
        /// </summary>
        /// <param name="stTrace">堆栈</param>
        /// <param name="strSkipClassName">跳过的类名</param>
        /// <returns>属性</returns>
        private string EnhancedStackTrace(StackTrace stTrace, string strSkipClassName)
        {
            //准备返回的字符串
            StringBuilder sbInfo = new StringBuilder();

            //添加信息
            sbInfo.Append(Environment.NewLine);
            sbInfo.Append("---- Stack Trace ----");
            sbInfo.Append(Environment.NewLine);

            //遍历
            for (int iFrame = 0; iFrame < stTrace.FrameCount - 1; iFrame++)
            {
                //获取堆栈内容
                StackFrame sfStack = stTrace.GetFrame(iFrame);

                //成员
                MemberInfo miInfo = sfStack.GetMethod();

                //如果类名不为空,并且到了堆栈的堆栈最后
                if ((strSkipClassName != string.Empty) && (miInfo.DeclaringType.Name.IndexOf(strSkipClassName) > -1))
                {

                }
                else
                {
                    sbInfo.Append(StackFrameToString(sfStack));
                }
            }

            //加一行
            sbInfo.Append(Environment.NewLine);

            //返回
            return sbInfo.ToString();
        }
        #endregion

        #region 取出对应的异常内容
        /// <summary>
        /// 取出对应的异常内容
        /// </summary>
        /// <param name="e">要处理的异常类</param>
        /// <returns>对应的异常内容</returns>
        private string ExceptionToString(Exception e)
        {
            //保存异常内容
            StringBuilder sbException = new StringBuilder();

            //添加内容
            sbException.Append(ExceptionToStringPrivate(e, true));

            try
            {
                //获取当前web程序的设置信息
                if (HttpContext.Current != null)
                {
                    sbException.Append(GetASPSettings());
                }
            }
            catch (Exception ee)
            {
                //直接写错误
                sbException.Append(ee.Message);
            }

            //加一行
            sbException.Append(Environment.NewLine);

            //返回异常内容
            return sbException.ToString();
        }
        #endregion

        #region 取出异常和上下文以及系统环境
        /// <summary>
        /// 取出异常和上下文以及系统环境
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isIncludeSysInfo">是否需要包含系统信息</param>
        /// <returns>异常信息</returns>
        private string ExceptionToStringPrivate(Exception e, bool isIncludeSysInfo)
        {
            //用于保存异常信息的stringbulider
            StringBuilder sbErrorInfo = new StringBuilder();

            //如果当前的异常对象不为空
            if (e.InnerException != null)
            {
                //如果当前异常的类型和基类型一致
                if (e.GetType().ToString() == m_rootException)
                {
                    //递归调用
                    return ExceptionToStringPrivate(e.InnerException, true);
                }
                else
                {
                    //否则添加异常信息
                    sbErrorInfo.Append(ExceptionToStringPrivate(e.InnerException, false));
                    sbErrorInfo.Append(Environment.NewLine);
                    sbErrorInfo.Append("(Outer Exception)");
                    sbErrorInfo.Append(Environment.NewLine);
                }
            }

            //如果需要获取系统信息
            if (isIncludeSysInfo)
            {
                //添加系统信息到异常信息
                sbErrorInfo.Append(SysInfoToString(false));
                sbErrorInfo.Append(AssemblyInfoToString(e));
                sbErrorInfo.Append(Environment.NewLine);
            }

            //异常类型
            sbErrorInfo.Append("Exception Type:         ");
            try
            {
                sbErrorInfo.Append(e.GetType().FullName);
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);

            //异常内容
            sbErrorInfo.Append("Exception Message:         ");
            try
            {
                sbErrorInfo.Append(e.Message);
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);

            //异常数据
            sbErrorInfo.Append("Exception Data:         ");
            try
            {
                if (e.Data != null)
                {
                    sbErrorInfo.Append(JsonConvert.SerializeObject(e.Data));
                }
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);


            //异常源
            sbErrorInfo.Append("Exception Source:         ");
            try
            {
                sbErrorInfo.Append(e.Source);
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);

            //异常目标站点
            sbErrorInfo.Append("Exception Target Site:         ");
            try
            {
                sbErrorInfo.Append(e.TargetSite.Name);
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);

            //堆内容
            try
            {
                sbErrorInfo.Append(EnhancedStackTrace(e));
            }
            catch (Exception ee)
            {
                sbErrorInfo.Append(ee.Message);
            }
            sbErrorInfo.Append(Environment.NewLine);

            //返回内容
            return sbErrorInfo.ToString();
        }
        #endregion

        #region 获取系统信息
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        private string SysInfoToString(bool includeStackTrace)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Date and Time:         ");
            sb.Append(DateTime.Now);
            sb.Append(Environment.NewLine);

            sb.Append("Machine Name:          ");
            try
            {
                sb.Append(Environment.MachineName);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }
            sb.Append(Environment.NewLine);

            sb.Append("Process User:          ");
            sb.Append(ProcessIdentity());
            sb.Append(Environment.NewLine);
            if (HttpContext.Current != null)
            {
                sb.Append("Remote User:           ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_USER"]);
                sb.Append(Environment.NewLine);

                sb.Append("Remote Address:        ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                sb.Append(Environment.NewLine);

                sb.Append("Remote Host:           ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]);
                sb.Append(Environment.NewLine);

                sb.Append("URL:                   ");
                sb.Append(WebCurrentUrl());
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            sb.Append("NET Runtime version:   ");
            sb.Append(Environment.Version.ToString());
            sb.Append(Environment.NewLine);

            sb.Append("Application Domain:    ");
            try
            {
                sb.Append(AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }
            sb.Append(Environment.NewLine);

            if (includeStackTrace)
            {
                sb.Append(EnhancedStackTrace());
            }

            return sb.ToString();
        }
        #endregion

        #region 获取用户ID
        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <returns>ID</returns>
        private string ProcessIdentity()
        {
            //当前系统ID
            string strTemp = CurrentWindowsIdentity();

            //如果空
            if (strTemp == string.Empty)
            {
                //获取用户名
                strTemp = CurrentEnvironmentIdentity();
            }

            //返回
            return strTemp;
        }
        #endregion

        #region WindowsID
        /// <summary>
        /// WindowsID
        /// </summary>
        /// <returns>WindowsID</returns>
        private string CurrentWindowsIdentity()
        {
            try
            {
                //返回对象的值
                return WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region 用户名
        /// <summary>
        /// 用户名
        /// </summary>
        /// <returns>用户名</returns>
        private string CurrentEnvironmentIdentity()
        {
            try
            {
                //域和用户名
                return Environment.UserDomainName + "\\" + Environment.UserName;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region 当前URL
        /// <summary>
        /// 当前URL
        /// </summary>
        /// <returns>URL</returns>
        private string WebCurrentUrl()
        {
            //URL
            string strUrl = string.Empty;

            if (HttpContext.Current != null)
            {
                //服务器名
                strUrl = "http://" + HttpContext.Current.Request.ServerVariables.Get("server_name");

                //如果不是80端口
                if (HttpContext.Current.Request.ServerVariables.Get("server_port") != "80")
                {
                    //获取端口
                    strUrl += ":" + HttpContext.Current.Request.ServerVariables.Get("server_port");
                }

                //获取相对url
                strUrl += HttpContext.Current.Request.ServerVariables.Get("url");

                //如果有查询字符串
                if (HttpContext.Current.Request.ServerVariables.Get("query_string") != null &&
                    HttpContext.Current.Request.ServerVariables.Get("query_string").Length > 0)
                {
                    //添加查询字符串
                    strUrl += "?" + HttpContext.Current.Request.ServerVariables.Get("query_string");
                }
            }

            //返回
            return strUrl;
        }
        #endregion

        #region 获取程序集信息
        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns>程序集信息</returns>
        private string AssemblyInfoToString(Exception e)
        {
            //发生异常的程序集
            Assembly aCurrent = GetAssemblyFromName(e.Source);

            //如果获取到了
            if (aCurrent != null)
            {
                //返回系统中当前程序域所有属性信息
                return AllAssemblyDetailsToString();
            }
            else
            {
                //只返回当前程序集的属性信息
                return AssemblyDetailsToString(aCurrent);
            }
        }
        #endregion

        #region 通过程序集名称获取程序集实例
        /// <summary>
        /// 通过程序集名称获取程序集实例
        /// </summary>
        /// <param name="strAssemblyName">程序集名称</param>
        /// <returns>程序集实例</returns>
        private Assembly GetAssemblyFromName(string strAssemblyName)
        {

            //遍历当程序域的程序集
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                //如果获取到相同名称的程序集
                if (a.GetName().Name == strAssemblyName)
                {
                    //设置为当前程序集
                    return a;
                }
            }

            //返回
            return null;
        }
        #endregion

        #region 获取当前系统中所有的程序集信息
        /// <summary>
        /// 获取当前系统中所有的程序集信息
        /// </summary>
        /// <returns>当前系统中所有的程序集信息</returns>
        private string AllAssemblyDetailsToString()
        {
            //用于搜集信息的stringbuilder对象
            StringBuilder sbAssemblyInfo = new StringBuilder();

            //一个名-值对象,用于搜集程序信息
            NameValueCollection nvc = new NameValueCollection();

            //程序集信息格式
            const string strLineFormat = "    {0, -30} {1, -15} {2}";

            //添加
            sbAssemblyInfo.Append(Environment.NewLine);
            sbAssemblyInfo.Append(string.Format(strLineFormat, "Assembly", "Version", "BuildDate"));
            sbAssemblyInfo.Append(Environment.NewLine);
            sbAssemblyInfo.Append(string.Format(strLineFormat, "--------", "-------", "---------"));
            sbAssemblyInfo.Append(Environment.NewLine);

            //遍历当前域中所有的程序对象
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                //取出程序集的属性
                nvc = AssemblyAttribs(a);

                //如果程序集的版本号不为0.0.0.0,即,程序集是发布版本
                if (nvc["Version"] != "0.0.0.0")
                {
                    //添加版本信息
                    sbAssemblyInfo.Append(string.Format(strLineFormat, Path.GetFileName(nvc["CodeBase"]), nvc["Version"], nvc["BuildDate"]));
                    sbAssemblyInfo.Append(Environment.NewLine);
                }
            }

            //返回字符串
            return sbAssemblyInfo.ToString();
        }
        #endregion

        #region 获取程序集信息
        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <param name="aCurrent">程序集</param>
        /// <returns>属性信息</returns>
        private string AssemblyDetailsToString(Assembly aCurrent)
        {
            //用于获取程序集信息的stringbuilder对象
            StringBuilder sbAttrib = new StringBuilder();

            //名-值对象,程序集的属性信息
            NameValueCollection nvcAttrib = AssemblyAttribs(aCurrent);

            //添加之
            sbAttrib.Append("Assembly Codebase:     ");

            try
            {
                sbAttrib.Append(nvcAttrib["CodeBase"]);
            }
            catch (Exception e)
            {
                sbAttrib.Append(e.Message);
            }

            sbAttrib.Append(Environment.NewLine);

            sbAttrib.Append("Assembly Full Name:     ");

            try
            {
                sbAttrib.Append(nvcAttrib["FullName"]);
            }
            catch (Exception e)
            {
                sbAttrib.Append(e.Message);
            }

            sbAttrib.Append(Environment.NewLine);

            sbAttrib.Append("Assembly Version:     ");

            try
            {
                sbAttrib.Append(nvcAttrib["Version"]);
            }
            catch (Exception e)
            {
                sbAttrib.Append(e.Message);
            }

            sbAttrib.Append(Environment.NewLine);

            sbAttrib.Append("Assembly Build Date:     ");

            try
            {
                sbAttrib.Append(nvcAttrib["BuildDate"]);
            }
            catch (Exception e)
            {
                sbAttrib.Append(e.Message);
            }

            //返回
            return sbAttrib.ToString();
        }
        #endregion

        #region 获取程序集对象的属性
        /// <summary>
        /// 获取程序集对象的属性
        /// </summary>
        /// <param name="aGetInfo">程序集</param>
        /// <returns>名-值集合</returns>
        private NameValueCollection AssemblyAttribs(Assembly aGetInfo)
        {
            //名称
            string strName;

            //值
            string strValue;

            //名-值集合
            NameValueCollection nvcAttribInfo = new NameValueCollection();

            try
            {
                //遍历其中的Object
                foreach (Object o in aGetInfo.GetCustomAttributes(false))
                {
                    //获取属性名称
                    strName = o.GetType().ToString();

                    //获取属性值
                    strValue = string.Empty;

                    switch (strName)
                    {
                        case "System.Diagnostics.DebuggableAttribute":
                            strName = "Debuggable";
                            strValue = ((DebuggableAttribute)o).IsJITTrackingEnabled.ToString();

                            break;

                        case "System.CLSCompliantAttribute":
                            strName = "CLSCompliant";
                            strValue = ((CLSCompliantAttribute)o).IsCompliant.ToString();

                            break;

                        case "System.Runtime.InteropServices.GuidAttribute":
                            strName = "GUID";
                            strValue = ((GuidAttribute)o).Value.ToString();

                            break;

                        case "System.Reflection.AssemblyTrademarkAttribute":
                            strName = "Trademark";
                            strValue = ((AssemblyTrademarkAttribute)o).Trademark.ToString();

                            break;

                        case "System.Reflection.AssemblyProductAttribute":
                            strName = "Product";
                            strValue = ((AssemblyProductAttribute)o).Product.ToString();

                            break;

                        case "System.Reflection.AssemblyCopyrightAttribute":
                            strName = "Copyright";
                            strValue = ((AssemblyCopyrightAttribute)o).Copyright.ToString();

                            break;

                        case "System.Reflection.AssemblyCompanyAttribute":
                            strName = "Company";
                            strValue = ((AssemblyCompanyAttribute)o).Company.ToString();

                            break;

                        case "System.Reflection.AssemblyTitleAttribute":
                            strName = "Title";
                            strValue = ((AssemblyTitleAttribute)o).Title.ToString();

                            break;

                        case "System.Reflection.AssemblyDescriptionAttribute":
                            strName = "Description";
                            strValue = ((AssemblyDescriptionAttribute)o).Description.ToString();

                            break;
                    }

                    //如果获取到值
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        //并且名-值集合中的该Kay为空
                        if (string.IsNullOrEmpty(nvcAttribInfo.Get(strName)))
                        {
                            //增加该记录
                            nvcAttribInfo.Add(strName, strValue);
                        }
                    }
                }

                //添加其他相关信息
                nvcAttribInfo.Add("CodeBase", aGetInfo.CodeBase.Replace("file:///", ""));
                nvcAttribInfo.Add("BuildDate", AssemblyBuildDate(aGetInfo, false).ToString());
                nvcAttribInfo.Add("Version", aGetInfo.GetName().Version.ToString());
                nvcAttribInfo.Add("FullName", aGetInfo.FullName);
            }
            catch { }

            //返回集合
            return nvcAttribInfo;
        }
        #endregion

        #region 程序集Build时间
        /// <summary>
        /// 程序集Build时间
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <param name="isForceFileDate">是否为主文件时间</param>
        /// <returns>build时间</returns>
        private DateTime AssemblyBuildDate(Assembly ass, bool isForceFileDate)
        {
            //获取当前程序集的版本对象
            Version vCurrent = ass.GetName().Version;

            //Build时间
            DateTime dtBuild;

            //如果是主文件
            if (isForceFileDate)
            {
                //写上次写的时间
                dtBuild = AssemblyLastWriteTime(ass);
            }
            else
            {
                //否则获取时间
                dtBuild = (Convert.ToDateTime("01/01/2000")).AddDays(vCurrent.Build).AddSeconds(vCurrent.Revision * 2);

                //如果时区不同
                if (TimeZone.IsDaylightSavingTime(dtBuild, TimeZone.CurrentTimeZone.GetDaylightChanges(dtBuild.Year)))
                {
                    //添加一个小时
                    dtBuild = dtBuild.AddHours(1);
                }

                //如果时间比现在大或者build号大于730,或者版本号为0
                if ((dtBuild > DateTime.Now) || (vCurrent.Build < 730) || (vCurrent.Revision == 0))
                {
                    //获取文件最后的写时间
                    dtBuild = AssemblyLastWriteTime(ass);
                }
            }

            //返回时间
            return dtBuild;
        }
        #endregion

        #region 程序集最后修改时间
        /// <summary>
        /// 程序集最后写时间
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <returns>最后修改时间</returns>
        private DateTime AssemblyLastWriteTime(Assembly ass)
        {
            try
            {
                //返回最后的时间
                return File.GetLastWriteTime(ass.Location);
            }
            catch
            {
                //异常则返回时间的极限
                return DateTime.MaxValue;
            }
        }
        #endregion

        #region 获取当前网站设置
        /// <summary>
        /// 获取当前网站设置
        /// </summary>
        /// <returns>设置文本</returns>
        private string GetASPSettings()
        {
            //用于获取当前设置的stringbuilder对象
            StringBuilder sbSettings = new StringBuilder();

            //常量
            const string strSuppressKeyPattern = "^ALL_HTTP|^ALL_RAW|VSDEBUGGER";

            //添加字符串
            sbSettings.Append("---- ASP.NET Collections ----");
            sbSettings.Append(Environment.NewLine);
            sbSettings.Append(Environment.NewLine);
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Request.QueryString, "QueryString", false, ""));
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Request.Form, "Form", false, ""));
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Request.Cookies));
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Session));

            if (m_iscahce)
            {
                sbSettings.Append(HttpVarsToString(HttpContext.Current.Cache));
            }
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Application));
            sbSettings.Append(HttpVarsToString(HttpContext.Current.Request.ServerVariables, "ServerVariables", true, strSuppressKeyPattern));

            //返回
            return sbSettings.ToString();
        }
        #endregion

        #region 获取系统Cookie的值
        /// <summary>
        /// 获取系统Cookie的值
        /// </summary>
        /// <param name="cookie">Cookie</param>
        /// <returns>描述</returns>
        private string HttpVarsToString(HttpCookieCollection cookie)
        {
            //用于保存字符串的stringbuilder对象
            StringBuilder sbCookie = new StringBuilder();

            //如果没有cookie
            if (cookie.Count == 0)
            {
                //添加空串
                sbCookie.Append(string.Empty);
            }
            else
            {
                //添加信息
                sbCookie.Append("Cookies");
                sbCookie.Append(Environment.NewLine);
                sbCookie.Append(Environment.NewLine);

                //遍历
                foreach (string s in cookie)
                {
                    //添加信息
                    AppendLine(sbCookie, s, cookie.Get(s).Value);
                }

                //加一行
                sbCookie.Append(Environment.NewLine);
            }

            //返回
            return sbCookie.ToString();
        }
        #endregion

        #region 获取State
        /// <summary>
        /// 获取State
        /// </summary>
        /// <param name="state">HttpApplicationState</param>
        /// <returns>HttpApplicationState</returns>
        private string HttpVarsToString(HttpApplicationState state)
        {
            //用于获取信息的StringBuilder
            StringBuilder sbInfo = new StringBuilder();

            //如果为0
            if (state.Count == 0)
            {
                //空串
                sbInfo.Append(string.Empty);
            }
            else
            {
                sbInfo.Append("Application");
                sbInfo.Append(Environment.NewLine);
                sbInfo.Append(Environment.NewLine);

                //对应的
                foreach (string s in state)
                {
                    AppendLine(sbInfo, s, state.Get(s));
                }

                //再加一行
                sbInfo.Append(Environment.NewLine);
            }

            //返回
            return sbInfo.ToString();
        }
        #endregion

        #region 获取缓存
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <returns>缓存信息</returns>
        private string HttpVarsToString(Cache cache)
        {
            //保存信息的stringbuilder对象
            StringBuilder sbInfo = new StringBuilder();

            //如果为0
            if (cache.Count == 0)
            {
                //空
                sbInfo.Append(string.Empty);
            }
            else
            {
                sbInfo.Append("Cache");
                sbInfo.Append(Environment.NewLine);
                sbInfo.Append(Environment.NewLine);

                //遍历
                foreach (DictionaryEntry de in cache)
                {
                    AppendLine(sbInfo, Convert.ToString(de.Key), de.Value);
                }

                sbInfo.Append(Environment.NewLine);
            }

            //返回
            return sbInfo.ToString();
        }
        #endregion

        #region 获取Session
        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>信息</returns>
        private string HttpVarsToString(HttpSessionState session)
        {
            //保存信息的StringBuilder对象
            StringBuilder sbInfo = new StringBuilder();

            //如果空
            if ((session == null) || (session.Count == 0))
            {
                return string.Empty;
            }
            else
            {
                sbInfo.Append("Session");
                sbInfo.Append(Environment.NewLine);
                sbInfo.Append(Environment.NewLine);

                //遍历
                foreach (string s in session)
                {
                    AppendLine(sbInfo, s, session.Contents[s]);
                }

                //添加一行
                sbInfo.Append(Environment.NewLine);
            }

            //返回
            return sbInfo.ToString();
        }
        #endregion

        #region 取其他信息
        /// <summary>
        /// 取其他信息
        /// </summary>
        /// <param name="nvcInfo">名-值集合</param>
        /// <param name="strTitle">Title</param>
        /// <param name="isSuppressEmpty">是否允许空</param>
        /// <param name="strSuppressKeyPattern">SuppressKeyPattern</param>
        /// <returns>值</returns>
        private string HttpVarsToString(NameValueCollection nvcInfo, string strTitle, bool isSuppressEmpty, string strSuppressKeyPattern)
        {
            //用于返回的实例
            StringBuilder sbString = new StringBuilder();

            //如果没值
            if (!nvcInfo.HasKeys())
            {
                //空
                sbString.Append(string.Empty);
            }
            else
            {
                sbString.Append(strTitle);
                sbString.Append(Environment.NewLine);
                sbString.Append(Environment.NewLine);

                bool isDisplay;

                //遍历
                foreach (string s in nvcInfo)
                {
                    isDisplay = true;

                    if (isSuppressEmpty)
                    {
                        isDisplay = (nvcInfo.Get(s) != string.Empty);
                    }

                    if (s == m_viewStateKey)
                    {
                        m_viewState = nvcInfo.Get(s);

                        isDisplay = false;
                    }

                    if (isDisplay && (strSuppressKeyPattern != ""))
                    {
                        isDisplay = !Regex.IsMatch(s, strSuppressKeyPattern);
                    }

                    if (isDisplay)
                    {
                        AppendLine(sbString, s, nvcInfo.Get(s));
                    }
                }

                //加一行
                sbString.Append(Environment.NewLine);
            }

            //返回
            return sbString.ToString();
        }
        #endregion

        #region 添加行
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sbInfo">准备修改的StringBuilder对象</param>
        /// <param name="strKey">Key</param>
        /// <param name="oValue">Value</param>
        private void AppendLine(StringBuilder sbInfo, string strKey, object oValue)
        {
            //值
            string strValue;

            //如果传入的是空
            if (oValue == null)
            {
                //返回空
                strValue = "(Nothing)";
            }
            else
            {
                try
                {
                    //否则转换值
                    strValue = oValue.ToString();
                }
                catch
                {
                    strValue = "(" + oValue.GetType().ToString() + ")";
                }
            }

            //添加行
            AppendLine(sbInfo, strKey, strValue);
        }
        #endregion

        #region 添加行
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sbInfo">准备修改的Stringbuilder对象</param>
        /// <param name="strKey">Key</param>
        /// <param name="strValue">Value</param>
        private void AppendLine(StringBuilder sbInfo, string strKey, string strValue)
        {
            sbInfo.Append(String.Format("    {0, -30}{1}", strKey, strValue));
            sbInfo.Append(Environment.NewLine);
        }
        #endregion

        #region 格式化错误信息
        /// <summary>
        /// 格式化错误信息
        /// </summary>
        /// <returns>按照用户的格式格式化异常信息</returns>
        private string FormatExceptionForUser()
        {
            StringBuilder sbError = new StringBuilder();

            string strBullet = "?";

            sbError.Append(Environment.NewLine);
            sbError.Append("The following information about the error was automatically captured: ");
            sbError.Append(Environment.NewLine);
            sbError.Append(Environment.NewLine);

            //记录到系统日志
            sbError.Append(" ");
            sbError.Append(strBullet);
            sbError.Append(" ");
            if (m_resultCollection.Get("LogToEventLog") == "")
            {
                sbError.Append("an event was written to the application log");
            }
            else
            {
                sbError.Append("an event could NOT be written to the application log due to an error:");
                sbError.Append(Environment.NewLine);
                sbError.Append("   '");
                sbError.Append(m_resultCollection.Get("LogToEventLog"));
                sbError.Append("'");
                sbError.Append(Environment.NewLine);
            }

            sbError.Append(Environment.NewLine);

            sbError.Append(Environment.NewLine);
            sbError.Append(Environment.NewLine);
            sbError.Append("Detailed error information follows:");
            sbError.Append(Environment.NewLine);
            sbError.Append(Environment.NewLine);
            sbError.Append(m_exceptionContent);

            //返回
            return sbError.ToString();
        }
        #endregion

        #region 记录Log
        /// <summary>
        /// 记录Log
        /// </summary>
        /// <returns>是否记录成功</returns>
        private bool ExceptionToEventLog()
        {
            //是否成功
            bool isSuccess = false;

            try
            {
                //记录到系统日志
                EventLog.WriteEntry(WebCurrentUrl(), Environment.NewLine + m_exceptionContent, EventLogEntryType.Error);

                //成功
                isSuccess = true;
            }
            catch (Exception e)
            {
                m_resultCollection.Add("LogToEventLog", e.Message);
            }

            //返回
            return isSuccess;
        }
        #endregion

        #region 记录日志到文件
        /// <summary>
        /// 记录日志到文件
        /// </summary>
        /// <returns>是否记录成功</returns>
        private bool ExceptionToFile()
        {
            //设置返回
            bool isSuccess = false;

            //string maillist = ConfigurationManager.AppSettings["maillist"];

            ////获取当前系统位置
            //string strFilePath = "";

            //try
            //{
            //    if (TextHelper.GetConfigItem<bool>("IsLinux"))
            //    {
            //        strFilePath = HttpRuntime.AppDomainAppPath + @"Log/";
            //    }
            //    else
            //    {
            //        strFilePath = HttpRuntime.AppDomainAppPath + @"Log\";
            //    }
            //}
            //catch
            //{
            //    if (TextHelper.GetConfigItem<bool>("IsLinux"))
            //    {
            //        strFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Log/";
            //    }
            //    else
            //    {
            //        strFilePath = HttpRuntime.AppDomainAppPath + @"Log\";
            //    }
            //}

            ////如果没有该文件夹则创建
            //try
            //{
            //    if (!Directory.Exists(strFilePath))
            //    {
            //        Directory.CreateDirectory(strFilePath);
            //    }
            //}
            //catch
            //{
            //}

            ////文件记录地址（限制单文件大小为3M）
            //DateTime now = DateTime.Now;

            //strFilePath += now.ToString("yyyyMMdd") + "Exception";
            //if (!Directory.Exists(strFilePath))
            //    Directory.CreateDirectory(strFilePath);
            //strFilePath += "\\UnHandleException_" + now.ToString("yyyyMMddHH");
            //int currentVernier = 0;
            //while (true)
            //{
            //    string pathLog = currentVernier == 0 ? strFilePath + ".log" : string.Format("{0}_{1}.log", strFilePath, currentVernier.ToString());
            //    if (!File.Exists(pathLog) || new FileInfo(pathLog).Length < 1024 * 5)
            //    {
            //        strFilePath = pathLog;
            //        break;
            //    }
            //    else
            //        currentVernier++;
            //}


            //try
            //{
            //    if (!string.IsNullOrEmpty(m_exceptionContent))
            //    {
            //        //文件对象
            //        FileInfo fiWriter = new FileInfo(strFilePath);

            //        // 文件流,用于写日志
            //        StreamWriter swFile = fiWriter.AppendText();

            //        //写到文件中
            //        swFile.WriteLine(m_exceptionContent);

            //        //分割行
            //        swFile.WriteLine("------------------------------------------------------");

            //        //输出到磁盘
            //        swFile.Flush();

            //        //关闭
            //        swFile.Close();

            //        try
            //        {
            //            string url = WebCurrentUrl();
            //            if (!url.StartsWith("http://localhost"))
            //            {
            //                //new SendEmailHelper().SendEmailFunc(maillist, url + "异常信息", m_exceptionContent.Replace("\r\n", "<br />"), "");
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }

            //    //返回成功
            //    isSuccess = true;
            //}
            //catch (Exception)
            //{
            //}
           
            var _logger = LogFactory.GetLogger(GetType());          
            _logger.Error(m_exceptionContent);
            //返回
            return isSuccess;
        }
        #endregion
    }
}
