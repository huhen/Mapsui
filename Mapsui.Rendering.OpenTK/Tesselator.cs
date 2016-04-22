using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace Mapsui.Rendering.OpenTK
{
    internal class Tesselator : IDisposable
    {
        private IntPtr _tess;

        delegate void BeginCallbackDelegate(BeginMode mode);
        delegate void EndCallbackDelegate();
        delegate void VertexCallbackDelegate(IntPtr v);
        delegate void ErrorCallbackDelegate(GluErrorCode code);
        unsafe delegate void CombineCallbackDelegate(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]double[] coordinates,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]double*[] vertexData,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]float[] weight,
            double** dataOut);

        private static BeginCallbackDelegate tessBegin=new BeginCallbackDelegate(BeginHandler);
        private static EndCallbackDelegate tessEnd = new EndCallbackDelegate(EndHandler);
        private static ErrorCallbackDelegate tessError = new ErrorCallbackDelegate(ErrorHandler);
        private static VertexCallbackDelegate tessVertex = new VertexCallbackDelegate(VertexHandler);
        private static unsafe CombineCallbackDelegate tessCombine = new CombineCallbackDelegate(CombineHandler);

        internal unsafe Tesselator()
        {
            _tess = Glu.NewTess();

            Glu.TessCallback(_tess, TessCallback.TessBegin, tessBegin);
            Glu.TessCallback(_tess, TessCallback.TessEnd, tessEnd);
            Glu.TessCallback(_tess, TessCallback.TessVertex, tessVertex);
            Glu.TessCallback(_tess, TessCallback.TessCombine, tessCombine);
            Glu.TessCallback(_tess, TessCallback.TessError, tessError);
        }

        internal void Tesselate(float[] points)
        {
            Glu.TessBeginPolygon(_tess, IntPtr.Zero);
            Glu.TessBeginContour(_tess);

            double[][] rect = new double[points.Length / 2][];
            for (var i = 0; i < points.Length / 2; i++)
            {
                rect[i] = new double[] { points[i * 2], points[i * 2 + 1], 0.0 };
            }

            for (var i = 0; i < rect.Length; i++)
            {
            //    Glu.TessVertex(_tess, rect[i], rect[i]);
            }

            Glu.TessVertex(_tess, rect[0], rect[0]);
            Glu.TessVertex(_tess, rect[1], rect[1]);
            Glu.TessVertex(_tess, rect[2], rect[2]);
            Glu.TessVertex(_tess, rect[3], rect[3]);

            Glu.TessEndContour(_tess);
            Glu.EndPolygon(_tess);
        }


        private static void BeginHandler(BeginMode mode)
        {
            GL.Begin(mode);
        }

        private static void EndHandler()
        {
            GL.End();
        }

        private static void VertexHandler(IntPtr v)
        {
            unsafe { GL.Vertex3((double*)v); }
        }

        private static void ErrorHandler(GluErrorCode code)
        {
            Debug.Print("GLU Error {0}: {1}", code, Glu.ErrorString(code));
        }

        private static unsafe double*[] combineData;
        private static int data_index = 0;
        private static unsafe void CombineHandler(double[] coordinates, double*[] data, float[] weight, double** dataOut)
        {
            // Workaround Mono 1.2.6 bug with unsafe inline initializers
            if (combineData == null)
                combineData = new double*[16];

            double* out_data = combineData[data_index] = (double*)Marshal.AllocHGlobal(6 * sizeof(double));
            int i;

            out_data[0] = coordinates[0];
            out_data[1] = coordinates[1];
            out_data[2] = coordinates[2];

            for (i = 3; i < 6; i++)
            {
                double* real_data = (double*)data[i - 3];
                out_data[i] = weight[0] * real_data[0] +
                              weight[1] * real_data[1] +
                              weight[2] * real_data[2] +
                              weight[3] * real_data[3];
            }
            data_index++;

            *dataOut = out_data;
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
                    Glu.DeleteTess(_tess);
                }
                _disposed = true;
            }
        }

        ~Tesselator()
        {
            Dispose();
        }
    }
}
