namespace ExeBoard
{
    partial class frmCopiarExes
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tabCopiarExes = new TabControl();
            tabCopiarDados1 = new TabPage();
            gbLog = new GroupBox();
            btnLimparLog = new Button();
            lbLog = new ListBox();
            gbServidores = new GroupBox();
            cbGroupServidores = new CheckedListBox();
            gbClientes = new GroupBox();
            cbGroupClientes = new CheckedListBox();
            gbAtualizadores = new GroupBox();
            cbGroupAtualizadores = new CheckedListBox();
            groupBox1 = new GroupBox();
            button1 = new Button();
            btnCriarConexao = new Button();
            btnCopiarDados = new Button();
            gbPastaDeDestino = new GroupBox();
            btnProcurarPastaDestino = new Button();
            edtPastaDestino = new TextBox();
            gbBranch = new GroupBox();
            btnBuscarCaminhoBranch = new Button();
            edtCaminhoBranch = new TextBox();
            tabServidores = new TabPage();
            groupboxServidores = new GroupBox();
            btnReiniciar = new Button();
            btnParar = new Button();
            btnIniciar = new Button();
            cbSelecionarParados = new CheckBox();
            cbEmExecucao = new CheckBox();
            groupBox2 = new GroupBox();
            lbLogServidores = new ListBox();
            tabConfiguracoes = new TabPage();
            btnCancelarAlteracoes = new Button();
            btnRemoverGlobal = new Button();
            btnSalvarConfiguracoes = new Button();
            gbConfigServidores = new GroupBox();
            clbServidores = new CheckedListBox();
            cmsMarcarDesmarcar = new ContextMenuStrip(components);
            tsmMarcarDesmarcarTodos = new ToolStripMenuItem();
            btnAdicionarExeServidor = new Button();
            btnAdicionarServico = new Button();
            gbConfigClientes = new GroupBox();
            clbClientes = new CheckedListBox();
            btnAdicionarCliente = new Button();
            tabSobre = new TabPage();
            gbSobre = new GroupBox();
            linkLabel1 = new LinkLabel();
            label5 = new Label();
            dgvColaboradores = new DataGridView();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            icBandeja = new NotifyIcon(components);
            timerStatusServidores = new System.Windows.Forms.Timer(components);
            tabCopiarExes.SuspendLayout();
            tabCopiarDados1.SuspendLayout();
            gbLog.SuspendLayout();
            gbServidores.SuspendLayout();
            gbClientes.SuspendLayout();
            gbAtualizadores.SuspendLayout();
            groupBox1.SuspendLayout();
            gbPastaDeDestino.SuspendLayout();
            gbBranch.SuspendLayout();
            tabServidores.SuspendLayout();
            groupBox2.SuspendLayout();
            tabConfiguracoes.SuspendLayout();
            gbConfigServidores.SuspendLayout();
            cmsMarcarDesmarcar.SuspendLayout();
            gbConfigClientes.SuspendLayout();
            tabSobre.SuspendLayout();
            gbSobre.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColaboradores).BeginInit();
            SuspendLayout();
            // 
            // tabCopiarExes
            // 
            tabCopiarExes.Controls.Add(tabCopiarDados1);
            tabCopiarExes.Controls.Add(tabServidores);
            tabCopiarExes.Controls.Add(tabConfiguracoes);
            tabCopiarExes.Controls.Add(tabSobre);
            tabCopiarExes.Dock = DockStyle.Fill;
            tabCopiarExes.Location = new Point(0, 0);
            tabCopiarExes.Name = "tabCopiarExes";
            tabCopiarExes.SelectedIndex = 0;
            tabCopiarExes.Size = new Size(800, 665);
            tabCopiarExes.TabIndex = 0;
            tabCopiarExes.SelectedIndexChanged += tabCopiarExes_SelectedIndexChanged;
            tabCopiarExes.Deselecting += tabCopiarExes_Deselecting;
            // 
            // tabCopiarDados1
            // 
            tabCopiarDados1.Controls.Add(gbLog);
            tabCopiarDados1.Controls.Add(gbServidores);
            tabCopiarDados1.Controls.Add(gbClientes);
            tabCopiarDados1.Controls.Add(gbAtualizadores);
            tabCopiarDados1.Controls.Add(groupBox1);
            tabCopiarDados1.Controls.Add(gbPastaDeDestino);
            tabCopiarDados1.Controls.Add(gbBranch);
            tabCopiarDados1.Location = new Point(4, 24);
            tabCopiarDados1.Name = "tabCopiarDados1";
            tabCopiarDados1.Padding = new Padding(3);
            tabCopiarDados1.Size = new Size(792, 637);
            tabCopiarDados1.TabIndex = 0;
            tabCopiarDados1.Text = "Copiar Dados";
            tabCopiarDados1.UseVisualStyleBackColor = true;
            // 
            // gbLog
            // 
            gbLog.Controls.Add(btnLimparLog);
            gbLog.Controls.Add(lbLog);
            gbLog.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbLog.Location = new Point(6, 484);
            gbLog.Name = "gbLog";
            gbLog.Size = new Size(777, 145);
            gbLog.TabIndex = 6;
            gbLog.TabStop = false;
            gbLog.Text = "Log de Atividades";
            // 
            // btnLimparLog
            // 
            btnLimparLog.Location = new Point(696, 22);
            btnLimparLog.Name = "btnLimparLog";
            btnLimparLog.Size = new Size(75, 23);
            btnLimparLog.TabIndex = 1;
            btnLimparLog.Text = "Limpar Log";
            btnLimparLog.UseVisualStyleBackColor = true;
            btnLimparLog.Click += btnLimparLog_Click;
            // 
            // lbLog
            // 
            lbLog.FormattingEnabled = true;
            lbLog.ItemHeight = 15;
            lbLog.Location = new Point(6, 22);
            lbLog.Name = "lbLog";
            lbLog.Size = new Size(684, 109);
            lbLog.TabIndex = 0;
            // 
            // gbServidores
            // 
            gbServidores.Controls.Add(cbGroupServidores);
            gbServidores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbServidores.Location = new Point(528, 205);
            gbServidores.Name = "gbServidores";
            gbServidores.Size = new Size(255, 273);
            gbServidores.TabIndex = 5;
            gbServidores.TabStop = false;
            gbServidores.Text = "Servidores";
            // 
            // cbGroupServidores
            // 
            cbGroupServidores.FormattingEnabled = true;
            cbGroupServidores.Location = new Point(6, 22);
            cbGroupServidores.Name = "cbGroupServidores";
            cbGroupServidores.Size = new Size(243, 238);
            cbGroupServidores.TabIndex = 0;
            cbGroupServidores.MouseDown += cbGroupServidores_MouseDown;
            // 
            // gbClientes
            // 
            gbClientes.Controls.Add(cbGroupClientes);
            gbClientes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbClientes.Location = new Point(267, 205);
            gbClientes.Name = "gbClientes";
            gbClientes.Size = new Size(255, 273);
            gbClientes.TabIndex = 4;
            gbClientes.TabStop = false;
            gbClientes.Text = "Clientes";
            // 
            // cbGroupClientes
            // 
            cbGroupClientes.FormattingEnabled = true;
            cbGroupClientes.Location = new Point(6, 22);
            cbGroupClientes.Name = "cbGroupClientes";
            cbGroupClientes.Size = new Size(243, 238);
            cbGroupClientes.TabIndex = 0;
            cbGroupClientes.MouseDown += cbGroupClientes_MouseDown;
            // 
            // gbAtualizadores
            // 
            gbAtualizadores.Controls.Add(cbGroupAtualizadores);
            gbAtualizadores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbAtualizadores.Location = new Point(6, 205);
            gbAtualizadores.Name = "gbAtualizadores";
            gbAtualizadores.Size = new Size(255, 273);
            gbAtualizadores.TabIndex = 3;
            gbAtualizadores.TabStop = false;
            gbAtualizadores.Text = "Atualizadores";
            // 
            // cbGroupAtualizadores
            // 
            cbGroupAtualizadores.FormattingEnabled = true;
            cbGroupAtualizadores.Location = new Point(6, 22);
            cbGroupAtualizadores.Name = "cbGroupAtualizadores";
            cbGroupAtualizadores.Size = new Size(243, 238);
            cbGroupAtualizadores.TabIndex = 0;
            cbGroupAtualizadores.MouseDown += cbGroupAtualizadores_MouseDown;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(btnCriarConexao);
            groupBox1.Controls.Add(btnCopiarDados);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(6, 136);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(777, 63);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ações";
            // 
            // button1
            // 
            button1.Location = new Point(238, 22);
            button1.Name = "button1";
            button1.Size = new Size(110, 23);
            button1.TabIndex = 2;
            button1.Text = "Abrir Atualizador";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnCriarConexao
            // 
            btnCriarConexao.Enabled = false;
            btnCriarConexao.Location = new Point(122, 22);
            btnCriarConexao.Name = "btnCriarConexao";
            btnCriarConexao.Size = new Size(110, 23);
            btnCriarConexao.TabIndex = 1;
            btnCriarConexao.Text = "Criar Conexão";
            btnCriarConexao.UseVisualStyleBackColor = true;
            // 
            // btnCopiarDados
            // 
            btnCopiarDados.Location = new Point(6, 22);
            btnCopiarDados.Name = "btnCopiarDados";
            btnCopiarDados.Size = new Size(110, 23);
            btnCopiarDados.TabIndex = 0;
            btnCopiarDados.Text = "Copiar Dados";
            btnCopiarDados.UseVisualStyleBackColor = true;
            btnCopiarDados.Click += btnCopiarDados_Click;
            // 
            // gbPastaDeDestino
            // 
            gbPastaDeDestino.Controls.Add(btnProcurarPastaDestino);
            gbPastaDeDestino.Controls.Add(edtPastaDestino);
            gbPastaDeDestino.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbPastaDeDestino.Location = new Point(6, 71);
            gbPastaDeDestino.Name = "gbPastaDeDestino";
            gbPastaDeDestino.Size = new Size(777, 59);
            gbPastaDeDestino.TabIndex = 1;
            gbPastaDeDestino.TabStop = false;
            gbPastaDeDestino.Text = "Pasta de Destino";
            // 
            // btnProcurarPastaDestino
            // 
            btnProcurarPastaDestino.Location = new Point(675, 22);
            btnProcurarPastaDestino.Name = "btnProcurarPastaDestino";
            btnProcurarPastaDestino.Size = new Size(75, 23);
            btnProcurarPastaDestino.TabIndex = 1;
            btnProcurarPastaDestino.Text = "Procurar";
            btnProcurarPastaDestino.UseVisualStyleBackColor = true;
            btnProcurarPastaDestino.Click += btnProcurarPastaDestino_Click;
            // 
            // edtPastaDestino
            // 
            edtPastaDestino.Location = new Point(6, 22);
            edtPastaDestino.Name = "edtPastaDestino";
            edtPastaDestino.Size = new Size(663, 23);
            edtPastaDestino.TabIndex = 0;
            // 
            // gbBranch
            // 
            gbBranch.Controls.Add(btnBuscarCaminhoBranch);
            gbBranch.Controls.Add(edtCaminhoBranch);
            gbBranch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbBranch.Location = new Point(6, 6);
            gbBranch.Name = "gbBranch";
            gbBranch.Size = new Size(777, 59);
            gbBranch.TabIndex = 0;
            gbBranch.TabStop = false;
            gbBranch.Text = "Branch (develop / main / tarefa)";
            // 
            // btnBuscarCaminhoBranch
            // 
            btnBuscarCaminhoBranch.Location = new Point(675, 22);
            btnBuscarCaminhoBranch.Name = "btnBuscarCaminhoBranch";
            btnBuscarCaminhoBranch.Size = new Size(75, 23);
            btnBuscarCaminhoBranch.TabIndex = 1;
            btnBuscarCaminhoBranch.Text = "Procurar";
            btnBuscarCaminhoBranch.UseVisualStyleBackColor = true;
            btnBuscarCaminhoBranch.Click += btnBuscarCaminhoBranch_Click;
            // 
            // edtCaminhoBranch
            // 
            edtCaminhoBranch.Location = new Point(6, 22);
            edtCaminhoBranch.Name = "edtCaminhoBranch";
            edtCaminhoBranch.Size = new Size(663, 23);
            edtCaminhoBranch.TabIndex = 0;
            // 
            // tabServidores
            // 
            tabServidores.Controls.Add(groupboxServidores);
            tabServidores.Controls.Add(btnReiniciar);
            tabServidores.Controls.Add(btnParar);
            tabServidores.Controls.Add(btnIniciar);
            tabServidores.Controls.Add(cbSelecionarParados);
            tabServidores.Controls.Add(cbEmExecucao);
            tabServidores.Controls.Add(groupBox2);
            tabServidores.Location = new Point(4, 24);
            tabServidores.Name = "tabServidores";
            tabServidores.Padding = new Padding(3);
            tabServidores.Size = new Size(792, 637);
            tabServidores.TabIndex = 1;
            tabServidores.Text = "Servidores";
            tabServidores.UseVisualStyleBackColor = true;
            // 
            // groupboxServidores
            // 
            groupboxServidores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupboxServidores.Location = new Point(12, 76);
            groupboxServidores.Name = "groupboxServidores";
            groupboxServidores.Size = new Size(765, 404);
            groupboxServidores.TabIndex = 13;
            groupboxServidores.TabStop = false;
            groupboxServidores.Text = "Servidores";
            // 
            // btnReiniciar
            // 
            btnReiniciar.Enabled = false;
            btnReiniciar.Font = new Font("Segoe UI", 12F);
            btnReiniciar.Location = new Point(254, 37);
            btnReiniciar.Name = "btnReiniciar";
            btnReiniciar.Size = new Size(115, 33);
            btnReiniciar.TabIndex = 12;
            btnReiniciar.Text = "Reiniciar";
            btnReiniciar.UseVisualStyleBackColor = true;
            btnReiniciar.Click += btnReiniciar_Click;
            // 
            // btnParar
            // 
            btnParar.Enabled = false;
            btnParar.Font = new Font("Segoe UI", 12F);
            btnParar.Location = new Point(133, 37);
            btnParar.Name = "btnParar";
            btnParar.Size = new Size(115, 33);
            btnParar.TabIndex = 11;
            btnParar.Text = "Parar";
            btnParar.UseVisualStyleBackColor = true;
            btnParar.Click += btnParar_Click;
            // 
            // btnIniciar
            // 
            btnIniciar.Enabled = false;
            btnIniciar.Font = new Font("Segoe UI", 12F);
            btnIniciar.Location = new Point(12, 37);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(115, 33);
            btnIniciar.TabIndex = 10;
            btnIniciar.Text = "Iniciar";
            btnIniciar.UseVisualStyleBackColor = true;
            btnIniciar.Click += btnIniciar_Click;
            // 
            // cbSelecionarParados
            // 
            cbSelecionarParados.AutoSize = true;
            cbSelecionarParados.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            cbSelecionarParados.Location = new Point(231, 6);
            cbSelecionarParados.Name = "cbSelecionarParados";
            cbSelecionarParados.Size = new Size(174, 25);
            cbSelecionarParados.TabIndex = 9;
            cbSelecionarParados.Text = "Selecionar Parados";
            cbSelecionarParados.UseVisualStyleBackColor = true;
            cbSelecionarParados.CheckedChanged += cbSelecionarParados_CheckedChanged;
            // 
            // cbEmExecucao
            // 
            cbEmExecucao.AutoSize = true;
            cbEmExecucao.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            cbEmExecucao.Location = new Point(12, 6);
            cbEmExecucao.Name = "cbEmExecucao";
            cbEmExecucao.Size = new Size(213, 25);
            cbEmExecucao.TabIndex = 8;
            cbEmExecucao.Text = "Selecionar em execução";
            cbEmExecucao.UseVisualStyleBackColor = true;
            cbEmExecucao.CheckedChanged += chkSelecionarEmExecucao_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lbLogServidores);
            groupBox2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox2.Location = new Point(6, 486);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(777, 145);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Log de Atividades";
            // 
            // lbLogServidores
            // 
            lbLogServidores.FormattingEnabled = true;
            lbLogServidores.ItemHeight = 15;
            lbLogServidores.Location = new Point(6, 22);
            lbLogServidores.Name = "lbLogServidores";
            lbLogServidores.Size = new Size(765, 109);
            lbLogServidores.TabIndex = 0;
            // 
            // tabConfiguracoes
            // 
            tabConfiguracoes.Controls.Add(btnCancelarAlteracoes);
            tabConfiguracoes.Controls.Add(btnRemoverGlobal);
            tabConfiguracoes.Controls.Add(btnSalvarConfiguracoes);
            tabConfiguracoes.Controls.Add(gbConfigServidores);
            tabConfiguracoes.Controls.Add(gbConfigClientes);
            tabConfiguracoes.Location = new Point(4, 24);
            tabConfiguracoes.Name = "tabConfiguracoes";
            tabConfiguracoes.Size = new Size(792, 637);
            tabConfiguracoes.TabIndex = 3;
            tabConfiguracoes.Text = "Configurações";
            tabConfiguracoes.UseVisualStyleBackColor = true;
            // 
            // btnCancelarAlteracoes
            // 
            btnCancelarAlteracoes.Enabled = false;
            btnCancelarAlteracoes.Location = new Point(526, 611);
            btnCancelarAlteracoes.Name = "btnCancelarAlteracoes";
            btnCancelarAlteracoes.Size = new Size(131, 23);
            btnCancelarAlteracoes.TabIndex = 10;
            btnCancelarAlteracoes.Text = "Cancelar Alterações";
            btnCancelarAlteracoes.UseVisualStyleBackColor = true;
            btnCancelarAlteracoes.Click += button2_Click_3;
            // 
            // btnRemoverGlobal
            // 
            btnRemoverGlobal.Location = new Point(378, 611);
            btnRemoverGlobal.Name = "btnRemoverGlobal";
            btnRemoverGlobal.Size = new Size(142, 23);
            btnRemoverGlobal.TabIndex = 9;
            btnRemoverGlobal.Text = "Remover Selecionados";
            btnRemoverGlobal.UseVisualStyleBackColor = true;
            btnRemoverGlobal.Click += btnRemoverGlobal_Click;
            // 
            // btnSalvarConfiguracoes
            // 
            btnSalvarConfiguracoes.Enabled = false;
            btnSalvarConfiguracoes.Location = new Point(663, 611);
            btnSalvarConfiguracoes.Name = "btnSalvarConfiguracoes";
            btnSalvarConfiguracoes.Size = new Size(121, 23);
            btnSalvarConfiguracoes.TabIndex = 7;
            btnSalvarConfiguracoes.Text = "Salvar Alterações";
            btnSalvarConfiguracoes.UseVisualStyleBackColor = true;
            btnSalvarConfiguracoes.Click += btnSalvarConfiguracoes_Click;
            // 
            // gbConfigServidores
            // 
            gbConfigServidores.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            gbConfigServidores.Controls.Add(clbServidores);
            gbConfigServidores.Controls.Add(btnAdicionarExeServidor);
            gbConfigServidores.Controls.Add(btnAdicionarServico);
            gbConfigServidores.Location = new Point(405, 10);
            gbConfigServidores.Name = "gbConfigServidores";
            gbConfigServidores.Padding = new Padding(3, 3, 3, 40);
            gbConfigServidores.Size = new Size(385, 600);
            gbConfigServidores.TabIndex = 1;
            gbConfigServidores.TabStop = false;
            gbConfigServidores.Text = "Gerenciar Aplicações/Serviços Servidores";
            // 
            // clbServidores
            // 
            clbServidores.CheckOnClick = true;
            clbServidores.ContextMenuStrip = cmsMarcarDesmarcar;
            clbServidores.Dock = DockStyle.Fill;
            clbServidores.FormattingEnabled = true;
            clbServidores.Location = new Point(3, 19);
            clbServidores.Name = "clbServidores";
            clbServidores.Size = new Size(379, 541);
            clbServidores.TabIndex = 8;
            clbServidores.ItemCheck += clb_ItemCheck;
            // 
            // cmsMarcarDesmarcar
            // 
            cmsMarcarDesmarcar.Items.AddRange(new ToolStripItem[] { tsmMarcarDesmarcarTodos });
            cmsMarcarDesmarcar.Name = "cmsMarcarDesmarcar";
            cmsMarcarDesmarcar.Size = new Size(207, 26);
            // 
            // tsmMarcarDesmarcarTodos
            // 
            tsmMarcarDesmarcarTodos.Name = "tsmMarcarDesmarcarTodos";
            tsmMarcarDesmarcarTodos.Size = new Size(206, 22);
            tsmMarcarDesmarcarTodos.Text = "Marcar/Desmarcar Todos";
            tsmMarcarDesmarcarTodos.Click += tsmMarcarDesmarcarTodos_Click;
            // 
            // btnAdicionarExeServidor
            // 
            btnAdicionarExeServidor.Location = new Point(121, 566);
            btnAdicionarExeServidor.Name = "btnAdicionarExeServidor";
            btnAdicionarExeServidor.Size = new Size(96, 23);
            btnAdicionarExeServidor.TabIndex = 7;
            btnAdicionarExeServidor.Text = "Adicionar Exe";
            btnAdicionarExeServidor.UseVisualStyleBackColor = true;
            btnAdicionarExeServidor.Click += btnAdicionarExeServidor_Click;
            // 
            // btnAdicionarServico
            // 
            btnAdicionarServico.Location = new Point(6, 566);
            btnAdicionarServico.Name = "btnAdicionarServico";
            btnAdicionarServico.Size = new Size(109, 23);
            btnAdicionarServico.TabIndex = 4;
            btnAdicionarServico.Text = "Adicionar Serviço";
            btnAdicionarServico.UseVisualStyleBackColor = true;
            btnAdicionarServico.Click += btnAdicionarServico_Click;
            // 
            // gbConfigClientes
            // 
            gbConfigClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gbConfigClientes.Controls.Add(clbClientes);
            gbConfigClientes.Controls.Add(btnAdicionarCliente);
            gbConfigClientes.Location = new Point(10, 10);
            gbConfigClientes.Name = "gbConfigClientes";
            gbConfigClientes.Padding = new Padding(3, 3, 3, 40);
            gbConfigClientes.Size = new Size(385, 600);
            gbConfigClientes.TabIndex = 0;
            gbConfigClientes.TabStop = false;
            gbConfigClientes.Text = "Gerenciar Aplicações Clientes";
            // 
            // clbClientes
            // 
            clbClientes.CheckOnClick = true;
            clbClientes.ContextMenuStrip = cmsMarcarDesmarcar;
            clbClientes.Dock = DockStyle.Fill;
            clbClientes.FormattingEnabled = true;
            clbClientes.Location = new Point(3, 19);
            clbClientes.Name = "clbClientes";
            clbClientes.Size = new Size(379, 541);
            clbClientes.TabIndex = 1;
            clbClientes.ItemCheck += clb_ItemCheck;
            // 
            // btnAdicionarCliente
            // 
            btnAdicionarCliente.Location = new Point(6, 566);
            btnAdicionarCliente.Name = "btnAdicionarCliente";
            btnAdicionarCliente.Size = new Size(96, 23);
            btnAdicionarCliente.TabIndex = 1;
            btnAdicionarCliente.Text = " Adicionar Exes";
            btnAdicionarCliente.UseVisualStyleBackColor = true;
            btnAdicionarCliente.Click += btnAdicionarCliente_Click;
            // 
            // tabSobre
            // 
            tabSobre.Controls.Add(gbSobre);
            tabSobre.Location = new Point(4, 24);
            tabSobre.Name = "tabSobre";
            tabSobre.Size = new Size(792, 637);
            tabSobre.TabIndex = 2;
            tabSobre.Text = "Sobre";
            tabSobre.UseVisualStyleBackColor = true;
            // 
            // gbSobre
            // 
            gbSobre.Controls.Add(linkLabel1);
            gbSobre.Controls.Add(label5);
            gbSobre.Controls.Add(dgvColaboradores);
            gbSobre.Controls.Add(label4);
            gbSobre.Controls.Add(label3);
            gbSobre.Controls.Add(label2);
            gbSobre.Controls.Add(label1);
            gbSobre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbSobre.Location = new Point(8, 3);
            gbSobre.Name = "gbSobre";
            gbSobre.Size = new Size(776, 626);
            gbSobre.TabIndex = 0;
            gbSobre.TabStop = false;
            gbSobre.Text = "Sobre o CopiarExes";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(114, 79);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(261, 15);
            linkLabel1.TabIndex = 6;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/giancarlogiulian/ExeBoard\r\n";
            linkLabel1.Click += linkLabel1_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 79);
            label5.Name = "label5";
            label5.Size = new Size(111, 15);
            label5.TabIndex = 5;
            label5.Text = "GitHub do Projeto:";
            // 
            // dgvColaboradores
            // 
            dgvColaboradores.AllowUserToAddRows = false;
            dgvColaboradores.AllowUserToDeleteRows = false;
            dgvColaboradores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvColaboradores.Location = new Point(6, 112);
            dgvColaboradores.MultiSelect = false;
            dgvColaboradores.Name = "dgvColaboradores";
            dgvColaboradores.ReadOnly = true;
            dgvColaboradores.Size = new Size(764, 495);
            dgvColaboradores.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 94);
            label4.Name = "label4";
            label4.Size = new Size(167, 15);
            label4.TabIndex = 3;
            label4.Text = "Contribuiram com o projeto: ";
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 64);
            label3.Name = "label3";
            label3.Size = new Size(84, 15);
            label3.TabIndex = 2;
            label3.Text = "Versão: v2.1.0";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 34);
            label2.Name = "label2";
            label2.Size = new Size(611, 30);
            label2.TabIndex = 1;
            label2.Text = "Programa para automatizar o download, instalação, inicialização de sistemas com arquitetura cliente-servidor \r\ne atualização de bancos de dados.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(97, 15);
            label1.TabIndex = 0;
            label1.Text = "Copiar Exes v2.1";
            // 
            // icBandeja
            // 
            icBandeja.Text = "CopiarExes";
            icBandeja.Visible = true;
            icBandeja.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // timerStatusServidores
            // 
            timerStatusServidores.Interval = 1000;
            timerStatusServidores.Tick += timerStatusServidores_Tick;
            // 
            // frmCopiarExes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 665);
            Controls.Add(tabCopiarExes);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCopiarExes";
            Text = "ExeBoard v2.1 [2025]";
            FormClosing += frmCopiarExes_FormClosing;
            Load += frmCopiarExes_Load;
            tabCopiarExes.ResumeLayout(false);
            tabCopiarDados1.ResumeLayout(false);
            gbLog.ResumeLayout(false);
            gbServidores.ResumeLayout(false);
            gbClientes.ResumeLayout(false);
            gbAtualizadores.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            gbPastaDeDestino.ResumeLayout(false);
            gbPastaDeDestino.PerformLayout();
            gbBranch.ResumeLayout(false);
            gbBranch.PerformLayout();
            tabServidores.ResumeLayout(false);
            tabServidores.PerformLayout();
            groupBox2.ResumeLayout(false);
            tabConfiguracoes.ResumeLayout(false);
            gbConfigServidores.ResumeLayout(false);
            cmsMarcarDesmarcar.ResumeLayout(false);
            gbConfigClientes.ResumeLayout(false);
            tabSobre.ResumeLayout(false);
            gbSobre.ResumeLayout(false);
            gbSobre.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColaboradores).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabCopiarExes;
        private TabPage tabCopiarDados1;
        private TabPage tabServidores;
        private TabPage tabSobre;
        private GroupBox gbBranch;
        private Button btnBuscarCaminhoBranch;
        private TextBox edtCaminhoBranch;
        private GroupBox gbPastaDeDestino;
        private Button btnProcurarPastaDestino;
        private TextBox edtPastaDestino;
        private GroupBox groupBox1;
        private Button btnCopiarDados;
        private Button btnCriarConexao;
        private Button button1;
        private GroupBox gbClientes;
        private GroupBox gbAtualizadores;
        private GroupBox gbServidores;
        private CheckedListBox cbGroupAtualizadores;
        private CheckedListBox cbGroupClientes;
        private CheckedListBox cbGroupServidores;
        private GroupBox gbLog;
        private ListBox lbLog;
        private NotifyIcon icBandeja;
        private GroupBox gbSobre;
        private Label label2;
        private Label label1;
        private Label label4;
        private Label label3;
        private DataGridView dgvColaboradores;
        private Label label5;
        private LinkLabel linkLabel1;
        private GroupBox groupBox2;
        private ListBox lbLogServidores;
        private Button btnLimparLog;
        private Button btnIniciar;
        private CheckBox cbSelecionarParados;
        private CheckBox cbEmExecucao;
        private Button btnParar;
        private GroupBox groupboxServidores;
        private Button btnReiniciar;
        private System.Windows.Forms.Timer timerStatusServidores;
        private TabPage tabConfiguracoes;
        private GroupBox gbConfigClientes;
        private GroupBox gbConfigServidores;
        private Button btnAdicionarCliente;
        private Button btnAdicionarServico;
        private Button btnSalvarConfiguracoes;
        private Button btnAdicionarExeServidor;
        private Button btnRemoverGlobal;
        private Button btnCancelarAlteracoes;
        private CheckedListBox clbServidores;
        private CheckedListBox clbClientes;
        private ContextMenuStrip cmsMarcarDesmarcar;
        private ToolStripMenuItem tsmMarcarDesmarcarTodos;
    }
}
