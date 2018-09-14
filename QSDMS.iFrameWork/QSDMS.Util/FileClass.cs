
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QSDMS.Util
{
    public class FileClass
    {
        /// <summary>
        /// 删除文件操作
        /// </summary>
        /// <param name="filePath">文件路径</param>      
        public static void DeleteFile(string filePath)
        {
            string destinationFile = HttpContext.Current.Server.MapPath(filePath);
            //如果文件存在，删除文件
            if (File.Exists(destinationFile))
            {
                FileInfo fi = new FileInfo(destinationFile);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;

                File.Delete(destinationFile);
            }
        }

        /// <summary>
        /// 根据字节返回MB
        /// </summary>
        /// <param name="fileByte"></param>
        /// <returns></returns>
        public static string GetFileSize(decimal fileByte)
        {
            decimal fileSize = 0;
            fileSize = Converter.ParseDecimal(fileByte) / 1024 / 1024;
            // fileSize = (decimal)fileByte / 1024 / 1024;
            return fileSize.ToString("f2");
        }
    }
}
