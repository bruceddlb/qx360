using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// 方法的返回对象
    /// </summary>
    public class ReturnMessage
    {
        private IDictionary<string, object> m_Data = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="IsSuccess">默认是true还是false</param>
        public ReturnMessage(bool IsSuccess)
        {
            this.IsSuccess = IsSuccess;
            this.Text = string.Empty;
            this.Message = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReturnMessage()
        {

        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回单项数据信息
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 返回跳转地址
        /// </summary>
        public string RetrunUrl { get; set; }

        /// <summary>
        /// 返多项值,以字典形式返回
        /// </summary>
        public IDictionary<string, object> ResultData
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
     
    }
}
