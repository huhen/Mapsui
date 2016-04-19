using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using OpenTK.Graphics.ES11;
using OpenTK.Graphics.OpenGL;

namespace Mapsui.Rendering.OpenTK
{
    public class MapRenderer : IRenderer
    {
        readonly IDictionary<int, TextureInfo> _symbolTextureCache = new Dictionary<int, TextureInfo>();
        readonly IDictionary<object, TextureInfo> _tileTextureCache = new Dictionary<object, TextureInfo>(new IdentityComparer<object>());
        private long _currentIteration;
        private const int TilesToKeepMultiplier = 3;

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
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.DeleteTextures(1, ref textureInfo.TextureId);
            }
            _symbolTextureCache.Clear();
        }

        private void DeleteAllTileTextures()
        {
            foreach (var key in _tileTextureCache.Keys)
            {
                var textureInfo = _tileTextureCache[key];
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.DeleteTextures(1, ref textureInfo.TextureId);
            }
            _tileTextureCache.Clear();
        }

        private void RemoveUnusedTextureInfos()
        {
            var numberOfTilesUsedInCurrentIteration = _tileTextureCache.Values.Count(i => i.IterationUsed == _currentIteration);

            var orderedKeys = _tileTextureCache.OrderBy(kvp => kvp.Value.IterationUsed).Select(kvp => kvp.Key).ToList();

            var counter = 0;
            var tilesToKeep = orderedKeys.Count() * TilesToKeepMultiplier;
            var numberToRemove = numberOfTilesUsedInCurrentIteration - tilesToKeep;
            foreach (var key in orderedKeys)
            {
                if (counter > numberToRemove)
                    break;
                var textureInfo = _tileTextureCache[key];
                _tileTextureCache.Remove(key);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.DeleteTextures(1, ref textureInfo.TextureId);
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
            //try
            //{
            if (feature.Geometry is Point)
            {
                PointRenderer.Draw(viewport, style, feature, _symbolTextureCache);
            }
            else if (feature.Geometry is MultiPoint)
            {
                //LineStringRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is LineString)
            {
                LineStringRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is MultiLineString)
            {
                //LineStringRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is Polygon)
            {
                PolygonRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is MultiPolygon)
            {
                //PolygonRenderer.Draw(viewport, style, feature);
            }
            else if (feature.Geometry is IRaster)
            {
                RasterRenderer.Draw(viewport, style, feature, _tileTextureCache, _currentIteration);
            }
            /*}
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }*/
        }

        public MemoryStream RenderToBitmapStream(IViewport viewport, IEnumerable<ILayer> layers)
        {
            throw new NotImplementedException();
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
