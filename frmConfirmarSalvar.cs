using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopiarExes
{
    public partial class frmConfirmarSalvar : Form
    {
        public frmConfirmarSalvar()
        {
            InitializeComponent();
        }

        private void frmConfirmarSalvar_Load(object sender, EventArgs e)
        {
            pbIconeAviso.Image = SystemIcons.Question.ToBitmap();
        }

        private void btnSalvarSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnSairSemSalvar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnPermanecer_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }
    }
}