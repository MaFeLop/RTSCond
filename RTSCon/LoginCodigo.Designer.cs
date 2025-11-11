namespace RTSCon
{
    partial class LoginCodigo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginCodigo));
            this.lblInfo = new Krypton.Toolkit.KryptonLabel();
            this.txtCodigo = new Krypton.Toolkit.KryptonMaskedTextBox();
            this.btnConfirm = new Krypton.Toolkit.KryptonButton();
            this.btnReenviar = new Krypton.Toolkit.KryptonButton();
            this.lblCountdown = new Krypton.Toolkit.KryptonLabel();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = false;
            this.lblInfo.Location = new System.Drawing.Point(202, 165);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.lblInfo.Size = new System.Drawing.Size(246, 32);
            this.lblInfo.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Values.Text = "Codigo de Verificación:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(454, 165);
            this.txtCodigo.Mask = "000000";
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(80, 32);
            this.txtCodigo.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(202, 260);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirm.Size = new System.Drawing.Size(332, 44);
            this.btnConfirm.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.TabIndex = 11;
            this.btnConfirm.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirm.Values.Text = "Confirmar";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click_1);
            // 
            // btnReenviar
            // 
            this.btnReenviar.Location = new System.Drawing.Point(541, 165);
            this.btnReenviar.Margin = new System.Windows.Forms.Padding(4);
            this.btnReenviar.Name = "btnReenviar";
            this.btnReenviar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnReenviar.Size = new System.Drawing.Size(91, 32);
            this.btnReenviar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReenviar.TabIndex = 12;
            this.btnReenviar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnReenviar.Values.Text = "Reenviar";
            this.btnReenviar.Click += new System.EventHandler(this.btnReenviar_Click_1);
            // 
            // lblCountdown
            // 
            this.lblCountdown.AutoSize = false;
            this.lblCountdown.Location = new System.Drawing.Point(639, 165);
            this.lblCountdown.Name = "lblCountdown";
            this.lblCountdown.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.lblCountdown.Size = new System.Drawing.Size(65, 32);
            this.lblCountdown.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountdown.TabIndex = 13;
            this.lblCountdown.Values.Text = "00:00";
            // 
            // LoginCodigo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 394);
            this.Controls.Add(this.lblCountdown);
            this.Controls.Add(this.btnReenviar);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "LoginCodigo";
            this.Text = "Verifique su Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonLabel lblInfo;
        private Krypton.Toolkit.KryptonMaskedTextBox txtCodigo;
        private Krypton.Toolkit.KryptonButton btnConfirm;
        private Krypton.Toolkit.KryptonButton btnReenviar;
        private Krypton.Toolkit.KryptonLabel lblCountdown;
    }
}