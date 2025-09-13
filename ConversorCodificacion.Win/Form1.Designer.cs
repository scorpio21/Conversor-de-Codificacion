namespace ConversorCodificacion.Win;

partial class Form1
{
    /// <summary>
    ///  Variable del diseñador necesaria.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.TextBox txtCarpeta;
    private System.Windows.Forms.Button btnProyecto;
    private System.Windows.Forms.Button btnElegir;
    private System.Windows.Forms.Label lblCarpeta;
    private System.Windows.Forms.GroupBox grpDireccion;
    private System.Windows.Forms.RadioButton rbAUtf8;
    private System.Windows.Forms.RadioButton rbAWin1252;
    private System.Windows.Forms.RadioButton rbFixMojibake;
    private System.Windows.Forms.RadioButton rbRestoreBak;
    private System.Windows.Forms.Label lblExtensiones;
    private System.Windows.Forms.TextBox txtExtensiones;
    private System.Windows.Forms.CheckBox chkRecursivo;
    private System.Windows.Forms.CheckBox chkBackup;
    private System.Windows.Forms.Button btnConvertir;
    private System.Windows.Forms.ListView lvResultados;
    private System.Windows.Forms.ColumnHeader colArchivo;
    private System.Windows.Forms.ColumnHeader colMensaje;
    private System.Windows.Forms.Label lblResumen;
    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.Label lblSubtitulo;
    private ConversorCodificacion.Win.Controles.PanelCard pnlCard;
    private System.Windows.Forms.TableLayoutPanel tblCard;
    private System.Windows.Forms.FlowLayoutPanel flpChecks;
    private ConversorCodificacion.Win.Controles.ProgressBarEx pbProgreso;
    private System.Windows.Forms.Label lblProgreso;
    private System.Windows.Forms.Panel pnlProgresoHost;
    private System.Windows.Forms.Label lblPct;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem abrirLogToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem abrirConfigToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem estilosBarraToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem barraBloquesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem barraContinuaToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem barraIndeterminadaToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorBarraToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorVerdeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorAmarilloToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorRojoToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorAzulToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorCianToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem colorElegirToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ayudaInstruccionesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ayudaAgradecimientosToolStripMenuItem;

    /// <summary>
    ///  Liberar los recursos que se estén usando.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Código generado por el Diseñador de Windows Forms

