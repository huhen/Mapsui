using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;

namespace MpLayer
{
    public class TrackLayer : Layer
    {
        private TrackProvider _trackProvider;

        public TrackLayer() : this("TrackLayer") { }

        public TrackLayer(string layername) : base(layername)
        {
            Style = CreateStyle();
            CRS = "EPSG:3857";
            _trackProvider = new TrackProvider();
            DataSource = _trackProvider;
        }

        private IStyle CreateStyle()
        {
            return new VectorStyle
            {
                Fill = null,
                Outline = null,
                Line = { Color = Color.Red, Width = 4 }
            };
        }
    }
}
