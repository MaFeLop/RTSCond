namespace RTSCon.Catalogos
{
    partial class CrearPropiedad
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrearPropiedad));
            this.btnBuscarPropietario = new Krypton.Toolkit.KryptonButton();
            this.chkTitular = new Krypton.Toolkit.KryptonCheckBox();
            this.lblPropietarioResponsable = new System.Windows.Forms.Label();
            this.txtNombrePropietario = new Krypton.Toolkit.KryptonTextBox();
            this.dtpFechaInicio = new Krypton.Toolkit.KryptonDateTimePicker();
            this.btnVolver = new Krypton.Toolkit.KryptonButton();
            this.btnConfirmar = new Krypton.Toolkit.KryptonButton();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCorreoNotificacion = new Krypton.Toolkit.KryptonTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPropietarioDocumento = new Krypton.Toolkit.KryptonTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIdUnidad = new Krypton.Toolkit.KryptonTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNombrePropiedad = new Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            this.txtPorcentaje = new Krypton.Toolkit.KryptonTextBox();
            this.dtpFechaFin = new Krypton.Toolkit.KryptonDateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBuscarUnidad = new Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // btnBuscarPropietario
            // 
            this.btnBuscarPropietario.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBuscarPropietario.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnBuscarPropietario.Location = new System.Drawing.Point(416, 365);
            this.btnBuscarPropietario.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscarPropietario.Name = "btnBuscarPropietario";
            this.btnBuscarPropietario.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnBuscarPropietario.Size = new System.Drawing.Size(177, 37);
            this.btnBuscarPropietario.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarPropietario.TabIndex = 153;
            this.btnBuscarPropietario.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnBuscarPropietario.Values.Text = "Buscar Propietario";
            this.btnBuscarPropietario.Click += new System.EventHandler(this.btnBuscarPropietario_Click);
            // 
            // chkTitular
            // 
            this.chkTitular.LabelStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.chkTitular.Location = new System.Drawing.Point(56, 331);
            this.chkTitular.Name = "chkTitular";
            this.chkTitular.Size = new System.Drawing.Size(350, 24);
            this.chkTitular.TabIndex = 152;
            this.chkTitular.Values.Text = "Usted es Titular Principal de esta Propiedad?";
            // 
            // lblPropietarioResponsable
            // 
            this.lblPropietarioResponsable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPropietarioResponsable.BackColor = System.Drawing.Color.Transparent;
            this.lblPropietarioResponsable.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPropietarioResponsable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPropietarioResponsable.Location = new System.Drawing.Point(52, 374);
            this.lblPropietarioResponsable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPropietarioResponsable.Name = "lblPropietarioResponsable";
            this.lblPropietarioResponsable.Size = new System.Drawing.Size(303, 28);
            this.lblPropietarioResponsable.TabIndex = 151;
            this.lblPropietarioResponsable.Text = "Propietario Responsable:";
            this.lblPropietarioResponsable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNombrePropietario
            // 
            this.txtNombrePropietario.Location = new System.Drawing.Point(52, 406);
            this.txtNombrePropietario.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombrePropietario.Name = "txtNombrePropietario";
            this.txtNombrePropietario.ReadOnly = true;
            this.txtNombrePropietario.Size = new System.Drawing.Size(541, 30);
            this.txtNombrePropietario.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombrePropietario.TabIndex = 150;
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(674, 198);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(541, 30);
            this.dtpFechaInicio.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaInicio.TabIndex = 149;
            // 
            // btnVolver
            // 
            this.btnVolver.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnVolver.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnVolver.Location = new System.Drawing.Point(766, 555);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(4);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnVolver.Size = new System.Drawing.Size(205, 55);
            this.btnVolver.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.TabIndex = 148;
            this.btnVolver.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnVolver.Values.Text = "Volver";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnConfirmar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnConfirmar.Location = new System.Drawing.Point(420, 555);
            this.btnConfirmar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirmar.Size = new System.Drawing.Size(205, 55);
            this.btnConfirmar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.TabIndex = 147;
            this.btnConfirmar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirmar.Values.Text = "Confirmar";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(670, 90);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(355, 28);
            this.label13.TabIndex = 146;
            this.label13.Text = "Correo donde recibirá Notificaciones:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCorreoNotificacion
            // 
            this.txtCorreoNotificacion.Location = new System.Drawing.Point(672, 122);
            this.txtCorreoNotificacion.Margin = new System.Windows.Forms.Padding(4);
            this.txtCorreoNotificacion.Name = "txtCorreoNotificacion";
            this.txtCorreoNotificacion.Size = new System.Drawing.Size(541, 30);
            this.txtCorreoNotificacion.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCorreoNotificacion.TabIndex = 145;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
            this.label7.Location = new System.Drawing.Point(417, 16);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 102);
            this.label7.TabIndex = 136;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
            this.label12.Location = new System.Drawing.Point(297, 16);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(140, 102);
            this.label12.TabIndex = 135;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(674, 167);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(303, 28);
            this.label5.TabIndex = 134;
            this.label5.Text = "Fecha de Inicio:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(52, 245);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(303, 28);
            this.label4.TabIndex = 132;
            this.label4.Text = "Porcentaje:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(52, 440);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(303, 28);
            this.label3.TabIndex = 131;
            this.label3.Text = "Identificación del Propietario:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPropietarioDocumento
            // 
            this.txtPropietarioDocumento.Location = new System.Drawing.Point(52, 472);
            this.txtPropietarioDocumento.Margin = new System.Windows.Forms.Padding(4);
            this.txtPropietarioDocumento.Name = "txtPropietarioDocumento";
            this.txtPropietarioDocumento.ReadOnly = true;
            this.txtPropietarioDocumento.Size = new System.Drawing.Size(541, 30);
            this.txtPropietarioDocumento.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPropietarioDocumento.TabIndex = 130;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(52, 167);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 28);
            this.label1.TabIndex = 129;
            this.label1.Text = "Id de la Unidad:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIdUnidad
            // 
            this.txtIdUnidad.Location = new System.Drawing.Point(52, 199);
            this.txtIdUnidad.Margin = new System.Windows.Forms.Padding(4);
            this.txtIdUnidad.Name = "txtIdUnidad";
            this.txtIdUnidad.ReadOnly = true;
            this.txtIdUnidad.Size = new System.Drawing.Size(541, 30);
            this.txtIdUnidad.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdUnidad.TabIndex = 128;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(48, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 28);
            this.label2.TabIndex = 127;
            this.label2.Text = "Nombre de Propiedad:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNombrePropiedad
            // 
            this.txtNombrePropiedad.Location = new System.Drawing.Point(52, 122);
            this.txtNombrePropiedad.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombrePropiedad.Name = "txtNombrePropiedad";
            this.txtNombrePropiedad.Size = new System.Drawing.Size(541, 30);
            this.txtNombrePropiedad.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombrePropiedad.TabIndex = 126;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.AutoSize = false;
            this.kryptonLabel1.Location = new System.Drawing.Point(17, 43);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(4);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(272, 31);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.TabIndex = 125;
            this.kryptonLabel1.Values.Text = "Crear Propiedad";
            // 
            // txtPorcentaje
            // 
            this.txtPorcentaje.Location = new System.Drawing.Point(52, 277);
            this.txtPorcentaje.Margin = new System.Windows.Forms.Padding(4);
            this.txtPorcentaje.Name = "txtPorcentaje";
            this.txtPorcentaje.Size = new System.Drawing.Size(541, 30);
            this.txtPorcentaje.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPorcentaje.TabIndex = 154;
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFin.Location = new System.Drawing.Point(674, 277);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(541, 30);
            this.dtpFechaFin.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaFin.TabIndex = 156;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(674, 246);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(303, 28);
            this.label6.TabIndex = 155;
            this.label6.Text = "Fecha de Terminación:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBuscarUnidad
            // 
            this.btnBuscarUnidad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBuscarUnidad.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnBuscarUnidad.Location = new System.Drawing.Point(449, 160);
            this.btnBuscarUnidad.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscarUnidad.Name = "btnBuscarUnidad";
            this.btnBuscarUnidad.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnBuscarUnidad.Size = new System.Drawing.Size(144, 37);
            this.btnBuscarUnidad.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarUnidad.TabIndex = 157;
            this.btnBuscarUnidad.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnBuscarUnidad.Values.Text = "Buscar Unidad";
            this.btnBuscarUnidad.Click += new System.EventHandler(this.btnBuscarUnidad_Click);
            // 
            // CrearPropiedad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1517, 676);
            this.Controls.Add(this.btnBuscarUnidad);
            this.Controls.Add(this.dtpFechaFin);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPorcentaje);
            this.Controls.Add(this.btnBuscarPropietario);
            this.Controls.Add(this.chkTitular);
            this.Controls.Add(this.lblPropietarioResponsable);
            this.Controls.Add(this.txtNombrePropietario);
            this.Controls.Add(this.dtpFechaInicio);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtCorreoNotificacion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPropietarioDocumento);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIdUnidad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNombrePropiedad);
            this.Controls.Add(this.kryptonLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CrearPropiedad";
            this.Text = "Crear Propiedad";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonButton btnBuscarPropietario;
        private Krypton.Toolkit.KryptonCheckBox chkTitular;
        private System.Windows.Forms.Label lblPropietarioResponsable;
        private Krypton.Toolkit.KryptonTextBox txtNombrePropietario;
        private Krypton.Toolkit.KryptonDateTimePicker dtpFechaInicio;
        private Krypton.Toolkit.KryptonButton btnVolver;
        private Krypton.Toolkit.KryptonButton btnConfirmar;
        private System.Windows.Forms.Label label13;
        private Krypton.Toolkit.KryptonTextBox txtCorreoNotificacion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Krypton.Toolkit.KryptonTextBox txtPropietarioDocumento;
        private System.Windows.Forms.Label label1;
        private Krypton.Toolkit.KryptonTextBox txtIdUnidad;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtNombrePropiedad;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonTextBox txtPorcentaje;
        private Krypton.Toolkit.KryptonDateTimePicker dtpFechaFin;
        private System.Windows.Forms.Label label6;
        private Krypton.Toolkit.KryptonButton btnBuscarUnidad;
    }
}