    /// <summary>
    ///  Método necesario para admitir el Diseñador. No modificar
    ///  el contenido de este método con el editor de código.
    /// </summary>
    private void InitializeComponent()
    {
        this.txtCarpeta = new System.Windows.Forms.TextBox();
        this.btnProyecto = new System.Windows.Forms.Button();
        this.btnElegir = new System.Windows.Forms.Button();
        this.lblCarpeta = new System.Windows.Forms.Label();
        this.grpDireccion = new System.Windows.Forms.GroupBox();
        this.rbRestoreBak = new System.Windows.Forms.RadioButton();
        this.rbFixMojibake = new System.Windows.Forms.RadioButton();
        this.rbAWin1252 = new System.Windows.Forms.RadioButton();
        this.rbAUtf8 = new System.Windows.Forms.RadioButton();
        this.lblExtensiones = new System.Windows.Forms.Label();
        this.txtExtensiones = new System.Windows.Forms.TextBox();
        this.chkRecursivo = new System.Windows.Forms.CheckBox();
        this.chkBackup = new System.Windows.Forms.CheckBox();
        this.btnConvertir = new System.Windows.Forms.Button();
        this.lvResultados = new System.Windows.Forms.ListView();
        this.colArchivo = new System.Windows.Forms.ColumnHeader();
        this.colMensaje = new System.Windows.Forms.ColumnHeader();
        this.lblResumen = new System.Windows.Forms.Label();
        this.lblTitulo = new System.Windows.Forms.Label();
        this.lblSubtitulo = new System.Windows.Forms.Label();
        this.pnlCard = new ConversorCodificacion.Win.Controles.PanelCard();
        this.grpDireccion.SuspendLayout();
        this.SuspendLayout();

        // 
        // menuStrip1
        // 
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.abrirLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.archivoToolStripMenuItem });
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new System.Drawing.Size(780, 24);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";

        // 
        // archivoToolStripMenuItem
        // 
        this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
        this.archivoToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
        this.archivoToolStripMenuItem.Text = "Archivo";

        // 
        // abrirLogToolStripMenuItem
        // 
        this.abrirLogToolStripMenuItem.Name = "abrirLogToolStripMenuItem";
        this.abrirLogToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
        this.abrirLogToolStripMenuItem.Text = "Abrir log";
        this.abrirLogToolStripMenuItem.Click += new System.EventHandler(this.abrirLogToolStripMenuItem_Click);
        // Agregar al menú Archivo ahora que está instanciado
        this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.abrirLogToolStripMenuItem });

        // 
        // abrirConfigToolStripMenuItem
        // 
        this.abrirConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.abrirConfigToolStripMenuItem.Name = "abrirConfigToolStripMenuItem";
        this.abrirConfigToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.abrirConfigToolStripMenuItem.Text = "Abrir carpeta Config";
        this.abrirConfigToolStripMenuItem.Click += new System.EventHandler(this.abrirConfigToolStripMenuItem_Click);

        // 
        // salirToolStripMenuItem
        // 
        this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
        this.salirToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
        this.salirToolStripMenuItem.Text = "Salir";
        this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);

        // Agregar al menú Archivo ahora que están instanciados
        this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.abrirConfigToolStripMenuItem, this.salirToolStripMenuItem });

        // 
        // ayudaToolStripMenuItem
        // 
        this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ayudaInstruccionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ayudaAgradecimientosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ayudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ayudaInstruccionesToolStripMenuItem,
            this.ayudaAgradecimientosToolStripMenuItem});
        this.ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
        this.ayudaToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
        this.ayudaToolStripMenuItem.Text = "Ayuda";

        this.ayudaInstruccionesToolStripMenuItem.Name = "ayudaInstruccionesToolStripMenuItem";
        this.ayudaInstruccionesToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
        this.ayudaInstruccionesToolStripMenuItem.Text = "Instrucciones de uso";
        this.ayudaInstruccionesToolStripMenuItem.Click += new System.EventHandler(this.ayudaInstruccionesToolStripMenuItem_Click);

        this.ayudaAgradecimientosToolStripMenuItem.Name = "ayudaAgradecimientosToolStripMenuItem";
        this.ayudaAgradecimientosToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
        this.ayudaAgradecimientosToolStripMenuItem.Text = "Agradecimientos";
        this.ayudaAgradecimientosToolStripMenuItem.Click += new System.EventHandler(this.ayudaAgradecimientosToolStripMenuItem_Click);

        // Agregar menú Ayuda al MenuStrip
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.ayudaToolStripMenuItem });

        // 
        // estilosBarraToolStripMenuItem
        // 
        this.estilosBarraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.barraBloquesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.barraContinuaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.barraIndeterminadaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.estilosBarraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.barraBloquesToolStripMenuItem,
            this.barraContinuaToolStripMenuItem,
            this.barraIndeterminadaToolStripMenuItem});
        this.estilosBarraToolStripMenuItem.Name = "estilosBarraToolStripMenuItem";
        this.estilosBarraToolStripMenuItem.Size = new System.Drawing.Size(109, 20);
        this.estilosBarraToolStripMenuItem.Text = "Estilos de barra";

        this.barraBloquesToolStripMenuItem.Name = "barraBloquesToolStripMenuItem";
        this.barraBloquesToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        this.barraBloquesToolStripMenuItem.Text = "Bloques";
        this.barraBloquesToolStripMenuItem.Click += new System.EventHandler(this.barraBloquesToolStripMenuItem_Click);

        this.barraContinuaToolStripMenuItem.Name = "barraContinuaToolStripMenuItem";
        this.barraContinuaToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        this.barraContinuaToolStripMenuItem.Text = "Continua";
        this.barraContinuaToolStripMenuItem.Click += new System.EventHandler(this.barraContinuaToolStripMenuItem_Click);

        this.barraIndeterminadaToolStripMenuItem.Name = "barraIndeterminadaToolStripMenuItem";
        this.barraIndeterminadaToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
        this.barraIndeterminadaToolStripMenuItem.Text = "Indeterminada";
        this.barraIndeterminadaToolStripMenuItem.Click += new System.EventHandler(this.barraIndeterminadaToolStripMenuItem_Click);

        // 
        // colorBarraToolStripMenuItem
        // 
        this.colorBarraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorVerdeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorAmarilloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorRojoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorBarraToolStripMenuItem.Name = "colorBarraToolStripMenuItem";
        this.colorBarraToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
        this.colorBarraToolStripMenuItem.Text = "Color de barra";

        this.colorVerdeToolStripMenuItem.Name = "colorVerdeToolStripMenuItem";
        this.colorVerdeToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.colorVerdeToolStripMenuItem.Text = "Verde";
        this.colorVerdeToolStripMenuItem.Click += new System.EventHandler(this.colorVerdeToolStripMenuItem_Click);

        this.colorAmarilloToolStripMenuItem.Name = "colorAmarilloToolStripMenuItem";
        this.colorAmarilloToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.colorAmarilloToolStripMenuItem.Text = "Amarillo";
        this.colorAmarilloToolStripMenuItem.Click += new System.EventHandler(this.colorAmarilloToolStripMenuItem_Click);

        this.colorRojoToolStripMenuItem.Name = "colorRojoToolStripMenuItem";
        this.colorRojoToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.colorRojoToolStripMenuItem.Text = "Rojo";
        this.colorRojoToolStripMenuItem.Click += new System.EventHandler(this.colorRojoToolStripMenuItem_Click);

        this.colorAzulToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorAzulToolStripMenuItem.Name = "colorAzulToolStripMenuItem";
        this.colorAzulToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.colorAzulToolStripMenuItem.Text = "Azul";
        this.colorAzulToolStripMenuItem.Click += new System.EventHandler(this.colorAzulToolStripMenuItem_Click);

        this.colorCianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.colorCianToolStripMenuItem.Name = "colorCianToolStripMenuItem";
        this.colorCianToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
        this.colorCianToolStripMenuItem.Text = "Cian";
        this.colorCianToolStripMenuItem.Click += new System.EventHandler(this.colorCianToolStripMenuItem_Click);

        // lblTitulo
        // 
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTitulo.Location = new System.Drawing.Point(12, 32);
        this.lblTitulo.Name = "lblTitulo";
        this.lblTitulo.Size = new System.Drawing.Size(268, 30);
        this.lblTitulo.TabIndex = 100;
        this.lblTitulo.Text = "Conversor de Codificación";
        // 
        // lblSubtitulo
        // 
        this.lblSubtitulo.AutoSize = true;
        this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblSubtitulo.Location = new System.Drawing.Point(14, 64);
        this.lblSubtitulo.Name = "lblSubtitulo";
        this.lblSubtitulo.Size = new System.Drawing.Size(458, 15);
        this.lblSubtitulo.TabIndex = 101;
        this.lblSubtitulo.Text = "Convierte .bas, .cls, .frm, .vbp entre UTF-8 (sin BOM) y Windows-1252, recorriendo subcarpetas.";
        // 
        // pnlCard
        // 
        this.pnlCard.Location = new System.Drawing.Point(12, 92);
        this.pnlCard.Name = "pnlCard";
        this.pnlCard.Size = new System.Drawing.Size(756, 220);
        this.pnlCard.TabIndex = 102;
        this.pnlCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));

        // 
        // tblCard
        // 
        this.tblCard = new System.Windows.Forms.TableLayoutPanel();
        this.tblCard.ColumnCount = 2;
        this.tblCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tblCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
        this.tblCard.RowCount = 8;
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // 0: label carpeta
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F)); // 1: ruta + botones
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F)); // 2: dirección
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // 3: label extensiones
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F)); // 4: extensiones
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F)); // 5: checks
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F)); // 6: progreso
        this.tblCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // 7: relleno
        this.tblCard.Dock = System.Windows.Forms.DockStyle.Fill;

        // Fila 0: etiqueta carpeta (colSpan 3)
        this.tblCard.Controls.Add(this.lblCarpeta, 0, 0);
        this.tblCard.SetColumnSpan(this.lblCarpeta, 2);

        // Fila 1: ruta + botones
        this.tblCard.Controls.Add(this.txtCarpeta, 0, 1);
        this.tblCard.Controls.Add(this.btnElegir, 1, 1);

        // Fila 2: dirección (colSpan 3)
        this.tblCard.Controls.Add(this.grpDireccion, 0, 2);
        this.tblCard.SetColumnSpan(this.grpDireccion, 2);

        // Fila 3: etiqueta extensiones (colSpan 3)
        this.tblCard.Controls.Add(this.lblExtensiones, 0, 3);
        this.tblCard.SetColumnSpan(this.lblExtensiones, 2);

        // Fila 4: extensiones (colSpan 3)
        this.tblCard.Controls.Add(this.txtExtensiones, 0, 4);
        this.tblCard.SetColumnSpan(this.txtExtensiones, 2);

        // Fila 5: checks (en FlowLayoutPanel, col 0) + botón Convertir (col 1)
        this.flpChecks = new System.Windows.Forms.FlowLayoutPanel();
        this.flpChecks.AutoSize = true;
        this.flpChecks.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.flpChecks.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.flpChecks.WrapContents = false;
        this.flpChecks.Controls.Add(this.chkRecursivo);
        this.flpChecks.Controls.Add(this.chkBackup);
        this.tblCard.Controls.Add(this.flpChecks, 0, 5);
        this.tblCard.Controls.Add(this.btnConvertir, 1, 5);
        this.btnConvertir.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

        // Fila 6: panel host con barra y porcentaje (col 0) + etiqueta n/total (col 1)
        this.pnlProgresoHost = new System.Windows.Forms.Panel();
        this.pnlProgresoHost.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlProgresoHost.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
        this.pnlProgresoHost.Visible = false;
        this.pbProgreso = new ConversorCodificacion.Win.Controles.ProgressBarEx();
        this.pbProgreso.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pbProgreso.Minimum = 0;
        this.pbProgreso.Maximum = 100;
        this.pbProgreso.Value = 0;
        this.lblPct = new System.Windows.Forms.Label();
        this.lblPct.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblPct.Text = "0%";
        this.lblPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.lblPct.BackColor = System.Drawing.Color.Transparent;
        this.lblPct.ForeColor = System.Drawing.Color.White;
        this.pnlProgresoHost.Controls.Add(this.pbProgreso);
        this.pnlProgresoHost.Controls.Add(this.lblPct);
        this.tblCard.Controls.Add(this.pnlProgresoHost, 0, 6);
        this.lblProgreso = new System.Windows.Forms.Label();
        this.lblProgreso.Text = "0/0";
        this.lblProgreso.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.lblProgreso.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblProgreso.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
        this.lblProgreso.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblProgreso.Visible = false;
        this.tblCard.Controls.Add(this.lblProgreso, 1, 6);

        // Añadir tabla al panel card
        this.pnlCard.Controls.Add(this.tblCard);
        // 
        // lblCarpeta
        // 
        this.lblCarpeta.AutoSize = true;
        this.lblCarpeta.Location = new System.Drawing.Point(16, 12);
        this.lblCarpeta.Name = "lblCarpeta";
        this.lblCarpeta.Size = new System.Drawing.Size(75, 15);
        this.lblCarpeta.TabIndex = 0;
        this.lblCarpeta.Text = "Carpeta base";
        // 
        // txtCarpeta
        // 
        this.txtCarpeta.Location = new System.Drawing.Point(16, 30);
        this.txtCarpeta.Name = "txtCarpeta";
        this.txtCarpeta.Size = new System.Drawing.Size(480, 23);
        this.txtCarpeta.TabIndex = 1;
        this.txtCarpeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        this.txtCarpeta.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtCarpeta.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
        // 
        // btnProyecto (oculto, en desuso)
        // 
        this.btnProyecto.Visible = false;
        this.btnProyecto.Enabled = false;
        // 
        // btnElegir
        // 
        this.btnElegir.Location = new System.Drawing.Point(600, 30);
        this.btnElegir.Name = "btnElegir";
        this.btnElegir.Size = new System.Drawing.Size(114, 23);
        this.btnElegir.TabIndex = 3;
        this.btnElegir.Text = "Examinar…";
        this.btnElegir.UseVisualStyleBackColor = true;
        this.btnElegir.Click += new System.EventHandler(this.btnElegir_Click);
        this.btnElegir.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnElegir.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
        this.btnElegir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // grpDireccion
        // 
        this.grpDireccion.Controls.Add(this.rbRestoreBak);
        this.grpDireccion.Controls.Add(this.rbFixMojibake);
        this.grpDireccion.Controls.Add(this.rbAWin1252);
        this.grpDireccion.Controls.Add(this.rbAUtf8);
        this.grpDireccion.Location = new System.Drawing.Point(16, 70);
        this.grpDireccion.Name = "grpDireccion";
        this.grpDireccion.Size = new System.Drawing.Size(738, 56);
        this.grpDireccion.TabIndex = 4;
        this.grpDireccion.TabStop = false;
        this.grpDireccion.Text = "Dirección de conversión";
        this.grpDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // rbAUtf8
        // 
        this.rbAUtf8.AutoSize = true;
        this.rbAUtf8.Location = new System.Drawing.Point(16, 26);
        this.rbAUtf8.Name = "rbAUtf8";
        this.rbAUtf8.Size = new System.Drawing.Size(176, 19);
        this.rbAUtf8.TabIndex = 0;
        this.rbAUtf8.TabStop = true;
        this.rbAUtf8.Text = "Windows-1252 → UTF-8 (sin BOM)";
        this.rbAUtf8.UseVisualStyleBackColor = true;
        // 
        // rbAWin1252
        // 
        this.rbAWin1252.AutoSize = true;
        this.rbAWin1252.Location = new System.Drawing.Point(218, 26);
        this.rbAWin1252.Name = "rbAWin1252";
        this.rbAWin1252.Size = new System.Drawing.Size(149, 19);
        this.rbAWin1252.TabIndex = 1;
        this.rbAWin1252.TabStop = true;
        this.rbAWin1252.Text = "UTF-8 → Windows-1252";
        this.rbAWin1252.UseVisualStyleBackColor = true;
        // 
        // rbFixMojibake
        // 
        this.rbFixMojibake.AutoSize = true;
        this.rbFixMojibake.Location = new System.Drawing.Point(395, 26);
        this.rbFixMojibake.Name = "rbFixMojibake";
        this.rbFixMojibake.Size = new System.Drawing.Size(111, 19);
        this.rbFixMojibake.TabIndex = 2;
        this.rbFixMojibake.TabStop = true;
        this.rbFixMojibake.Text = "Arreglar mojibake";
        this.rbFixMojibake.UseVisualStyleBackColor = true;
        // 
        // rbRestoreBak
        // 
        this.rbRestoreBak.AutoSize = true;
        this.rbRestoreBak.Location = new System.Drawing.Point(527, 26);
        this.rbRestoreBak.Name = "rbRestoreBak";
        this.rbRestoreBak.Size = new System.Drawing.Size(119, 19);
        this.rbRestoreBak.TabIndex = 3;
        this.rbRestoreBak.TabStop = true;
        this.rbRestoreBak.Text = "Restaurar desde .bak";
        this.rbRestoreBak.UseVisualStyleBackColor = true;
        // 
        // lblExtensiones
        // 
        this.lblExtensiones.AutoSize = true;
        this.lblExtensiones.Location = new System.Drawing.Point(16, 125);
        this.lblExtensiones.Name = "lblExtensiones";
        this.lblExtensiones.Size = new System.Drawing.Size(125, 15);
        this.lblExtensiones.TabIndex = 5;
        this.lblExtensiones.Text = "Extensiones a procesar";
        // 
        // txtExtensiones
        // 
        this.txtExtensiones.Location = new System.Drawing.Point(16, 143);
        this.txtExtensiones.Name = "txtExtensiones";
        this.txtExtensiones.Size = new System.Drawing.Size(282, 23);
        this.txtExtensiones.TabIndex = 6;
        this.txtExtensiones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // chkRecursivo
        // 
        this.chkRecursivo.AutoSize = true;
        this.chkRecursivo.Location = new System.Drawing.Point(313, 145);
        this.chkRecursivo.Name = "chkRecursivo";
        this.chkRecursivo.Size = new System.Drawing.Size(128, 19);
        this.chkRecursivo.TabIndex = 7;
        this.chkRecursivo.Text = "Incluir subcarpetas";
        this.chkRecursivo.UseVisualStyleBackColor = true;
        this.chkRecursivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        // 
        // chkBackup
        // 
        this.chkBackup.AutoSize = true;
        this.chkBackup.Location = new System.Drawing.Point(461, 145);
        this.chkBackup.Name = "chkBackup";
        this.chkBackup.Size = new System.Drawing.Size(185, 19);
        this.chkBackup.TabIndex = 8;
        this.chkBackup.Text = "Crear backup .bak si no existe";
        this.chkBackup.UseVisualStyleBackColor = true;
        this.chkBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        // 
        // btnConvertir
        // 
        this.btnConvertir.Location = new System.Drawing.Point(650, 142);
        this.btnConvertir.Name = "btnConvertir";
        this.btnConvertir.Size = new System.Drawing.Size(104, 25);
        this.btnConvertir.TabIndex = 9;
        this.btnConvertir.Text = "Convertir";
        this.btnConvertir.UseVisualStyleBackColor = true;
        this.btnConvertir.Click += new System.EventHandler(this.btnConvertir_Click);
        this.btnConvertir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // lvResultados
        // 
        this.lvResultados.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colArchivo, this.colMensaje });
        this.lvResultados.FullRowSelect = true;
        this.lvResultados.GridLines = true;
        this.lvResultados.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        this.lvResultados.Location = new System.Drawing.Point(12, 360);
        this.lvResultados.Name = "lvResultados";
        this.lvResultados.Size = new System.Drawing.Size(756, 178);
        this.lvResultados.TabIndex = 10;
        this.lvResultados.UseCompatibleStateImageBehavior = false;
        this.lvResultados.View = System.Windows.Forms.View.Details;
        this.lvResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // colArchivo
        // 
        this.colArchivo.Text = "Archivo";
        this.colArchivo.Width = 450;
        // 
        // colMensaje
        // 
        this.colMensaje.Text = "Mensaje";
        this.colMensaje.Width = 280;
        // 
        // lblResumen
        // 
        this.lblResumen.AutoSize = true;
        this.lblResumen.Location = new System.Drawing.Point(12, 336);
        this.lblResumen.Name = "lblResumen";
        this.lblResumen.Size = new System.Drawing.Size(134, 15);
        this.lblResumen.TabIndex = 11;
        this.lblResumen.Text = "Procesados: 0 | Conv.: 0 | Err.: 0";
        this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(780, 520);
        this.MainMenuStrip = this.menuStrip1;
        this.Controls.Add(this.menuStrip1);
        this.Controls.Add(this.pnlCard);
        this.Controls.Add(this.lblSubtitulo);
        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.lblResumen);
        this.Controls.Add(this.lvResultados);
        // Controles dentro del panel (card) gestionados por tblCard
        this.MinimumSize = new System.Drawing.Size(796, 489);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Conversor de Codificación (UTF-8 ↔ Windows-1252)";
        this.grpDireccion.ResumeLayout(false);
        this.grpDireccion.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}
