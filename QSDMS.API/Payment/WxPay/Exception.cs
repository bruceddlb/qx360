using System;
using System.Collections.Generic;
using System.Web;

namespace QSMS.API.Payment.WxPay
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}