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
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Graphics;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace;
using PolygonMode = OpenTK.Graphics.OpenGL.PolygonMode;
using VertexPointerType = OpenTK.Graphics.OpenGL.VertexPointerType;

namespace Mapsui.Rendering.OpenTK
{
    public class LineStringRenderer
    {
        public static void Draw(IViewport viewport, IStyle style, IFeature feature)
        {
            var lineString = (LineString)feature.Geometry;
            Draw(viewport, lineString, style, feature);
        }

        public static void Draw(IViewport viewport, LineString lineString, IStyle style, IFeature feature)
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

            //float[] points = ToOpenTK(vertices);
            float[] points = ToPolygone(vertices, lineWidth);
            WorldToScreen(viewport, points);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(2, VertexPointerType.Float, 0, points);
            if (drawOutLine)
            {
                //GL.LineWidth(1);
                //GL.Color4((byte)outLineColor.R, (byte)outLineColor.G, (byte)outLineColor.B, (byte)outLineColor.A);
                //GL.DrawArrays(PrimitiveType.LineStrip, 0, points.Length / 2);
            }
            GL.LineWidth(1);
            GL.Color4((byte)lineColor.R, (byte)lineColor.G, (byte)lineColor.B, (byte)lineColor.A);

            //GL.PointSize(5f);
            //GL.LineWidth(1f);
            
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);

            var tess = new Tesselator();
     
