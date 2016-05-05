using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities.SpatialIndexing;

namespace MpLayer
{
    public class TrackProvider : IProvider, IDisposable
    {
        private string _crs = "EPSG:3857";
        private object _syncRoot;
        private Feature _trackFeature;
        private Feature _positionFeature;

        public string CRS
        {
            get { return _crs; }
            set { _crs = value; }
        }

        public static IStyle CreateTrackStyle()
        {
            return new VectorStyle
            {
                Fill = null,
                Outline = { Color = Color.Green, Width = 1 },
                Line = { Color = Color.FromArgb(160, 255, 0, 0), Width = 4 }
            };
        }

        //674114.95 6887329.56
        //674268.58 6887340.31
        //674288.74 6887298.96
        //674019.29 6887175.55
        //673939.27 6887303.61
        //673805.90 6887226.61
        //673862.09 6887087.53
        //673559.20 6886939.80
        //673503.63 6887180.47
        //673792.03 6887500.76
        //673889.99 6887446.46
        //673989.76 6887499.51
        //673994.31 6887412.63
        //674117.95 6887372.46
        //674114.95 6887329.56
        private static string wkt3857_1 =
            "PROJCS[\"WGS 84 / Pseudo-Mercator\", GEOGCS[\"Popular Visualisation CRS\", DATUM[\"Popular_Visualisation_Datum\", " +
            "SPHEROID[\"Popular Visualisation Sphere\", 6378137, 0, AUTHORITY[\"EPSG\", \"7059\"]], TOWGS84[0, 0, 0, 0, 0, 0, 0], AUTHORITY[\"EPSG\", \"6055\"]], " +
            "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], UNIT[\"degree\", 0.01745329251994328, AUTHORITY[\"EPSG\", \"9122\"]], " +
            "AUTHORITY[\"EPSG\", \"4055\"]], UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]], PROJECTION[\"Mercator_1SP\"], PARAMETER[\"central_meridian\", 0], " +
            "PARAMETER[\"scale_factor\", 1], PARAMETER[\"false_easting\", 0], PARAMETER[\"false_northing\", 0], AUTHORITY[\"EPSG\", \"3785\"], AXIS[\"X\", EAST], " +
            "AXIS[\"Y\", NORTH]]";

        private static string wkt3857 = "PROJCS[\"WGS 84 / World Mercator\", GEOGCS[\"WGS 84\", DATUM[\"WGS_1984\", SPHEROID[\"WGS 84\", 6378137, 298.257223563, " +
                    "AUTHORITY[\"EPSG\", \"7030\"]], AUTHORITY[\"EPSG\", \"6326\"]], PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], " +
                    "UNIT[\"degree\", 0.01745329251994328, AUTHORITY[\"EPSG\", \"9122\"]],AUTHORITY[\"EPSG\", \"4326\"]],PROJECTION[\"Mercator_1SP\"], " +
                    "PARAMETER[\"latitude_of_origin\", 45], PARAMETER[\"central_meridian\", 0],PARAMETER[\"scale_factor\", 1],PARAMETER[\"false_easting\", 500000], " +
                    "PARAMETER[\"false_northing\", 0],UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]],AXIS[\"Easting\", EAST],AXIS[\"Northing\", NORTH]," +
                    "AUTHORITY[\"EPSG\", \"3395\"]]";

        private static string wkt3857_2 = "PROJECTION[\"Mercator_1SP\"], " +
                    "PARAMETER[\"latitude_of_origin\", 45], " +
                    "PARAMETER[\"central_meridian\", 0], " +
                    "PARAMETER[\"scale_factor\", 1], " +
                    "PARAMETER[\"false_easting\", 500000], " +
                    "PARAMETER[\"false_northing\", 0], " +
                    "UNIT[\"metre\", 1]";

        private static string wkt_WGS84 = "GEOGCS[\"GCS_WGS_1984\"," +
         "DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]]," +
         "PRIMEM[\"Greenwich\",0]," +
         "UNIT[\"Degree\",0.0174532925199433]]";

