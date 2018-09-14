using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.BaiduMap
{
    public class GetJWD
    {
        public class GetMBR
        {

            public double MaxLatitude;

            public double MinLatitude;

            public double MaxLongitude;

            public double MinLongitude;



            public double MaxLatitude2;

            public double MinLatitude2;

            public double MaxLongitude2;

            public double MinLongitude2;

            public GetMBR(double centorlatitude, double centorLogitude, double distance)
            {

                GetRectRange(centorlatitude, centorLogitude, distance, out MaxLatitude, out MinLatitude, out MaxLongitude, out MinLongitude);

                GetRectRange2(centorlatitude, centorLogitude, distance, out MaxLatitude2, out MinLatitude2, out MaxLongitude2, out MinLongitude2);
                
            }



            public const double Ea = 6378137;     //   赤道半径  

            public const double Eb = 6356725;     //   极半径 



            private static void GetlatLon(double LAT, double LON, double distance, double angle, out double newLon, out double newLat)
            {

                double dx = distance * 1000 * Math.Sin(angle * Math.PI / 180.0);

                double dy = distance * 1000 * Math.Cos(angle * Math.PI / 180.0);

                double ec = Eb + (Ea - Eb) * (90.0 - LAT) / 90.0;

                double ed = ec * Math.Cos(LAT * Math.PI / 180);



                double a = (dx / ed + LON * Math.PI / 180.0) * 180.0 / Math.PI;

                double b = (dy / ec + LAT * Math.PI / 180.0) * 180.0 / Math.PI;
                newLat = b;
                newLon = a;
                //转换坐标
               // ConvertToBaidu(a, b, out newLat, out newLon);

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

                λ2 = (λ2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI; // normalise to -180..+180°



                double a = ConvertRadiansToDegrees(φ2);

                double b = ConvertRadiansToDegrees(λ2);

                //转换坐标
                ConvertToBaidu(a, b, out lat2, out lon2);

            }



            public static double ConvertDegreesToRadians(double degrees)
            {

                return degrees * Math.PI / 180;

            }



            public static double ConvertRadiansToDegrees(double radian)
            {

                return radian * 180.0 / Math.PI;

            }

            public static void ConvertToBaidu(double lng, double lat, out double newlng, out double newlat)
            {
                newlng = 0;
                newlat = 0;
                //Dictionary<double, double> dic = new Dictionary<double, double>();
                //转换前的GPS坐标  
                double x = 116.397428;
                double y = 39.90923;
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

        }
    }
}
