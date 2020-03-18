using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BodyControlModule
{
    public partial class TLable : Label
    {
        private float _angle;
        [Browsable(true)]
        [Description("The angle to rotate the text"), Category("Appearance"), DefaultValue("0")]
        public float Angle { get { return _angle; } set { _angle = value; Invalidate(); } }


        public TLable()
        {
            this.AutoSize = false;
            this.BackColor = Color.Transparent;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            float w = Width;
            float h = Height;
            //将graphics坐标原点移到矩形中心点
            g.TranslateTransform(w / 2, h / 2);
            g.RotateTransform(Angle);
            SizeF sz = g.MeasureString(Text, this.Font);
            float x = -sz.Width / 2;
            float y = -sz.Height / 2;
            Brush brush = new SolidBrush(this.ForeColor);
            g.DrawString(Text, this.Font, brush, new PointF(x, y));
        }
    }
}
