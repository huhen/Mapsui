using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Mapsui;
using Mapsui.Rendering.OpenGL;
using Mapsui.Fetcher;
using Mapsui.Utilities;

namespace Mapsui.UI.OpenGL
{
    /// <summary>
    /// OpenGL-aware WinForms control.
    /// The WinForms designer will always call the default constructor.
    /// Inherit from this class and call one of its specialized constructors
    /// to enable antialiasing or custom <see cref="GraphicsMode"/>s.
    /// </summary>
    public partial class MapGLControl : UserControl
    {
        //======MapControl==========
        private Map _map;
        private string _errorMessage;
        //private Bitmap _buffer;
        //private Graphics _bufferGraphics;
        //private readonly Brush _whiteBrush = new SolidBrush(Color.White);
        private Mapsui.Geometries.Point _mousePosition;
        //Indicates that a redraw is needed. This often coincides with 
        //manipulation but not in the case of new data arriving.
        private bool _viewInitialized;
        private readonly MapRenderer _renderer = new MapRenderer();

        public event EventHandler ErrorMessageChanged;

        public IViewport Transform => _map.Viewport;

        public Map Map
        {
            get
            {
                return _map;
            }
            set
            {
                var temp = _map;
                _map = null;

                if (temp != null)
                {
                    temp.DataChanged -= MapDataChanged;
                    temp.Dispose();
                }

                _map = value;
                _map.DataChanged += MapDataChanged;

                ViewChanged(true);
                Invalidate();
            }
        }

        void MapDataChanged(object sender, DataChangedEventArgs e)
        {
            //ViewChanged should not be called here. This would cause a loop
            BeginInvoke((Action)(() => DataChanged(sender, e)));
        }

        public void ZoomIn()
        {
            Map.Viewport.Resolution = ZoomHelper.ZoomIn(_map.Resolutions, Map.Viewport.Resolution);
            ViewChanged(true);
            Invalidate();
        }

        public void ZoomIn(PointF mapPosition)
        {
            // When zooming in we want the mouse position to stay above the same world coordinate.
            // We do that in 3 steps.

            // 1) Temporarily center on where the mouse is
            Map.Viewport.Center = Map.Viewport.ScreenToWorld(mapPosition.X, mapPosition.Y);

            // 2) Then zoom 
            Map.Viewport.Resolution = ZoomHelper.ZoomIn(_map.Resolutions, Map.Viewport.Resolution);

            // 3) Then move the temporary center back to the mouse position
            Map.Viewport.Center = Map.Viewport.ScreenToWorld(
              Map.Viewport.Width - mapPosition.X,
              Map.Viewport.Height - mapPosition.Y);

            ViewChanged(true);
            Invalidate();
        }

        public void ZoomOut()
        {
            Map.Viewport.Resolution = ZoomHelper.ZoomOut(_map.Resolutions, Map.Viewport.Resolution);
            ViewChanged(true);
            Invalidate();
        }

        private void ViewChanged(bool changeEnd)
        {
            _map?.ViewChanged(changeEnd);
        }

