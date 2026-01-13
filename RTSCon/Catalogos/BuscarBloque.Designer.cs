namespace RTSCon.Catalogos
{
    partial class BuscarBloque
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuscarBloque));
            this.chkSoloActivos = new Krypton.Toolkit.KryptonCheckBox();
            this.btnCancelar = new Krypton.Toolkit.KryptonButton();
            this.btnConfirmar = new Krypton.Toolkit.KryptonButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBuscar = new Krypton.Toolkit.KryptonTextBox();
            this.btnLimpiarFiltros = new Krypton.Toolkit.KryptonButton();
            this.dgvBloques = new Krypton.Toolkit.KryptonDataGridView();
            this.lblSinResultados = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBloques)).BeginInit();
            this.SuspendLayout();
            // 
            // chkSoloActivos
            // 
            this.chkSoloActivos.LabelStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.chkSoloActivos.Location = new System.Drawing.Point(911, 78);
            this.chkSoloActivos.Name = "chkSoloActivos";
            this.chkSoloActivos.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.chkSoloActivos.Size = new System.Drawing.Size(117, 24);
            this.chkSoloActivos.TabIndex = 114;
            this.chkSoloActivos.Values.Text = "Solo Activos";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(797, 483);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCancelar.Size = new System.Drawing.Size(237, 63);
            this.btnCancelar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.TabIndex = 113;
            this.btnCancelar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCancelar.Values.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Location = new System.Drawing.Point(434, 483);
            this.btnConfirmar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnConfirmar.Size = new System.Drawing.Size(237, 63);
            this.btnConfirmar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.TabIndex = 112;
            this.btnConfirmar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfirmar.Values.Text = "Confirmar";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(13, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(333, 28);
            this.label2.TabIndex = 111;
            this.label2.Text = "Buscar por Nombre:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(13, 78);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(743, 27);
            this.txtBuscar.TabIndex = 110;
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(764, 78);
            this.btnLimpiarFiltros.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueLightMode;
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(140, 28);
            this.btnLimpiarFiltros.TabIndex = 109;
            this.btnLimpiarFiltros.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnLimpiarFiltros.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiarFiltros.Values.Image")));
            this.btnLimpiarFiltros.Values.Text = "Limpiar Filtros";
            // 
            // dgvBloques
            // 
            this.dgvBloques.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBloques.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBloques.Location = new System.Drawing.Point(13, 113);
            this.dgvBloques.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBloques.Name = "dgvBloques";
            this.dgvBloques.RowHeadersWidth = 51;
            this.dgvBloques.Size = new System.Drawing.Size(1431, 340);
            this.dgvBloques.TabIndex = 108;
            // 
            // lblSinResultados
            // 
            this.lblSinResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSinResultados.BackColor = System.Drawing.Color.Transparent;
            this.lblSinResultados.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSinResultados.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSinResultados.Location = new System.Drawing.Point(459, 246);
            this.lblSinResultados.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSinResultados.Name = "lblSinResultados";
            this.lblSinResultados.Size = new System.Drawing.Size(333, 28);
            this.lblSinResultados.TabIndex = 115;
            this.lblSinResultados.Text = "No se han encontrado registros";
            this.lblSinResultados.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BuscarBloque
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1526, 531);
            this.Controls.Add(this.lblSinResultados);
            this.Controls.Add(this.chkSoloActivos);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.btnLimpiarFiltros);
            this.Controls.Add(this.dgvBloques);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "BuscarBloque";
            this.Text = "Buscar Bloque";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBloques)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonCheckBox chkSoloActivos;
        private Krypton.Toolkit.KryptonButton btnCancelar;
        private Krypton.Toolkit.KryptonButton btnConfirmar;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtBuscar;
        private Krypton.Toolkit.KryptonButton btnLimpiarFiltros;
        private Krypton.Toolkit.KryptonDataGridView dgvBloques;
        private System.Windows.Forms.Label lblSinResultados;
    }
}