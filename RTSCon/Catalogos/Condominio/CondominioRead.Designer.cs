namespace RTSCon.Catalogos.Condominio
{
    partial class CondominioRead
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CondominioRead));
            this.dgvCondominios = new Krypton.Toolkit.KryptonDataGridView();
            this.btnCrear = new Krypton.Toolkit.KryptonButton();
            this.btnUpdate = new Krypton.Toolkit.KryptonButton();
            this.btnDesactivar = new Krypton.Toolkit.KryptonButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBuscar = new Krypton.Toolkit.KryptonTextBox();
            this.btnLimpiarFiltros = new Krypton.Toolkit.KryptonButton();
            this.chkSoloActivos = new Krypton.Toolkit.KryptonCheckBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCondominios)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvCondominios
            // 
            this.dgvCondominios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCondominios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCondominios.Location = new System.Drawing.Point(0, 81);
            this.dgvCondominios.Margin = new System.Windows.Forms.Padding(4);
            this.dgvCondominios.Name = "dgvCondominios";
            this.dgvCondominios.RowHeadersWidth = 51;
            this.dgvCondominios.Size = new System.Drawing.Size(1567, 371);
            this.dgvCondominios.TabIndex = 0;
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(214, 10);
            this.btnCrear.Margin = new System.Windows.Forms.Padding(4);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCrear.Size = new System.Drawing.Size(237, 63);
            this.btnCrear.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.TabIndex = 1;
            this.btnCrear.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCrear.Values.Text = "Crear";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(588, 9);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnUpdate.Size = new System.Drawing.Size(237, 63);
            this.btnUpdate.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnUpdate.Values.Text = "Update";
            // 
            // btnDesactivar
            // 
            this.btnDesactivar.Location = new System.Drawing.Point(951, 9);
            this.btnDesactivar.Margin = new System.Windows.Forms.Padding(4);
            this.btnDesactivar.Name = "btnDesactivar";
            this.btnDesactivar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnDesactivar.Size = new System.Drawing.Size(237, 63);
            this.btnDesactivar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesactivar.TabIndex = 3;
            this.btnDesactivar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnDesactivar.Values.Text = "Desactivar";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(1, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(359, 28);
            this.label2.TabIndex = 93;
            this.label2.Text = "Buscar por Nombre:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(4, 41);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(743, 27);
            this.txtBuscar.TabIndex = 92;
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(951, 36);
            this.btnLimpiarFiltros.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueLightMode;
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(140, 28);
            this.btnLimpiarFiltros.TabIndex = 91;
            this.btnLimpiarFiltros.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnLimpiarFiltros.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiarFiltros.Values.Image")));
            this.btnLimpiarFiltros.Values.Text = "Limpiar Filtros";
            // 
            // chkSoloActivos
            // 
            this.chkSoloActivos.LabelStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.chkSoloActivos.Location = new System.Drawing.Point(811, 36);
            this.chkSoloActivos.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkSoloActivos.Name = "chkSoloActivos";
            this.chkSoloActivos.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.chkSoloActivos.Size = new System.Drawing.Size(117, 24);
            this.chkSoloActivos.TabIndex = 94;
            this.chkSoloActivos.Values.Text = "Solo Activos";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTotal.Location = new System.Drawing.Point(1201, 16);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(362, 28);
            this.lblTotal.TabIndex = 95;
            this.lblTotal.Text = "Total: ";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnLimpiarFiltros);
            this.pnlTop.Controls.Add(this.chkSoloActivos);
            this.pnlTop.Controls.Add(this.label2);
            this.pnlTop.Controls.Add(this.txtBuscar);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1567, 74);
            this.pnlTop.TabIndex = 96;
            this.pnlTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDesactivar);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.lblTotal);
            this.panel1.Controls.Add(this.btnCrear);
            this.panel1.Location = new System.Drawing.Point(0, 451);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1567, 100);
            this.panel1.TabIndex = 97;
            // 
            // CondominioRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1567, 551);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.dgvCondominios);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CondominioRead";
            this.Text = "Condominios";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCondominios)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonDataGridView dgvCondominios;
        private Krypton.Toolkit.KryptonButton btnCrear;
        private Krypton.Toolkit.KryptonButton btnUpdate;
        private Krypton.Toolkit.KryptonButton btnDesactivar;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtBuscar;
        private Krypton.Toolkit.KryptonButton btnLimpiarFiltros;
        private Krypton.Toolkit.KryptonCheckBox chkSoloActivos;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel panel1;
    }
}