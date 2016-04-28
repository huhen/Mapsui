using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Mapsui.Styles;
using InteropRender32;
using Bitmap = System.Drawing.Bitmap;

namespace Mapsui.Rendering.OpenGL
{
    public static class TextureHelper
    {
        public static TextureInfo LoadTexture(Stream textureData)
        {
            var textureInfo = new TextureInfo();
            textureData.Position = 0;
            var bitmap = (Bitmap)Image.FromStream(textureData);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // The texture that is loaded below is attached to the TextureID that was bound in an earlier call with 'GL.BindTexture'
            textureInfo.TextureId = OpenGlRender.TexImage2D(bitmapData.Width, bitmapData.Height, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            textureInfo.Width = bitmap.Width;
            textureInfo.Height = bitmap.Height;

            return textureInfo;
        }

        public static void RenderTexture(TextureInfo textureInfo, float x, float y, float orientation = 0,
            float offsetX = 0, float offsetY = 0,
            LabelStyle.HorizontalAlignmentEnum horizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
            LabelStyle.VerticalAlignmentEnum verticalAlignment = LabelStyle.VerticalAlignmentEnum.Center,
            float opacity = 1f,
            float scale = 1f)
        {
            var ox = offsetX + DetermineHorizontalAlignmentCorrection(horizontalAlignment, textureInfo.Width);
            var oy = -offsetY + DetermineVerticalAlignmentCorrection(verticalAlignment, textureInfo.Height);

            var halfWidth = textureInfo.Width / 2;
            var halfHeight = textureInfo.Height / 2;

            var vertextArray = new[]
            {
                ox - halfWidth, oy - halfHeight,
                ox + halfWidth, oy - halfHeight,
                ox + halfWidth, oy + halfHeight,
                ox - halfWidth, oy + halfHeight
            };

            OpenGlRender.RenderTexture(textureInfo.TextureId, x, y, orientation, scale, opacity, vertextArray);
        }

        private static int DetermineHorizontalAlignmentCorrection(LabelStyle.HorizontalAlignmentEnum horizontalAlignment,
            int width)
        {
            if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Left) return width / 2;
            if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Right) return -width / 2;
            return 0; // center
        }

        private static int DetermineVerticalAlignmentCorrection(LabelStyle.VerticalAlignmentEnum verticalAlignment,
            int height)
        {
            if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Top) return -height / 2;
            if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Bottom) return height / 2;
            return 0; // center
        }

    }
}
