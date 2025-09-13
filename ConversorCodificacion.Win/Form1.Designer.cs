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
        this.grpDireccion.SuspendLayout();
        this.SuspendLayout();
        // 
        // lblCarpeta
        // 
        this.lblCarpeta.AutoSize = true;
        this.lblCarpeta.Location = new System.Drawing.Point(12, 15);
        this.lblCarpeta.Name = "lblCarpeta";
        this.lblCarpeta.Size = new System.Drawing.Size(75, 15);
        this.lblCarpeta.TabIndex = 0;
        this.lblCarpeta.Text = "Carpeta base";
        // 
        // txtCarpeta
        // 
        this.txtCarpeta.Location = new System.Drawing.Point(12, 33);
        this.txtCarpeta.Name = "txtCarpeta";
        this.txtCarpeta.Size = new System.Drawing.Size(540, 23);
        this.txtCarpeta.TabIndex = 1;
        // 
        // btnProyecto
        // 
        this.btnProyecto.Location = new System.Drawing.Point(558, 33);
        this.btnProyecto.Name = "btnProyecto";
        this.btnProyecto.Size = new System.Drawing.Size(90, 23);
        this.btnProyecto.TabIndex = 2;
        this.btnProyecto.Text = "Proyecto";
        this.btnProyecto.UseVisualStyleBackColor = true;
        this.btnProyecto.Click += new System.EventHandler(this.btnProyecto_Click);
        // 
        // btnElegir
        // 
        this.btnElegir.Location = new System.Drawing.Point(654, 33);
        this.btnElegir.Name = "btnElegir";
        this.btnElegir.Size = new System.Drawing.Size(114, 23);
        this.btnElegir.TabIndex = 3;
        this.btnElegir.Text = "Elegir carpeta…";
        this.btnElegir.UseVisualStyleBackColor = true;
        this.btnElegir.Click += new System.EventHandler(this.btnElegir_Click);
        // 
        // grpDireccion
        // 
        this.grpDireccion.Controls.Add(this.rbRestoreBak);
        this.grpDireccion.Controls.Add(this.rbFixMojibake);
        this.grpDireccion.Controls.Add(this.rbAWin1252);
        this.grpDireccion.Controls.Add(this.rbAUtf8);
        this.grpDireccion.Location = new System.Drawing.Point(12, 70);
        this.grpDireccion.Name = "grpDireccion";
        this.grpDireccion.Size = new System.Drawing.Size(756, 60);
        this.grpDireccion.TabIndex = 4;
        this.grpDireccion.TabStop = false;
        this.grpDireccion.Text = "Dirección de conversión";
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
        this.lblExtensiones.Location = new System.Drawing.Point(12, 140);
        this.lblExtensiones.Name = "lblExtensiones";
        this.lblExtensiones.Size = new System.Drawing.Size(125, 15);
        this.lblExtensiones.TabIndex = 5;
        this.lblExtensiones.Text = "Extensiones a procesar";
        // 
        // txtExtensiones
        // 
        this.txtExtensiones.Location = new System.Drawing.Point(12, 158);
        this.txtExtensiones.Name = "txtExtensiones";
        this.txtExtensiones.Size = new System.Drawing.Size(282, 23);
        this.txtExtensiones.TabIndex = 6;
        // 
        // chkRecursivo
        // 
        this.chkRecursivo.AutoSize = true;
        this.chkRecursivo.Location = new System.Drawing.Point(313, 160);
        this.chkRecursivo.Name = "chkRecursivo";
        this.chkRecursivo.Size = new System.Drawing.Size(128, 19);
        this.chkRecursivo.TabIndex = 7;
        this.chkRecursivo.Text = "Incluir subcarpetas";
        this.chkRecursivo.UseVisualStyleBackColor = true;
        // 
        // chkBackup
        // 
        this.chkBackup.AutoSize = true;
        this.chkBackup.Location = new System.Drawing.Point(461, 160);
        this.chkBackup.Name = "chkBackup";
        this.chkBackup.Size = new System.Drawing.Size(185, 19);
        this.chkBackup.TabIndex = 8;
        this.chkBackup.Text = "Crear backup .bak si no existe";
        this.chkBackup.UseVisualStyleBackColor = true;
        // 
        // btnConvertir
        // 
        this.btnConvertir.Location = new System.Drawing.Point(664, 157);
        this.btnConvertir.Name = "btnConvertir";
        this.btnConvertir.Size = new System.Drawing.Size(104, 25);
        this.btnConvertir.TabIndex = 9;
        this.btnConvertir.Text = "Convertir";
        this.btnConvertir.UseVisualStyleBackColor = true;
        this.btnConvertir.Click += new System.EventHandler(this.btnConvertir_Click);
        // 
        // lvResultados
        // 
        this.lvResultados.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colArchivo, this.colMensaje });
        this.lvResultados.FullRowSelect = true;
        this.lvResultados.GridLines = true;
        this.lvResultados.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        this.lvResultados.Location = new System.Drawing.Point(12, 214);
        this.lvResultados.Name = "lvResultados";
        this.lvResultados.Size = new System.Drawing.Size(756, 224);
        this.lvResultados.TabIndex = 10;
        this.lvResultados.UseCompatibleStateImageBehavior = false;
        this.lvResultados.View = System.Windows.Forms.View.Details;
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
        this.lblResumen.Location = new System.Drawing.Point(12, 192);
        this.lblResumen.Name = "lblResumen";
        this.lblResumen.Size = new System.Drawing.Size(134, 15);
        this.lblResumen.TabIndex = 11;
        this.lblResumen.Text = "Procesados: 0 | Conv.: 0 | Err.: 0";

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(780, 450);
        this.Controls.Add(this.lblResumen);
        this.Controls.Add(this.lvResultados);
        this.Controls.Add(this.btnConvertir);
        this.Controls.Add(this.chkBackup);
        this.Controls.Add(this.chkRecursivo);
        this.Controls.Add(this.txtExtensiones);
        this.Controls.Add(this.lblExtensiones);
        this.Controls.Add(this.grpDireccion);
        this.Controls.Add(this.btnElegir);
        this.Controls.Add(this.btnProyecto);
        this.Controls.Add(this.txtCarpeta);
        this.Controls.Add(this.lblCarpeta);
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
