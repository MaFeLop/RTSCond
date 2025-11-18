namespace RTSCon.Catalogos
{
    partial class BloqueRead
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BloqueRead));
            this.lblTotal = new System.Windows.Forms.Label();
            this.chkSoloActivos = new Krypton.Toolkit.KryptonCheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBuscar = new Krypton.Toolkit.KryptonTextBox();
            this.btnLimpiarFiltros = new Krypton.Toolkit.KryptonButton();
            this.btnDesactivar = new Krypton.Toolkit.KryptonButton();
            this.btnUpdate = new Krypton.Toolkit.KryptonButton();
            this.btnCrear = new Krypton.Toolkit.KryptonButton();
            this.dgvBloques = new Krypton.Toolkit.KryptonDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBloques)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTotal.Location = new System.Drawing.Point(1036, 443);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(292, 28);
            this.lblTotal.TabIndex = 104;
            this.lblTotal.Text = "Total: ";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkSoloActivos
            // 
            this.chkSoloActivos.LabelStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.chkSoloActivos.Location = new System.Drawing.Point(762, 53);
            this.chkSoloActivos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkSoloActivos.Name = "chkSoloActivos";
            this.chkSoloActivos.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.chkSoloActivos.Size = new System.Drawing.Size(117, 24);
            this.chkSoloActivos.TabIndex = 103;
            this.chkSoloActivos.Values.Text = "Solo Activos";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(10, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(408, 28);
            this.label2.TabIndex = 102;
            this.label2.Text = "Buscar por Nombre:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(13, 53);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(743, 27);
            this.txtBuscar.TabIndex = 101;
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(888, 53);
            this.btnLimpiarFiltros.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueLightMode;
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(140, 28);
            this.btnLimpiarFiltros.TabIndex = 100;
            this.btnLimpiarFiltros.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnLimpiarFiltros.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiarFiltros.Values.Image")));
            this.btnLimpiarFiltros.Values.Text = "Limpiar Filtros";
            // 
            // btnDesactivar
            // 
            this.btnDesactivar.Location = new System.Drawing.Point(964, 485);
            this.btnDesactivar.Margin = new System.Windows.Forms.Padding(4);
            this.btnDesactivar.Name = "btnDesactivar";
            this.btnDesactivar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnDesactivar.Size = new System.Drawing.Size(237, 63);
            this.btnDesactivar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesactivar.TabIndex = 99;
            this.btnDesactivar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnDesactivar.Values.Text = "Desactivar";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(601, 485);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnUpdate.Size = new System.Drawing.Size(237, 63);
            this.btnUpdate.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.TabIndex = 98;
            this.btnUpdate.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnUpdate.Values.Text = "Update";
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(238, 485);
            this.btnCrear.Margin = new System.Windows.Forms.Padding(4);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCrear.Size = new System.Drawing.Size(237, 63);
            this.btnCrear.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.TabIndex = 97;
            this.btnCrear.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCrear.Values.Text = "Crear";
            // 
            // dgvBloques
            // 
            this.dgvBloques.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBloques.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBloques.Location = new System.Drawing.Point(13, 88);
            this.dgvBloques.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBloques.Name = "dgvBloques";
            this.dgvBloques.RowHeadersWidth = 51;
            this.dgvBloques.Size = new System.Drawing.Size(1315, 340);
            this.dgvBloques.TabIndex = 96;
            // 
            // BloqueRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1430, 770);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.chkSoloActivos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.btnLimpiarFiltros);
            this.Controls.Add(this.btnDesactivar);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.dgvBloques);
            this.Name = "BloqueRead";
            this.Text = "BloqueRead";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBloques)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTotal;
        private Krypton.Toolkit.KryptonCheckBox chkSoloActivos;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtBuscar;
        private Krypton.Toolkit.KryptonButton btnLimpiarFiltros;
        private Krypton.Toolkit.KryptonButton btnDesactivar;
        private Krypton.Toolkit.KryptonButton btnUpdate;
        private Krypton.Toolkit.KryptonButton btnCrear;
        private Krypton.Toolkit.KryptonDataGridView dgvBloques;
    }
}