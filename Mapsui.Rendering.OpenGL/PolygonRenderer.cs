using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using System.Collections.Generic;
using InteropRender32;

namespace Mapsui.Rendering.OpenGL
{
    class PolygonRenderer
    {
        public static void Draw(IViewport viewport, IStyle style, IFeature feature)
        {
            var polygon = (Polygon) feature.Geometry;
            Draw(viewport, polygon, style, feature);
        }

        public static void Draw(IViewport viewport, Polygon polygon, IStyle style, IFeature feature)
        {
            var lineString = polygon.ExteriorRing.Vertices;

            float lineWidth = 1;
            var lineColor = Color.Black; // default
            var fillColor = Color.Gray; // default

            var vectorStyle = style as VectorStyle;

            if (vectorStyle != null && vectorStyle.Outline!=null && vectorStyle.Fill!=null)
            {
                lineWidth = (float)vectorStyle.Outline.Width;
                lineColor = vectorStyle.Outline.Color;
                fillColor = vectorStyle.Fill.Color;
            }

            float[] points = ToOpenTK(lineString);
            WorldToScreen(viewport, points);

            // Fill
            // Not implemented. It might be hard to draw a concave polygon with holes.             
            
            // Outline
            OpenGlRender.DrawSimplePolygone(points, lineWidth, lineColor.ToArgb);
        }

        private static float[] ToOpenTK(IList<Point> vertices)
        {
            const int dimensions = 2; // x and y are both in one array
            var points = new float[vertices.Count * dimensions];

            for (var i = 0; i < vertices.Count; i++)
            {
                points[i * 2 + 0] = (float)vertices[i].X;
                points[i * 2 + 1] = (float)vertices[i].Y;
            }

            return points;
        }

        private static void WorldToScreen(IViewport viewport, float[] points)
        {
            for (var i = 0; i < points.Length / 2; i++)
            {
                var point = viewport.WorldToScreen(points[i * 2], points[i * 2 + 1]);
                points[i * 2] = (float)point.X;
                points[i * 2 + 1] = (float)point.Y;
            }
        }
    }
}
