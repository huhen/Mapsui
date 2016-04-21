using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BruTile.Predefined;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using MpLayer;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace WindowsFormsApplication61
{
    public partial class Form1 : Form
    {
        private bool _loaded = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.SkyBlue);
            //SetupViewport();
            _loaded = true;

            var ts = KnownTileSources.Create();
            ts.Name = "OSM";
            //ts.Schema.Srs = "EPSG:3857";
            var tl = new TileLayer(ts);
            //var s = tl.IsCrsSupported("wgs84");
            mapGLControl1.Map.Layers.Add(tl);
            mapGLControl1.Map.Layers.Add(new TrackLayer());
            //mapGLControl1.Map.Layers.Add(LineStringSample.CreateLineStringLayer(LineStringSample.CreateLineStringStyle()));

            /*var style= new LabelStyle();
            style.LabelMethod = GetText;
            style.BackColor=new Brush(Mapsui.Styles.Color.FromArgb(0,255,255,255));
            
            var ml = new MemoryLayer();
            var p = new Point(0, 0);
            
             var p1 = new Point(11110, 111110);
            IFeature
            var mp = new MemoryProvider(new [] {p, p1 });
            
            ml.DataSource = mp;
            ml.Name="TestLabel";
            ml.Style = style;
            
            mapGLControl1.Map.Layers.Add(ml);*/

            //var l = ShapefileSample.CreateLayers();
            //foreach (var ll in l)
            //{
            //ll.Opacity = 0.5;
            //mapGLControl1.Map.Layers.Add(ll);
            //}

            //mapGLControl1.ZoomToFullEnvelope();
            mapGLControl1.Refresh();
        }

        private string GetText(IFeature arg)
        {
            return "t";
        }

        private void mapGLControl1_Resize(object sender, EventArgs e)
        {
            //SetupViewport();
            //mapGLControl1.Refresh();
        }
    }
}