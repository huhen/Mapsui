using System.Collections.Generic;
using System.Linq;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
//using OpenTK.Graphics.ES11;
using OpenTK.Graphics.OpenGL;

namespace Mapsui.Rendering.OpenTK
{
    public class PointRenderer
    {
        public static void Draw(IViewport viewport, IStyle style, IFeature feature, IDictionary<int, TextureInfo> bitmapCache)
        {
            var point = feature.Geometry as Point;
            Draw(viewport, point, style, feature, bitmapCache);
         }

        public static void Draw(IViewport viewport, Point point, IStyle style, IFeature feature, IDictionary<int, TextureInfo> bitmapCache)
        {
            var destination = viewport.WorldToScreen(point);

            if (style is LabelStyle)
            {
                var labelStyle = (LabelStyle)style;
                LabelRenderer.Draw(labelStyle, labelStyle.GetLabelText(feature), (float)destination.X, (float)destination.Y);
            }
            var symbolStyle = style as SymbolStyle;
            if (symbolStyle != null && symbolStyle.BitmapId >= 0) DrawPointWithSymbolStyle(symbolStyle, destination, bitmapCache);
            else if (style is VectorStyle) DrawPointWithVectorStyle((VectorStyle)style, destination);
        }

        private static void DrawPointWithVectorStyle(VectorStyle vectorStyle, Point destination)
        {
            var color = vectorStyle.Fill.Color;
            GL.Color4((byte) color.R, (byte) color.G, (byte) color.B, (byte) color.A);
            GL.PointSize((float) SymbolStyle.DefaultWidth);
            GL.EnableClientState(ArrayCap.VertexArray);
            var destAsArray = new[] {(float) destination.X, (float) destination.Y};
            GL.VertexPointer(2, VertexPointerType.Float, 0, destAsArray);
            GL.DrawArrays(BeginMode.Points, 0, 1);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        private static void DrawPointWithSymbolStyle(SymbolStyle symbolStyle, Point destination, IDictionary<int, TextureInfo> bitmapCache)
        {
            TextureInfo textureInfo;
            if (!bitmapCache.Keys.Contains(symbolStyle.BitmapId))
            {
                textureInfo = TextureHelper.LoadTexture(BitmapRegistry.Instance.Get(symbolStyle.BitmapId));
                bitmapCache[symbolStyle.BitmapId] = textureInfo;
            }
            else
            {
                textureInfo = bitmapCache[symbolStyle.BitmapId];
            }

            TextureHelper.RenderTexture(textureInfo, (float)destination.X, (float)destination.Y, 
                (float)symbolStyle.SymbolRotation, 
                (float)symbolStyle.SymbolOffset.X, (float)symbolStyle.SymbolOffset.Y, 
                opacity:(float)symbolStyle.Opacity, scale:(float)symbolStyle.SymbolScale);
        }
    }
}
