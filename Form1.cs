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
using static System.Windows.Forms.DataFormats;

namespace ExeBoard {

    public partial class frmCopiarExes : Form
    {
        private void RepopularLogs(Color? filtroCor = null)
        {
            rtbLog.Clear(); // Limpa a tela

            List<LogEntry> logsParaExibir;

            // Trava a lista mestra para fazer uma cópia segura
            lock (listaDeLogs)
            {
                if (filtroCor == null)
                {
                    // Pega todos os logs
                    logsParaExibir = new List<LogEntry>(listaDeLogs);
                }
                else
                {
                    // Pega apenas os logs que batem com a cor do filtro
                    logsParaExibir = listaDeLogs.Where(log => log.Cor == filtroCor).ToList();
                }
            }

            // Agora desenha os logs filtrados na tela
            foreach (var logEntry in logsParaExibir)
            {
                AnexarLogAoRtb(logEntry.Mensagem, logEntry.Cor);
            }
        }

        // Este método auxiliar APENAS desenha na tela
        private void AnexarLogAoRtb(string mensagem, Color cor)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.BeginInvoke(new Action<string, Color>(AnexarLogAoRtb), mensagem, cor);
                return;
            }

            string log = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {mensagem}{Environment.NewLine}";

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = cor;
            rtbLog.AppendText(log);
            rtbLog.SelectionColor = rtbLog.ForeColor;
            rtbLog.ScrollToCaret();
        }

        // Lista mestra que armazena TODOS os logs
        private List<LogEntry> listaDeLogs = new List<LogEntry>();

        private bool configuracoesForamAlteradas = false;

        // Importa a função do Windows para LER arquivos INI
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        // Importa a função do Windows para ESCREVER arquivos INI
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);


        string caminhoIni = Application.StartupPath + @"\Inicializar.ini";

        // Estrutura para armazenar cada entrada de log com sua cor
        private class LogEntry
        {
            public string Mensagem { get; set; }
            public Color Cor { get; set; }
        }

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
                        "PARA=C:\\\n" +
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
            // --- LIMPA E CONFIGURA AS NOVAS LISTAS ---
            clbClientes.Items.Clear();
            clbServidores.Items.Clear();
            clbAtualizadores.Items.Clear(); // Limpa a nova lista

            // Diz às listas para usarem a propriedade "Nome" dos objetos para exibição
            clbClientes.DisplayMember = "Nome";
            clbServidores.DisplayMember = "Nome";

            RegistrarLogCopiarDados("Carregando dados na aba de configurações...");

            // --- LÊ O ARQUIVO .INI E POPULA AS LISTAS ---

            // Carregar Clientes (código existente)
            string countClientesStr = LerValorIni("APLICACOES_CLIENTE", "Count", caminhoIni);
            if (int.TryParse(countClientesStr, out int countClientes))
            {
                for (int i = 0; i < countClientes; i++)
                {
                    string clienteNome = LerValorIni("APLICACOES_CLIENTE", $"Cliente{i}", caminhoIni);
                    string categoria = LerValorIni("APLICACOES_CLIENTE", $"Categoria{i}", caminhoIni);
                    if (!string.IsNullOrWhiteSpace(clienteNome))
                    {
                        clbClientes.Items.Add(new ClienteItem { Nome = clienteNome, Categoria = categoria });
                    }
                }
            }

            // Carregar Servidores (código existente)
            string countServidoresStr = LerValorIni("APLICACOES_SERVIDORAS", "Count", caminhoIni);
            if (int.TryParse(countServidoresStr, out int countServidores))
            {
                for (int i = 0; i < countServidores; i++)
                {
                    string servidorNome = LerValorIni("APLICACOES_SERVIDORAS", $"Servidor{i}", caminhoIni);
                    string tipo = LerValorIni("APLICACOES_SERVIDORAS", $"Tipo{i}", caminhoIni);
                    // CORREÇÃO: Carrega também a flag de replicação
                    string replicarStr = LerValorIni("APLICACOES_SERVIDORAS", $"Replicar{i}", caminhoIni);
                    bool replicar = string.Equals(replicarStr, "Sim", StringComparison.OrdinalIgnoreCase);

                    if (!string.IsNullOrWhiteSpace(servidorNome))
                    {
                        clbServidores.Items.Add(new ServidorItem { Nome = servidorNome, Tipo = tipo, ReplicarParaCopia = replicar });
                    }
                }
            }

            // --- INÍCIO DA NOVA LÓGICA ---
            // Carregar Atualizadores/Bancos
            string countBancosStr = LerValorIni("BANCO_DE_DADOS", "Count", caminhoIni);
            if (int.TryParse(countBancosStr, out int countBancos))
            {
                for (int i = 0; i < countBancos; i++)
                {
                    // Lê no novo formato "BancoX"
                    string bancoNome = LerValorIni("BANCO_DE_DADOS", $"Banco{i}", caminhoIni);
                    if (string.IsNullOrWhiteSpace(bancoNome))
                    {
                        // Tenta ler no formato antigo "BancoDadosX" por compatibilidade
                        bancoNome = LerValorIni("BANCO_DE_DADOS", $"BancoDados{i}", caminhoIni);
                    }

                    if (!string.IsNullOrWhiteSpace(bancoNome))
                    {
                        clbAtualizadores.Items.Add(bancoNome); // Adiciona como string
                    }
                }
            }
            // --- FIM DA NOVA LÓGICA ---

            configuracoesForamAlteradas = false;
            RegistrarLogCopiarDados("Dados de configuração carregados.");
            AtualizarEstadoBotoesConfig();
        }
        private void btnBuscarCaminhoBranch_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório da branch (Origem)";
                dialogo.ShowNewFolderButton = true;

                // --- CORREÇÃO: LER DO LOCAL CORRETO ---
                // Lê o caminho da seção [CAMINHOS], chave "DE", que é onde o app carrega
                string ultimoCaminho = LerValorIni("CAMINHOS", "DE", caminhoIni);

                if (!string.IsNullOrEmpty(ultimoCaminho) && Directory.Exists(ultimoCaminho))
                {
                    dialogo.SelectedPath = ultimoCaminho;
                }
                else if (Directory.Exists(edtCaminhoBranch.Text)) // Usa o texto atual se for válido
                {
                    dialogo.SelectedPath = edtCaminhoBranch.Text;
                }

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    edtCaminhoBranch.Text = dialogo.SelectedPath;

                    // --- CORREÇÃO: SALVAR NO LOCAL CORRETO ---
                    // Salva o novo caminho de volta na seção [CAMINHOS], chave "DE"
                    WritePrivateProfileString("CAMINHOS", "DE", dialogo.SelectedPath, caminhoIni);

                    // (Opcional) Remove a chave antiga para limpeza
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoBranch", null, caminhoIni);
                }
            }
        }
        private void frmCopiarExes_Load(object sender, EventArgs e)
        {
            // SEU CÓDIGO ORIGINAL (MANTIDO)
            this.Location = new Point(this.Location.X, 0);
            RegistrarLogCopiarDados("CopiarExes aberto");
            preencheAutomaticamenteOCampoDe();
            RegistrarLogCopiarDados("Preencheu o campo DE.");

            // --- INÍCIO DA CORREÇÃO 1 (Carregar caminhos de destino) ---
            // Removemos o log "Preencheu o campo PARA" e o substituímos por esta lógica

            string placeholder = "Informe o caminho da pasta aqui";

            // Carrega o caminho para Atualizadores (Dados)
            string pastaDados = LerValorIni("CAMINHOS", "PASTA_DADOS", caminhoIni);
            if (!string.IsNullOrWhiteSpace(pastaDados) && Directory.Exists(pastaDados))
            {
                txtDestinoAtualizadores.Text = pastaDados;
                txtDestinoAtualizadores.ForeColor = SystemColors.WindowText;
            }
            else
            {
                txtDestinoAtualizadores.Text = placeholder;
                txtDestinoAtualizadores.ForeColor = SystemColors.GrayText;
            }

            // Carrega o caminho para Clientes
            string pastaCliente = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni);
            if (!string.IsNullOrWhiteSpace(pastaCliente) && Directory.Exists(pastaCliente))
            {
                txtDestinoClientes.Text = pastaCliente;
                txtDestinoClientes.ForeColor = SystemColors.WindowText;
            }
            else
            {
                txtDestinoClientes.Text = placeholder;
                txtDestinoClientes.ForeColor = SystemColors.GrayText;
            }

            // Carrega o caminho para Servidores
            string pastaServidor = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);
            if (!string.IsNullOrWhiteSpace(pastaServidor) && Directory.Exists(pastaServidor))
            {
                txtDestinoServidores.Text = pastaServidor;
                txtDestinoServidores.ForeColor = SystemColors.WindowText;
            }
            else
            {
                txtDestinoServidores.Text = placeholder;
                txtDestinoServidores.ForeColor = SystemColors.GrayText;
            }

            RegistrarLogCopiarDados("Carregou os caminhos de destino (PARA).");
            // --- FIM DA CORREÇÃO 1 ---

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
                icBandeja.Icon = this.Icon;
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

                        if (!categorias.TryGetValue(categoria, out categoriaMenu))
                        {
                            categoriaMenu = new ToolStripMenuItem(categoria);
                            clientesItem.DropDownItems.Add(categoriaMenu);
                            categorias[categoria] = categoriaMenu;
                        }

                        categoriaMenu.DropDownItems.Add(clienteSemExtensaoExe, null, (s, e) =>
                        {
                            RegistrarLogCopiarDados($"Abrindo sistema {cliente.Nome} da categoria {categoria}");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = cliente.CaminhoCompletoCliente,
                                UseShellExecute = true
                            });
                        });
                    }
                }
                // ... (o resto do seu código para a bandeja continua aqui, está correto)
            }

            // =============================================================
            // --- (CÓDIGO DE CONFIGURAÇÃO INICIAL - MANTIDO) ---
            // =============================================================
            if (cbGroupClientes.Items.Count == 0 && cbGroupServidores.Items.Count == 0)
            {
                DialogResult resultado = MessageBox.Show(
                    "Nenhuma aplicação cliente ou servidora foi encontrada na sua configuração.\n\n" +
                    "Deseja ir para a aba de 'Configurações' para adicioná-las agora?",
                    "Configuração Inicial",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (resultado == DialogResult.Yes)
                {
                    tabCopiarExes.SelectedTab = tabConfiguracoes;
                }
            }

            // --- (CÓDIGO DA LUPA - MANTIDO) ---
            try
            {
                string caminhoIcone = Path.Combine(Application.StartupPath, "search.png");

                if (File.Exists(caminhoIcone))
                {
                    Image iconeLupa = Image.FromFile(caminhoIcone);

                    btnProcurarAtualizadores.Image = iconeLupa;
                    btnProcurarClientes.Image = iconeLupa;
                    btnProcurarServidores.Image = iconeLupa;
                }
                else
                {
                    RegistrarLogCopiarDados("Aviso: 'search.png' não foi encontrado na pasta do .exe.");
                }
            }
            catch (Exception ex)
            {
                RegistrarLogCopiarDados($"ERRO ao carregar o ícone 'search.png': {ex.Message}");
            }
        }

        private void ReiniciarServidor(ServidorItem servidor, bool acionadoNaBandeja) // Alterado para receber ServidorItem
        {
            if (servidor == null) return; // Segurança extra

            // Pega o caminho raiz de destino dos servidores do novo TextBox
            string caminhoServidorDestino = txtDestinoServidores.Text;
            // Pega o nome da subpasta do .ini
            string pastaServerIni = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);

            if (servidor.Tipo == "Servico")
            {
                try
                {
                    ServiceController sc = new ServiceController(servidor.Nome);
                    if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        RegistrarLogCopiarDados("Parou o serviço " + servidor.Nome);
                    }
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    RegistrarLogCopiarDados("Reiniciou o serviço " + servidor.Nome);
                    if (acionadoNaBandeja) MessageBox.Show($"Serviço {servidor.Nome} reiniciado.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao reiniciar serviço {servidor.Nome}: {ex.Message}");
                    if (acionadoNaBandeja) MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (servidor.Tipo == "Aplicacao")
            {
                try
                {
                    // Monta o caminho completo do executável
                    string caminhoCompletoExeDestino = Path.Combine(caminhoServidorDestino, pastaServerIni, servidor.SubDiretorios ?? "", servidor.Nome);
                    string nomeProcesso = Path.GetFileNameWithoutExtension(servidor.Nome);

                    foreach (var processo in Process.GetProcessesByName(nomeProcesso))
                    {
                        processo.Kill();
                        processo.WaitForExit();
                        RegistrarLogCopiarDados("Parou a aplicação: " + servidor.Nome);
                    }
                    Process.Start(new ProcessStartInfo { FileName = caminhoCompletoExeDestino, UseShellExecute = true });
                    RegistrarLogCopiarDados("Reiniciou a aplicação: " + servidor.Nome);
                    if (acionadoNaBandeja) MessageBox.Show($"Aplicação {servidor.Nome} reiniciada.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao reiniciar aplicação {servidor.Nome}: {ex.Message}");
                    if (acionadoNaBandeja) MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Nova assinatura: adiciona 'validarParaCopia'
        private void PararServidor(ServidorItem servidor, bool acionadoNaBandeja, bool validarParaCopia = false)
        {
            if (servidor == null) return;

            // --- ETAPA 1: ENVIAR COMANDO DE PARADA ---
            if (servidor.Tipo == "Servico")
            {
                try
                {
                    ServiceController sc = new ServiceController(servidor.Nome);
                    if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        RegistrarLogCopiarDados("Parou o serviço " + servidor.Nome);
                    }
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao parar serviço {servidor.Nome}: {ex.Message}");
                }
            }
            else if (servidor.Tipo == "Aplicacao")
            {
                try
                {
                    string nomeExe = servidor.Nome;
                    Process p = new Process();
                    p.StartInfo.FileName = "taskkill";
                    p.StartInfo.Arguments = $"/f /im \"{nomeExe}\" /t";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    p.WaitForExit(5000);
                    string output = p.StandardOutput.ReadToEnd();

                    if (output.StartsWith("SUCCESS") || output.Contains("process with PID"))
                        RegistrarLogCopiarDados("Comando para parar aplicação enviado: " + servidor.Nome);
                    else if (output.Contains("process not found"))
                        RegistrarLogCopiarDados("Aplicação já estava parada: " + servidor.Nome);
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao parar aplicação {servidor.Nome}: {ex.Message}");
                }
            }

            // --- ETAPA 2: VALIDAÇÃO (SE SOLICITADO) ---
            if (validarParaCopia)
            {
                RegistrarLogCopiarDados($"Validando encerramento de {servidor.Nome}...");
                bool parado = false;
                for (int i = 0; i < 10; i++) // Tenta por 10 segundos
                {
                    string status = ObterStatusServidor(servidor);
                    if (status == "Parado")
                    {
                        parado = true;
                        break;
                    }
                    Thread.Sleep(1000);
                }

                if (parado)
                {
                    RegistrarLogCopiarDados($"OK: {servidor.Nome} confirmado como 'Parado'.");
                }
                else
                {
                    RegistrarLogCopiarDados($"ERRO: {servidor.Nome} não parou a tempo.");
                    throw new Exception($"Falha ao parar {servidor.Nome}. Cópia abortada.");
                }
            }
        }
        private void IniciarServidor(ServidorItem servidor, bool acionadoNaBandeja) // Alterado para receber ServidorItem
        {
            if (servidor == null) return;

            string caminhoServidorDestino = txtDestinoServidores.Text;
            string pastaServerIni = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);

            if (servidor.Tipo == "Servico")
            {
                try
                {
                    ServiceController sc = new ServiceController(servidor.Nome);
                    if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                    {
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        RegistrarLogCopiarDados("Iniciou o serviço " + servidor.Nome);
                        if (acionadoNaBandeja) MessageBox.Show($"Serviço {servidor.Nome} iniciado.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (acionadoNaBandeja) MessageBox.Show($"Serviço {servidor.Nome} já estava iniciado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao iniciar serviço {servidor.Nome}: {ex.Message}");
                    if (acionadoNaBandeja) MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (servidor.Tipo == "Aplicacao")
            {
                try
                {
                    string nomeProcesso = Path.GetFileNameWithoutExtension(servidor.Nome);
                    if (Process.GetProcessesByName(nomeProcesso).Length == 0)
                    {
                        string caminhoCompletoExeDestino = Path.Combine(caminhoServidorDestino, pastaServerIni, servidor.SubDiretorios ?? "", servidor.Nome);
                        Process.Start(new ProcessStartInfo { FileName = caminhoCompletoExeDestino, UseShellExecute = true });
                        RegistrarLogCopiarDados("Iniciou a aplicação: " + servidor.Nome);
                        if (acionadoNaBandeja) MessageBox.Show($"Aplicação {servidor.Nome} iniciada.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (acionadoNaBandeja) MessageBox.Show($"Aplicação {servidor.Nome} já estava iniciada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao iniciar aplicação {servidor.Nome}: {ex.Message}");
                    if (acionadoNaBandeja) MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void CarregarBancoDeDados()
        {
            cbGroupAtualizadores.Items.Clear(); // Limpa a lista primeiro

            string countStr = LerValorIni("BANCO_DE_DADOS", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                // --- INÍCIO DA CORREÇÃO ---
                for (int i = 0; i < count; i++) // Loop corrigido para i < count
                {
                    // Lê no novo formato "BancoX"
                    string chave = $"Banco{i}";
                    string valor = LerValorIni("BANCO_DE_DADOS", chave, caminhoIni);

                    if (string.IsNullOrWhiteSpace(valor))
                    {
                        // Tenta ler no formato antigo "BancoDadosX" por compatibilidade
                        chave = $"BancoDados{i}";
                        valor = LerValorIni("BANCO_DE_DADOS", chave, caminhoIni);
                    }

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        cbGroupAtualizadores.Items.Add(valor);
                        cbGroupAtualizadores.SetItemChecked(cbGroupAtualizadores.Items.Count - 1, true); // Marca o item recém-adicionado
                        RegistrarLogCopiarDados("Carregou a informação do banco " + valor + ". Parâmetro: " + chave);
                    }
                }
                // --- FIM DA CORREÇÃO ---
            }
        }
        private void CarregarClientes()
        {
            cbGroupClientes.Items.Clear(); // Mantém a limpeza

            string countStr = LerValorIni("APLICACOES_CLIENTE", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                // CORREÇÃO: O loop deve ir até count-1 ou usar i < count
                for (int i = 0; i < count; i++)
                {
                    string clienteId = $"Cliente{i}";
                    string categoriaId = $"Categoria{i}";
                    string subDiretoriosId = $"SubDiretorios{i}";

                    string cliente = LerValorIni("APLICACOES_CLIENTE", clienteId, caminhoIni);
                    string categoria = LerValorIni("APLICACOES_CLIENTE", categoriaId, caminhoIni);
                    string subDiretorio = LerValorIni("APLICACOES_CLIENTE", subDiretoriosId, caminhoIni);

                    if (!string.IsNullOrWhiteSpace(cliente))
                    {
                        // REMOVIDO: A linha que chamava getCaminhoCompletoAplicacao
                        // A propriedade CaminhoCompletoCliente será montada dinamicamente onde for necessária

                        cbGroupClientes.Items.Add(new ClienteItem
                        {
                            Nome = cliente,
                            Categoria = categoria,
                            SubDiretorios = subDiretorio
                            // CaminhoCompletoCliente = caminhoCompletoCliente // Removido
                        });
                        // CORREÇÃO: Usar Count-1 para marcar o último item adicionado
                        cbGroupClientes.SetItemChecked(cbGroupClientes.Items.Count - 1, true);
                        RegistrarLogCopiarDados("Carregou a informação da aplicação cliente " + cliente + ". Parâmetro: " + clienteId);
                    }
                }
            }
        }

        private void CarregarServidores()
        {
            cbGroupServidores.Items.Clear();

            string countStr = LerValorIni("APLICACOES_SERVIDORAS", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i < count; i++) // Loop correto
                {
                    string servidor = LerValorIni("APLICACOES_SERVIDORAS", $"Servidor{i}", caminhoIni);
                    string tipo = LerValorIni("APLICACOES_SERVIDORAS", $"Tipo{i}", caminhoIni);
                    string subDir = LerValorIni("APLICACOES_SERVIDORAS", $"SubDiretorios{i}", caminhoIni);
                    string replicar = LerValorIni("APLICACOES_SERVIDORAS", $"Replicar{i}", caminhoIni);

                    if (!string.IsNullOrWhiteSpace(servidor) && !string.IsNullOrWhiteSpace(tipo))
                    {
                        if (string.Equals(replicar, "Sim", StringComparison.OrdinalIgnoreCase))
                        {
                            // REMOVIDO: A linha que chamava getCaminhoCompletoAplicacao
                            // A propriedade CaminhoCompletoAplicacao será montada dinamicamente

                            cbGroupServidores.Items.Add(new ServidorItem
                            {
                                Nome = servidor,
                                Tipo = tipo,
                                ReplicarParaCopia = true,
                                // CaminhoCompletoAplicacao = caminhoCompletoAplicacao, // Removido
                                SubDiretorios = subDir
                            });
                            cbGroupServidores.SetItemChecked(cbGroupServidores.Items.Count - 1, true); // Marca o último adicionado
                        }
                    }
                }
                RegistrarLogCopiarDados("Carregou informações de servidores para a aba 'Copiar Dados'.");
            }
        }

        private string LerValorIni(string secao, string chave, string caminhoArquivo)
        {
            StringBuilder buffer = new StringBuilder(255);
            GetPrivateProfileString(secao, chave, "", buffer, buffer.Capacity, caminhoArquivo);
            return buffer.ToString();
        }

        // 1. (Substituir) RegistrarLogCopiarDados
        // Versão principal que aceita cor
        private void RegistrarLogCopiarDados(string mensagem, Color cor)
        {
            // 1. Cria a entrada do log
            var logEntry = new LogEntry { Mensagem = mensagem, Cor = cor };

            // 2. Salva na lista mestra (de forma segura)
            lock (listaDeLogs)
            {
                listaDeLogs.Add(logEntry);
            }

            // 3. Desenha na tela (usando o novo auxiliar)
            AnexarLogAoRtb(mensagem, cor);
        }
        // Sobrecarga (Opcional, mas recomendado):
        // Mantém o método antigo para que o resto do seu código continue funcionando
        private void RegistrarLogCopiarDados(string mensagem)
        {
            // Chama a versão principal com a cor padrão (preto)
            RegistrarLogCopiarDados(mensagem, Color.Black);
        }


        // 2. (Substituir) RegistrarLogServidores
        // Versão principal que aceita cor
        private void RegistrarLogServidores(string mensagem, Color cor)
        {
            if (rtbLogServidores.InvokeRequired)
            {
                rtbLogServidores.BeginInvoke(new Action<string, Color>(RegistrarLogServidores), mensagem, cor);
                return;
            }

            string log = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {mensagem}{Environment.NewLine}";

            rtbLogServidores.SelectionStart = rtbLogServidores.TextLength;
            rtbLogServidores.SelectionLength = 0;
            rtbLogServidores.SelectionColor = cor;
            rtbLogServidores.AppendText(log);
            rtbLogServidores.SelectionColor = rtbLogServidores.ForeColor;
            rtbLogServidores.ScrollToCaret();
        }

        // Sobrecarga para manter o código antigo funcionando
        private void RegistrarLogServidores(string mensagem)
        {
            RegistrarLogServidores(mensagem, Color.Black);
        }

        private async void btnCopiarDados_Click(object sender, EventArgs e)
        {
            btnCopiarDados.Enabled = false;
            // --- CORREÇÃO: LOG AZUL (INFORMAÇÃO) ---
            RegistrarLogCopiarDados("Iniciando processo de cópia...", Color.Blue);

            // ... (Seu código de coleta de listas está correto) ...
            string caminhoClienteDestino = txtDestinoClientes.Text;
            string caminhoServidorDestino = txtDestinoServidores.Text;
            string caminhoBranch = edtCaminhoBranch.Text;
            string caminhoAtualizadores = txtDestinoAtualizadores.Text;
            var clientesParaCopiar = cbGroupClientes.CheckedItems.OfType<ClienteItem>().ToList();
            var servidoresParaCopiar = cbGroupServidores.CheckedItems.OfType<ServidorItem>().ToList();
            var atualizadoresParaCopiar = cbGroupAtualizadores.CheckedItems.Cast<object>().ToList();

            try
            {
                bool sucesso = await Task.Run(() =>
                {
                    RegistrarLogCopiarDados("Parando aplicações clientes...");
                    if (!encerrarClientes(caminhoClienteDestino, clientesParaCopiar))
                    {
                        RegistrarLogCopiarDados("ERRO: Falha ao encerrar clientes. Abortando.", Color.Red);
                        return false;
                    }

                    RegistrarLogCopiarDados("Parando aplicações servidoras...");
                    if (!encerrarServidores(servidoresParaCopiar))
                    {
                        RegistrarLogCopiarDados("ERRO: Falha ao encerrar servidores. Abortando.", Color.Red);
                        return false;
                    }

                    RegistrarLogCopiarDados("Copiando executáveis via C#...");
                    copiarArquivos(caminhoBranch, caminhoClienteDestino, caminhoServidorDestino, caminhoAtualizadores,
                                   clientesParaCopiar, servidoresParaCopiar, atualizadoresParaCopiar);

                    RegistrarLogCopiarDados("Iniciando servidores...");
                    iniciarServidores(caminhoServidorDestino, servidoresParaCopiar);

                    return true;
                });

                if (sucesso)
                    // --- CORREÇÃO: LOG AZUL (INFORMAÇÃO) ---
                    RegistrarLogCopiarDados("Processo de cópia concluído!", Color.Blue);
                else
                    // --- CORREÇÃO: LOG VERMELHO ---
                    RegistrarLogCopiarDados("Processo de cópia falhou. Verifique os logs.", Color.Red);
            }
            catch (Exception ex)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO FATAL no processo de cópia: {ex.Message}", Color.Red);
            }
            finally
            {
                btnCopiarDados.Enabled = true;
            }
        }
        private bool encerrarServidores(List<ServidorItem> servidoresParaParar)
        {
            try
            {
                foreach (var servidor in servidoresParaParar)
                {
                    // O 'true' no final força a VALIDAÇÃO que você pediu
                    PararServidor(servidor, false, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                RegistrarLogCopiarDados($"FALHA: {ex.Message}");
                return false;
            }
        }                 // Assinatura modificada para receber a lista
        private bool encerrarClientes(string caminhoClienteDestino, List<ClienteItem> clientesParaParar)
        {
            // string pastaClientIni = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni); // <-- REMOVIDO

            // ... (Lógica de "Matar Processos" permanece igual) ...
            foreach (var cliente in clientesParaParar)
            {
                // ... (código do taskkill) ...
            }

            // ... (Lógica de "Validação" permanece igual) ...

            // --- ETAPA 3: DELETAR ARQUIVOS ANTIGOS (COM O CAMINHO CORRIGIDO) ---
            RegistrarLogCopiarDados("OK: Clientes encerrados. Excluindo arquivos antigos...");
            foreach (var cliente in clientesParaParar)
            {
                try
                {
                    // --- CORREÇÃO: "pastaClientIni" foi removido do Path.Combine ---
                    string caminhoCompletoExeDestino = Path.Combine(caminhoClienteDestino, cliente.SubDiretorios ?? "", cliente.Nome);
                    if (File.Exists(caminhoCompletoExeDestino))
                    {
                        File.Delete(caminhoCompletoExeDestino);
                        RegistrarLogCopiarDados($"OK: Arquivo antigo {cliente.Nome} excluído.");
                    }
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"ERRO ao excluir {cliente.Nome}: {ex.Message}");
                }
            }
            return true;
        }
        private void copiarArquivos(string de, string paraClientes, string paraServidores, string paraAtualizadores,
                                    List<ClienteItem> clientesParaCopiar, List<ServidorItem> servidoresParaCopiar, List<object> atualizadoresParaCopiar)
        {
            string dePastaClient = LerValorIni("CAMINHOS", "DE_PASTA_CLIENT", caminhoIni);
            string dePastaServer = LerValorIni("CAMINHOS", "DE_PASTA_SERVER", caminhoIni);
            string dePastaDados = LerValorIni("CAMINHOS", "DE_PASTA_DADOS", caminhoIni);
            // --- LINHAS REMOVIDAS ---
            // string pastaClient = LerValorIni("CAMINHOS", "PASTA_CLIENT", caminhoIni);
            // string pastaServer = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni);
            // string pastaDados = LerValorIni("CAMINHOS", "PASTA_DADOS", caminhoIni);
            // --- FIM DA REMOÇÃO ---

            RegistrarLogCopiarDados("Iniciando cópia de arquivos via C#...");

            // --- COPIANDO ARQUIVOS DE CLIENTES ---
            foreach (var cliente in clientesParaCopiar)
            {
                string sourcePath = Path.Combine(de, dePastaClient, cliente.SubDiretorios ?? "", cliente.Nome);
                // --- CORREÇÃO: "pastaClient" foi removido do Path.Combine ---
                string destinationPath = Path.Combine(paraClientes, cliente.SubDiretorios ?? "", cliente.Nome);
                string destinationDir = Path.GetDirectoryName(destinationPath);
                CopiarArquivoComLog(sourcePath, destinationPath, destinationDir, cliente.Nome);
            }

            // --- COPIANDO ARQUIVOS DE SERVIDORES ---
            foreach (var servidor in servidoresParaCopiar)
            {
                if (servidor.ReplicarParaCopia)
                {
                    string nomeArquivoParaCopiar = "";
                    if (servidor.Tipo == "Aplicacao") { nomeArquivoParaCopiar = servidor.Nome; }
                    else if (servidor.Tipo == "Servico") { nomeArquivoParaCopiar = servidor.Nome + ".exe"; }

                    if (!string.IsNullOrEmpty(nomeArquivoParaCopiar))
                    {
                        string sourcePath = Path.Combine(de, dePastaServer, servidor.SubDiretorios ?? "", nomeArquivoParaCopiar);
                        // --- CORREÇÃO: "pastaServer" foi removido do Path.Combine ---
                        string destinationPath = Path.Combine(paraServidores, servidor.SubDiretorios ?? "", nomeArquivoParaCopiar);
                        string destinationDir = Path.GetDirectoryName(destinationPath);
                        CopiarArquivoComLog(sourcePath, destinationPath, destinationDir, nomeArquivoParaCopiar);
                    }
                }
            }

            // --- COPIANDO ATUALIZADORES (Pastas) ---
            foreach (var item in atualizadoresParaCopiar)
            {
                string nomePasta = item.ToString();
                string sourceDir = Path.Combine(de, dePastaDados, nomePasta);
                // --- CORREÇÃO: "pastaDados" foi removido do Path.Combine ---
                string destinationDir = Path.Combine(paraAtualizadores, nomePasta);

                CopiarDiretorioComLog(sourceDir, destinationDir, nomePasta);
            }

            RegistrarLogCopiarDados("Cópia de arquivos concluída.");
        }
        private void CopiarArquivoComLog(string sourcePath, string destinationPath, string destinationDir, string nomeArquivo)
        {
            try
            {
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                    RegistrarLogCopiarDados($"Criado diretório: {destinationDir}", Color.Gray);
                }
                File.Copy(sourcePath, destinationPath, true);

                // --- CORREÇÃO: LOG VERDE ---
                RegistrarLogCopiarDados($"OK: {nomeArquivo} copiado para {Path.GetFileName(destinationDir)}", Color.DarkGreen);
            }
            catch (FileNotFoundException)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO: Arquivo de origem não encontrado: {sourcePath}", Color.Red);
            }
            catch (DirectoryNotFoundException)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO: Diretório de origem não encontrado para {nomeArquivo}: {Path.GetDirectoryName(sourcePath)}", Color.Red);
            }
            catch (UnauthorizedAccessException)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO: Sem permissão para acessar/copiar para {destinationPath}", Color.Red);
            }
            catch (Exception ex)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO ao copiar {nomeArquivo}: {ex.Message}", Color.Red);
            }
        }
        // Nova assinatura: recebe caminho como parâmetro
        // Assinatura modificada para receber caminho e lista
        private void iniciarServidores(string caminhoServidorDestino, List<ServidorItem> servidoresParaIniciar)
        {
            // string pastaServerIni = LerValorIni("CAMINHOS", "PASTA_SERVER", caminhoIni); // <-- REMOVIDO

            foreach (var servidor in servidoresParaIniciar)
            {
                try
                {
                    if (servidor.Tipo == "Servico")
                    {
                        // ... (Lógica de iniciar serviço permanece igual) ...
                    }
                    else if (servidor.Tipo == "Aplicacao")
                    {
                        // --- CORREÇÃO: "pastaServerIni" foi removido do Path.Combine ---
                        string caminhoCompletoExeDestino = Path.Combine(caminhoServidorDestino, servidor.SubDiretorios ?? "", servidor.Nome);

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = caminhoCompletoExeDestino,
                            UseShellExecute = true
                        });
                        RegistrarLogCopiarDados("Iniciou a aplicacao " + servidor.Nome);
                    }
                }
                catch (Exception ex)
                {
                    RegistrarLogCopiarDados($"Erro ao iniciar {servidor.Tipo.ToLower()} {servidor.Nome}: {ex.Message}");
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
            rtbLog.Clear(); // Limpa a tela
            lock (listaDeLogs)
            {
                listaDeLogs.Clear(); // Limpa a memória
            }
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

            // --- INÍCIO DA CORREÇÃO ---
            // Agora lemos DIRETAMENTE do .INI, não da lista cbGroupServidores.
            // Isso garante que TODOS os servidores (Exe e Serviço) apareçam aqui,
            // independentemente da flag "Replicar".
            string countStr = LerValorIni("APLICACOES_SERVIDORAS", "Count", caminhoIni);
            if (int.TryParse(countStr, out int count))
            {
                for (int i = 0; i < count; i++)
                {
                    string servidorNome = LerValorIni("APLICACOES_SERVIDORAS", $"Servidor{i}", caminhoIni);
                    string tipo = LerValorIni("APLICACOES_SERVIDORAS", $"Tipo{i}", caminhoIni);
                    string subDir = LerValorIni("APLICACOES_SERVIDORAS", $"SubDiretorios{i}", caminhoIni);
                    string replicar = LerValorIni("APLICACOES_SERVIDORAS", $"Replicar{i}", caminhoIni);

                    if (!string.IsNullOrWhiteSpace(servidorNome) && !string.IsNullOrWhiteSpace(tipo))
                    {
                        // Criamos o objeto ServidorItem com todas as informações lidas
                        ServidorItem servidor = new ServidorItem
                        {
                            Nome = servidorNome,
                            Tipo = tipo,
                            ReplicarParaCopia = string.Equals(replicar, "Sim", StringComparison.OrdinalIgnoreCase),
                            SubDiretorios = subDir
                        };

                        // O resto da lógica para criar os controles visuais continua...
                        CheckBox chk = new CheckBox();
                        chk.Text = servidor.Nome;
                        chk.Tag = servidor;
                        chk.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                        chk.Location = new Point(10, y);
                        chk.AutoSize = true;

                        Label lblStatus = new Label();
                        lblStatus.Location = new Point(200, y + 1);
                        lblStatus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                        lblStatus.AutoSize = true;

                        string status = ObterStatusServidor(servidor);

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
            }
            // --- FIM DA CORREÇÃO ---

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
                            return "Parado"; // Corrigido (removido 'return' duplicado)
                        default:
                            return sc.Status.ToString();
                    }
                }
                else if (servidor.Tipo == "Aplicacao")
                {
                    // --- INÍCIO DA CORREÇÃO 2 (Verificar processo, não iniciar) ---
                    // Removemos o Process.Start que causava o loop infinito

                    string nomeProcesso = Path.GetFileNameWithoutExtension(servidor.Nome);

                    // Procura por processos com esse nome
                    Process[] processos = Process.GetProcessesByName(nomeProcesso);

                    if (processos.Length > 0)
                    {
                        // Se encontrou (pelo menos um), está "Iniciado"
                        return "Iniciado";
                    }
                    else
                    {
                        // Se não encontrou nenhum, está "Parado"
                        return "Parado";
                    }
                    // --- FIM DA CORREÇÃO 2 ---
                }
            }
            catch (InvalidOperationException)
            {
                // Ocorre se o serviço (sc.Status) não for encontrado no Windows
                return "Não Encontrado";
            }
            catch (Exception)
            {
                // Captura outros erros (ex: acesso negado)
                return "Desconhecido";
            }

            return "Desconhecido"; // Fallback
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
                        // --- INÍCIO DA CORREÇÃO ---
                        // Chamar o método helper que já tem a lógica correta
                        IniciarServidor(servidor, false);
                        // Atualiza visualmente após a tentativa
                        string status = ObterStatusServidor(servidor);
                        AtualizarStatus(lblStatus, status);
                        // --- FIM DA CORREÇÃO ---
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
                        // --- INÍCIO DA CORREÇÃO ---
                        // Chamar o método helper que já tem a lógica correta
                        PararServidor(servidor, false);
                        // Atualiza visualmente após a tentativa
                        string status = ObterStatusServidor(servidor);
                        AtualizarStatus(lblStatus, status);
                        // --- FIM DA CORREÇÃO ---
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
                        // --- INÍCIO DA CORREÇÃO ---
                        // Chamar o método helper que já tem a lógica correta
                        ReiniciarServidor(servidor, false);
                        // Atualiza visualmente após a tentativa
                        string status = ObterStatusServidor(servidor);
                        AtualizarStatus(lblStatus, status);
                        // --- FIM DA CORREÇÃO ---
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

        private void btnRemoverGlobal_Click(object sender, EventArgs e)
        {
            // Adiciona a contagem da nova lista
            int totalSelecionado = clbClientes.CheckedItems.Count + clbServidores.CheckedItems.Count + clbAtualizadores.CheckedItems.Count;

            if (totalSelecionado > 0)
            {
                if (MessageBox.Show($"Você tem certeza que deseja remover {totalSelecionado} item(ns) selecionado(s)?", "Confirmar Remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // Adiciona o loop para remover da nova lista
                    while (clbClientes.CheckedItems.Count > 0) { clbClientes.Items.Remove(clbClientes.CheckedItems[0]); }
                    while (clbServidores.CheckedItems.Count > 0) { clbServidores.Items.Remove(clbServidores.CheckedItems[0]); }
                    while (clbAtualizadores.CheckedItems.Count > 0) { clbAtualizadores.Items.Remove(clbAtualizadores.CheckedItems[0]); }

                    RegistrarLogCopiarDados($"{totalSelecionado} item(ns) removido(s) da lista de configuração.");
                    configuracoesForamAlteradas = true;
                    AtualizarEstadoBotoesConfig();
                }
            }
        }
        private void btnSalvarConfiguracoes_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja salvar as alterações no arquivo de configuração?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            try
            {
                RegistrarLogCopiarDados("Salvando alterações no arquivo Inicializar.ini...");

                // --- SALVANDO APLICAÇÕES CLIENTE (código existente) ---
                WritePrivateProfileString("APLICACOES_CLIENTE", null, null, caminhoIni);
                int i = 0;
                foreach (ClienteItem item in clbClientes.Items)
                {
                    WritePrivateProfileString("APLICACOES_CLIENTE", $"Cliente{i}", item.Nome, caminhoIni);
                    WritePrivateProfileString("APLICACOES_CLIENTE", $"Categoria{i}", item.Categoria, caminhoIni);
                    WritePrivateProfileString("APLICACOES_CLIENTE", $"SubDiretorios{i}", item.SubDiretorios ?? "", caminhoIni);
                    i++;
                }
                WritePrivateProfileString("APLICACOES_CLIENTE", "Count", clbClientes.Items.Count.ToString(), caminhoIni);

                // --- SALVANDO APLICAÇÕES/SERVIÇOS SERVIDORES (código existente) ---
                WritePrivateProfileString("APLICACOES_SERVIDORAS", null, null, caminhoIni);
                i = 0;
                foreach (ServidorItem item in clbServidores.Items)
                {
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"Servidor{i}", item.Nome, caminhoIni);
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"Tipo{i}", item.Tipo, caminhoIni);
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"Replicar{i}", item.ReplicarParaCopia ? "Sim" : "Nao", caminhoIni);
                    WritePrivateProfileString("APLICACOES_SERVIDORAS", $"SubDiretorios{i}", item.SubDiretorios ?? "", caminhoIni);
                    i++;
                }
                WritePrivateProfileString("APLICACOES_SERVIDORAS", "Count", clbServidores.Items.Count.ToString(), caminhoIni);

                // --- INÍCIO DA NOVA LÓGICA ---
                // --- SALVANDO ATUALIZADORES/BANCOS (Novo Formato) ---
                WritePrivateProfileString("BANCO_DE_DADOS", null, null, caminhoIni);
                i = 0;
                foreach (string item in clbAtualizadores.Items)
                {
                    // Salva no novo formato "BancoX"
                    WritePrivateProfileString("BANCO_DE_DADOS", $"Banco{i}", item, caminhoIni);
                    i++;
                }
                WritePrivateProfileString("BANCO_DE_DADOS", "Count", clbAtualizadores.Items.Count.ToString(), caminhoIni);
                // --- FIM DA NOVA LÓGICA ---

                configuracoesForamAlteradas = false;
                AtualizarEstadoBotoesConfig();
                RegistrarLogCopiarDados("Alterações salvas com sucesso.");
                MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RegistrarLogCopiarDados("Atualizando listas da aba principal...");
                CarregarClientes();
                CarregarServidores();
                CarregarBancoDeDados(); // Adiciona a recarga dos bancos
                RegistrarLogCopiarDados("Listas da aba principal atualizadas com as novas configurações.");
            }
            catch (Exception ex)
            {
                RegistrarLogCopiarDados($"Erro ao salvar o arquivo de configuração: {ex.Message}");
                MessageBox.Show($"Ocorreu um erro ao salvar o arquivo de configuração:\n\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCancelarAlteracoes_Click(object sender, EventArgs e)
        {
            CarregarDadosDeConfiguracao();
            RegistrarLogCopiarDados("Alterações nas configurações foram canceladas.");
        }
        private void AtualizarEstadoBotoesConfig()
        {
            // Adiciona a verificação da nova lista
            bool temItensMarcados = clbClientes.CheckedItems.Count > 0 || clbServidores.CheckedItems.Count > 0 || clbAtualizadores.CheckedItems.Count > 0;

            btnRemoverGlobal.Enabled = temItensMarcados;
            btnSalvarConfiguracoes.Enabled = configuracoesForamAlteradas;
            btnCancelarAlteracoes.Enabled = configuracoesForamAlteradas;
        }
        private void btnAdicionarExeServidor_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Selecione a(s) Aplicação(ões) Servidora (.exe)";
                dialogo.Filter = "Aplicações (*.exe)|*.exe";
                dialogo.Multiselect = true;
                // ... (o resto da configuração do diálogo está OK) ...

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    int adicionados = 0;
                    foreach (string caminhoCompleto in dialogo.FileNames)
                    {
                        string nomeDoArquivoExe = Path.GetFileName(caminhoCompleto);

                        // --- INÍCIO DA SUA NOVA IDEIA ---
                        DialogResult resultado = MessageBox.Show(
                            $"O arquivo '{nomeDoArquivoExe}' roda como um Serviço do Windows?\n\n" +
                            "Sim = Será parado/iniciado como 'Serviço' (Ex: ViasoftServerAgroX)\n" +
                            "Não = Será parado/iniciado como 'Aplicação' (Ex: um .exe comum)",
                            "Tipo de Servidor",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        string nomeParaSalvar;
                        string tipo;

                        if (resultado == DialogResult.Yes) // Roda como Serviço
                        {
                            tipo = "Servico";
                            // Salva o nome SEM o .exe (Ex: "ViasoftServerAgroX")
                            nomeParaSalvar = Path.GetFileNameWithoutExtension(nomeDoArquivoExe);
                        }
                        else // Roda como Aplicação
                        {
                            tipo = "Aplicacao";
                            // Salva o nome COM o .exe (Ex: "MeuApp.exe")
                            nomeParaSalvar = nomeDoArquivoExe;
                        }
                        // --- FIM DA SUA NOVA IDEIA ---

                        // Verifica se o nome (de serviço ou app) já existe
                        bool jaExiste = clbServidores.Items.OfType<ServidorItem>().Any(item => string.Equals(item.Nome, nomeParaSalvar, StringComparison.OrdinalIgnoreCase));

                        if (!jaExiste)
                        {
                            // Replicar é SEMPRE true, pois o usuário clicou em "Adicionar Exe"
                            clbServidores.Items.Add(new ServidorItem { Nome = nomeParaSalvar, Tipo = tipo, ReplicarParaCopia = true });
                            adicionados++;
                        }
                        else
                        {
                            RegistrarLogCopiarDados($"Item de servidor '{nomeParaSalvar}' já existe na lista. Ignorando.");
                        }
                    }

                    if (adicionados > 0)
                    {
                        RegistrarLogCopiarDados($"{adicionados} aplicação(ões)/serviço(s) adicionado(s) à lista.");
                        configuracoesForamAlteradas = true;
                        AtualizarEstadoBotoesConfig();
                    }
                }
            }
        }
        private void btnAdicionarServico_Click(object sender, EventArgs e)
        {
            string nomeDoServico = "";
            using (frmAdicionarServico formAdicionar = new frmAdicionarServico())
            {
                if (formAdicionar.ShowDialog(this) == DialogResult.OK)
                {
                    nomeDoServico = formAdicionar.NomeDoServico;
                }
            }

            if (!string.IsNullOrWhiteSpace(nomeDoServico))
            {
                bool jaExiste = clbServidores.Items.OfType<ServidorItem>().Any(item => string.Equals(item.Nome, nomeDoServico, StringComparison.OrdinalIgnoreCase));
                if (jaExiste)
                {
                    MessageBox.Show($"O item '{nomeDoServico}' já existe na lista de servidores.", "Item Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- INÍCIO DA CORREÇÃO ---
                // Removemos a pergunta "Deseja replicar?"
                // Hardcoded para 'false' (Replicar = Nao)
                bool replicar = false;
                // --- FIM DA CORREÇÃO ---

                clbServidores.Items.Add(new ServidorItem { Nome = nomeDoServico, Tipo = "Servico", ReplicarParaCopia = replicar });
                RegistrarLogCopiarDados($"Serviço '{nomeDoServico}' adicionado à lista de configuração.");
                configuracoesForamAlteradas = true;
                AtualizarEstadoBotoesConfig();
            }
        }
        private void btnAdicionarCliente_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Selecione a(s) Aplicação(ões) Cliente";
                dialogo.Filter = "Aplicações (*.exe)|*.exe";
                dialogo.Multiselect = true;

                string ultimoDiretorio = LerValorIni("ULTIMOS_CAMINHOS", "UltimoCaminhoAdicionarExe", caminhoIni);
                if (!string.IsNullOrEmpty(ultimoDiretorio) && Directory.Exists(ultimoDiretorio))
                {
                    dialogo.InitialDirectory = ultimoDiretorio;
                }

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    string novoDiretorio = Path.GetDirectoryName(dialogo.FileName);
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoAdicionarExe", novoDiretorio, caminhoIni);
                    int adicionados = 0;
                    foreach (string caminhoCompleto in dialogo.FileNames)
                    {
                        string nomeDoArquivo = Path.GetFileName(caminhoCompleto);
                        bool jaExiste = clbClientes.Items.OfType<ClienteItem>().Any(item => string.Equals(item.Nome, nomeDoArquivo, StringComparison.OrdinalIgnoreCase));
                        if (jaExiste)
                        {
                            RegistrarLogCopiarDados($"Aplicação '{nomeDoArquivo}' já existe na lista. Ignorando.");
                            continue;
                        }
                        clbClientes.Items.Add(new ClienteItem { Nome = nomeDoArquivo, Categoria = "Categoria Padrão" });
                        adicionados++;
                    }
                    if (adicionados > 0)
                    {
                        RegistrarLogCopiarDados($"{adicionados} aplicação(ões) cliente adicionada(s) à lista de configuração.");
                        configuracoesForamAlteradas = true;
                        AtualizarEstadoBotoesConfig();
                    }
                }
            }
        }
        private void btnRemoverCliente_Click_1(object sender, EventArgs e)
        {
            // Lista para guardar as linhas que serão removidas
            List<DataGridViewRow> linhasParaRemover = new List<DataGridViewRow>();


            // Se o usuário selecionou alguma linha para remover
            if (linhasParaRemover.Count > 0)
            {
                // Pede uma última confirmação
                if (MessageBox.Show($"Você tem certeza que deseja remover {linhasParaRemover.Count} cliente(s)?",
                                      "Confirmar Remoção",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    configuracoesForamAlteradas = true;
                    RegistrarLogCopiarDados($"{linhasParaRemover.Count} cliente(s) removidos da lista de configuração.");
                }
            }
            else
            {
                MessageBox.Show("Nenhum item foi selecionado para remoção.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            // Apenas recarrega os dados do arquivo .ini, descartando quaisquer alterações
            CarregarDadosDeConfiguracao();
            RegistrarLogCopiarDados("Alterações nas configurações foram canceladas.");

        }


        private void tabCopiarExes_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabConfiguracoes && configuracoesForamAlteradas)
            {
                // VOLTAMOS A USAR O MESSAGEBOX PADRÃO
                DialogResult resultado = MessageBox.Show(
                    "Você possui alterações não salvas. Deseja salvá-las antes de sair?",
                    "Alterações Pendentes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                // O switch continua funcionando perfeitamente
                switch (resultado)
                {
                    case DialogResult.Yes: // Usuário clicou em "Sim" (Salvar)
                        btnSalvarConfiguracoes_Click(null, null);
                        break;

                    case DialogResult.No: // Usuário clicou em "Não" (Não Salvar)
                        CarregarDadosDeConfiguracao();
                        break;

                    case DialogResult.Cancel: // Usuário clicou em "Cancelar"
                        e.Cancel = true;
                        break;
                }
            }
        }
        private void clb_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // A trava foi removida. Agora ele apenas atualiza o estado dos botões após o clique.
            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate { AtualizarEstadoBotoesConfig(); });
        }

        private void tsmMarcarDesmarcarTodos_Click(object sender, EventArgs e)
        {
            // Pega o item de menu que foi clicado (o "Marcar/Desmarcar Todos")
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            // A partir do item, descobre qual ContextMenuStrip é o "dono" dele
            ContextMenuStrip contextMenu = menuItem.Owner as ContextMenuStrip;
            if (contextMenu == null) return;

            // AGORA SIM: Pergunta diretamente ao menu qual controle o abriu
            CheckedListBox listBox = contextMenu.SourceControl as CheckedListBox;

            // Se o controle encontrado for de fato um CheckedListBox...
            if (listBox != null)
            {
                // A partir daqui, a nossa lógica inteligente continua a mesma e vai funcionar!
                bool deveMarcar = (listBox.CheckedItems.Count == 0);

                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    listBox.SetItemChecked(i, deveMarcar);
                }
            }
        }

        private void Placeholder_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            string placeholder = "Informe o caminho da pasta aqui"; // Mantenha este texto igual ao que você colocou no Design

            if (txt != null && txt.Text == placeholder && txt.ForeColor == SystemColors.GrayText)
            {
                txt.Text = "";
                txt.ForeColor = SystemColors.WindowText; // Cor normal do texto
            }
        }

        private void Placeholder_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            string placeholder = "Informe o caminho da pasta aqui";

            if (txt != null && string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = placeholder;
                txt.ForeColor = SystemColors.GrayText; // Cor cinza do placeholder
            }
        }

        private void btnProcurarAtualizadores_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório de destino para Atualizadores";
                dialogo.ShowNewFolderButton = true;
                string ultimoCaminho = LerValorIni("ULTIMOS_CAMINHOS", "UltimoCaminhoAtualizadores", caminhoIni);
                if (!string.IsNullOrEmpty(ultimoCaminho) && Directory.Exists(ultimoCaminho))
                {
                    dialogo.SelectedPath = ultimoCaminho;
                }

                else
                {
                    // Só define o caminho se o texto NÃO for o placeholder E se o caminho existir
                    if (txtDestinoAtualizadores.Text != "Informe o caminho da pasta aqui" && Directory.Exists(txtDestinoAtualizadores.Text))
                    {
                        dialogo.SelectedPath = txtDestinoAtualizadores.Text;
                    }
                }

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txtDestinoAtualizadores.Text = dialogo.SelectedPath;
                    // Força a saída do placeholder visualmente
                    txtDestinoAtualizadores.ForeColor = SystemColors.WindowText;
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoAtualizadores", dialogo.SelectedPath, caminhoIni);
                    // Salva também na seção [CAMINHOS] para compatibilidade com a lógica de cópia atual?
                    WritePrivateProfileString("CAMINHOS", "PASTA_DADOS", dialogo.SelectedPath, caminhoIni); // VERIFICAR SE ISSO É NECESSÁRIO
                }
            }
        }

        private void btnProcurarClientes_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório de destino para Clientes";
                dialogo.ShowNewFolderButton = true;
                string ultimoCaminho = LerValorIni("ULTIMOS_CAMINHOS", "UltimoCaminhoClientes", caminhoIni);
                if (!string.IsNullOrEmpty(ultimoCaminho) && Directory.Exists(ultimoCaminho))
                {
                    dialogo.SelectedPath = ultimoCaminho;
                }
                else
                {
                    if (txtDestinoClientes.Text != "Informe o caminho da pasta aqui" && Directory.Exists(txtDestinoClientes.Text))
                    {
                        dialogo.SelectedPath = txtDestinoClientes.Text;
                    }
                }

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txtDestinoClientes.Text = dialogo.SelectedPath;
                    txtDestinoClientes.ForeColor = SystemColors.WindowText;
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoClientes", dialogo.SelectedPath, caminhoIni);
                    WritePrivateProfileString("CAMINHOS", "PASTA_CLIENT", dialogo.SelectedPath, caminhoIni); // VERIFICAR
                }
            }
        }

        private void btnProcurarServidores_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecione o diretório de destino para Servidores";
                dialogo.ShowNewFolderButton = true;
                string ultimoCaminho = LerValorIni("ULTIMOS_CAMINHOS", "UltimoCaminhoServidores", caminhoIni);
                if (!string.IsNullOrEmpty(ultimoCaminho) && Directory.Exists(ultimoCaminho))
                {
                    dialogo.SelectedPath = ultimoCaminho;
                }
                else
                {
                    if (txtDestinoServidores.Text != "Informe o caminho da pasta aqui" && Directory.Exists(txtDestinoServidores.Text))
                    {
                        dialogo.SelectedPath = txtDestinoServidores.Text;
                    }
                }
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txtDestinoServidores.Text = dialogo.SelectedPath;
                    txtDestinoServidores.ForeColor = SystemColors.WindowText;
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoServidores", dialogo.SelectedPath, caminhoIni);
                    WritePrivateProfileString("CAMINHOS", "PASTA_SERVER", dialogo.SelectedPath, caminhoIni); // VERIFICAR
                }
            }
        }

        private void btnAdicionarAtualizador_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                // --- 1. MENSAGEM CORRIGIDA (conforme sua imagem) ---
                dialogo.Description = "Selecione a pasta do Atualizador (ex: Firebird, Oracle)";
                dialogo.ShowNewFolderButton = false;

                // --- 2. LÓGICA DE SALVAR/CARREGAR CAMINHO (conforme sua imagem) ---
                // Tenta carregar o *último* caminho salvo para esta janela específica
                string ultimoCaminho = LerValorIni("ULTIMOS_CAMINHOS", "UltimoCaminhoPastaAtualizador", caminhoIni);

                if (!string.IsNullOrEmpty(ultimoCaminho) && Directory.Exists(ultimoCaminho))
                {
                    dialogo.InitialDirectory = ultimoCaminho;
                }
                else
                {
                    // Fallback: Tenta abrir na pasta BD dentro da Branch
                    string caminhoBranch = edtCaminhoBranch.Text; //
                    string dePastaDados = LerValorIni("CAMINHOS", "DE_PASTA_DADOS", caminhoIni); //
                    string caminhoPadrao = Path.Combine(caminhoBranch, dePastaDados);

                    if (Directory.Exists(caminhoPadrao))
                    {
                        dialogo.InitialDirectory = caminhoPadrao;
                    }
                    else if (Directory.Exists(caminhoBranch))
                    {
                        dialogo.InitialDirectory = caminhoBranch;
                    }
                }

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    // Salva o caminho selecionado para a próxima vez
                    WritePrivateProfileString("ULTIMOS_CAMINHOS", "UltimoCaminhoPastaAtualizador", dialogo.SelectedPath, caminhoIni);

                    // Pega apenas o NOME da pasta selecionada (ex: "Firebird")
                    string nomePasta = new DirectoryInfo(dialogo.SelectedPath).Name;

                    // O resto da sua lógica original continua
                    bool jaExiste = clbAtualizadores.Items.OfType<string>().Any(item => string.Equals(item, nomePasta, StringComparison.OrdinalIgnoreCase));

                    if (jaExiste)
                    {
                        RegistrarLogCopiarDados($"Atualizador '{nomePasta}' já existe na lista. Ignorando.");
                        MessageBox.Show($"A pasta '{nomePasta}' já está na lista.", "Item Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        clbAtualizadores.Items.Add(nomePasta);
                        RegistrarLogCopiarDados($"Pasta de atualizador '{nomePasta}' adicionada à lista de configuração.");
                        configuracoesForamAlteradas = true;
                        AtualizarEstadoBotoesConfig();
                    }
                }
            }
        }
        private void CopiarDiretorioComLog(string sourceDir, string destDir, string nomePasta)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDir);
                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException($"Diretório de origem não encontrado: {sourceDir}");
                }

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    RegistrarLogCopiarDados($"Criado diretório: {destDir}", Color.Gray);
                }

                // Copia os arquivos
                foreach (FileInfo file in dir.GetFiles())
                {
                    string tempPath = Path.Combine(destDir, file.Name);
                    file.CopyTo(tempPath, true);
                }

                // Copia os subdiretórios recursivamente
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    string tempPath = Path.Combine(destDir, subdir.Name);
                    CopiarDiretorioComLog(subdir.FullName, tempPath, subdir.Name);
                }

                // --- CORREÇÃO: LOG VERDE ---
                RegistrarLogCopiarDados($"OK: Pasta {nomePasta} copiada para {destDir}", Color.DarkGreen);
            }
            catch (Exception ex)
            {
                // --- CORREÇÃO: LOG VERMELHO ---
                RegistrarLogCopiarDados($"ERRO ao copiar a pasta {nomePasta}: {ex.Message}", Color.Red);
            }
        }

        private void btnFiltrarErros_Click(object sender, EventArgs e)
        {
            // Mostra apenas logs vermelhos (erros)
            RepopularLogs(Color.Red);
        }

        private void btnFiltrarSucesso_Click(object sender, EventArgs e)
        {
            // Mostra apenas logs verdes (sucesso)
            RepopularLogs(Color.DarkGreen);
        }

        private void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            // Mostra todos os logs (sem filtro)
            RepopularLogs(null);
        }
    }
}
