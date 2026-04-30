using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace RTSCon
{
    public partial class FrmSesionExpirada : KryptonForm
    {
        private readonly Timer _timer;

        public FrmSesionExpirada(string mensaje)
        {
            Text = "Sesión cerrada";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            ControlBox = false;
            ClientSize = new Size(520, 170);

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            Label lblMensaje = new Label();
            lblMensaje.Dock = DockStyle.Fill;
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
            lblMensaje.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
            lblMensaje.Text = mensaje;

            Button btnAceptar = new Button();
            btnAceptar.Text = "Aceptar";
            btnAceptar.Width = 110;
            btnAceptar.Height = 34;
            btnAceptar.Anchor = AnchorStyles.None;
            btnAceptar.Click += btnAceptar_Click;

            Panel panelBoton = new Panel();
            panelBoton.Dock = DockStyle.Fill;
            panelBoton.Controls.Add(btnAceptar);

            panelBoton.Resize += delegate
            {
                btnAceptar.Left = (panelBoton.Width - btnAceptar.Width) / 2;
                btnAceptar.Top = (panelBoton.Height - btnAceptar.Height) / 2;
            };

            layout.Controls.Add(lblMensaje, 0, 0);
            layout.Controls.Add(panelBoton, 0, 1);

            Controls.Add(layout);

            _timer = new Timer();
            _timer.Interval = 1800;
            _timer.Tick += timer_Tick;

            Shown += FrmSesionExpirada_Shown;
            FormClosed += FrmSesionExpirada_FormClosed;
        }

        private void FrmSesionExpirada_Shown(object sender, EventArgs e)
        {
            _timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmSesionExpirada_FormClosed(object sender, FormClosedEventArgs e)
        {
            _timer.Stop();
            _timer.Tick -= timer_Tick;
            _timer.Dispose();
        }
    }
}
