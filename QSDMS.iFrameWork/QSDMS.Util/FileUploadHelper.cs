using System;
using System.IO;
using System.Web;
using System.Collections;
using System.Globalization;
using Kaliko.ImageLibrary;
using System.Drawing.Imaging;
using System.Drawing;
using Kaliko.ImageLibrary.Scaling;
using iFramework.Framework.Log;

namespace QSDMS.Util
{
    #region  图片上传
    /// <summary>
    /// 上传文件实体类
    /// </summary>
    public class FileModel
    {
        /// <summary> 
        ///  上传文件是否成功
        /// </summary> 
        public bool Status { get; set; }

        /// <summary> 
        ///  上传文件夹类型(图片(Image) 其他文件(File)
        /// </summary> 
        public string FileType { get; set; }

        /// <summary> 
        ///  上传文件名
        /// </summary> 
        public string FileName { get; set; }

        /// <summary> 
        ///  上传文件扩展名
        /// </summary> 
        public string FileExtension { get; set; }

        /// <summary> 
        ///  新文件名
        /// </summary> 
        public string FileNewName { get; set; }

        /// <summary> 
        ///  新文件名(无后缀)
        /// </summary> 
        public string FileNewNoExtensionName { get; set; }

        /// <summary> 
        ///  文件物理路径
        /// </summary> 
        public string PhysicFullPath { get; set; }

        /// <summary> 
        ///  文件大小
        /// </summary> 
        public Int64 FileSize { get; set; }

        /// <summary> 
        ///  返回换算后的类型(字节,K字节,M字节)
        /// </summary> 
        public string FileSizeType { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrorString { get; set; }
    }

    /// <summary>
    /// 文件上传类
    /// </summary>
    public class FileUpload
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }
        /// <summary>
        /// 设置上传文件的配置文件名称
        /// </summary>
        public string UploadConfig { get; set; }

        /// <summary>
        /// 上传文件扩展名控制
        /// </summary>
        public string Filter { get; set; }

