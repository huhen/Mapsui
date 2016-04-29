// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Copyright 2010 - Paul den Dulk (Geodan) - Adapted SharpMap for Mapsui
//
// This file is part of Mapsui.
// Mapsui is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Mapsui is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Mapsui; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using System.Collections.Generic;
using InteropRender32;

namespace Mapsui.Rendering.OpenGL
{
    public class LineStringRenderer
    {
        public static void Draw(OpenGlRender _gl, IViewport viewport, IStyle style, IFeature feature)
        {
            var lineString = (LineString)feature.Geometry;
            Draw(_gl, viewport, lineString, style, feature);
        }

        public static void Draw(OpenGlRender _gl, IViewport viewport, LineString lineString, IStyle style, IFeature feature)
        {
            var vertices = lineString.Vertices;

            float lineWidth = 1;
            var lineColor = Color.White;
            float outLineWidth = 1;
            var outLineColor = Color.White;
            var drawOutLine = false;

            var vectorStyle = style as VectorStyle;

            if (vectorStyle != null && vectorStyle.Line != null)
            {
                lineWidth = (float)vectorStyle.Line.Width;
                lineColor = vectorStyle.Line.Color;
                if (vectorStyle.Outline != null)
                {
                    outLineWidth = lineWidth + (float)vectorStyle.Outline.Width * 2;
                    outLineColor = vectorStyle.Outline.Color;
                    drawOutLine = true;
                }
            }

            float[] points = ToOpenTK(vertices);
            //float[] points = ToPolygone(vertices, lineWidth);
            WorldToScreen(viewport, points);
            _gl.DrawFinePolyLine(points, lineWidth, lineColor.ToArgb());
            //OpenGlRender.DrawSimplePolyLine(points, lineWidth, lineColor.ToArgb());
        }

        private static float[] ToOpenTK(IList<Point> vertices)
        {
            const int dimensions = 2; // x and y are both in one array
            int numberOfCoordinates = vertices.Count * 2 - 2; // Times two because of duplicate begin en end. Minus two because the very begin and end need no duplicate
            var points = new float[vertices.Count * dimensions];

            for (var i = 0; i < vertices.Count - 1; i++)
            {
                points[i * 2 + 0] = (float)vertices[i].X;
                points[i * 2 + 1] = (float)vertices[i].Y;
                //points[i * 4 + 2] = (float)vertices[i + 1].X;
                //points[i * 4 + 3] = (float)vertices[i + 1].Y;
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