        private static string wkt_WGS84_M =
        "PROJCS[\"WGS 84 / World Mercator\", " +
            "GEOGCS[\"WGS 84\", " +
                "DATUM[\"WGS_1984\", " +
                    "SPHEROID[\"WGS 84\", 6378137, 298.257223563, " +
                        "AUTHORITY[\"EPSG\", \"7030\"]], " +
                    "AUTHORITY[\"EPSG\", \"6326\"]], " +
                "PRIMEM[\"Greenwich\", 0, " +
                    "AUTHORITY[\"EPSG\", \"8901\"]], " +
                "UNIT[\"degree\", 0.01745329251994328, AUTHORITY[\"EPSG\", \"9122\"]], " +
                "AUTHORITY[\"EPSG\", \"4326\"]], " +
            "PROJECTION[\"Mercator_1SP\"], " +
            "PARAMETER[\"latitude_of_origin\", 0], " +// required, missing from spatialreference.org
            "PARAMETER[\"central_meridian\", 0], " +
            "PARAMETER[\"scale_factor\", 1], " +
            "PARAMETER[\"false_easting\", 500000], " +
            "PARAMETER[\"false_northing\", 0], " +
            "UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]], " +
            "AXIS[\"Easting\", EAST], " +
            "AXIS[\"Northing\", NORTH], " +
            "AUTHORITY[\"EPSG\", \"3395\"]]";

        private const double PI_2 = Math.PI * 0.5;
        private const double MAX_LAT = 89.5;
        private const double DEG_RAD = Math.PI / 180.0;
        private const double RAD_DEG = 180.0 / Math.PI;
        private const double R_MAJOR = 6378137.0;
        private const double R_MINOR = 6356752.3142;
        private static double ECCENT = Math.Sqrt(1 - Math.Pow(R_MINOR / R_MAJOR, 2));
        private static double ECCNTH = ECCENT * 0.5;

        public static double lonToX(double longitude)
        {
            return longitude * DEG_RAD * R_MAJOR;
        }
        public static double xToLon(double x)
        {
            return x * RAD_DEG / R_MAJOR;
        }

        public static double latToY(double latitude)
        {
            if (latitude > MAX_LAT) latitude = MAX_LAT;
            if (latitude < -MAX_LAT) latitude = -MAX_LAT;

            var phi = latitude * DEG_RAD;
            var con = ECCENT * Math.Sin(phi);
            con = Math.Pow((1.0 - con) / (1.0 + con), ECCNTH);

            return -R_MAJOR * Math.Log(Math.Tan(0.5 * (PI_2 - phi)) / con);
        }

