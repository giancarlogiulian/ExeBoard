using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

// CORREÇÃO: O namespace foi ajustado para "ExeBoard" para corresponder ao seu projeto.
namespace ExeBoard 
{
    public partial class frmAdicionarServico : Form
    {
        // Propriedade pública para o Form1 poder ler o resultado
        public string NomeDoServico { get; private set; }

        public frmAdicionarServico()
        {
            InitializeComponent(); // Agora esta linha funcionará
        }

        private void frmAdicionarServico_Load(object sender, EventArgs e)
        {
            var autoCompleteCollection = new AutoCompleteStringCollection();
            try
            {
                var todosOsServicos = ServiceController.GetServices().Select(s => s.ServiceName).ToArray();
                autoCompleteCollection.AddRange(todosOsServicos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar la lista de serviços do Windows.\nVocê pode digitar o nome manualmente.\n\nErro: " + ex.Message, 
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Agora o txtNomeServico será encontrado
            txtNomeServico.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtNomeServico.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtNomeServico.AutoCompleteCustomSource = autoCompleteCollection;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNomeServico.Text))
            {
                this.NomeDoServico = txtNomeServico.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, digite um nome de serviço.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}