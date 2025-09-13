using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ConversorCodificacion.Win.Controles
{
    /// <summary>
    /// Barra de progreso personalizada, dibujada por GDI+, con soporte de colores y texto superpuesto.
    /// Usa BackColor para el fondo y ForeColor para el relleno.
    /// </summary>
    public class ProgressBarEx : Control
    {
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 0;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Comportamiento")]
        public int Minimum
        {
            get => _minimum;
            set { _minimum = Math.Min(value, _maximum); Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Comportamiento")]
        public int Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); if (_value > _maximum) _value = _maximum; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Comportamiento")]
        public int Value
        {
            get => _value;
            set { _value = Math.Max(_minimum, Math.Min(_maximum, value)); Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Apariencia")]
        public Color BorderColor { get; set; } = Color.FromArgb(64, 80, 100);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Apariencia")]
        public bool ShowPercentageText { get; set; } = false; // el texto lo dibuja un label externo normalmente

        public ProgressBarEx()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Color.FromArgb(17, 24, 39); // coherente con tema oscuro
            ForeColor = Color.MediumSeaGreen;       // color del relleno
            Size = new Size(300, 18);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);

            // borde
            using (var pen = new Pen(BorderColor))
            {
                var rb = ClientRectangle;
                rb.Width -= 1; rb.Height -= 1;
                g.DrawRectangle(pen, rb);
            }

            // relleno
            float percent = _maximum == _minimum ? 0f : (float)(_value - _minimum) / (_maximum - _minimum);
            var fill = ClientRectangle;
            fill.Inflate(-1, -1);
            fill.Width = (int)(fill.Width * percent);
            if (fill.Width > 0)
            {
                using var br = new SolidBrush(ForeColor);
                g.FillRectangle(br, fill);
            }

            if (ShowPercentageText)
            {
                string text = _maximum == 0 ? "0%" : $"{(int)(percent * 100)}%";
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var txtBr = new SolidBrush(Color.FromArgb(230, 230, 230));
                g.DrawString(text, Font, txtBr, ClientRectangle, sf);
            }
        }
    }
}