        public static double yToLat(double y)
        {
            var ts = Math.Exp(-y / R_MAJOR);
            var phi = PI_2 - 2.0 * Math.Atan(ts);

            uint i = 0;
            var dPhi = 1.0;
            while ((dPhi >= 0 ? dPhi : -dPhi) > 0.000000001 && i++ < 15)
            {
                var con = ECCENT * Math.Sin(phi);
                dPhi = PI_2 - 2.0 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), ECCNTH)) - phi;
                phi += dPhi;
            }

            return phi * RAD_DEG;
        }

        private static void ToGeographic(ref double mercatorX_lon, ref double mercatorY_lat)
        {
            if (Math.Abs(mercatorX_lon) < 180 && Math.Abs(mercatorY_lat) < 90)
                return;

            if ((Math.Abs(mercatorX_lon) > 20037508.3427892) || (Math.Abs(mercatorY_lat) > 20037508.3427892))
                return;

            double x = mercatorX_lon - 500000.0;
            double y = mercatorY_lat;
            double num3 = x / 6378137.0;
            double num4 = num3 * 57.295779513082323 + 45.0;
            double num5 = Math.Floor((double)((num4 + 180.0) / 360.0));
            double num6 = num4 - (num5 * 360.0);
            double num7 = 1.5707963267948966 - (2.0 * Math.Atan(Math.Exp((-1.0 * y) / 6378137.0)));
            mercatorX_lon = num6;
            mercatorY_lat = num7 * 57.295779513082323;
        }

        private static IGeometry CreateTestGeometry()
        {
            /*
            var cf = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
            var f = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();

            var sys_WGS84 = cf.CreateFromWkt(wkt_WGS84);
            var sys3857 = cf.CreateFromWkt(wkt_WGS84_M);

            var transformTo3875 = f.CreateFromCoordinateSystems(sys3857, sys_WGS84);
            var pg = transformTo3875.MathTransform.Transform(new[] { 461359.52, 6743368.67, 743589.66, 6942723.41 });
            var p = xToLon(461359.52 - 500000) + 45;
            var p2 = xToLon(743589.66 - 500000) + 45;

            var p22 = yToLat(6743368.67);
            var p222 = yToLat(6942723.41);
            var mercatorX_lon = 461359.52;
            var mercatorY_lat = 6743368.67;
            ToGeographic(ref mercatorX_lon, ref mercatorY_lat);
            var p1 = 2000000;
            */
            var p = new List<Point>();
            using (var f = new StreamReader("123.plt"))
            {
                string s;
                while ((s = f.ReadLine()) != null)
                {
                    var sa = s.Split(',');
                    if (sa.Length != 7) continue;
                    double x, y;
                    if (double.TryParse(sa[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x) &&
                        double.TryParse(sa[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                    {
                        p.Add(new Point(lonToX(y), latToY(x)+38640));
                    }
                }
            }
            return new LineString(p);
            /*return new LineString(new[]
            {
                new Point(0, 0),
                new Point(674114.95+p, 6887329.56+p1),
                new Point(674268.58+p, 6887340.31+p1),
                new Point(674288.74+p, 6887298.96+p1),
                new Point(674019.29+p, 6887175.55+p1),
                new Point(673939.27+p, 6887303.61+p1),
                new Point(673805.90+p, 6887226.61+p1),
                new Point(673862.09+p, 6887087.53+p1),
                new Point(673559.20+p, 6886939.80+p1),
                new Point(673503.63+p, 6887180.47+p1),
                new Point(673792.03+p, 6887500.76+p1),
                new Point(673889.99+p, 6887446.46+p1),
                new Point(673989.76+p, 6887499.51+p1),
                new Point(673994.31+p, 6887412.63+p1),
                new Point(674117.95+p, 6887372.46+p1),
                new Point(674114.95+p, 6887329.56+p1),

                new Point(673529.98+p, 6887067.32+p1),
                new Point(673525.48+p, 6887086.81+p1),
                new Point(674054.63+p, 6887392.31+p1),
                new Point(674077.83+p, 6887384.77+p1),
                new Point(673529.98+p, 6887067.32+p1)
                /*new Point(-10000, 10000),
                new Point(0, 0),
                new Point(10000, 10000),
                new Point(10000, 0),
                new Point(0, 10000),
                new Point(100000, 100000),
                new Point(100000, 0),
                new Point(0, 100000),
                new Point(1000000, 1000000),
                new Point(1000000, 0),
                new Point(0, 1000000),
                new Point(10000000, 10000000),
                new Point(10000000, 0),
                new Point(9000000, 1000)*/
            //});
        }

        public TrackProvider()
        {
            _syncRoot = new object();
            _trackFeature = new Feature();
            _positionFeature = new Feature();
            _trackFeature.Geometry = CreateTestGeometry();

            _trackFeature.Styles.Add(CreateTrackStyle());
        }

        public void SetTrack()
        {
            lock (_syncRoot)
            {
            }
        }

        public void ClearTrack()
        {
            lock (_syncRoot)
            {
                _trackFeature.Geometry = null;
                _positionFeature.Geometry = null;
            }
        }


        public IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            lock (_syncRoot)
            {
                var features = new Features();
                if (_trackFeature.Geometry != null && _trackFeature.Geometry.GetBoundingBox().Intersects(box)) features.Add(_trackFeature);
                if (_positionFeature.Geometry != null && _positionFeature.Geometry.GetBoundingBox().Intersects(box)) features.Add(_positionFeature);
                return features;
            }
        }

        public BoundingBox GetExtents()
        {
            lock (_syncRoot)
            {
                return _trackFeature.Geometry != null
                    ? _trackFeature.Geometry.GetBoundingBox()
                    : new BoundingBox(0, 0, 0, 0);
            }
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearTrack();
                    _trackFeature.Dispose();
                    _positionFeature.Dispose();
                }
                _disposed = true;
            }
        }

        ~TrackProvider()
        {
            Dispose();
        }
    }
}
