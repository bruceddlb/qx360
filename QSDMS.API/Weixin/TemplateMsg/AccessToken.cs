using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.Weixin.TemplateMsg
{
    /// <summary>
    /// 微信许可令牌
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// 保存已获取到的许可令牌，键为公众号，值为公众号最后一次获取到的令牌
        /// </summary>
        private static ConcurrentDictionary<string, Tuple<AccessToken, DateTime>> accessTokens = new ConcurrentDictionary<string, Tuple<AccessToken, DateTime>>();

        /// <summary>
        /// 获取access token的地址
        /// </summary>
        private const string urlForGettingAccessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        /// <summary>
        /// 获取access token的http方法
        /// </summary>
        private const string httpMethodForGettingAccessToken = WebRequestMethods.Http.Get;
        /// <summary>
        /// 保存access token的最长时间（单位：秒），超过时间之后，需要重新获取
        /// </summary>
        private const int accessTokenSavingSeconds = 7000;

        /// <summary>
        /// access token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_access_token">access token</param>
        /// <param name="_expires_in">有效时间</param>
        internal AccessToken(string _access_token, int _expires_in)
        {
            access_token = _access_token;
            expires_in = _expires_in;
        }

        /// <summary>
        /// 返回AccessToken字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("AccessToken：{0}\r\n有效时间：{1}秒", access_token, expires_in);
        }

        /// <summary>
        /// 从JSON字符串解析AccessToken
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回AccessToken</returns>
        internal static AccessToken ParseFromJson(string json)
        {
            var at = JsonConvert.DeserializeAnonymousType(json, new { access_token = "", expires_in = 1 });
            return new AccessToken(at.access_token, at.expires_in);
        }

        /// <summary>
        /// 尝试从JSON字符串解析AccessToken
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <param name="msg">如果解析成功，返回AccessToken；否则，返回null。</param>
        /// <returns>返回是否解析成功</returns>
        internal static bool TryParseFromJson(string json, out AccessToken token)
        {
            bool success = false;
            token = null;
            try
            {
                token = ParseFromJson(json);
                success = true;
            }
            catch { }
            return success;
        }

        /// <summary>
        /// 得到access token
        /// </summary>
        /// <param name="userName">公众号</param>
        /// <returns>返回access token</returns>
        public static AccessToken Get(string appId, string appSecret)
        {
            AccessToken token = new AccessToken(null, -1);
            ErrorMessage msg = new ErrorMessage(-1, "获取token失败");
            var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(appId, appSecret);
            if (result == null)
            {
                return token;
            }
            token.access_token = result.access_token;
            token.expires_in = result.expires_in;
            //AccessToken token = GetFromWeixinServer(appId,appSecret, out msg);            
            return token;
        }

    }
}