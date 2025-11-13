namespace RTSCon.Catalogos.Propiedad
{
    partial class UpdatePropiedad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatePropiedad));
            this.dtpFechaFin = new Krypton.Toolkit.KryptonDateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPorcentaje = new Krypton.Toolkit.KryptonTextBox();
            this.btnBuscarPropietario = new Krypton.Toolkit.KryptonButton();
            this.chkTitular = new Krypton.Toolkit.KryptonCheckBox();
            this.txtAdministrador = new System.Windows.Forms.Label();
            this.txtNombrePropietario = new Krypton.Toolkit.KryptonTextBox();
            this.dtpFechaInicio = new Krypton.Toolkit.KryptonDateTimePicker();
            this.btnVolver = new Krypton.Toolkit.KryptonButton();
            this.btnConfirmar = new Krypton.Toolkit.KryptonButton();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCorreo = new Krypton.Toolkit.KryptonTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIdPropietario = new Krypton.Toolkit.KryptonTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIdPropiedad = new Krypton.Toolkit.KryptonTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNombrePropiedad = new Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFin.Location = new System.Drawing.Point(674, 270);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(541, 30);
            this.dtpFechaFin.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaFin.TabIndex = 179;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(674, 239);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(303, 28);
            this.label6.TabIndex = 178;
            this.label6.Text = "Fecha de Terminación:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPorcentaje
            // 
            this.txtPorcentaje.Location = new System.Drawing.Point(52, 270);
            this.txtPorcentaje.Margin = new System.Windows.Forms.Padding(4);
            this.txtPorcentaje.Name = "txtPorcentaje";
            this.txtPorcentaje.Size = new System.Drawing.Size(541, 30);
            this.txtPorcentaje.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPorcentaje.TabIndex = 177;
            // 
            // btnBuscarPropietario
            // 
            this.btnBuscarPropietario.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBuscarPropietario.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnBuscarPropietario.Location = new System.Drawing.Point(416, 358);
            this.btnBuscarPropietario.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscarPropietario.Name = "btnBuscarPropietario";
            this.btnBuscarPropietario.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnBuscarPropietario.Size = new System.Drawing.Size(177, 37);
            this.btnBuscarPropietario.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarPropietario.TabIndex = 176;
            this.btnBuscarPropietario.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnBuscarPropietario.Values.Text = "Buscar Propietario";
            // 
            // chkTitular
            // 
            this.chkTitular.LabelStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.chkTitular.Location = new System.Drawing.Point(56, 324);
            this.chkTitular.Name = "chkTitular";
            this.chkTitular.Size = new System.Drawing.Size(350, 24);
            this.chkTitular.TabIndex = 175;
            this.chkTitular.Values.Text = "Usted es Titular Principal de esta Propiedad?";
            // 
            // txtAdministrador
            // 
            this.txtAdministrador.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdministrador.BackColor = System.Drawing.Color.Transparent;
            this.txtAdministrador.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdministrador.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtAdministrador.Location = new System.Drawing.Point(52, 367);
            this.txtAdministrador.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtAdministrador.Name = "txtAdministrador";
            this.txtAdministrador.Size = new System.Drawing.Size(303, 28);
            this.txtAdministrador.TabIndex = 174;
            this.txtAdministrador.Text = "Propietario Responsable:";
            this.txtAdministrador.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNombrePropietario
            // 
            this.txtNombrePropietario.Location = new System.Drawing.Point(52, 399);
            this.txtNombrePropietario.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombrePropietario.Name = "txtNombrePropietario";
            this.txtNombrePropietario.ReadOnly = true;
            this.txtNombrePropietario.Size = new System.Drawing.Size(541, 30);
            this.txtNombrePropietario.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombrePropietario.TabIndex = 173;
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(674, 191);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(541, 30);
            this.dtpFechaInicio.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaInicio.TabIndex = 172;
            // 
            // btnVolver
            // 
            this.btnVolver.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnVolver.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnVolver.Location = new System.Drawing.Point(766, 548);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(4);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnVolver.Size = new System.Drawing.Size(205, 55);
            this.btnVolver.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.TabIndex = 171;
            this.btnVolver.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnVolver.Values.Text = "Volver";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnConfirmar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnConfirmar.Location = new System.Drawing.Point(420, 548);
            this.btnConfirmar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirmar.Size = new System.Drawing.Size(205, 55);
            this.btnConfirmar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.TabIndex = 170;
            this.btnConfirmar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirmar.Values.Text = "Confirmar";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(670, 83);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(355, 28);
            this.label13.TabIndex = 169;
            this.label13.Text = "Correo donde recibirá Notificaciones:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCorreo
            // 
            this.txtCorreo.Location = new System.Drawing.Point(672, 115);
            this.txtCorreo.Margin = new System.Windows.Forms.Padding(4);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(541, 30);
            this.txtCorreo.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCorreo.TabIndex = 168;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
            this.label12.Location = new System.Drawing.Point(297, 9);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(140, 102);
            this.label12.TabIndex = 166;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(674, 160);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(303, 28);
            this.label5.TabIndex = 165;
            this.label5.Text = "Fecha de Inicio:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(52, 238);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(303, 28);
            this.label4.TabIndex = 164;
            this.label4.Text = "Porcentaje:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(52, 433);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(303, 28);
            this.label3.TabIndex = 163;
            this.label3.Text = "Identificacion del Propietario:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIdPropietario
            // 
            this.txtIdPropietario.Location = new System.Drawing.Point(52, 465);
            this.txtIdPropietario.Margin = new System.Windows.Forms.Padding(4);
            this.txtIdPropietario.Name = "txtIdPropietario";
            this.txtIdPropietario.ReadOnly = true;
            this.txtIdPropietario.Size = new System.Drawing.Size(541, 30);
            this.txtIdPropietario.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdPropietario.TabIndex = 162;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(52, 160);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 28);
            this.label1.TabIndex = 161;
            this.label1.Text = "Id de la propiedad:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIdPropiedad
            // 
            this.txtIdPropiedad.Location = new System.Drawing.Point(52, 192);
            this.txtIdPropiedad.Margin = new System.Windows.Forms.Padding(4);
            this.txtIdPropiedad.Name = "txtIdPropiedad";
            this.txtIdPropiedad.Size = new System.Drawing.Size(541, 30);
            this.txtIdPropiedad.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdPropiedad.TabIndex = 160;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(48, 83);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 28);
            this.label2.TabIndex = 159;
            this.label2.Text = "Nombre de Propiedad:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNombrePropiedad
            // 
            this.txtNombrePropiedad.Location = new System.Drawing.Point(52, 115);
            this.txtNombrePropiedad.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombrePropiedad.Name = "txtNombrePropiedad";
            this.txtNombrePropiedad.Size = new System.Drawing.Size(541, 30);
            this.txtNombrePropiedad.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombrePropiedad.TabIndex = 158;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.AutoSize = false;
            this.kryptonLabel1.Location = new System.Drawing.Point(17, 36);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(4);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(272, 31);
            this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kryptonLabel1.TabIndex = 157;
            this.kryptonLabel1.Values.Text = "Update Propiedad";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
            this.label7.Location = new System.Drawing.Point(413, 9);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 102);
            this.label7.TabIndex = 180;
            // 
            // UpdatePropiedad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1517, 676);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dtpFechaFin);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPorcentaje);
            this.Controls.Add(this.btnBuscarPropietario);
            this.Controls.Add(this.chkTitular);
            this.Controls.Add(this.txtAdministrador);
            this.Controls.Add(this.txtNombrePropietario);
            this.Controls.Add(this.dtpFechaInicio);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIdPropietario);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIdPropiedad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNombrePropiedad);
            this.Controls.Add(this.kryptonLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdatePropiedad";
            this.Text = "Update Propiedad";
            this.Load += new System.EventHandler(this.UpdatePropiedad_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonDateTimePicker dtpFechaFin;
        private System.Windows.Forms.Label label6;
        private Krypton.Toolkit.KryptonTextBox txtPorcentaje;
        private Krypton.Toolkit.KryptonButton btnBuscarPropietario;
        private Krypton.Toolkit.KryptonCheckBox chkTitular;
        private System.Windows.Forms.Label txtAdministrador;
        private Krypton.Toolkit.KryptonTextBox txtNombrePropietario;
        private Krypton.Toolkit.KryptonDateTimePicker dtpFechaInicio;
        private Krypton.Toolkit.KryptonButton btnVolver;
        private Krypton.Toolkit.KryptonButton btnConfirmar;
        private System.Windows.Forms.Label label13;
        private Krypton.Toolkit.KryptonTextBox txtCorreo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Krypton.Toolkit.KryptonTextBox txtIdPropietario;
        private System.Windows.Forms.Label label1;
        private Krypton.Toolkit.KryptonTextBox txtIdPropiedad;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtNombrePropiedad;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.Label label7;
    }
}