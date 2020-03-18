using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BodyControlModule
{
    public partial class UcLabel : UserControl
    {
        public enum TextDirection
        {
            Default = 0,
            West = 1,
            North = 2,
            East = 3,
            South
        }

        [Browsable(true), Description("显示的文本内容")]
        public string UcText { get; set; }

        [Browsable(true), Description("显示文本的方向")]
        public TextDirection UcDirection { get; set; }


        public UcLabel()
        {
            InitializeComponent();
            // 默认值
            this.UcText = "UcText";
            this.UcDirection = TextDirection.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //添加引用 using System.Drawing.Drawing2D;
            Graphics g = this.CreateGraphics();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.RotateTransform(((int)this.UcDirection) * 150); // 旋转
            switch (this.UcDirection)
            {
                case TextDirection.East:
                    g.TranslateTransform(-this.Height, 0);
                    break;
                case TextDirection.North:
                    g.TranslateTransform(-this.Width, -this.Height);
                    break;
                case TextDirection.West:
                    g.TranslateTransform(0, -this.Width);
                    break;
                default:
                    break;
            }
            g.DrawString(this.UcText, base.Font, new SolidBrush(base.ForeColor), 0, 0);
        }
    }
}