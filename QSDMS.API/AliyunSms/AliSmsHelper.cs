using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.AliyunSms
{
    public class AliSmsHelper
    {

        //产品名称:云通信短信API产品,开发者无需替换
        const String product = "Dysmsapi";
        //产品域名,开发者无需替换
        const String domain = "dysmsapi.aliyuncs.com";

        // TODO 此处需要替换成开发者自己的AK(在阿里云访问控制台寻找)
        const String accessKeyId = "LTAI4Vjs2YexvLLB";
        const String accessKeySecret = "TjyueUkdCYsvOVmVQX360F6dFkDkLi16b";

        /// <summary>
        /// 发送方法
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="tempid"></param>
        /// <param name="templateParam">"{\"code\":\"" + code + "\"}"</param>
        /// <returns></returns>
        public static bool SendSms(string mobile, string tempid, string templateParam)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            SendSmsResponse response = null;
            try
            {

                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = mobile;//"18602707864";
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "人车行";
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = tempid;//"SMS_114370118";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = templateParam;// "{\"code\":\"" + code + "\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常
                response = acsClient.GetAcsResponse(request);
                if (response.Code != null && response.Code == "OK")
                {
                    return true;
                }

            }
            catch (ServerException e)
            {
                
            }
            catch (ClientException e)
            {
               
            }
            return false;

        }
    }
}
