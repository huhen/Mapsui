using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities.SpatialIndexing;

namespace MpLayer
{
    public class TrackProvider : IProvider, IDisposable
    {
        private string _crs = "";
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
                Line = { Color = Color.Red, Width = 40 }
            };
        }

        private static IGeometry CreateTestGeometry()
        {
            return new LineString(new[]
            {
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
                new Point(0, 10000000)
            });
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