        private void DataChanged(object sender, DataChangedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                Invalidate();
            }
            else if (e.Cancelled)
            {
                _errorMessage = "Cancelled";
                OnErrorMessageChanged();
            }
            else if (e.Error is System.Net.WebException)
            {
                _errorMessage = "WebException: " + e.Error.Message;
                OnErrorMessageChanged();
            }
            else if (e.Error == null)
            {
                _errorMessage = "Unknown Exception";
                OnErrorMessageChanged();
            }
            else
            {
                _errorMessage = "Exception: " + e.Error.Message;
                OnErrorMessageChanged();
            }
        }

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            _mousePosition = new Mapsui.Geometries.Point(e.X, e.Y);
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_mousePosition == null) return;
                var newMousePosition = new Mapsui.Geometries.Point(e.X, e.Y);
                MapTransformHelpers.Pan(Map.Viewport, newMousePosition, _mousePosition);
                _mousePosition = newMousePosition;

                ViewChanged(false);
                Invalidate();
            }
        }

        private void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_mousePosition == null) return;
                var newMousePosition = new Mapsui.Geometries.Point(e.X, e.Y);
                MapTransformHelpers.Pan(Map.Viewport, newMousePosition, _mousePosition);
                _mousePosition = newMousePosition;

                ViewChanged(true);
                Invalidate();
            }
        }

        private void InitializeView()
        {
            if (double.IsNaN(Width) || Width == 0) return;
            if (_map?.Envelope == null || double.IsNaN(_map.Envelope.Width) || _map.Envelope.Width <= 0) return;
            if (_map.Envelope.GetCentroid() == null) return;

            Map.Viewport.Center = _map.Envelope.GetCentroid();
            Map.Viewport.Resolution = _map.Envelope.Width / Width;
            _viewInitialized = true;
            ViewChanged(true);
        }

        private void MapControl_Disposed(object sender, EventArgs e)
        {
            Map.Dispose();
        }

        protected void OnErrorMessageChanged()
        {
            ErrorMessageChanged?.Invoke(this, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode) base.OnPaintBackground(e);
            //by overriding this method and not calling the base class implementation 
            //we prevent flickering.
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                ZoomIn(e.Location);
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }
        //======EndMapControl==========

        bool resize_event_suppressed;
        // Indicates whether the control is in design mode. Due to issues
        // wiith the DesignMode property and nested controls,we need to
        // evaluate this in the constructor.
        bool design_mode;

        #region --- Constructors ---

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public MapGLControl()
        {
            //===MapControl====
            Map = new Map();
            MouseDown += MapControl_MouseDown;
            MouseMove += MapControl_MouseMove;
            MouseUp += MapControl_MouseUp;
            Disposed += MapControl_Disposed;
            //===EndMapControl====

            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Note: the DesignMode property may be incorrect when nesting controls.
            // We use LicenseManager.UsageMode as a workaround (this only works in
            // the constructor).
            design_mode =
                DesignMode ||
                LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            InitializeComponent();
        }

        #endregion

        #region --- Protected Methods ---

        /// <summary>
        /// Gets the <c>CreateParams</c> instance for this <c>GLControl</c>
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_VREDRAW = 0x1;
                const int CS_HREDRAW = 0x2;
                const int CS_OWNDC = 0x20;

                CreateParams cp = base.CreateParams;

                cp.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;

                return cp;
            }
        }

        /// <summary>Raises the HandleCreated event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            DoubleBuffered = false;

            _renderer.OnHandleCreated(Handle);
            base.OnHandleCreated(e);

            if (resize_event_suppressed)
            {
                OnResize(EventArgs.Empty);
                resize_event_suppressed = false;
            }
        }

        /// <summary>Raises the HandleDestroyed event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Ensure that context is still alive when passing to events
            // => This allows to perform cleanup operations in OnHandleDestroyed handlers
            base.OnHandleDestroyed(e);
            _renderer.OnHandleDestroyed();
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.Paint event.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (design_mode)
            {
                e.Graphics.Clear(BackColor);
                base.OnPaint(e);
                return;
            }

            base.OnPaint(e);

            //===MapControl===
            if (!_viewInitialized) InitializeView();
            if (!_viewInitialized) return; //initialize in the line above failed. 

            _renderer.Clear();
            _renderer.Render(Map.Viewport, _map.Layers);
            _renderer.SwapBuffers();
            //===EndMapControl====
        }

        /// <summary>
        /// Raises the Resize event.
        /// Note: this method may be called before the OpenGL context is ready.
        /// Check that IsHandleCreated is true before using any OpenGL methods.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            // Do not raise OnResize event before the handle and context are created.
            if (!IsHandleCreated)
            {
                resize_event_suppressed = true;
                return;
            }

            base.OnResize(e);

            if (Width == 0) return;
            if (Height == 0) return;

            Map.Viewport.Width = Width;
            Map.Viewport.Height = Height;

            _renderer.SetupViewport(ClientSize.Width, ClientSize.Height);

            ViewChanged(true);
            Invalidate();
        }


        /// <summary>
        /// Raises the ParentChanged event.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            _renderer.OnParentChanged();
            base.OnParentChanged(e);
        }

        #endregion


        /// <summary>
        /// Gets the aspect ratio of this MapGLControl.
        /// </summary>
        public float AspectRatio => ClientSize.Width / (float)ClientSize.Height;
    }
}
