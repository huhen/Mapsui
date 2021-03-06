﻿using Mapsui.Layers;

namespace Mapsui.Samples.Common
{
    public static class TmsSample
    {
        public static ILayer CreateLayer()
        {
            return new TileLayer(() => TmsTileSourceBuilder.Build("http://geoserver.nl/tiles/tilecache.aspx/1.0.0/worlddark_GM", true))
            {
                Name = "TMS"
            };
        }
    }
}
