namespace RTSCon.Catalogos.Propiedad
{
    partial class PropiedadConfirmarDesactivacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropiedadConfirmarDesactivacion));
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCancelar = new Krypton.Toolkit.KryptonButton();
            this.btnConfirmar = new Krypton.Toolkit.KryptonButton();
            this.txtPassword = new Krypton.Toolkit.KryptonTextBox();
            this.lblDesactivacion = new Krypton.Toolkit.KryptonLabel();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
            this.label7.Location = new System.Drawing.Point(509, 48);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 102);
            this.label7.TabIndex = 143;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
            this.label12.Location = new System.Drawing.Point(389, 48);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(140, 102);
            this.label12.TabIndex = 142;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancelar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnCancelar.Location = new System.Drawing.Point(560, 313);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCancelar.Size = new System.Drawing.Size(205, 55);
            this.btnCancelar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.TabIndex = 141;
            this.btnCancelar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCancelar.Values.Text = "Cancelar";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnConfirmar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnConfirmar.Location = new System.Drawing.Point(265, 313);
            this.btnConfirmar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirmar.Size = new System.Drawing.Size(205, 55);
            this.btnConfirmar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.TabIndex = 140;
            this.btnConfirmar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirmar.Values.Text = "Confirmar";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(37, 239);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(996, 30);
            this.txtPassword.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.TabIndex = 139;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblDesactivacion
            // 
            this.lblDesactivacion.AutoSize = false;
            this.lblDesactivacion.Location = new System.Drawing.Point(37, 158);
            this.lblDesactivacion.Margin = new System.Windows.Forms.Padding(4);
            this.lblDesactivacion.Name = "lblDesactivacion";
            this.lblDesactivacion.Size = new System.Drawing.Size(996, 63);
            this.lblDesactivacion.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesactivacion.TabIndex = 138;
            this.lblDesactivacion.Values.Text = "Esta seguro que quiere borrar (nombre del condominio)? Coloque su contraseña abaj" +
    "o para confirmar:";
            // 
            // PropiedadConfirmarDesactivacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 417);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblDesactivacion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropiedadConfirmarDesactivacion";
            this.Text = "Confirmar Desactivación";
            this.Load += new System.EventHandler(this.PropiedadConfirmarDesactivacion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private Krypton.Toolkit.KryptonButton btnCancelar;
        private Krypton.Toolkit.KryptonButton btnConfirmar;
        private Krypton.Toolkit.KryptonTextBox txtPassword;
        private Krypton.Toolkit.KryptonLabel lblDesactivacion;
    }
}