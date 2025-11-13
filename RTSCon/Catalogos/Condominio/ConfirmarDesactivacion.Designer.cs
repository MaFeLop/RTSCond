namespace RTSCon.Catalogos
{
    partial class ConfirmarDesactivacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmarDesactivacion));
            this.lblDesactivacion = new Krypton.Toolkit.KryptonLabel();
            this.txtPassword = new Krypton.Toolkit.KryptonTextBox();
            this.btnConfirmar = new Krypton.Toolkit.KryptonButton();
            this.btnCancelar = new Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // lblDesactivacion
            // 
            this.lblDesactivacion.AutoSize = false;
            this.lblDesactivacion.Location = new System.Drawing.Point(26, 119);
            this.lblDesactivacion.Margin = new System.Windows.Forms.Padding(4);
            this.lblDesactivacion.Name = "lblDesactivacion";
            this.lblDesactivacion.Size = new System.Drawing.Size(996, 63);
            this.lblDesactivacion.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesactivacion.TabIndex = 1;
            this.lblDesactivacion.Values.Text = "Esta seguro que quiere borrar (nombre del condominio)? Coloque su contraseña abaj" +
    "o para confirmar:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(26, 200);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(996, 30);
            this.txtPassword.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.TabIndex = 95;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnConfirmar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnConfirmar.Location = new System.Drawing.Point(254, 286);
            this.btnConfirmar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirmar.Size = new System.Drawing.Size(205, 55);
            this.btnConfirmar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.TabIndex = 119;
            this.btnConfirmar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirmar.Values.Text = "Confirmar";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancelar.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnCancelar.Location = new System.Drawing.Point(549, 286);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCancelar.Size = new System.Drawing.Size(205, 55);
            this.btnCancelar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.TabIndex = 120;
            this.btnCancelar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCancelar.Values.Text = "Cancelar";
            // 
            // ConfirmarDesactivacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 452);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblDesactivacion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfirmarDesactivacion";
            this.Text = "Confirmar Desactivacion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonLabel lblDesactivacion;
        private Krypton.Toolkit.KryptonTextBox txtPassword;
        private Krypton.Toolkit.KryptonButton btnConfirmar;
        private Krypton.Toolkit.KryptonButton btnCancelar;
    }
}