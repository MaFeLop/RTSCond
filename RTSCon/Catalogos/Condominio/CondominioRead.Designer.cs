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
            ((System.ComponentModel.ISupportInitialize)(this.dgvCondominios)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCondominios
            // 
            this.dgvCondominios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCondominios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCondominios.Location = new System.Drawing.Point(16, 81);
            this.dgvCondominios.Margin = new System.Windows.Forms.Padding(4);
            this.dgvCondominios.Name = "dgvCondominios";
            this.dgvCondominios.RowHeadersWidth = 51;
            this.dgvCondominios.Size = new System.Drawing.Size(1431, 340);
            this.dgvCondominios.TabIndex = 0;
            this.dgvCondominios.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCondominios_CellContentClick);
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(241, 478);
            this.btnCrear.Margin = new System.Windows.Forms.Padding(4);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnCrear.Size = new System.Drawing.Size(237, 63);
            this.btnCrear.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.TabIndex = 1;
            this.btnCrear.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCrear.Values.Text = "Crear";
            this.btnCrear.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(604, 478);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnUpdate.Size = new System.Drawing.Size(237, 63);
            this.btnUpdate.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnUpdate.Values.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDesactivar
            // 
            this.btnDesactivar.Location = new System.Drawing.Point(967, 478);
            this.btnDesactivar.Margin = new System.Windows.Forms.Padding(4);
            this.btnDesactivar.Name = "btnDesactivar";
            this.btnDesactivar.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueDarkMode;
            this.btnDesactivar.Size = new System.Drawing.Size(237, 63);
            this.btnDesactivar.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesactivar.TabIndex = 3;
            this.btnDesactivar.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnDesactivar.Values.Text = "Desactivar";
            this.btnDesactivar.Click += new System.EventHandler(this.btnDesactivar_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(13, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(283, 28);
            this.label2.TabIndex = 93;
            this.label2.Text = "Buscar por Nombre:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(16, 46);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(743, 27);
            this.txtBuscar.TabIndex = 92;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.ButtonStyle = Krypton.Toolkit.ButtonStyle.NavigatorStack;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(767, 45);
            this.btnLimpiarFiltros.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.PaletteMode = Krypton.Toolkit.PaletteMode.Microsoft365BlueLightMode;
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(140, 28);
            this.btnLimpiarFiltros.TabIndex = 91;
            this.btnLimpiarFiltros.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnLimpiarFiltros.Values.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiarFiltros.Values.Image")));
            this.btnLimpiarFiltros.Values.Text = "Limpiar Filtros";
            this.btnLimpiarFiltros.Click += new System.EventHandler(this.btnLimpiarFiltros_Click);
            // 
            // CondominioRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1476, 598);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.btnLimpiarFiltros);
            this.Controls.Add(this.btnDesactivar);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.dgvCondominios);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CondominioRead";
            this.Text = "Condominios";
            this.Load += new System.EventHandler(this.CondominioRead_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCondominios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonDataGridView dgvCondominios;
        private Krypton.Toolkit.KryptonButton btnCrear;
        private Krypton.Toolkit.KryptonButton btnUpdate;
        private Krypton.Toolkit.KryptonButton btnDesactivar;
        private System.Windows.Forms.Label label2;
        private Krypton.Toolkit.KryptonTextBox txtBuscar;
        private Krypton.Toolkit.KryptonButton btnLimpiarFiltros;
    }
}