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

namespace RTSCon.Catalogos
{
    public partial class UpdateBloque : KryptonForm
    {
        private readonly int _id;

        public UpdateBloque(int id)
        {
            InitializeComponent();
            _id = id;
        }
    }
}
