using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExeBoard
{
    public partial class frmAtualizador : Form
    {
        public Atualizador atualizadorSelecionado { get; private set; }

        public frmAtualizador()
        {
            InitializeComponent();
        }

        public frmAtualizador(List<Atualizador> atualizadores)
        {
            InitializeComponent();
            lbAtualizador.DataSource = atualizadores;
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            if (lbAtualizador.SelectedItem != null)
            {
                atualizadorSelecionado = (Atualizador) lbAtualizador.SelectedItem;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Selecione um atualizador.");
            }

        }
    }
}
