using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ConversorCodificacion.Win.Estilos;

namespace ConversorCodificacion.Win.Controles
{
    /// <summary>
    /// Panel estilo "card" con fondo y borde al estilo del tema oscuro.
    /// </summary>
    public class PanelCard : Panel
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RadioBorde { get; set; } = 6;

        public PanelCard()
        {
            DoubleBuffered = true;
            BackColor = TemaOscuro.ColorPanel;
            ForeColor = Color.White;
            Padding = new Padding(8);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            using var bg = new SolidBrush(BackColor);
            using var pen = new Pen(TemaOscuro.ColorBorde, 1);

            if (RadioBorde > 0)
            {
                using var path = CrearRectanguloRedondeado(rect, RadioBorde);
                g.FillPath(bg, path);
                g.DrawPath(pen, path);
            }
            else
            {
                g.FillRectangle(bg, rect);
                g.DrawRectangle(pen, rect);
            }
        }

        private static GraphicsPath CrearRectanguloRedondeado(Rectangle r, int radius)
        {
            int d = radius * 2;
            var gp = new GraphicsPath();
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }
    }
}
