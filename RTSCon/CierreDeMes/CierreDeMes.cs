using DocumentFormat.OpenXml.Spreadsheet;
using Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSCon.CierreDeMes
{
    public partial class CierreDeMes : KryptonForm
    {
        public CierreDeMes()
        {
            InitializeComponent();
        }

        private void cmbSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void CierreDeMes_Load(object sender, EventArgs e)
        {
            cmbSeleccion.Items.AddRange(new object[] { "Todos", "OK", "ERR", "SKIP" });
        }
    }
}
