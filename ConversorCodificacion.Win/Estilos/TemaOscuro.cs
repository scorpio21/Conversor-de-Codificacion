using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConversorCodificacion.Win.Estilos
{
    /// <summary>
    /// Aplica un tema oscuro a una jerarquía de controles WinForms.
    /// Paleta inspirada en el diseño web del proyecto.
    /// </summary>
    public static class TemaOscuro
    {
        // Paleta
        private static readonly Color Fondo = ColorTranslator.FromHtml("#0F172A");     // fondo general
        private static readonly Color FondoPanel = ColorTranslator.FromHtml("#111827");     // contenedores/inputs
        private static readonly Color Borde = ColorTranslator.FromHtml("#1F2937");     // bordes sutiles
        private static readonly Color Texto = ColorTranslator.FromHtml("#E5E7EB");     // texto claro
        private static readonly Color TextoSec = ColorTranslator.FromHtml("#9CA3AF");  // texto secundario
        private static readonly Color Primario = ColorTranslator.FromHtml("#2563EB");  // botón primario
        private static readonly Color PrimarioHover = ColorTranslator.FromHtml("#1D4ED8");
        private static readonly Color Secundario = ColorTranslator.FromHtml("#374151"); // botón secundario
        private static readonly Color SecundarioHover = ColorTranslator.FromHtml("#4B5563");

        // Exponer algunos colores para usos puntuales (p.ej., dibujar borde de cards)
        public static Color ColorBorde => Borde;
        public static Color ColorPanel => FondoPanel;

        public static void Aplicar(Control raiz)
        {
            if (raiz is null) return;

            if (raiz is Form f)
            {
                f.BackColor = Fondo;
                f.ForeColor = Texto;
                f.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            }

            EstilizarControl(raiz);

            foreach (Control c in raiz.Controls)
                Aplicar(c);
        }

        private static void EstilizarControl(Control c)
        {
            switch (c)
            {
                case Label lbl:
                    lbl.ForeColor = Texto;
                    break;
                case TextBox txt:
                    txt.BackColor = FondoPanel;
                    txt.ForeColor = Texto;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case Button btn:
                    EstilizarBotonSecundario(btn);
                    break;
                case GroupBox gb:
                    gb.ForeColor = Texto;
                    gb.BackColor = Color.Transparent;
                    break;
                case RadioButton rb:
                    rb.ForeColor = Texto;
                    break;
                case CheckBox cb:
                    cb.ForeColor = Texto;
                    break;
                case ListView lv:
                    lv.BackColor = FondoPanel;
                    lv.ForeColor = Texto;
                    lv.BorderStyle = BorderStyle.FixedSingle;
                    lv.GridLines = true;
                    break;
                default:
                    // Contenedor genérico
                    if (c is Panel or FlowLayoutPanel or TableLayoutPanel)
                    {
                        c.BackColor = FondoPanel;
                        c.ForeColor = Texto;
                    }
                    break;
            }
        }

        public static void EstilizarBotonPrimario(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Primario;
            btn.BackColor = Primario;
            btn.ForeColor = Color.White;
            btn.MouseEnter += (_, __) => btn.BackColor = PrimarioHover;
            btn.MouseLeave += (_, __) => btn.BackColor = Primario;
        }

        public static void EstilizarBotonSecundario(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Borde;
            btn.BackColor = Secundario;
            btn.ForeColor = Texto;
            btn.MouseEnter += (_, __) => btn.BackColor = SecundarioHover;
            btn.MouseLeave += (_, __) => btn.BackColor = Secundario;
        }
    }
}
