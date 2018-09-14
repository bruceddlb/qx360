using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace iFramework.Framework.Util
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public static class CaptchaUtil
    {
        #region 私有常量

        /// <summary>
        /// 颜色数组
        /// </summary>
        private static readonly Color[] COLORS = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

        /// <summary>
        /// 字符数组
        /// </summary>
        private static readonly char[] CHARS = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        #endregion

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="code">返回验证码的字符</param>
        public static Bitmap Create(out string code)
        {
            code = MakeCheckCode(4);

            Bitmap bitmap = new Bitmap(80, 28, PixelFormat.Format24bppRgb);
            Random random = new Random();
            Font font = new Font("tahoma", 14, FontStyle.Italic | FontStyle.Bold);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);

                for (int i = 0; i < 20; i++)
                {
                    graphics.DrawRectangle(new Pen(MakeRandomColor(random), 0), random.Next(bitmap.Width), random.Next(bitmap.Height), 1, 1);
                }

                for (int i = 0; i < code.Length; i++)
                {
                    Brush brush = new SolidBrush(MakeRandomColor(random));
                    graphics.DrawString(code[i].ToString(), font, brush, i * 18, random.Next(1, 3));
                }

                graphics.Save();
            }

            return bitmap;
        }

        /// <summary>
        /// 获取随机颜色
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private static Color MakeRandomColor(Random random)
        {
            return COLORS[random.Next(0, COLORS.Length)];
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string MakeCheckCode(int length)
        {
            Random random = new Random();

            string result = "";

            for (int i = 0; i < length; i++)
            {
                result += CHARS[random.Next(0, CHARS.Length)].ToString();
            }

            return result;
        }
    }
}
