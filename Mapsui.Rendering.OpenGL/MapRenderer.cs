using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InteropRender32;

namespace Mapsui.Rendering.OpenGL
{
    public class MapRenderer : IRenderer
    {
        readonly IDictionary<int, TextureInfo> _symbolTextureCache = new Dictionary<int, TextureInfo>();
        readonly IDictionary<object, TextureInfo> _tileTextureCache = new Dictionary<object, TextureInfo>(new IdentityComparer<object>());
        private long _currentIteration;
        private const int _tilesToKeepMultiplier = 3;

        private OpenGlRender _gl = new OpenGlRender();

        public void Render(IViewport viewport, IEnumerable<ILayer> layers)
        {

            layers = layers.ToList();

            SetAllTextureInfosToUnused();

            VisibleFeatureIterator.IterateLayers(viewport, layers, RenderFeature);

            RemoveUnusedTextureInfos();

            _currentIteration++;

        }

        public void DeleteAllBoundTextures()
        {
            DeleteAllTileTextures();
            DeleteAllSymbolTextures();
        }

        private void DeleteAllSymbolTextures()
        {
            foreach (var key in _symbolTextureCache.Keys)
            {
                var textureInfo = _symbolTextureCache[key];
                OpenGlRender.DeleteTexture(textureInfo.TextureId);
            }
            _symbolTextureCache.Clear();
        }

        private void DeleteAllTileTextures()
        {
            foreach (var key in _tileTextureCache.Keys)
            {
                var textureInfo = _tileTextureCache[key];
                OpenGlRender.DeleteTexture(textureInfo.TextureId);
            }
            _tileTextureCache.Clear();
        }

        private void RemoveUnusedTextureInfos()
        {
            var numberOfTilesUsedInCurrentIteration = _tileTextureCache.Values.Count(i => i.IterationUsed == _currentIteration);

            var orderedKeys = _tileTextureCache.OrderBy(kvp => kvp.Value.IterationUsed).Select(kvp => kvp.Key).ToList();

            var counter = 0;
            var tilesToKeep = orderedKeys.Count() * _tilesToKeepMultiplier;
            var numberToRemove = numberOfTilesUsedInCurrentIteration - tilesToKeep;
            foreach (var key in orderedKeys)
            {
                if (counter > numberToRemove)
                    break;
                var textureInfo = _tileTextureCache[key];
                _tileTextureCache.Remove(key);
                OpenGlRender.DeleteTexture(textureInfo.TextureId);
                counter++;
            }
        }

        private void SetAllTextureInfosToUnused()
        {
            foreach (var key in _tileTextureCache.Keys.ToList())
            {
                var textureInfo = _tileTextureCache[key];
                textureInfo.IterationUsed = _currentIteration;
                _tileTextureCache[key] = textureInfo;
            }
        }

        private void RenderFeature(IViewport viewport, IStyle style, IFeature feature)
        {
            if (feature.Geometry is Point)
            {
                PointRenderer.Draw(viewport, style, feature, _symbolTextureCache);
            }
            else if (feature.Geometry is MultiPoint)
            {
                MultiPointRenderer.Draw(viewport, style, feature, _symbolTextureCache);
            }
            else if (feature.Geometry is LineString)
            {
                LineStringRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is MultiLineString)
            {
                MultiLineStringRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is Polygon)
            {
                PolygonRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is MultiPolygon)
            {
                MultiPolygonRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is IRaster)
            {
                RasterRenderer.Draw(viewport, style, feature, _tileTextureCache, _currentIteration);
            }
        }

        public MemoryStream RenderToBitmapStream(IViewport viewport, IEnumerable<ILayer> layers)
        {
            throw new NotImplementedException();
        }

        public void OnHandleCreated(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public void OnHandleDestroyed()
        {
            /*
            if (context != null)
            {
                context.Dispose();
                context = null;
            }

            if (implementation != null)
            {
                implementation.WindowInfo.Dispose();
                implementation = null;
            }*/
        }

        public void Clear()
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        void ValidateState()
        {
            /*if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!IsHandleCreated)
                CreateControl();

            if (implementation == null || context == null || context.IsDisposed)
                RecreateHandle();*/
        }

        public void SetupViewport(int width, int height)
        {
            /*var w = Width;
            var h = Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, h, 0, -1, 1); // Верхний левый угол имеет кооординаты(0, 0)
            GL.Viewport(0, 0, w, h); // Использовать всю поверхность GLControl под рисование*/
            //GL.ClearColor(Color.SkyBlue);
        }

        public void SwapBuffers()
        {
            /* ValidateState();
             Context.SwapBuffers();*/
        }


        public void OnParentChanged()
        {

        }
    }

    public class IdentityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T obj, T otherObj)
        {
            return obj == otherObj;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
