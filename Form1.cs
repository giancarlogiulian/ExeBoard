using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using static CopiarExes.frmAtualizador;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CopiarExes {

    public partial class frmCopiarExes : Form
    {

        private bool emModoEdicaoGlobal = false;
        private bool configuracoesForamAlteradas = false;

        // Importa a função do Windows para LER arquivos INI
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        // Importa a função do Windows para ESCREVER arquivos INI
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);


        string caminhoIni = Application.StartupPath + @"\Inicializar.ini";

        public frmCopiarExes()
        {
            VerificarECriarIniSeNaoExistir();

            InitializeComponent();
        }

        private void VerificarECriarIniSeNaoExistir()
        {
            // Verifica se o arquivo .ini NÃO existe no caminho esperado
            if (!System.IO.File.Exists(caminhoIni))
            {
                try
                {
                    // Cria um conteúdo padrão para o arquivo .ini, com as seções principais vazias.
                    string conteudoPadrao =
                        "[CONFIG_GERAIS]\n" +
                        "RODAR_NA_BANDEJA=Sim\n\n" +
                        "[CAMINHOS]\n" +
                        "DE=\n" +
                        "DE_PASTA_CLIENT=EXES\n" +
                        "DE_PASTA_SERVER=EXES\n" +
                        "DE_PASTA_DADOS=BD\n" +
                        "PARA=C:\\Viasoft\n" +
                        "PASTA_CLIENT=Client\n" +
                        "PASTA_SERVER=Server\n" +
                        "PASTA_DADOS=Dados\n\n" +
                        "[APLICACOES_CLIENTE]\n" +
                        "Count=0\n\n" +
                        "[APLICACOES_SERVIDORAS]\n" +
                        "Count=0\n\n" +
                        "[BANCO_DE_DADOS]\n" +
                        "Count=0\n\n" +
                        "[ATUALIZADORES]\n" +
                        "Count=0\n";

                    // Escreve o conteúdo padrão no novo arquivo .ini
                    System.IO.File.WriteAllText(caminhoIni, conteudoPadrao);

                    // Avisa ao usuário (opcional, mas bom para a primeira execução)
                    MessageBox.Show("Arquivo 'Inicializar.ini' não encontrado. Um novo arquivo de configuração foi criado.",
                                    "Aviso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Se der erro ao criar o arquivo, avisa o usuário e fecha a aplicação.
                    MessageBox.Show($"Erro crítico ao tentar criar o arquivo de configuração 'Inicializar.ini'.\n\nErro: {ex.Message}",
                                    "Erro Fatal",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        private void CarregarDadosDeConfiguracao()
        {
            // --- PARTE 1: CONFIGURAR E LIMPAR AS TABELAS ---

            // Configura a tabela de Clientes
            dgvConfigClientes.Rows.Clear(); // Limpa dados antigos
            if (dgvConfigClientes.Columns.Count == 0) // Adiciona colunas apenas se não existirem
            {
                dgvConfigClientes.Columns.Add("Nome", "Nome do Executável");
                dgvConfigClientes.Columns.Add("Categoria", "Categoria");
                dgvConfigClientes.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvConfigClientes.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // Configura a tabela de Servidores
            dgvConfigServidores.Rows.Clear();
            if (dgvConfigServidores.Columns.Count == 0) // Adiciona colunas apenas se não existirem
            {
                dgvConfigServidores.Columns.Add("Nome", "Nome do Serviço/App");
                dgvConfigServidores.Columns.Add("Tipo", "Tipo (Servico/Aplicacao)");
                dgvConfigServidores.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvConfigServidores.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            RegistrarLogCopiarDados("Carregando dados na aba de configurações...");

            // --- PARTE 2: LER O ARQUIVO .INI E POPULAR AS TABELAS ---

            // Carregar Clientes
            string countClientesStr = LerValorIni("APLICACOES_CLIENTE", "Count", caminhoIni);
            if (int.TryParse(countClientesStr, out int countClientes))
            {
                for (int i = 0; i <= countClientes; i++)
                {
                    string cliente = LerValorIni("APLICACOES_CLIENTE", $"Cliente{i}", caminhoIni);
                    string categoria = LerValorIni("APLICACOES_CLIENTE", $"Categoria{i}", caminhoIni);
                    if (!string.IsNullOrWhiteSpace(cliente))
                    {
                        dgvConfigClientes.Rows.Add(cliente, categoria);
                    }
                }
            }

            // Carregar Servidores
            string countServidoresStr = LerValorIni("APLICACOES_SERVIDORAS", "Count", caminhoIni);
            if (int.TryParse(countServidoresStr, out int countServidores))
            {
                for (int i = 0; i <= countServidores; i++)
                {
                    string servidor = LerValorIni("APLICACOES_SERVIDORAS", $"Servidor{i}", caminhoIni);
                    string tipo = LerValorIni("APLICACOES_SERVIDORAS", $"Tipo{i}", caminhoIni);
                    if (!string.IsNullOrWhiteSpace(servidor))
                    {
                        dgvConfigServidores.Rows.Add(servidor, tipo);
                    }
                }
            }

            RegistrarLogCopiarDados("Dados de configuração carregados.");

            // Atualiza o estado dos botões após carregar
            AtualizarEstadoBotoesConfig();
        }

        public class ServidorItem
        {
            public string Nome { get; set; }
            public string Tipo { get; set; } // "Servico" ou "Aplicacao"

            public string SubDiretorios { get; set; } // Caso tiver outras pastas dentro da pasta server

            public string CaminhoCompletoAplicacao { get; set; } // "Servico" ou "Aplicacao"

            public override string ToString()
            {
                return Nome;
            }
        }

        public class ClienteItem
        {
            public string Nome { get; set; }
            public string Categoria { get; set; } // "Servico" ou "Aplicacao"

            public string SubDiretorios { get; set; } // Caso tiver outras pastas dentro da pasta server

            public string CaminhoCompletoCliente { get; set; } // "Servico" ou "Aplicacao"

            public override string ToString()
            {
                return Nome;
            }
        }

        public class Colaborador
        {
            public string Nome { get; set; }

            public string Funcao { get; set; }

            public string GitHub { get; set; }

            public override string ToString()
            {
                return Nome;
            }
        }

        public class Atualizador
        {
            public string Nome { get; set; }
            public string Caminho { get; set; }

            public override string ToString()
            {
                return Nome;
            }
        }

        private void btnBuscarCaminhoBranch_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório da branch";
                dialogo.ShowNewFolderButton = true;
                dialogo.SelectedPath = edtCaminhoBranch.Text;

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    edtCaminhoBranch.Text = dialogo.SelectedPath;
                }
            }

        }

        private void frmCopiarExes_Load(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X, 0);
            lbLog.HorizontalScrollbar = true;
            lbLogServidores.HorizontalScrollbar = true;
            RegistrarLogCopiarDados("CopiarExes aberto");
            preencheAutomaticamenteOCampoDe();
            RegistrarLogCopiarDados("Preencheu o campo DE.");
            preencheAutomaticamenteOCampoPara();
            RegistrarLogCopiarDados("Preencheu o campo PARA");
            CarregarBancoDeDados();
            RegistrarLogCopiarDados("Carregou informações de banco de dados");
            CarregarClientes();
            RegistrarLogCopiarDados("Carregou informações de aplicações cliente");
            CarregarServidores();
            RegistrarLogCopiarDados("Carregou informações de servidores");
            carregarColaboradores();

            string rodarNaBandeja = LerValorIni("CONFIG_GERAIS", "RODAR_NA_BANDEJA", this.caminhoIni);

            if (rodarNaBandeja == "Sim")
            {
                icBandeja.Visible = true;
                icBandeja.Icon = this.Icon; // pode usar o mesmo ícone do form
                icBandeja.Text = "CopiarExes";
                icBandeja.Visible = false;

                ContextMenuStrip menu = new ContextMenuStrip();
                var clientesItem = new ToolStripMenuItem("Clientes");
                Dictionary<string, ToolStripMenuItem> categorias = new Dictionary<string, ToolStripMenuItem>();
                foreach (var item in cbGroupClientes.Items)
                {
                    if (item is ClienteItem cliente)
                    {
                        string categoria = cliente.Categoria;
                        string clienteSemExtensaoExe = Path.GetFileNameWithoutExtension(cliente.Nome);

                        ToolStripMenuItem categoriaMenu;

                        // Verifica se a categoria já existe dentro do menu "Clientes"
                        if (!categorias.TryGetValue(categoria, out categoriaMenu))
                        {
                            // Cria a categoria e adiciona dentro do menu "Clientes"
                            categoriaMenu = new ToolStripMenuItem(categoria);
                            clientesItem.DropDownItems.Add(categoriaMenu);

                            // Guarda no dicionário para reutilizar
                            categorias[categoria] = categoriaMenu;
                        }

                        // Agora adiciona o cliente dentro da categoria
                        categoriaMenu.DropDownItems.Add(clienteSemExtensaoExe, null, (s, e) =>
                        {
                            RegistrarLogCopiarDados($"Abrindo sistema {cliente.Nome} da categoria {categoria}");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = cliente.CaminhoCompletoCliente, // precisa ser o caminho completo do .exe
                                UseShellExecute = true
                            });
                        });
                    }
                }

                menu.Items.Add(clientesItem);
                var servidoresItem = new ToolStripMenuItem("Servidores");

                foreach (var item in cbGroupServidores.Items)
                {
                    ServidorItem servidor = item as ServidorItem;
                    string servidorSemExtensaoExe = Path.GetFileNameWithoutExtension(servidor.Nome);
                    var servidorItem = new ToolStripMenuItem(servidorSemExtensaoExe);

                    servidorItem.DropDownItems.Add("Iniciar", null, (s, e) => IniciarServidor(servidor, true));
                    servidorItem.DropDownItems.Add("Parar", null, (s, e) => PararServidor(servidor, true));
                    servidorItem.DropDownItems.Add("Reiniciar", null, (s, e) => ReiniciarServidor(servidor, true));

                    servidoresItem.DropDownItems.Add(servidorItem);
                }

                menu.Items.Add(servidoresItem);
                menu.Items.Add(new ToolStripSeparator());

                menu.Items.Add("Restaurar", null, (s, e) =>
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    icBandeja.Visible = false;
                });
                menu.Items.Add("Sair", null, (s, e) =>
                {
                    icBandeja.Visible = false;
                    Application.Exit();
                });

                icBandeja.ContextMenuStrip = menu;
            }
        }

        private void ReiniciarServidor(ServidorItem item, Boolean acionadoNaBandeja)
        {
            if (item is ServidorItem servidor)
            {
                if (servidor.Tipo == "Servico")
                {
                    try
                    {
                        ServiceController sc = new ServiceController(servidor.Nome);

                        // Se estiver rodando, para primeiro
                        if (sc.Status != ServiceControllerStatus.Stopped &&
                            sc.Status != ServiceControllerStatus.StopPending)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                            RegistrarLogCopiarDados("Parou o serviço " + servidor.Nome);
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Serviço {servidor.Nome} parado com sucesso.",
                                                "Serviço Parado",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                        }

                        // Agora inicia novamente
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        RegistrarLogCopiarDados("Reiniciou o serviço " + servidor.Nome);
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Serviço {servidor.Nome} reiniciado com sucesso.",
                                            "Serviço Reiniciado",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao reiniciar serviço {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao reiniciar serviço {servidor.Nome}: {ex.Message}",
                                            "Erro",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                    }
                }
                else if (servidor.Tipo == "Aplicacao")
                {
                    try
                    {
                        var processos = Process.GetProcessesByName(
                            System.IO.Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao));

                        // Se estiver rodando, mata primeiro
                        foreach (var processo in processos)
                        {
                            processo.Kill();
                            processo.WaitForExit();
                            RegistrarLogCopiarDados("Parou a aplicação: " + servidor.Nome);
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Aplicação {servidor.Nome} parada com sucesso.",
                                                "Aplicação Parada",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                        }

                        // Agora inicia novamente
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = servidor.CaminhoCompletoAplicacao, // aqui precisa ser o caminho completo do .exe
                            UseShellExecute = true
                        });

                        RegistrarLogCopiarDados("Reiniciou a aplicação: " + servidor.Nome);
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao reiniciar aplicação {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao reiniciar aplicação {servidor.Nome}: {ex.Message}",
                                            "Erro",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PararServidor(ServidorItem item, Boolean acionadoNaBandeja)
        {
            if (item is ServidorItem servidor)
            {
                if (servidor.Tipo == "Servico")
                {
                    try
                    {
                        ServiceController sc = new ServiceController(servidor.Nome);
                        if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Serviço {servidor.Nome} já está parado.", "Serviço não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return; // Já está parado
                        }

                        if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                            RegistrarLogCopiarDados("Parou o serviço " + servidor.Nome);
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Serviço {servidor.Nome} parado com sucesso.", "Serviço Parado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao parar serviço {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao parar serviço {servidor.Nome}: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (servidor.Tipo == "Aplicacao")
                {
                    try
                    {
                        var processos = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao));
                        if (processos.Length == 0 && acionadoNaBandeja)
                        {
                            // Nenhum processo encontrado
                            MessageBox.Show($"A aplicação {servidor.Nome} já está fechada.",
                                            "Aplicação não encontrada",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }

                        foreach (var processo in processos)
                        {
                            processo.Kill();
                            RegistrarLogCopiarDados("Parou a aplicação: " + servidor.Nome);
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Aplicação {servidor.Nome} parada com sucesso.", "Aplicação Parada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao fechar aplicação {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao fechar aplicação {servidor.Nome}: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void IniciarServidor(ServidorItem item, Boolean acionadoNaBandeja)
        {
            if (item is ServidorItem servidor)
            {
                if (servidor.Tipo == "Servico")
                {
                    try
                    {
                        ServiceController sc = new ServiceController(servidor.Nome);

                        if (sc.Status == ServiceControllerStatus.Running ||
                            sc.Status == ServiceControllerStatus.StartPending)
                        {
                            if (acionadoNaBandeja)
                                MessageBox.Show($"Serviço {servidor.Nome} já está em execução.",
                                                "Serviço em execução",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            return;
                        }

                        // Inicia o serviço
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));

                        RegistrarLogCopiarDados("Iniciou o serviço " + servidor.Nome);
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Serviço {servidor.Nome} iniciado com sucesso.",
                                            "Serviço Iniciado",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao iniciar serviço {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao iniciar serviço {servidor.Nome}: {ex.Message}",
                                            "Erro",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                    }
                }
                else if (servidor.Tipo == "Aplicacao")
                {
                    try
                    {
                        var processos = Process.GetProcessesByName(
                            System.IO.Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao));

                        if (processos.Length > 0)
                        {
                            if (acionadoNaBandeja)
                                MessageBox.Show($"A aplicação {servidor.Nome} já está em execução.",
                                                "Aplicação em execução",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            return;
                        }

                        // Inicia a aplicação
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = servidor.CaminhoCompletoAplicacao, // precisa ser o caminho completo do .exe
                            UseShellExecute = true
                        });

                        RegistrarLogCopiarDados("Iniciou a aplicação: " + servidor.Nome);
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao iniciar aplicação {servidor.Nome}: {ex.Message}");
                        if (acionadoNaBandeja)
                            MessageBox.Show($"Erro ao iniciar aplicação {servidor.Nome}: {ex.Message}",
                                            "Erro",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void preencheAutomaticamenteOCampoDe()
        {
            string valorDE = LerValorIni("CAMINHOS", "DE", this.caminhoIni);
            if (Directory.Exists(valorDE))
            {
                edtCaminhoBranch.Text = valorDE;
            }
            else
            {
                RegistrarLogCopiarDados("Não encontrou o arquivo Inicializar.ini ou parâmetro DE não existe.");
                MessageBox.Show("O arquivo Inicializar.ini ou parâmetro DE não existe. Consulte a documentação no repositório do projeto. Site: https://github.com/giancarlogiulian/CopiarExes", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void preencheAutomaticamenteOCampoPara()
        {
            string valorPARA = LerValorIni("CAMINHOS", "PARA", this.caminhoIni);
            if (Directory.Exists(valorPARA))
            {
                edtPastaDestino.Text = valorPARA;
            }
            else
            {
                RegistrarLogCopiarDados("Não encontrou o arquivo Inicializar.ini ou parâmetro PARA não existe.");
                MessageBox.Show("O arquivo Inicializar.ini ou parâmetro PARA não existe. Consulte a documentação no repositório do projeto. Site: https://github.com/giancarlogiulian/CopiarExes", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CarregarBancoDeDados()
        {
            string countStr = LerValorIni("BANCO_DE_DADOS", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i <= count; i++)
                {
                    string chave = $"BancoDados{i}";
                    string valor = LerValorIni("BANCO_DE_DADOS", chave, caminhoIni);
                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        cbGroupAtualizadores.Items.Add(valor);
                        cbGroupAtualizadores.SetItemChecked(i, true);
                        RegistrarLogCopiarDados("Carregou a informação do banco " + valor + ". Parâmetro: " + chave);
                    }
                }
            }
        }

        private void CarregarClientes()
        {
            string countStr = LerValorIni("APLICACOES_CLIENTE", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i <= count - 1; i++)
                {
                    string clienteId = $"Cliente{i}";
                    string categoriaId = $"Categoria{i}";
                    string subDiretoriosId = $"SubDiretorios{i}";

                    string cliente = LerValorIni("APLICACOES_CLIENTE", clienteId, caminhoIni);
                    string categoria = LerValorIni("APLICACOES_CLIENTE", categoriaId, caminhoIni);
                    string subDiretorio = LerValorIni("APLICACOES_CLIENTE", subDiretoriosId, caminhoIni);


                    if (!string.IsNullOrWhiteSpace(cliente))
                    {
                        string pastaClient = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni);
                        string caminhoCompletoCliente = getCaminhoCompletoAplicacao(cliente, subDiretorio, pastaClient);

                        cbGroupClientes.Items.Add(new ClienteItem
                        {
                            Nome = cliente,
                            Categoria = categoria,
                            SubDiretorios = subDiretorio,
                            CaminhoCompletoCliente = caminhoCompletoCliente
                        });
                        cbGroupClientes.SetItemChecked(i, true);
                        RegistrarLogCopiarDados("Carregou a informação da aplicação cliente " + cliente + ". Parâmetro: " + clienteId);
                    }
                }
            }
        }

        private void CarregarServidores()
        {
            string countStr = LerValorIni("APLICACOES_SERVIDORAS", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i <= count - 1; i++)
                {
                    string servidorId = $"Servidor{i}";
                    string tipoId = $"Tipo{i}";
                    string subDiretoriosId = $"SubDiretorios{i}";
                    string servidor = LerValorIni("APLICACOES_SERVIDORAS", servidorId, caminhoIni);
                    string tipo = LerValorIni("APLICACOES_SERVIDORAS", tipoId, caminhoIni);
                    string subDir = LerValorIni("APLICACOES_SERVIDORAS", subDiretoriosId, caminhoIni);
                    if (!string.IsNullOrWhiteSpace(servidor) && !string.IsNullOrWhiteSpace(tipo))
                    {
                        string pastaClient = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);
                        string caminhoCompletoAplicacao = getCaminhoCompletoAplicacao(servidor, subDir, pastaClient);
                        cbGroupServidores.Items.Add(new ServidorItem
                        {
                            Nome = servidor,
                            Tipo = tipo,
                            CaminhoCompletoAplicacao = caminhoCompletoAplicacao,
                            SubDiretorios = subDir
                        });

                        cbGroupServidores.SetItemChecked(i, true);
                        RegistrarLogCopiarDados("Carregou a informação da aplicação servidora " + servidor + ". Parâmetro: " + servidorId);
                    }
                }
            }
        }

        private string LerValorIni(string secao, string chave, string caminhoArquivo)
        {
            StringBuilder buffer = new StringBuilder(255);
            GetPrivateProfileString(secao, chave, "", buffer, buffer.Capacity, caminhoArquivo);
            return buffer.ToString();
        }

        private void btnProcurarPastaDestino_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório de destino";
                dialogo.ShowNewFolderButton = true;
                dialogo.SelectedPath = edtPastaDestino.Text;

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    edtPastaDestino.Text = dialogo.SelectedPath;
                }
            }
        }

        private void RegistrarLogCopiarDados(string mensagem)
        {
            if (lbLog.InvokeRequired)
            {
                lbLog.BeginInvoke(new Action<string>(RegistrarLogCopiarDados), mensagem);
                return;
            }

            string log = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {mensagem}";
            lbLog.Items.Add(log);
            lbLog.TopIndex = lbLog.Items.Count - 1;
            AjustarHorizontalExtentLbLog();
        }


        private void RegistrarLogServidores(string mensagem)
        {
            if (lbLogServidores.InvokeRequired)
            {
                lbLogServidores.BeginInvoke(new Action<string>(RegistrarLogServidores), mensagem);
                return;
            }

            string log = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {mensagem}";
            lbLogServidores.Items.Add(log);
            lbLogServidores.TopIndex = lbLogServidores.Items.Count - 1;
            AjustarHorizontalExtentLbLogServidores();
        }

        private void btnCopiarDados_Click(object sender, EventArgs e)
        {
            RegistrarLogCopiarDados("Parando aplicações clientes");
            encerrarClientes();
        }

        private void encerrarServidores()
        {
            foreach (var item in cbGroupServidores.CheckedItems)
            {
                ServidorItem servidor = item as ServidorItem;
                PararServidor(servidor, false);
            }
        }

        public string getCaminhoCompletoAplicacao(string item, string pastaClient)
        {
            string caminho = edtPastaDestino.Text;
            string nomeExe = item.ToString();

            return caminho + "\\" + pastaClient + "\\" + nomeExe;
        }

        public string getCaminhoCompletoAplicacao(string item, string subDiretorios, string pastaClient)
        {

            string caminho = edtPastaDestino.Text;
            string nomeExe = item.ToString();
            if (!string.IsNullOrWhiteSpace(subDiretorios))
            {
                // Se tiver subdiretórios, adiciona ao caminho
                caminho = Path.Combine(caminho, pastaClient, subDiretorios);
                return Path.Combine(caminho, nomeExe);
            }

            return caminho + "\\" + pastaClient + "\\" + nomeExe;
        }

        private void encerrarClientes()
        {
            string pastaComandos = Path.Combine(Application.StartupPath, "ComandosExecutados");
            // Cria a pasta se não existir
            if (!Directory.Exists(pastaComandos))
            {
                Directory.CreateDirectory(pastaComandos);
            }

            string pastaClient = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni);
            string timestamp = DateTime.Now.ToString("ddMMyyyy-HHmmss");
            string nomeArquivo = $"CopiarExes{timestamp}-EXCLUSAO-APLICACOES-CLIENTES.bat";
            string caminhoBat = Path.Combine(pastaComandos, nomeArquivo);

            List<string> linhas = new List<string>();

            linhas.Add("@echo on");
            linhas.Add("echo Finalizando aplicacoes clientes...");

            foreach (var item in cbGroupClientes.CheckedItems)
            {
                string nomeExe = item.ToString(); // Ex: Cliente1.exe
                linhas.Add($"taskkill /f /im {nomeExe} /t /fi \"status eq running\" >nul");
            }

            linhas.Add("");
            linhas.Add("echo Excluindo os arquivos das aplicacoes clientes...");

            foreach (var item in cbGroupClientes.CheckedItems)
            {
                string caminho = edtPastaDestino.Text;
                string nomeExe = item.ToString();
                string caminhoExe = getCaminhoCompletoAplicacao(nomeExe, pastaClient);
                linhas.Add($"del \"{caminhoExe}\" /s /f /q");
            }

            linhas.Add("");
            linhas.Add("echo Operacao finalizada.");
            linhas.Add("exit");

            // Grava o arquivo
            File.WriteAllLines(caminhoBat, linhas);

            var processoBat = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = caminhoBat,
                    UseShellExecute = false,       // permite esperar o processo
                    RedirectStandardOutput = true,    // captura saída padrão
                    RedirectStandardError = true,     // captura erros
                    CreateNoWindow = false,        // mostra a janela do .BAT
                    WindowStyle = ProcessWindowStyle.Hidden // oculta a janela do .BAT
                },
                EnableRaisingEvents = true
            };

            // Assina os eventos de saída
            processoBat.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    RegistrarLogCopiarDados($"[OUT] {e.Data}");
                }
            };

            processoBat.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    RegistrarLogCopiarDados($"[ERR] {e.Data}");
                }
            };

            // Quando terminar
            processoBat.Exited += (s, e) =>
            {
                processoBat.CancelOutputRead();
                processoBat.CancelErrorRead();
                RegistrarLogCopiarDados("Processo .BAT de excluir aplicacoes clientes finalizado.");

                RegistrarLogCopiarDados("Parando aplicações servidoras");
                encerrarServidores();
                RegistrarLogCopiarDados("Copiando executáveis... Por favor, aguarde...");
                copiarArquivos();
            };
            processoBat.Start();

            processoBat.BeginOutputReadLine();
            processoBat.BeginErrorReadLine();
        }

        private void copiarArquivos()
        {
            string pastaComandos = Path.Combine(Application.StartupPath, "ComandosExecutados");
            // Cria a pasta se não existir
            if (!Directory.Exists(pastaComandos))
            {
                Directory.CreateDirectory(pastaComandos);
            }

            string de = edtCaminhoBranch.Text;
            string para = edtPastaDestino.Text;
            string dePastaClient = LerValorIni("CAMINHOS", "DE_PASTA_CLIENT", caminhoIni);
            string dePastaServer = LerValorIni("CAMINHOS", "DE_PASTA_SERVER", caminhoIni);
            string dePastaDados = LerValorIni("CAMINHOS", "DE_PASTA_DADOS", caminhoIni);

            string pastaClient = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni);
            string pastaServer = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);
            string pastaDados = LerValorIni("CAMINHOS", "PASTA_DADOS", caminhoIni);

            string timestamp = DateTime.Now.ToString("ddMMyyyy-HHmmss");
            string nomeArquivo = $"CopiarExes{timestamp}-COPIAR-ARQUIVOS.bat";
            string caminhoBat = Path.Combine(pastaComandos, nomeArquivo);

            List<string> linhas = new List<string>();

            linhas.Add("@echo on");
            linhas.Add("echo Copiando os arquivos do sistema...");

            foreach (var item in cbGroupClientes.CheckedItems)
            {
                if (item is ClienteItem cliente)
                {

                    string nomeExe = item.ToString(); // Ex: Cliente1.exe

                    linhas.Add($"echo Copiando \"{de}\\{dePastaClient}\\{nomeExe}\" -> \"{cliente.CaminhoCompletoCliente}\"");
                    linhas.Add($"copy \"{de}\\{dePastaClient}\\{nomeExe}\" \"{cliente.CaminhoCompletoCliente}\" /y");
                }
            }

            foreach (var item in cbGroupServidores.CheckedItems)
            {
                if (item is ServidorItem servidor)
                {
                    string nomeExe = item.ToString(); // Ex: Cliente1.exe

                    string destino = servidor.CaminhoCompletoAplicacao;
                    if (destino.EndsWith("\\"))
                        destino = destino.Substring(0, servidor.CaminhoCompletoAplicacao.Length - 1);

                    // Monta o comando com xcopy
                    linhas.Add($"xcopy \"{de}\\{dePastaServer}\\{nomeExe}\" \"{destino}\" /Y /F");
                }
            }

            foreach (var item in cbGroupAtualizadores.CheckedItems)
            {
                string nomeAtualizador = item.ToString(); // Ex: Cliente1.exe
                linhas.Add($"copy \"{de}\\{dePastaDados}\\{nomeAtualizador}\" \"{para}\\{pastaDados}\\{item}\" /y");
            }

            linhas.Add("");
            linhas.Add("echo Operacao finalizada.");
            linhas.Add("exit");

            // Grava o arquivo
            File.WriteAllLines(caminhoBat, linhas);

            // Executa o .BAT
            var processoBat = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = caminhoBat,
                    UseShellExecute = false,       // permite esperar o processo
                    RedirectStandardOutput = true,    // captura saída padrão
                    RedirectStandardError = true,     // captura erros
                    CreateNoWindow = false,        // mostra a janela do .BAT
                    WindowStyle = ProcessWindowStyle.Hidden // oculta a janela do .BAT
                },
                EnableRaisingEvents = true
            };

            // Assina os eventos de saída
            processoBat.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    RegistrarLogCopiarDados($"[OUT] {e.Data}");
                }
            };

            processoBat.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    RegistrarLogCopiarDados($"[ERR] {e.Data}");
                }
            };

            processoBat.Exited += (s, e) =>
            {
                processoBat.CancelOutputRead();
                processoBat.CancelErrorRead();

                RegistrarLogCopiarDados("Processo .BAT de copiar EXEs finalizado.");
                RegistrarLogCopiarDados("Iniciando servidores...");
                iniciarServidores(); // será chamado quando o .BAT terminar
            };


            processoBat.Start();

            processoBat.BeginOutputReadLine();
            processoBat.BeginErrorReadLine();
        }

        private void AjustarHorizontalExtentLbLog()
        {
            int maxWidth = 0;
            using (Graphics g = lbLog.CreateGraphics())
            {
                foreach (var item in lbLog.Items)
                {
                    int itemWidth = (int)g.MeasureString(item.ToString(), lbLog.Font).Width;
                    if (itemWidth > maxWidth)
                        maxWidth = itemWidth;
                }
            }
            lbLog.HorizontalExtent = maxWidth;
        }

        private void AjustarHorizontalExtentLbLogServidores()
        {
            int maxWidth = 0;
            using (Graphics g = lbLogServidores.CreateGraphics())
            {
                foreach (var item in lbLogServidores.Items)
                {
                    int itemWidth = (int)g.MeasureString(item.ToString(), lbLog.Font).Width;
                    if (itemWidth > maxWidth)
                        maxWidth = itemWidth;
                }
            }
            lbLogServidores.HorizontalExtent = maxWidth;
        }

        private void iniciarServidores()
        {
            string para = edtPastaDestino.Text;
            string pastaServer = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);

            foreach (var item in cbGroupServidores.CheckedItems)
            {
                if (item is ServidorItem servidor)
                {
                    try
                    {
                        if (servidor.Tipo == "Servico")
                        {
                            ServiceController sc = new ServiceController(servidor.Nome);
                            if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                            {
                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                RegistrarLogCopiarDados("Iniciou o servico " + servidor.Nome);
                            }
                        }
                        else if (servidor.Tipo == "Aplicacao")
                        {
                            // Aqui servidor.Nome deve ser o caminho completo do .exe
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = para + "\\" + pastaServer + "\\" + servidor.Nome,
                                UseShellExecute = true // garante que abra como aplicação normal
                            });
                            RegistrarLogCopiarDados("Iniciou a aplicacao " + servidor.Nome);
                        }
                    }
                    catch (Exception ex)
                    {
                        RegistrarLogCopiarDados($"Erro ao iniciar {servidor.Tipo.ToLower()} {servidor.Nome}: {ex.Message}");
                    }
                }
            }

            RegistrarLogCopiarDados("Sistema pronto para uso...");
            RegistrarLogCopiarDados("Se necessário, atualize o banco de dados a ser utilizado...");
        }

        protected override void OnResize(EventArgs e)
        {
            string rodarNaBandeja = LerValorIni("CONFIG_GERAIS", "RODAR_NA_BANDEJA", this.caminhoIni);

            if (rodarNaBandeja == "Sim")
            {

                base.OnResize(e);

                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    icBandeja.Visible = true;
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show(); // mostra novamente o formulário
            this.WindowState = FormWindowState.Normal; // restaura se estava minimizado
            this.BringToFront(); // garante que venha para frente
            icBandeja.Visible = false; // opcional: esconde o ícone da bandeja
        }

        private void frmCopiarExes_FormClosing(object sender, FormClosingEventArgs e)
        {
            string rodarNaBandeja = LerValorIni("CONFIG_GERAIS", "RODAR_NA_BANDEJA", this.caminhoIni);

            if (rodarNaBandeja == "Sim")
            {
                // Se o usuário clicou no X (CloseButton)
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;          // Cancela o fechamento
                    this.Hide();              // Esconde a janela
                    icBandeja.Visible = true; // Mantém o ícone na bandeja
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Atualizador> atualizadores = new List<Atualizador>();
            string countStr = LerValorIni("ATUALIZADORES", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i <= count; i++)
                {
                    string nomeAtualizadorId = $"NomeAtualizador{i}";
                    string caminhoAtualizadorId = $"CaminhoAtualizador{i}";
                    string nomeAtualizador = LerValorIni("ATUALIZADORES", nomeAtualizadorId, caminhoIni);
                    string caminhoAtualizador = LerValorIni("ATUALIZADORES", caminhoAtualizadorId, caminhoIni);
                    if (!string.IsNullOrWhiteSpace(nomeAtualizador) && !string.IsNullOrWhiteSpace(caminhoAtualizador))
                    {
                        atualizadores.Add(new Atualizador
                        {
                            Nome = nomeAtualizador,
                            Caminho = caminhoAtualizador
                        });

                    }
                }
            }

            if (atualizadores.Any())
            {
                using (frmAtualizador form = new frmAtualizador(atualizadores))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        Atualizador escolhido = form.atualizadorSelecionado;

                        if (escolhido != null && !string.IsNullOrWhiteSpace(escolhido.Caminho))
                        {
                            try
                            {
                                Process.Start(escolhido.Caminho);
                            }
                            catch (Exception ex)
                            {
                                RegistrarLogCopiarDados($"Erro ao abrir o atualizador: {ex.Message}");
                                MessageBox.Show($"Erro ao abrir o programa: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void carregarColaboradores()
        {

            List<Colaborador> contribuidores = new List<Colaborador>
            {
                new Colaborador { Nome = "Fernando Bolson Dias",
                    Funcao = "Arquiteto de Software",
                    GitHub = "Não informado"},

            new Colaborador { Nome = "Giancarlo Abel Giulian",
                Funcao = "Analista de Testes",
                GitHub = "https://github.com/giancarlogiulian"},

            new Colaborador { Nome = "Nicolas Schiochet",
                Funcao = "Analista de Testes",
                GitHub = "https://github.com/nicochet01"}
            };

            dgvColaboradores.DataSource = contribuidores;
            dgvColaboradores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvColaboradores.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                // Abre o link no navegador padrão
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/nicochet01/CopiarExes",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir o link: " + ex.Message);
            }

        }

        private void cbGroupAtualizadores_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                ContextMenuStrip cmsAtualizadores = new ContextMenuStrip();
                var marcarTodosItem = cmsAtualizadores.Items.Add("Marcar todos");
                var desmarcarTodosItem = cmsAtualizadores.Items.Add("Desmarcar todos");
                marcarTodosItem.Click += tsmSelecionarTodosAtualizadores_Click;
                desmarcarTodosItem.Click += tsmDesmarcarTodosAtualizadores_Click;

                cmsAtualizadores.Show(cbGroupAtualizadores, e.Location);
            }
        }

        private void tsmSelecionarTodosAtualizadores_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupAtualizadores.Items.Count; i++)
            {
                cbGroupAtualizadores.SetItemChecked(i, true);
            }
        }

        private void tsmDesmarcarTodosAtualizadores_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupAtualizadores.Items.Count; i++)
            {
                cbGroupAtualizadores.SetItemChecked(i, false);
            }
        }

        private void cbGroupClientes_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                ContextMenuStrip cmsClientes = new ContextMenuStrip();
                var marcarTodosItem = cmsClientes.Items.Add("Marcar todos");
                var desmarcarTodosItem = cmsClientes.Items.Add("Desmarcar todos");
                marcarTodosItem.Click += tsmSelecionarTodosClientes_Click;
                desmarcarTodosItem.Click += tsmDesmarcarTodosClientes_Click;

                cmsClientes.Show(cbGroupClientes, e.Location);
            }
        }

        private void tsmSelecionarTodosClientes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupClientes.Items.Count; i++)
            {
                cbGroupClientes.SetItemChecked(i, true);
            }
        }

        private void tsmDesmarcarTodosClientes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupClientes.Items.Count; i++)
            {
                cbGroupClientes.SetItemChecked(i, false);
            }
        }

        private void cbGroupServidores_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                ContextMenuStrip cmsServidores = new ContextMenuStrip();
                var marcarTodosItem = cmsServidores.Items.Add("Marcar todos");
                var desmarcarTodosItem = cmsServidores.Items.Add("Desmarcar todos");
                marcarTodosItem.Click += tsmSelecionarTodosServidores_Click;
                desmarcarTodosItem.Click += tsmDesmarcarTodosServidores_Click;

                cmsServidores.Show(cbGroupServidores, e.Location);
            }
        }

        private void tsmSelecionarTodosServidores_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupServidores.Items.Count; i++)
            {
                cbGroupServidores.SetItemChecked(i, true);
            }
        }

        private void tsmDesmarcarTodosServidores_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbGroupServidores.Items.Count; i++)
            {
                cbGroupServidores.SetItemChecked(i, false);
            }
        }

        private void btnLimparLog_Click(object sender, EventArgs e)
        {
            lbLog.Items.Clear();
        }

        private void tabCopiarExes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCopiarExes.SelectedTab == tabServidores)
            {
                CarregarServidoresNoGroupBox();
            }
            // ADICIONE ESTA NOVA VERIFICAÇÃO "ELSE IF" ABAIXO
            else if (tabCopiarExes.SelectedTab == tabConfiguracoes)
            {
                // Esta chamada vai "acender" e EXECUTAR o seu método
                CarregarDadosDeConfiguracao();
            }

        }

        private void CarregarServidoresNoGroupBox()
        {
            groupboxServidores.Controls.Clear(); // limpa os controles anteriores

            int y = 30; // posição vertical inicial

            foreach (var item in cbGroupServidores.Items)
            {
                if (item is ServidorItem servidor)
                {
                    // Cria o CheckBox
                    CheckBox chk = new CheckBox();
                    chk.Text = servidor.Nome;
                    chk.Tag = servidor; // guarda o objeto para uso posterior
                    chk.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                    chk.Location = new Point(10, y);
                    chk.AutoSize = true;

                    // Cria o Label de status
                    Label lblStatus = new Label();
                    lblStatus.Location = new Point(200, y + 1);
                    lblStatus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    lblStatus.AutoSize = true;

                    string status = ObterStatusServidor(servidor);
                    // Define a cor conforme o status

                    lblStatus.Text = status;
                    lblStatus.Tag = status;
                    chk.Tag = (servidor, lblStatus);
                    chk.CheckedChanged += (s, e) => AtualizarBotoes();

                    AtualizarStatus(lblStatus, status);
                    groupboxServidores.Controls.Add(chk);
                    groupboxServidores.Controls.Add(lblStatus);

                    y += 25;
                }
            }

            AtualizarBotoes();
            timerStatusServidores.Interval = 1000; // 1 segundo
            timerStatusServidores.Tick += timerStatusServidores_Tick;
            timerStatusServidores.Start();

            RegistrarLogServidores("Atualizou o status dos servidores.");
        }

        private void AtualizarStatus(Label lblStatus, string status)
        {
            lblStatus.Text = status;

            switch (status)
            {
                case "Iniciado":
                    lblStatus.ForeColor = Color.Green;
                    break;
                case "Parado":
                    lblStatus.ForeColor = Color.Red;
                    break;
                case "Não Encontrado":
                    lblStatus.ForeColor = Color.DarkGoldenrod;
                    break;
                case "Desconhecido":
                    lblStatus.ForeColor = Color.DarkGray;
                    break;
                default:
                    lblStatus.ForeColor = Color.Black;
                    break;
            }
        }

        private string ObterStatusServidor(ServidorItem servidor)
        {
            try
            {
                if (servidor.Tipo == "Servico")
                {
                    ServiceController sc = new ServiceController(servidor.Nome);
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            return "Iniciado";
                        case ServiceControllerStatus.Stopped:
                            return "Parado";
                            return "Parado";
                        default:
                            return sc.Status.ToString();
                    }
                }
                else if (servidor.Tipo == "Aplicacao")
                {
                    var processos = Process.GetProcessesByName(
                        Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao));

                    return processos.Length > 0 ? "Iniciado" : "Parado";
                }
            }
            catch
            {
                return "Não Encontrado";
            }

            return "Desconhecido";
        }

        private void chkSelecionarEmExecucao_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelecionarParados.Checked)
            {
                // Desmarca a outra opção para evitar conflito
                cbSelecionarParados.Checked = false;
            }

            foreach (Control ctrl in groupboxServidores.Controls)
            {
                if (ctrl is CheckBox chk && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    Label lblStatus = dados.Item2;

                    if (cbEmExecucao.Checked)
                    {
                        // Marca apenas os que estão "Iniciado"
                        chk.Checked = string.Equals(lblStatus.Text, "Iniciado", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // Se desmarcar a opção, limpa todos
                        chk.Checked = false;
                    }
                }
            }

            AtualizarBotoes();
        }
        private void cbSelecionarParados_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelecionarParados.Checked)
            {
                // Desmarca a outra opção para evitar conflito
                cbEmExecucao.Checked = false;
            }

            foreach (Control ctrl in groupboxServidores.Controls)
            {
                if (ctrl is CheckBox chk && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    Label lblStatus = dados.Item2;

                    if (cbSelecionarParados.Checked)
                    {
                        // Marca apenas os que estão "Parado"
                        chk.Checked = string.Equals(lblStatus.Text, "Parado", StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // Se desmarcar a opção, limpa todos
                        chk.Checked = false;
                    }
                }
            }

            AtualizarBotoes();
        }
        private void AtualizarBotoes()
        {
            bool podeIniciar = false;
            bool podeParar = false;
            bool podeReiniciar = false;

            foreach (var chk in groupboxServidores.Controls.OfType<CheckBox>())
            {
                if (chk.Checked && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    Label lblStatus = dados.Item2;
                    string status = lblStatus.Text;

                    if (status == "Parado")
                    {
                        podeIniciar = true;
                        podeReiniciar = true;
                    }
                    else if (status == "Iniciado")
                    {
                        podeParar = true;
                        podeReiniciar = true;
                    }
                }

            }

            btnIniciar.Enabled = podeIniciar;
            btnParar.Enabled = podeParar;
            btnReiniciar.Enabled = podeReiniciar;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            foreach (var chk in groupboxServidores.Controls.OfType<CheckBox>())
            {
                if (chk.Checked && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    ServidorItem servidor = dados.Item1;
                    Label lblStatus = dados.Item2;

                    if (lblStatus.Text == "Parado")
                    {
                        try
                        {
                            if (servidor.Tipo == "Servico")
                            {
                                ServiceController sc = new ServiceController(servidor.Nome);
                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                RegistrarLogServidores("Iniciou o serviço " + servidor.Nome);
                            }
                            else if (servidor.Tipo == "Aplicacao")
                            {
                                Process.Start(servidor.CaminhoCompletoAplicacao);
                                RegistrarLogServidores("Iniciou a aplicação " + servidor.Nome);
                            }

                            // Atualiza visualmente
                            AtualizarStatus(lblStatus, "Iniciado");
                        }
                        catch
                        {
                            AtualizarStatus(lblStatus, "Não Encontrado");
                        }
                    }
                }
            }

            cbSelecionarParados.Checked = false;
            cbEmExecucao.Checked = false;
            AtualizarBotoes();
        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            foreach (var chk in groupboxServidores.Controls.OfType<CheckBox>())
            {
                if (chk.Checked && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    ServidorItem servidor = dados.Item1;
                    Label lblStatus = dados.Item2;

                    if (lblStatus.Text == "Iniciado")
                    {
                        try
                        {
                            if (servidor.Tipo == "Servico")
                            {
                                ServiceController sc = new ServiceController(servidor.Nome);
                                sc.Stop();
                                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                                RegistrarLogServidores("Parou o serviço " + servidor.Nome);
                            }
                            else if (servidor.Tipo == "Aplicacao")
                            {
                                foreach (var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao)))
                                {
                                    proc.Kill();
                                    RegistrarLogServidores("Parou a aplicação " + servidor.Nome);
                                }
                            }

                            AtualizarStatus(lblStatus, "Parado");
                        }
                        catch
                        {
                            AtualizarStatus(lblStatus, "Não Encontrado");
                        }
                    }
                }
            }

            cbSelecionarParados.Checked = false;
            cbEmExecucao.Checked = false;
            AtualizarBotoes();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            foreach (var chk in groupboxServidores.Controls.OfType<CheckBox>())
            {
                if (chk.Checked && chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    ServidorItem servidor = dados.Item1;
                    Label lblStatus = dados.Item2;

                    if (lblStatus.Text == "Iniciado" || lblStatus.Text == "Parado")
                    {
                        try
                        {
                            if (servidor.Tipo == "Servico")
                            {
                                ServiceController sc = new ServiceController(servidor.Nome);
                                if (sc.Status == ServiceControllerStatus.Running)
                                {
                                    sc.Stop();
                                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                                    RegistrarLogServidores("Parou o serviço " + servidor.Nome);
                                }

                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                RegistrarLogServidores("Iniciou o serviço " + servidor.Nome);
                            }
                            else if (servidor.Tipo == "Aplicacao")
                            {
                                foreach (var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(servidor.CaminhoCompletoAplicacao)))
                                {
                                    proc.Kill();
                                    RegistrarLogServidores("Parou a aplicação " + servidor.Nome);
                                }

                                Process.Start(servidor.CaminhoCompletoAplicacao);
                                RegistrarLogServidores("Iniciou a aplicação " + servidor.Nome);
                            }

                            AtualizarStatus(lblStatus, "Iniciado");
                        }
                        catch
                        {
                            AtualizarStatus(lblStatus, "Não Encontrado");
                        }
                    }
                }
            }

            cbSelecionarParados.Checked = false;
            cbEmExecucao.Checked = false;
            AtualizarBotoes();
        }

        private void timerStatusServidores_Tick(object sender, EventArgs e)
        {
            foreach (var chk in groupboxServidores.Controls.OfType<CheckBox>())
            {
                if (chk.Tag is ValueTuple<ServidorItem, Label> dados)
                {
                    ServidorItem servidor = dados.Item1;
                    Label lblStatus = dados.Item2;

                    string statusAtual = ObterStatusServidor(servidor);

                    if (lblStatus.Text != statusAtual)
                    {
                        AtualizarStatus(lblStatus, statusAtual);
                    }
                }
            }

            AtualizarBotoes();
        }

        private void btnAdicionarCliente_Click(object sender, EventArgs e)
        {
            // Cria uma janela de diálogo para abrir arquivos
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Selecione a Aplicação Cliente";
                dialogo.Filter = "Aplicações (*.exe)|*.exe";

                // Se o usuário escolher um arquivo e clicar em "OK"
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    string nomeDoArquivo = System.IO.Path.GetFileName(dialogo.FileName);

                    // ===============================================
                    // --- INÍCIO DA NOVA VALIDAÇÃO DE DUPLICATAS ---
                    // ===============================================
                    bool jaExiste = false;
                    foreach (DataGridViewRow row in dgvConfigClientes.Rows)
                    {
                        // Verifica se o valor da célula não é nulo antes de comparar
                        if (row.Cells["Nome"].Value != null &&
                            // Compara o nome do arquivo, ignorando se é maiúsculo ou minúsculo
                            string.Equals(row.Cells["Nome"].Value.ToString(), nomeDoArquivo, StringComparison.OrdinalIgnoreCase))
                        {
                            jaExiste = true;
                            break; // Se já achamos, não precisa continuar procurando
                        }
                    }

                    if (jaExiste)
                    {
                        // Se o item já existe, avisa o usuário e não faz mais nada.
                        MessageBox.Show($"A aplicação '{nomeDoArquivo}' já existe na lista.",
                                        "Item Duplicado",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return; // O comando 'return' sai imediatamente do método.
                    }
                    // ===============================================
                    // --- FIM DA VALIDAÇÃO DE DUPLICATAS ---
                    // ===============================================

                    // Se passou pela validação, adiciona o novo item
                    dgvConfigClientes.Rows.Add(nomeDoArquivo, "Categoria Padrão");

                    RegistrarLogCopiarDados($"Aplicação cliente '{nomeDoArquivo}' adicionada à lista de configuração.");

                    configuracoesForamAlteradas = true;

                    AtualizarEstadoBotoesConfig();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void dgvServidores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void scConfiguracoes_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void btnSalvarConfiguracoes_Click(object sender, EventArgs e)
        {
            // Pede uma confirmação final ao usuário
            if (MessageBox.Show("Você tem certeza que deseja salvar todas as alterações no arquivo de configuração?",
                                  "Confirmar",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == DialogResult.No)
            {
                return; // Se o usuário disser não, cancela a operação
            }

            try
            {
                RegistrarLogCopiarDados("Salvando alterações no arquivo Inicializar.ini...");

                // --- SALVANDO APLICAÇÕES CLIENTE ---

                // Limpa a seção antiga de clientes no arquivo .ini
                WritePrivateProfileString("APLICACOES_CLIENTE", null, null, caminhoIni);

                int i = 0;
                foreach (DataGridViewRow row in dgvConfigClientes.Rows)
                {
                    string nome = row.Cells["Nome"].Value?.ToString() ?? "";
                    string categoria = row.Cells["Categoria"].Value?.ToString() ?? "";

                    WritePrivateProfileString("APLICACOES_CLIENTE", $"Cliente{i}", nome, caminhoIni);
                    WritePrivateProfileString("APLICACOES_CLIENTE", $"Categoria{i}", categoria, caminhoIni);
                    WritePrivateProfileString("APLICACOES_CLIENTE", $"SubDiretorios{i}", "", caminhoIni); // Mantém a estrutura
                    i++;
                }
                // Escreve a contagem final de clientes
                WritePrivateProfileString("APLICACOES_CLIENTE", "Count", dgvConfigClientes.Rows.Count.ToString(), caminhoIni);


                // --- SALVANDO APLICAÇÕES/SERVIÇOS SERVIDORES ---

                // Limpa a seção antiga de servidores no arquivo .ini
                WritePrivateProfileString("APLICACOES_SERVIDORAS", null, null, caminhoIni);

                i = 0;
                foreach (DataGridViewRow row in dgvConfigServidores.Rows)
                {
                    string nome = row.Cells["Nome"].Value?.ToString() ?? "";
                    string tipo = row.Cells["Tipo"].Value?.ToString() ?? "";

                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"Servidor{i}", nome, caminhoIni);
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"Tipo{i}", tipo, caminhoIni);
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"SubDiretorios{i}", "", caminhoIni); // Mantém a estrutura
                    i++;
                }
                // Escreve a contagem final de servidores
                WritePrivateProfileString("APLICACOES_SERVIDORAS", "Count", dgvConfigServidores.Rows.Count.ToString(), caminhoIni);

                // --- FINALIZAÇÃO ---

                // Reseta a "bandeira" de alterações, pois agora está tudo salvo
                configuracoesForamAlteradas = false;

                // Atualiza o estado dos botões (vai desabilitar Salvar e Cancelar)
                AtualizarEstadoBotoesConfig();

                RegistrarLogCopiarDados("Alterações salvas com sucesso.");
                MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                RegistrarLogCopiarDados($"Erro ao salvar o arquivo de configuração: {ex.Message}");
                MessageBox.Show($"Ocorreu um erro ao salvar o arquivo de configuração:\n\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdicionarExeServidor_Click(object sender, EventArgs e)
        {
            // Cria uma janela de diálogo para abrir arquivos
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Selecione a Aplicação Servidora (.exe)";
                dialogo.Filter = "Aplicações (*.exe)|*.exe";

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    string nomeDoArquivo = System.IO.Path.GetFileName(dialogo.FileName);

                    // --- VALIDAÇÃO DE DUPLICATAS PARA SERVIDORES ---
                    bool jaExiste = false;
                    foreach (DataGridViewRow row in dgvConfigServidores.Rows)
                    {
                        if (row.Cells["Nome"].Value != null &&
                            string.Equals(row.Cells["Nome"].Value.ToString(), nomeDoArquivo, StringComparison.OrdinalIgnoreCase))
                        {
                            jaExiste = true;
                            break;
                        }
                    }

                    if (jaExiste)
                    {
                        MessageBox.Show($"O item '{nomeDoArquivo}' já existe na lista de servidores.",
                                        "Item Duplicado",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }
                    // --- FIM DA VALIDAÇÃO ---

                    dgvConfigServidores.Rows.Add(nomeDoArquivo, "Aplicacao");

                    RegistrarLogCopiarDados($"Aplicação servidora '{nomeDoArquivo}' adicionada à lista de configuração.");

                    configuracoesForamAlteradas = true;

                    AtualizarEstadoBotoesConfig();
                }
            }
        }

        private void AtualizarEstadoBotoesConfig()
        {
            // --- Lógica para o botão EDITAR ---
            bool temItens = dgvConfigClientes.Rows.Count > 0 || dgvConfigServidores.Rows.Count > 0;
            btnEditarGlobal.Enabled = temItens;

            // --- Lógica para os botões SALVAR e CANCELAR ---
            // Eles só são habilitados se houver alterações pendentes.
            btnSalvarConfiguracoes.Enabled = configuracoesForamAlteradas;
            btnCancelarAlteracoes.Enabled = configuracoesForamAlteradas;

            // --- Lógica para o botão REMOVER ---
            bool algumCheckboxMarcado = false;
            if (emModoEdicaoGlobal) // Só verifica se estivermos em modo de edição
            {
                // Procura por checkboxes marcados nos Clientes
                foreach (DataGridViewRow row in dgvConfigClientes.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["colunaCheckbox"].Value))
                    {
                        algumCheckboxMarcado = true;
                        break; // Achou um, não precisa procurar mais
                    }
                }
                // Se ainda não achou, procura nos Servidores
                if (!algumCheckboxMarcado)
                {
                    foreach (DataGridViewRow row in dgvConfigServidores.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["colunaCheckbox"].Value))
                        {
                            algumCheckboxMarcado = true;
                            break;
                        }
                    }
                }
            }
            // O botão Remover só é habilitado se estivermos em modo de edição E algum item estiver selecionado
            btnRemoverGlobal.Enabled = emModoEdicaoGlobal && algumCheckboxMarcado;

            // Força a saída do modo de edição se as listas ficarem vazias
            if (!temItens && emModoEdicaoGlobal)
            {
                btnEditarGlobal_Click(null, null);
            }
        }

        private void btnRemoverCliente_Click_1(object sender, EventArgs e)
        {
            // Lista para guardar as linhas que serão removidas
            List<DataGridViewRow> linhasParaRemover = new List<DataGridViewRow>();

            // Percorre todas as linhas da tabela
            foreach (DataGridViewRow row in dgvConfigClientes.Rows)
            {
                // Verifica se a célula do checkbox não é nula e se está marcada
                if (row.Cells["colunaCheckbox"] != null && Convert.ToBoolean(row.Cells["colunaCheckbox"].Value))
                {
                    linhasParaRemover.Add(row);
                }
            }

            // Se o usuário selecionou alguma linha para remover
            if (linhasParaRemover.Count > 0)
            {
                // Pede uma última confirmação
                if (MessageBox.Show($"Você tem certeza que deseja remover {linhasParaRemover.Count} cliente(s)?",
                                      "Confirmar Remoção",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // Remove as linhas selecionadas da tabela
                    foreach (DataGridViewRow row in linhasParaRemover)
                    {
                        dgvConfigClientes.Rows.Remove(row);
                    }
                    configuracoesForamAlteradas = true;
                    RegistrarLogCopiarDados($"{linhasParaRemover.Count} cliente(s) removidos da lista de configuração.");
                }
            }
            else
            {
                MessageBox.Show("Nenhum item foi selecionado para remoção.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Após remover, atualiza o estado dos botões
            AtualizarEstadoBotoesConfig();

        }

        private void btnAdicionarServico_Click(object sender, EventArgs e)
        {
            string nomeDoServico = Interaction.InputBox("Digite o nome exato do Serviço do Windows:", "Adicionar Serviço", "");

            if (!string.IsNullOrWhiteSpace(nomeDoServico))
            {
                // --- VALIDAÇÃO DE DUPLICATAS PARA SERVIDORES ---
                bool jaExiste = false;
                foreach (DataGridViewRow row in dgvConfigServidores.Rows)
                {
                    if (row.Cells["Nome"].Value != null &&
                        string.Equals(row.Cells["Nome"].Value.ToString(), nomeDoServico, StringComparison.OrdinalIgnoreCase))
                    {
                        jaExiste = true;
                        break;
                    }
                }

                if (jaExiste)
                {
                    MessageBox.Show($"O item '{nomeDoServico}' já existe na lista de servidores.",
                                    "Item Duplicado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
                // --- FIM DA VALIDAÇÃO ---

                dgvConfigServidores.Rows.Add(nomeDoServico, "Servico");

                RegistrarLogCopiarDados($"Serviço '{nomeDoServico}' adicionado à lista de configuração.");

                configuracoesForamAlteradas = true;

                AtualizarEstadoBotoesConfig();
            }
        }

        private void btnRemoverGlobal_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> linhasParaRemover = new List<DataGridViewRow>();

            // Coleta itens marcados dos Clientes
            foreach (DataGridViewRow row in dgvConfigClientes.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colunaCheckbox"].Value))
                {
                    linhasParaRemover.Add(row);
                }
            }
            // Coleta itens marcados dos Servidores
            foreach (DataGridViewRow row in dgvConfigServidores.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colunaCheckbox"].Value))
                {
                    linhasParaRemover.Add(row);
                }
            }

            if (linhasParaRemover.Count > 0)
            {
                if (MessageBox.Show($"Você tem certeza que deseja remover {linhasParaRemover.Count} item(ns) selecionado(s)?",
                                      "Confirmar Remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (var row in linhasParaRemover)
                    {
                        // Remove a linha da sua respectiva tabela
                        (row.DataGridView).Rows.Remove(row);
                    }
                    RegistrarLogCopiarDados($"{linhasParaRemover.Count} item(ns) removido(s) da lista de configuração.");
                }
            }
            else
            {
                MessageBox.Show("Nenhum item foi selecionado para remoção.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            AtualizarEstadoBotoesConfig();
        }

        private void btnEditarGlobal_Click(object sender, EventArgs e)
        {
            emModoEdicaoGlobal = !emModoEdicaoGlobal;

            if (emModoEdicaoGlobal)
            {
                // --- ENTRANDO NO MODO DE EDIÇÃO GLOBAL ---
                // Adiciona checkbox na tabela de Clientes
                DataGridViewCheckBoxColumn chkClientes = new DataGridViewCheckBoxColumn { Name = "colunaCheckbox", HeaderText = "Remover?", Width = 80 };
                dgvConfigClientes.Columns.Insert(0, chkClientes);

                // Adiciona checkbox na tabela de Servidores
                DataGridViewCheckBoxColumn chkServidores = new DataGridViewCheckBoxColumn { Name = "colunaCheckbox", HeaderText = "Remover?", Width = 80 };
                dgvConfigServidores.Columns.Insert(0, chkServidores);

                btnEditarGlobal.Text = "Concluir Edição";
                btnRemoverGlobal.Enabled = true;
            }
            else
            {
                // --- SAINDO DO MODO DE EDIÇÃO GLOBAL ---
                // Remove a coluna de ambas as tabelas (se existirem)
                if (dgvConfigClientes.Columns.Contains("colunaCheckbox"))
                    dgvConfigClientes.Columns.Remove("colunaCheckbox");

                if (dgvConfigServidores.Columns.Contains("colunaCheckbox"))
                    dgvConfigServidores.Columns.Remove("colunaCheckbox");

                btnEditarGlobal.Text = "Editar Itens...";
                btnRemoverGlobal.Enabled = false;
            }
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            // Apenas recarrega os dados do arquivo .ini, descartando quaisquer alterações
            CarregarDadosDeConfiguracao();
            RegistrarLogCopiarDados("Alterações nas configurações foram canceladas.");

        }

        private void dgvConfigClientes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Se a coluna alterada for a de checkbox, atualize os botões
            if (e.ColumnIndex == 0 && emModoEdicaoGlobal)
            {
                AtualizarEstadoBotoesConfig();
            }
        }

        private void tabCopiarExes_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            // Esta lógica só roda se o usuário ESTAVA na aba de configurações E existem alterações pendentes
            if (e.TabPage == tabConfiguracoes && configuracoesForamAlteradas)
            {
                DialogResult resultado;
                using (frmConfirmarSalvar formConfirmacao = new frmConfirmarSalvar())
                {
                    resultado = formConfirmacao.ShowDialog();
                }

                // --- A PARTE QUE FALTAVA VEM AQUI ---
                // Avalia a escolha do usuário
                switch (resultado)
                {
                    // Caso 1: Usuário escolheu "Salvar e Sair"
                    case DialogResult.Yes:
                        btnSalvarConfiguracoes_Click(null, null); // Salva as alterações
                                                                  // Deixa a troca de aba continuar
                        break;

                    // Caso 2: Usuário escolheu "Sair sem Salvar"
                    case DialogResult.No:
                        CarregarDadosDeConfiguracao(); // Descarta as alterações
                                                       // Deixa a troca de aba continuar
                        break;

                    // Caso 3: Usuário escolheu "Permanecer na Aba"
                    case DialogResult.Cancel:
                        e.Cancel = true; // IMPEDE a troca de aba
                        break;
                }
            }
        }
    }
}