        //上传文件最大长度 单位：KB
        /// <summary>
        /// 上传文件最大长度 单位：KB
        /// </summary>
        public static long MaxFileLength { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uploadconfig">参数配置如：PMG（PMG_Extn：允许上传的后缀【.GIF|.JPEG|.JPE】，PMG_MaxFileLength：允许上传的最大长度K）</param>
        public FileUpload(string uploadconfig)
        {
            UploadConfig = uploadconfig;
            Filter = string.IsNullOrEmpty(TextHelper.GetConfigItem(UploadConfig + "_Extn")) ? ".GIF|.JPEG|.JPE|.JPG|.PNG|.BMP|.TIFF|.TIF|.SWF" : TextHelper.GetConfigItem(UploadConfig + "_Extn");
            MaxFileLength = TextHelper.GetConfigItem(UploadConfig + "_MaxFileLength") == null ? 4096 : long.Parse(TextHelper.GetConfigItem(UploadConfig + "_MaxFileLength"));
        }

        /// <summary>
        /// HttpPostedFile保存上传文件
        /// </summary>
        /// <param name="postedFile">HttpPostedFile对象</param>
        /// <param name="SavePath">保存目录</param>
        /// <returns>返回FileInfo对象</returns>
        public FileModel Save(HttpPostedFile postedFile, string SavePath)
        {
            return SaveAsFile(postedFile, string.Empty, SavePath);
        }

        /// <summary>
        /// HttpPostedFile保存上传文件
        /// </summary>
        /// <param name="postedFile">HttpPostedFile对象</param>
        /// <param name="NewFileNameNoExt">上传文件名称（不带扩展名）</param>
        /// <param name="SavePath">保存目录</param>
        /// <returns>返回FileInfo对象</returns>
        public FileModel Save(HttpPostedFile postedFile, string NewFileNameNoExt, string SavePath)
        {
            return SaveAsFile(postedFile, NewFileNameNoExt, SavePath);
        }

        /// <summary>
        /// HttpPostedFileBase保存上传文件
        /// </summary>
        /// <param name="postedFileBase">HttpPostedFileBase对象</param>
        /// <param name="SavePath">保存目录</param>
        /// <returns>返回FileInfo对象</returns>
        public FileModel Save(HttpPostedFileBase postedFileBase, string SavePath)
        {
            return SaveAsFileBase(postedFileBase, string.Empty, SavePath);
        }

        /// <summary>
        /// HttpPostedFileBase保存上传文件
        /// </summary>
        /// <param name="postedFileBase">HttpPostedFileBase对象</param>
        /// <param name="NewFileNameNoExt">上传文件名称（不带扩展名）</param>
        /// <param name="SavePath">保存目录</param>
        /// <param name="Extension">文件后缀，如.jpg</param>
        /// <returns>返回FileInfo对象</returns>
        public FileModel Save(HttpPostedFileBase postedFileBase, string NewFileNameNoExt, string SavePath, string Extension = null)
        {
            return SaveAsFileBase(postedFileBase, NewFileNameNoExt, SavePath, Extension);
        }

        /// <summary>
        /// 储存文件方法
        /// </summary>
        /// <param name="postedFile">上传对象</param>
        /// <param name="SavePath">保存路径，含文件名</param>
        /// <param name="NewFileNameNoExt">上传文件名称（不带扩展名）</param>
        /// <returns></returns>
        private FileModel SaveAsFile(HttpPostedFile postedFile, string NewFileNameNoExt, string SavePath)
        {
            FileModel file = new FileModel();

            //上传文件夹类型(图片(Image) 其他文件(File) 用户匿名上传(Other))
            file.FileType = "Image";
            //上传文件名
            file.FileName = string.Empty;
            //上传文件扩展名
            file.FileExtension = string.Empty;
            //新文件名
            file.FileNewName = string.Empty;
            //文件物理路径
            file.PhysicFullPath = string.Empty;

            Int32 FileSize = 0;

            try
            {
                file.FileName = Path.GetFileName(postedFile.FileName);
                file.FileExtension = Path.GetExtension(file.FileName);
                file.FileNewName = System.Guid.NewGuid().ToString().Replace("-", "") + file.FileExtension;
                if (!string.IsNullOrEmpty(NewFileNameNoExt)) file.FileNewName = NewFileNameNoExt + file.FileExtension;

                FileSize = postedFile.ContentLength;

                if (!IsValidatedFileName(file.FileExtension))
                {
                    file.Status = false;
                    file.ErrorString = "上传的文件不符合要求";
                    return file;
                }
                if (file.FileSize > MaxFileLength)
                {
                    file.Status = false;
                    file.ErrorString = "上传的文件不能大于" + MaxFileLength + "K";
                    return file;
                }

                if (IsAllowedExtension(file.FileExtension.ToLower()))
                    file.FileType = "Image";
                else
                    file.FileType = "File";

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(SavePath);
                if (!di.Exists) // 如果不存在目录则创建
                    di.Create();

                file.PhysicFullPath = SavePath;

                postedFile.SaveAs(SavePath + "\\" + file.FileNewName);

                file.Status = true;
            }
            catch
            {
                file.Status = false;
                return file;
            }
            return file;
        }

        /// <summary>
        /// 储存文件方法
        /// </summary>
        /// <param name="postedFile">上传对象</param>
        /// <param name="SavePath">保存路径</param>
        /// <param name="NewFileNameNoExt">上传文件名称（不带扩展名）</param>
        /// <param name="Extension">文件后缀，如.jpg</param>        
        /// <returns></returns>
        private FileModel SaveAsFileBase(HttpPostedFileBase postedFile, string NewFileNameNoExt, string SavePath, string Extension = null)
        {
            FileModel file = new FileModel();

            //上传文件夹类型(图片(Image) 其他文件(File) 用户匿名上传(Other))
            file.FileType = "Image";
            //上传文件名
            file.FileName = string.Empty;
            //上传文件扩展名
            file.FileExtension = string.Empty;
            //新文件名
            file.FileNewName = string.Empty;
            //文件物理路径
            file.PhysicFullPath = string.Empty;


            try
            {
                if (!IsValidatedFileName(Path.GetExtension(postedFile.FileName)))
                {
                    file.Status = false;
                    file.ErrorString = "上传的文件类型不符合";
                    return file;
                }
                if ((postedFile.ContentLength / 1024) > MaxFileLength)
                {
                    file.Status = false;
                    file.ErrorString = "上传的文件大小不能大于" + MaxFileLength + "K";
                    return file;
                }

                file.FileName = Path.GetFileName(postedFile.FileName);
                file.FileExtension = Path.GetExtension(file.FileName);
                if (!string.IsNullOrEmpty(Extension)) file.FileExtension = Extension;
                file.FileNewNoExtensionName = System.Guid.NewGuid().ToString().Replace("-", "");
                file.FileNewName = file.FileNewNoExtensionName + file.FileExtension;
                if (!string.IsNullOrEmpty(NewFileNameNoExt)) file.FileNewName = NewFileNameNoExt + file.FileExtension;

                file.FileSize = postedFile.ContentLength;

                decimal Size;
                if (file.FileSize >= 0 && file.FileSize < 1024)
                {
                    file.FileSizeType = "字节";
                }
                else if (file.FileSize >= 1024 && file.FileSize < (1024 * 1024))
                {
                    Size = Convert.ToDecimal(file.FileSize) / 1024;
                    file.FileSize = file.FileSize / 1024;
                    file.FileSizeType = Math.Round(Size, 3) + "K";
                }
                else if (file.FileSize >= 1024 * 1024)
                {
                    Size = Convert.ToDecimal(file.FileSize) / (1024 * 1024);
                    file.FileSize = file.FileSize / (1024 * 1024);
                    file.FileSizeType = Math.Round(Size, 3) + "M";
                }


                if (IsAllowedExtension(file.FileExtension.ToLower()))
                    file.FileType = "Image";
                else
                    file.FileType = "File";

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(SavePath);
                if (!di.Exists) // 如果不存在目录则创建
                    di.Create();

                file.PhysicFullPath = SavePath;

                postedFile.SaveAs(SavePath + "\\" + file.FileNewName);

                file.Status = true;
            }
            catch(Exception ex)
            {
                file.Status = false;
                file.ErrorString = ex.Message;
                Logger.Error("上传图片错误：" + ex.ToString());
                return file;
            }
            return file;
        }


        /// <summary>
        /// 检查文件格式
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsValidatedFileName(string fileName)
        {
            if (Filter == "*") return true;
            string type = fileName.Substring(fileName.LastIndexOf("."));
            bool sReturn = false;
            string[] s = Filter.Split(new char[] { '|' });

            for (int i = 0; i < s.Length; i++)
            {
                if (type.ToLower() == s[i].ToLower())
                    sReturn = true;
            }
            return sReturn;
        }

        /// <summary>
        /// 判断文件扩展名是否为图片
        /// </summary>
        /// <param name="strExtension"></param>
        /// <returns></returns>
        public bool IsAllowedExtension(string strExtension)
        {
            //允许上传的扩展名，可以改成从配置文件中读出   
            //string[] arrExtension = { ".gif", ".jpg", ".jpeg", ".bmp", ".png" };
            //从配置文件读取
            if (Filter == "*") return false;

            string[] arrExtension = Filter.Split(new char[] { '|' });
            strExtension = strExtension.ToLower();
            if (strExtension != string.Empty)
            {
                for (int i = 0; i < arrExtension.Length; i++)
                {
                    if (strExtension.Equals(arrExtension[i].ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    #endregion

    #region 生成缩略图
    /// <summary>
    /// 缩放方式
    /// </summary>
    public enum ThumbnailMode
    {
        /// <summary>
        /// 指定高、宽缩放（可能变形）
        /// </summary>
        HW,
        /// <summary>
        /// 指定宽，高按比例缩放 
        /// </summary>
        W,
        /// <summary>
        /// 指定高，宽按比例缩放 
        /// </summary>
        H,
        /// <summary>
        /// 指定高宽裁减（不变形） 
        /// </summary>
        Cut,
        /// <summary>
        /// 
        /// </summary>
        None
    }

    /// <summary>
    /// 上传文件缩略图类
    /// </summary>
    public class Thumbnail
    {
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="thumbnailMode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, ThumbnailMode thumbnailMode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (thumbnailMode)
            {
                case ThumbnailMode.None:
                    if (ow > oh)
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    else if (oh > ow)
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    break;
                case ThumbnailMode.HW://指定高宽缩放（可能变形）                
                    break;
                case ThumbnailMode.W://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.H://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            try
            {
                thumbnailPath = thumbnailPath.Replace("/", "\\");
                string savePath = thumbnailPath.Substring(0, thumbnailPath.LastIndexOf("\\"));
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                bitmap.Save(thumbnailPath, GetImageFormat(thumbnailPath));
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #region Thumbnail
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="height">高度</param>
        /// <param name="width"></param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns></returns>
        public static bool MakeThumbnail(string sFile, string dFile, int height, int width, ThumbnailMode type, int flag = 100)
        {
            try
            {
                KalikoImage image = new KalikoImage(sFile);
                image.BackgroundColor = Color.Aquamarine;
                //缩放后的宽度和高度
                int towidth = width;
                int toheight = height;
                //
                int x = 0;
                int y = 0;
                int ow = image.Width;
                int oh = image.Height;

                switch (type)
                {
                    case ThumbnailMode.HW://指定高宽缩放（可能变形）           
                        {
                            break;
                        }
                    case ThumbnailMode.W://指定宽，高按比例     
                        {
                            toheight = image.Height * width / image.Width;
                            break;
                        }
                    case ThumbnailMode.H://指定高，宽按比例
                        {
                            towidth = image.Width * height / image.Height;
                            break;
                        }
                    case ThumbnailMode.Cut://指定高宽裁减（不变形）     
                        {
                            if ((double)image.Width / (double)image.Height > (double)towidth / (double)toheight)
                            {
                                oh = image.Height;
                                ow = image.Height * towidth / toheight;
                                y = 0;
                                x = (image.Width - ow) / 2;
                            }
                            else
                            {
                                ow = image.Width;
                                oh = image.Width * height / towidth;
                                x = 0;
                                y = (image.Height - oh) / 2;
                            }
                            break;
                        }
                    default:
                        break;
                }
                var img = image.Scale(new FitScaling(towidth, toheight));
                var extension = System.IO.Path.GetExtension(sFile);
                switch (extension.ToLower())
                {
                    case ".png":
                        img.SavePng(dFile);
                        break;
                    case ".gif":
                        img.SaveGif(dFile);
                        break;
                    case ".ico":
                        img.SaveImage(dFile, ImageFormat.Icon);
                        break;
                    case ".bmp":
                        img.SaveBmp(dFile);
                        break;
                    case ".jpg":
                        img.SaveJpg(dFile, flag);
                        break;
                    default:
                        img.SaveBmp(dFile);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion

        private static System.Drawing.Imaging.ImageFormat GetImageFormat(string filePath)
        {
            var imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            var extension = System.IO.Path.GetExtension(filePath);
            switch (extension.ToLower())
            {
                case ".png":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                case ".gif":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ".ico":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Icon;
                    break;
                case ".bmp":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                default:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
            }

            return imageFormat;
        }
    }


    #endregion
}
