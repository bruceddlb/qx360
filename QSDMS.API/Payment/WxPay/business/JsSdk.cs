using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LitJson;

using System.Collections;

namespace QSMS.API.Payment.WxPay
{
    public class JsSdk
    {
        private static JsSdk instance = new JsSdk();
        public static JsSdk GetInstance(){
            return instance;
        }
        private JsSdk() {
            AccessToken = new Token();
            Ticket = new Ticket(AccessToken);
        }

        public Token AccessToken { get; set; }

        public Ticket Ticket { get; set; }

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        public string MakeSign(string url,string nonce,long timestamp) 
        {
            m_values.Clear();
            m_values.Add("noncestr", nonce);
            m_values.Add("jsapi_ticket", Ticket.AuthTicket);
            m_values.Add("timestamp", timestamp);
            m_values.Add("url", url);

            var data= ToUrl();
            return Util.Sha1(data);
        }

        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
    }

    public class Ticket 
    {
        private Token token;
        private string ticket;
        public Ticket(Token token) 
        {
            this.token = token;
        }

        public string AuthTicket { get { return GetTicket(); } }

        public DateTime CreateDate { get; private set; }

        private string GetTicket()
        {
            if (string.IsNullOrWhiteSpace(ticket))
            {
                ticket = GetRemoteTicket();
                CreateDate = DateTime.Now;
            }
            else
            {
                if ((DateTime.Now - CreateDate).TotalSeconds >= 7100)
                {
                    ticket = GetRemoteTicket();
                    CreateDate = DateTime.Now;
                }
            }

            return ticket;
        }

        private string GetRemoteTicket()
        {
            string ticket = string.Empty;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", token.AccessToken);
            var result = HttpService.Get(url);
            JsonData jd = JsonMapper.ToObject(result);
            if (((IDictionary)jd).Contains("errcode")&& jd["errcode"].ToString() == "0")
            {
                ticket = jd["ticket"].ToString();
            }
            return ticket;
        }
        
    }

    public class Token 
    {
        private string access_token;

        public string AccessToken {
            get { return GetAccessToken();}
        }

        public DateTime CreateDate { get; private set; }

        private string GetAccessToken()
        {
            if (string.IsNullOrWhiteSpace(access_token))
            {
                access_token=GetRemoteAccountToken();
                CreateDate = DateTime.Now;
            }
            else {
                if ((DateTime.Now - CreateDate).TotalSeconds >= 7100) 
                {
                    access_token = GetRemoteAccountToken();
                    CreateDate = DateTime.Now;
                }
            }

            return access_token;
        }

        private string GetRemoteAccountToken() 
        {
            string token = string.Empty;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", WxPayConfig.APPID, WxPayConfig.APPSECRET);
            var result = HttpService.Get(url);
            JsonData jd = JsonMapper.ToObject(result);
            if (((IDictionary)jd).Contains("access_token"))
            {
                token = jd["access_token"].ToString();
            }
            return token;
        }
    }
}