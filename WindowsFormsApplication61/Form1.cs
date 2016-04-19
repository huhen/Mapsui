using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BruTile.Predefined;
using Mapsui.Layers;
using OpenTK.Graphics.OpenGL;

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

            mapGLControl1.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            //mapGLControl1.Map.Layers.Add(LineStringSample.CreateLineStringLayer(LineStringSample.CreateLineStringStyle()));
            

            var l = ShapefileSample.CreateLayers();
            foreach (var ll in l)
            {
                //ll.Opacity = 0.5;
                mapGLControl1.Map.Layers.Add(ll);
            }
            
            //mapGLControl1.ZoomToFullEnvelope();
            mapGLControl1.Refresh();
        }

        private void mapGLControl1_Resize(object sender, EventArgs e)
        {
            //SetupViewport();
            //mapGLControl1.Refresh();
        }
    }
}