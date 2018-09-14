using iFramework.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace QSDMS.Util
{
    /// <summary>
    /// 生成二维码
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QRCodeHelper()
        {

        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="txtData">需要生成二维码的字符串</param>
        /// <param name="errorCorrect">L,M,Q,H</param>
        /// <param name="savePath">图片保存路径</param>
        /// <param name="version">大于等于1小于等于14</param>
        /// <param name="encode">Byte,AlphaNumeric,Numeric</param>
        /// <param name="textSize">字体大小</param>
        /// <returns></returns>
        public static bool GeneralQRCode(string txtData, string savePath, string errorCorrect = "M", int version = 10, string encode = "Byte", string textSize = "12")
        {
            string result = string.Empty;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = encode;
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(textSize);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                result = "Invalid size!" + ex.Message;
                return false;
            }
            try
            {
                qrCodeEncoder.QRCodeVersion = version;

            }
            catch (Exception ex)
            {
                result = ex.Message;
                return false;
            }

            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            else
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            try
            {
                String ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";

                if (string.IsNullOrWhiteSpace(System.IO.Path.GetPathRoot(savePath)))
                {
                    if (!System.IO.Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + savePath.Trim('\\')))
                    {
                        System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + savePath.Trim('\\'));
                    }
                    ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
                    String ls_savePath = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + savePath.Trim('\\') + "\\" + ls_fileName;
                    qrCodeEncoder.Encode(txtData).Save(ls_savePath);
                }
                else
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                    }
                    qrCodeEncoder.Encode(txtData).Save(savePath);
                    ls_fileName = System.IO.Path.GetFileName(savePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "QRCodeHelper>>GeneralQRCode";
                new ExceptionHelper().LogException(ex);
            }

            return false;
        }
    }
}
