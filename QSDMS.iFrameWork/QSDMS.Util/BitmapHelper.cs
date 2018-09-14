using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace QSDMS.Util
{
    /// <summary>
    /// Bitmap扩展类
    /// </summary>
    public static class BitmapHelper
    {
        /// <summary>
        /// 缩放图片到指定尺寸
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Zoom(this Bitmap image, int width, int height)
        {
            if (image == null || width <= 0 || height <= 0)
            {
                throw new ArgumentException();
            }

            Bitmap result = new Bitmap(width, height);//新图片的大小  

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PageUnit = GraphicsUnit.Pixel;
                g.Clear(Color.White);
                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }
    }
}
