namespace WindowsFormsApplication61
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            Mapsui.Map map1 = new Mapsui.Map();
            Mapsui.Styles.Color color1 = new Mapsui.Styles.Color();
            this.mapGLControl1 = new OpenTK.MapGLControl();
            this.SuspendLayout();
            // 
            // mapGLControl1
            // 
            this.mapGLControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapGLControl1.BackColor = System.Drawing.Color.Black;
            this.mapGLControl1.Location = new System.Drawing.Point(12, 12);
            color1.A = 255;
            color1.B = 255;
            color1.G = 255;
            color1.R = 255;
            map1.BackColor = color1;
            map1.CRS = null;
            map1.Lock = false;
            map1.Transformation = null;
            this.mapGLControl1.Map = map1;
            this.mapGLControl1.Name = "mapGLControl1";
            this.mapGLControl1.Size = new System.Drawing.Size(754, 408);
            this.mapGLControl1.TabIndex = 0;
            this.mapGLControl1.VSync = false;
            this.mapGLControl1.Resize += new System.EventHandler(this.mapGLControl1_Resize);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 432);
            this.Controls.Add(this.mapGLControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.MapGLControl mapGLControl1;
    }
}

