using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.BaiduMap
{
    public class HarvenSin
    {
        public static double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }


        static double EARTH_RADIUS = 6371.0;//km 地球半径 平均值，千米

        /// <summary>
        /// 给定的经度1，纬度1；经度2，纬度2. 计算2个经纬度之间的距离。
        /// </summary>
        /// <param name="lat1">经度1</param>
        /// <param name="lon1">纬度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lon2">纬度2</param>
        /// <returns>距离（公里、千米）</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            //用haversine公式计算球面两点间的距离。
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lon1 = ConvertDegreesToRadians(lon1);
            lat2 = ConvertDegreesToRadians(lat2);
            lon2 = ConvertDegreesToRadians(lon2);

            //差值
            var vLon = Math.Abs(lon1 - lon2);
            var vLat = Math.Abs(lat1 - lat2);

            //h is the great circle distance in radians, great circle就是一个球体上的切面，它的圆心即是球心的一个周长最大的圆。
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);

            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

            return distance;
        }

        /// <summary>
        /// 将角度换算为弧度。
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        /// <summary>
        /// 获取距离 和百度api 一致
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double GetDistance(Point2D point1, Point2D point2)
        {
            return Wv(point1, point2)/1000;
        }  
        private static double Wv(Point2D a, Point2D b)
        {
            if (a == null || b == null)
            {
                return 0;
            }
            double a_lng = ew(a.Lng, -180, 180);
            double a_lat = lw(a.Lat, -74, 74);
            double b_lng = ew(b.Lng, -180, 180);
            double b_lat = lw(b.Lat, -74, 74);
            return Td(oi(a_lng), oi(b_lng), oi(a_lat), oi(b_lat));
        }
        private static double oi(double a)
        {
            return Math.PI * a / 180;
        }
        private static double Td(double a, double b, double c, double d)
        {
            return 6370996.81 * Math.Acos(Math.Sin(c) * Math.Sin(d) + Math.Cos(c) * Math.Cos(d) * Math.Cos(b - a));

        }
        private static double ew(double a, double b, double c)
        {
            if (a > c)
            {
                a -= c - b;
            }
            else if (a < b)
            {
                a += c - b;
            }
            return a;
        }
        private static double lw(double a, double b, double c)
        {
            a = max(a, b);
            a = min(a, c);
            return a;
        }
        private static double max(double a, double b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }
        private static double min(double a, double c)
        {
            if (a > c)
            {
                return c;
            }
            return a;
        }
    }

    public class Point2D
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
    public class BaiduMap
    {

        public int status { get; set; }
        public int total { get; set; }
        public int size { get; set; }
        public List<Contents> contents { get; set; }
    }
    public class Contents
    {

        public long uid { get; set; }
        public string province { get; set; }
        public long geotable_id { get; set; }
        public string district { get; set; }
        public int create_time { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public float[] location { get; set; }
        public string title { get; set; }
        public int coord_type { get; set; }
        public int distance { get; set; }
        public int weight { get; set; }
    }
}
