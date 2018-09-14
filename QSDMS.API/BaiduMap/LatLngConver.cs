using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.BaiduMap
{
    /// <summary>
    /// 地图帮助类
    /// </summary>
    public class LatLngConver
    {
        //纬度
        private double MaxLatitude;

        private double MinLatitude;
        //经度
        private double MaxLongitude;

        private double MinLongitude;
        private double MaxLatitude2;

        private double MinLatitude2;

        private double MaxLongitude2;

        private double MinLongitude2;

        private const double Ea = 6378140;//6378137;     //   赤道半径 米

        private const double Eb = 6356755;     //   极半径 
        //var ra = 6378.140; // 赤道半径 (km)  
        //var rb = 6356.755;  // 极半径 (km)  

        /// <summary>
        /// 根据一个点的坐标和距离计算另外一个点的坐标
        /// </summary>
        /// <param name="LAT"></param>
        /// <param name="LON"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="newLon"></param>
        /// <param name="newLat"></param>
        private static void GetlatLon(double LAT, double LON, double distance, double angle, out double newLon, out double newLat)
        {

            double dx = distance * 1000 * Math.Sin(angle * Math.PI / 180.0);

            double dy = distance * 1000 * Math.Cos(angle * Math.PI / 180.0);

            double ec = Eb + (Ea - Eb) * (90.0 - LAT) / 90.0;

            double ed = ec * Math.Cos(LAT * Math.PI / 180);

            double a = (dx / ed + LON * Math.PI / 180.0) * 180.0 / Math.PI;

            double b = (dy / ec + LAT * Math.PI / 180.0) * 180.0 / Math.PI;
            //转换坐标
            LatLngConver.ConvertToBaidu(a, b, out newLat, out newLon);
        }

        public static void GetRectRange(double centorlatitude, double centorLogitude, double distance,

                                      out double maxLatitude, out double minLatitude, out double maxLongitude, out double minLongitude)
        {

            double temp = 0.0;

            GetlatLon(centorlatitude, centorLogitude, distance, 0, out temp, out maxLatitude);

            GetlatLon(centorlatitude, centorLogitude, distance, 180, out temp, out minLatitude);

            GetlatLon(centorlatitude, centorLogitude, distance, 90, out maxLongitude, out temp);

            GetlatLon(centorlatitude, centorLogitude, distance, 270, out minLongitude, out temp);

        }

        public static void GetRectRange2(double centorlatitude, double centorLogitude, double distance,

                                      out double maxLatitude, out double minLatitude, out double maxLongitude, out double minLongitude)
        {

            double temp = 0.0;

            GetNewLatLon(centorlatitude, centorLogitude, distance, 0, out maxLatitude, out temp);

            GetNewLatLon(centorlatitude, centorLogitude, distance, 180, out minLatitude, out temp);

            GetNewLatLon(centorlatitude, centorLogitude, distance, 90, out temp, out maxLongitude);

            GetNewLatLon(centorlatitude, centorLogitude, distance, 270, out temp, out minLongitude);

        }





        /// <summary>

        /// where    φ is latitude, λ is longitude, θ is the bearing (clockwise from north),

        /// δ is the angular distance d/R; d being the distance travelled, R the earth’s radius

        /// bearing 方位 0，90，180，270

        /// </summary>

        private static void GetNewLatLon(double lat, double lon, double d, double bearing, out double lat2, out double lon2)
        {

            lat2 = 0.0;

            lon2 = 0.0;

            double R = 6378.137;

            var φ1 = ConvertDegreesToRadians(lat);

            var λ1 = ConvertDegreesToRadians(lon);

            var θ = ConvertDegreesToRadians(bearing);

            var φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(d / R) +

                             Math.Cos(φ1) * Math.Sin(d / R) * Math.Cos(θ));

            var λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(d / R) * Math.Cos(φ1),

                                     Math.Cos(d / R) - Math.Sin(φ1) * Math.Sin(φ2));

            λ2 = (λ2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;
            lat2 = ConvertRadiansToDegrees(φ2);

            lon2 = ConvertRadiansToDegrees(λ2);
        }


        /// <summary>
        /// 输入两点的经纬度计算两者距离
        /// </summary>
        /// <param name="Lat_A">纬度A</param>
        /// <param name="Lng_A">经度A</param>
        /// <param name="Lat_B">纬度B</param>
        /// <param name="Lng_B">纬度B</param>
        /// <returns></returns>
        public static string CalcDistance(double Lat_A, double Lng_A, double Lat_B, double Lng_B)
        {

            var flatten = (Ea - Eb) / Ea;  //# 地球扁率  
            var rad_lat_A = ConvertDegreesToRadians(Lat_A);
            var rad_lng_A = ConvertDegreesToRadians(Lng_A);
            var rad_lat_B = ConvertDegreesToRadians(Lat_B);
            var rad_lng_B = ConvertDegreesToRadians(Lng_B);
            var pA = Math.Atan(Eb / Ea * Math.Tan(rad_lat_A));
            var pB = Math.Atan(Eb / Ea * Math.Tan(rad_lat_B));
            var xx = Math.Acos(Math.Sin(pA) * Math.Sin(pB) + Math.Cos(pA) * Math.Cos(pB) * Math.Cos(rad_lng_A - rad_lng_B));
            var c1 = (Math.Sin(xx) - xx) * (Math.Sin(pA) + Math.Sin(pB)) * 2 / Math.Cos(xx / 2) * 2;
            var c2 = (Math.Sin(xx) + xx) * (Math.Sin(pA) - Math.Sin(pB)) * 2 / Math.Sin(xx / 2) * 2;
            var dr = flatten / 8 * (c1 - c2);
            var distance = Ea * (xx + dr);
            distance = (distance * 0.001);
            return distance.ToString("f2");
        }

        /// <summary>
        /// 计算两个百度坐标的距离
        /// </summary>
        /// <param name="lng_a">当前位置的经度</param>
        /// <param name="lat_a">当前位置的纬度</param>
        /// <param name="lng_b">目标点的经度</param>
        /// <param name="lat_b">目标点的纬度</param>
        /// <returns></returns>
        public static double CacleLong(double lng_a, double lat_a, double lng_b, double lat_b)
        {
            double pk = 180 / 3.14169;
            double a1 = lat_a / pk;
            double a2 = lng_a / pk;
            double b1 = lat_b / pk;
            double b2 = lng_b / pk;
            double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
            double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
            double t3 = Math.Sin(a1) * Math.Sin(b1);
            double tt = Math.Acos(t1 + t2 + t3);
            return ((6366000 * tt)/1000);
        }

        /// <summary>
        /// 将微信坐标转换为百度坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns>百度坐标，key表示经度，value表示纬度</returns>
        public static void ConvertToBaidu(double lng, double lat, out double newlng, out double newlat)
        {
            newlng = 0;
            newlat = 0;
            //Dictionary<double, double> dic = new Dictionary<double, double>();
            //转换前的GPS坐标  
            double x = 116.397428;
            double y = 39.90923;
            //google 坐标转百度链接 //http://api.map.baidu.com/ag/coord/convert?from=2&to=4&x=116.32715863448607&y=39.990912172420714&callback=BMap.Convertor.cbk_3694  
            //gps坐标的type=0  
            //google坐标的type=2  
            //baidu坐标的type=4  
            String path = "http://api.map.baidu.com/ag/coord/convert?from=0&to=4&x=" + lng + "+&y=" + lat + "&callback=BMap.Convertor.cbk_7594";
            string res = SendDataByGET(path);
            if (res.IndexOf("(") > 0 && res.IndexOf(")") > 0)
            {
                int sint = res.IndexOf("(") + 1;
                int eint = res.IndexOf(")");
                int ls = res.Length;
                String str = res.Substring(sint, eint - sint);
                int errint = res.IndexOf("error") + 7;
                int enderr = res.IndexOf("error") + 8;
                String err = res.Substring(errint, 1);
                if ("0".Equals(err))
                {
                    int sx = str.IndexOf(",\"x\":\"") + 6;
                    int sy = str.IndexOf("\",\"y\":\"");
                    int endy = str.IndexOf("\"}");
                    int sl = str.Length;
                    string xp = str.Substring(sx, sy - sx);
                    string yp = str.Substring(sy + 7, endy - sy - 7);
                    byte[] outputb = Convert.FromBase64String(xp);
                    string XStr = Encoding.Default.GetString(outputb);
                    outputb = Convert.FromBase64String(yp);
                    string YStr = Encoding.Default.GetString(outputb);
                    if (XStr != "" && YStr != "")
                    {
                        newlat = Convert.ToDouble(XStr);
                        newlng = Convert.ToDouble(YStr);
                    }
                }
            }
        }

        /// 通过GET方式发送数据  
        /// url  
        /// GET数据  
        /// GET容器  
        public static string SendDataByGET(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>
        /// 度数转换到弧度 将弧度转为角度, 弧度 /  p/180
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double ConvertRadiansToDegrees(double radian)
        {

            return radian * 180.0 / Math.PI;

        }
    }
}
