using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using InteropRender32;

namespace Mapsui.Rendering.OpenGL
{
    /// <summary>
    /// <remarks>This class is specific for the Windows Desktop platform</remarks>
    /// </summary>
    public static class PlatformTextureLoader
    {
        public static void TexImage2D(Stream data, out int width, out int height)
        {
            data.Position = 0;
            var bitmap = (Bitmap)Image.FromStream(data);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // The texture that is loaded below is attached to the TextureID that was bound in an earlier call with 'GL.BindTexture'
            OpenGlRender.TexImage2D(bitmapData.Width, bitmapData.Height, bitmapData.Scan0); 
            bitmap.UnlockBits(bitmapData);
            width = bitmap.Width;
            height = bitmap.Height;
        }
    }
}