            tess.Tesselate(points);
            tess.Dispose();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, points.Length / 2);

            GL.DisableClientState(ArrayCap.VertexArray);
        }

        private static float[] ToPolygone(IList<Point> vertices, float width)
        {
            var countVert = vertices.Count;
            if (countVert < 2) return new float[0];

            var points = new Point2D[(countVert - 1) * 4];

            var lastX = (float)vertices[0].X;
            var lastY = (float)vertices[0].Y;
            for (var i = 1; i < vertices.Count; i++)
            {
                var curX = (float)vertices[i].X;
                var curY = (float)vertices[i].Y;
                var vectX = curX - lastX;
                var vectY = curY - lastY;
                var perpX = -vectY;
                var perpY = vectX;
                var len = (float)Math.Sqrt(perpX * perpX + perpY * perpY);
                len /= width;

                var curI = i * 4;
                points[curI - 4].x = perpX / len + lastX;
                points[curI - 4].y = perpY / len + lastY;
                points[curI - 3].x = perpX / len + curX;
                points[curI - 3].y = perpY / len + curY;
                points[curI - 2].x = -perpX / len + curX;
                points[curI - 2].y = -perpY / len + curY;
                points[curI - 1].x = -perpX / len + lastX;
                points[curI - 1].y = -perpY / len + lastY;

                lastX = curX;
                lastY = curY;
            }

            var p = new List<float>(points.Length * 2);
            var p1 = new List<float>(points.Length * 2);

            p.Add(points[0].x);
            p.Add(points[0].y);//первая точка

            /*p1.Add(points[3].x);
            p1.Add(points[3].y);
            p1.Add(points[0].x);
            p1.Add(points[0].y);*/
            var tmp = new Point2D();
            var lastIntersect = new Point2D();
            lastIntersect.x = points[0].x;
            lastIntersect.y = points[0].y;
            var latUp = true;
            for (var i = 0; i < points.Length - 4; i += 4)
            {
                if (intersection(points[i], points[i + 1], points[i + 4], points[i + 5], ref tmp))
                {
                    p.Add(tmp.x);
                    p.Add(tmp.y);

                    p1.Add(lastIntersect.x);
                    p1.Add(lastIntersect.y);
                    p1.Add(tmp.x);
                    p1.Add(tmp.y);
                    p1.Add(points[i + 3].x);
                    p1.Add(points[i + 3].y);

                    p1.Add(tmp.x);
                    p1.Add(tmp.y);
                    p1.Add(points[i + 2].x);
                    p1.Add(points[i + 2].y);
                    p1.Add(points[i + 3].x);
                    p1.Add(points[i + 3].y);

                    p1.Add(tmp.x);
                    p1.Add(tmp.y);
                    p1.Add(points[i + 7].x);
                    p1.Add(points[i + 7].y);
                    p1.Add(points[i + 2].x);
                    p1.Add(points[i + 2].y);

                    latUp = true;
                }
                else//линии не пересекаются, соединяем
                {
                    p.Add(points[i + 1].x);
                    p.Add(points[i + 1].y);
                    p.Add(points[i + 4].x);
                    p.Add(points[i + 4].y);

                    intersection(points[i + 3], points[i + 2], points[i + 7], points[i + 6], ref tmp);

                    p1.Add(lastIntersect.x);
                    p1.Add(lastIntersect.y);
                    p1.Add(points[i+1].x);
                    p1.Add(points[i+1].y);
                    p1.Add(tmp.x);
                    p1.Add(tmp.y);

                    if (latUp)
                    {
                        p1.Add(tmp.x);
                        p1.Add(tmp.y);
                        p1.Add(points[i + 3].x);
                        p1.Add(points[i + 3].y);
                        p1.Add(lastIntersect.x);
                        p1.Add(lastIntersect.y);

                        p1.Add(points[i + 1].x);
                        p1.Add(points[i + 1].y);
                        p1.Add(points[i + 4].x);
                        p1.Add(points[i + 4].y);
                        p1.Add(tmp.x);
                        p1.Add(tmp.y);
                    }
                    else
                    {
                        p1.Add(tmp.x);
                        p1.Add(tmp.y);
                        p1.Add(points[i + 0].x);
                        p1.Add(points[i + 0].y);
                        p1.Add(points[i + 1].x);
                        p1.Add(points[i + 1].y);

                        p1.Add(points[i + 1].x);
                        p1.Add(points[i + 1].y);
                        p1.Add(points[i + 4].x);
                        p1.Add(points[i + 4].y);
                        p1.Add(tmp.x);
                        p1.Add(tmp.y);
                    }

                    latUp = false;
                }
                lastIntersect.x = tmp.x;
                lastIntersect.y = tmp.y;
            }

            p.Add(points[points.Length - 3].x);
            p.Add(points[points.Length - 3].y);
            p.Add(points[points.Length - 2].x);
            p.Add(points[points.Length - 2].y);

            for (var i = points.Length - 1; i > 4; i -= 4)
            {
                if (intersection(points[i - 1], points[i], points[i - 5], points[i - 4], ref tmp))
                {
                    p.Add(tmp.x);
                    p.Add(tmp.y);
                }
                else//линии не пересекаются, соединяем
                {
                    p.Add(points[i].x);
                    p.Add(points[i].y);
                    p.Add(points[i - 5].x);
                    p.Add(points[i - 5].y);
                }
            }
            p.Add(points[3].x);
            p.Add(points[3].y);
            //p.Add(points[0].x);
            //p.Add(points[0].y);
            return p.ToArray();
        }

        internal struct Point2D
        {
            internal float x;
            internal float y;
        }

        private static bool intersection(Point2D start1, Point2D end1, Point2D start2, Point2D end2, ref Point2D out_intersection)
        {
            //Point2D dir1 = end1 - start1;
            var dir1 = new Point2D() { x = end1.x - start1.x, y = end1.y - start1.y };
            //Point2D dir2 = end2 - start2;
            var dir2 = new Point2D() { x = end2.x - start2.x, y = end2.y - start2.y };

            //считаем уравнения прямых проходящих через отрезки
            var a1 = -dir1.y;
            var b1 = +dir1.x;
            var d1 = -(a1 * start1.x + b1 * start1.y);

            var a2 = -dir2.y;
            var b2 = +dir2.x;
            var d2 = -(a2 * start2.x + b2 * start2.y);

            //подставляем концы отрезков, для выяснения в каких полуплоскотях они
            var seg1_line2_start = a2 * start1.x + b2 * start1.y + d2;
            var seg1_line2_end = a2 * end1.x + b2 * end1.y + d2;

            var seg2_line1_start = a1 * start2.x + b1 * start2.y + d1;
            var seg2_line1_end = a1 * end2.x + b1 * end2.y + d1;

            //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.
            if ((seg1_line2_start * seg1_line2_end >= 0.0f) || (seg2_line1_start * seg2_line1_end >= 0.0f)) return false;

            var u = seg1_line2_start / (seg1_line2_start - seg1_line2_end);
            //out_intersection = start1 + u * dir1;
            out_intersection.x = start1.x + u * dir1.x;
            out_intersection.y = start1.y + u * dir1.y;

            if ((float.IsNaN(out_intersection.x)) || (float.IsNaN(out_intersection.y))) return false;
            return true;
        }

        private static float[] ToOpenTK(IList<Point> vertices)
        {
            const int dimensions = 2; // x and y are both in one array
            int numberOfCoordinates = vertices.Count * 2 - 2; // Times two because of duplicate begin en end. Minus two because the very begin and end need no duplicate
            var points = new float[numberOfCoordinates * dimensions];

            for (var i = 0; i < vertices.Count - 1; i++)
            {
                points[i * 4 + 0] = (float)vertices[i].X;
                points[i * 4 + 1] = (float)vertices[i].Y;
                points[i * 4 + 2] = (float)vertices[i + 1].X;
                points[i * 4 + 3] = (float)vertices[i + 1].Y;
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
