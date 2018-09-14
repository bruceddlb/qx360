using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.BaiduMap
{
    public class GeoHash
    {
        public const string BASE32 = "0123456789bcdefghjkmnpqrstuvwxyz"; //对经纬度进行GeoHash编码

        /// <summary>
        /// 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string Encode_Geohash(double latitude, double longitude, int precision)
        {
            char[] geohash = new char[precision];
            bool is_even = true;
            //1
            int i = 0; double[] lat = new double[2];
            double[] lon = new double[2]; double mid;
            //char[] bits = new char[] { 16, 8, 4, 2, 1 }; 
            int[] bits = new int[] { 16, 8, 4, 2, 1 };
            int bit = 0, ch = 0;
            lat[0] = -90.0;
            lat[1] = 90.0;
            //纬度(-90,90)
            lon[0] = -180.0; lon[1] = 180.0;
            //经度(-180,180) 
            while (i < precision)
            {
                if (is_even == true)
                {
                    mid = (lon[0] + lon[1]) / 2;
                    //求中间经度值
                    if (longitude > mid)
                    {
                        ch |= bits[bit]; lon[0] = mid;
                    }
                    else
                    {
                        lon[1] = mid;
                    }
                }
                else
                {
                    mid = (lat[0] + lat[1]) / 2;
                    //求中间纬度值
                    if (latitude > mid)
                    {
                        ch |= bits[bit];
                        lat[0] = mid;
                    }
                    else
                    {
                        lat[1] = mid;
                    };
                }
                is_even = !is_even;
                if (bit < 4)
                {
                    bit++;
                }
                else
                {
                    geohash[i++] = GeoHash.BASE32[ch];
                    bit = 0;
                    ch = 0;
                }
            }
            //geohash[i] = '0';
            string rbc = "";
            for (int k = 0; k < geohash.Length; k++)
            {
                rbc += geohash[k].ToString();

            }
            return rbc;
        }
    }
}
