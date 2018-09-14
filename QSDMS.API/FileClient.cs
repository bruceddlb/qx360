using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace QSMS.API
{
    public class FileClient
    {
        /// <summary>
        /// http
        /// </summary>
        public HttpMethod http = new HttpMethod();

        /// <summary>
        /// Cookie
        /// </summary>
        public static string Cookie = "";

        public ReturnMessage Get(string url)
        {
            var result = new ReturnMessage(false) { Message = "获取接口信息失败!" };
            try
            {
                string resp = new HttpMethod().HttpGet(url, UTF8Encoding.UTF8, ref Cookie);
                result = JsonConvert.DeserializeObject<ReturnMessage>(resp);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = this.ToString() + ">>Get";
                new ExceptionHelper().LogException(ex);
            }
            return result;
        }
        public ReturnMessage Post(string url, string data)
        {
            var result = new ReturnMessage(false) { Message = "获取接口信息失败!" };
            try
            {
                string resp = new HttpMethod().HttpPost(url, data, Encoding.UTF8, ref Cookie);
                result = JsonConvert.DeserializeObject<ReturnMessage>(resp);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = this.ToString() + "\r\n" + url + "\r\n" + data;
                new ExceptionHelper().LogException(ex);
            }
            return result;
        }

        public ReturnMessage Post(string url, string data, string filePath)
        {
            var result = new ReturnMessage(false) { Message = "获取接口信息失败!" };
            try
            {
                string resp = new HttpMethod().HttpPost(url, filePath, data, ref Cookie, Encoding.UTF8);
                result = JsonConvert.DeserializeObject<ReturnMessage>(resp);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = this.ToString() + ">>Post";
                new ExceptionHelper().LogException(ex);
            }
            return result;
        }

        public ReturnMessage Post(string url, string data, HttpPostedFileBase file)
        {
            var result = new ReturnMessage(false) { Message = "获取接口信息失败!" };
            try
            {
                byte[] bytes = null;
                if (file != null)
                {
                    bytes = StreamToBytes(file);
                }

                string resp = new HttpMethod().HttpPost(url, file.FileName, bytes, data, ref Cookie, Encoding.UTF8);
                result = JsonConvert.DeserializeObject<ReturnMessage>(resp);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = this.ToString() + ">>Post";
                new ExceptionHelper().LogException(ex);
            }
            return result;
        }

        /// <summary>
        /// 文件上传 add by bruced
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ReturnMessage Post(string url, HttpPostedFileBase file, Dictionary<string, object> parameters)
        {
            var result = new ReturnMessage(false) { Message = "获取接口信息失败!" };
            try
            {

                string resp = new HttpMethod().HttpPost(url, file, parameters, Encoding.UTF8);
                result = JsonConvert.DeserializeObject<ReturnMessage>(resp);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = this.ToString() + ">>Post";
                new ExceptionHelper().LogException(ex);
            }
            return result;
        }

        public byte[] StreamToBytes(HttpPostedFileBase file)
        {
            byte[] bytes = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                bytes = binaryReader.ReadBytes(file.ContentLength);
            }
            return bytes;
        }

    }
}