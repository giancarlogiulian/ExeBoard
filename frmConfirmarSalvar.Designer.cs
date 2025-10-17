namespace CopiarExes
{
    partial class frmConfirmarSalvar
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
            panel1 = new Panel();
            btnSairSemSalvar = new Button();
            btnSalvarSair = new Button();
            btnPermanecer = new Button();
            pbIconeAviso = new PictureBox();
            lblTitulo = new Label();
            lblSubtexto = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbIconeAviso).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlLight;
            panel1.Controls.Add(btnSairSemSalvar);
            panel1.Controls.Add(btnSalvarSair);
            panel1.Controls.Add(btnPermanecer);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 91);
            panel1.Name = "panel1";
            panel1.Size = new Size(434, 50);
            panel1.TabIndex = 0;
            // 
            // btnSairSemSalvar
            // 
            btnSairSemSalvar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSairSemSalvar.BackColor = Color.White;
            btnSairSemSalvar.FlatAppearance.BorderColor = SystemColors.ControlDark;
            btnSairSemSalvar.FlatAppearance.MouseDownBackColor = Color.AliceBlue;
            btnSairSemSalvar.FlatAppearance.MouseOverBackColor = Color.AliceBlue;
            btnSairSemSalvar.FlatStyle = FlatStyle.Flat;
            btnSairSemSalvar.Location = new Point(252, 12);
            btnSairSemSalvar.Name = "btnSairSemSalvar";
            btnSairSemSalvar.Size = new Size(85, 26);
            btnSairSemSalvar.TabIndex = 6;
            btnSairSemSalvar.Text = "Não Salvar";
            btnSairSemSalvar.UseVisualStyleBackColor = false;
            btnSairSemSalvar.Click += btnSairSemSalvar_Click;
            // 
            // btnSalvarSair
            // 
            btnSalvarSair.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalvarSair.BackColor = Color.White;
            btnSalvarSair.FlatAppearance.BorderColor = SystemColors.ControlDark;
            btnSalvarSair.FlatAppearance.MouseDownBackColor = Color.AliceBlue;
            btnSalvarSair.FlatAppearance.MouseOverBackColor = Color.AliceBlue;
            btnSalvarSair.FlatStyle = FlatStyle.Flat;
            btnSalvarSair.Location = new Point(161, 12);
            btnSalvarSair.Name = "btnSalvarSair";
            btnSalvarSair.Size = new Size(85, 26);
            btnSalvarSair.TabIndex = 5;
            btnSalvarSair.Text = "Salvar";
            btnSalvarSair.UseVisualStyleBackColor = false;
            btnSalvarSair.Click += btnSalvarSair_Click;
            // 
            // btnPermanecer
            // 
            btnPermanecer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPermanecer.BackColor = Color.White;
            btnPermanecer.FlatAppearance.BorderColor = SystemColors.ControlDark;
            btnPermanecer.FlatAppearance.MouseDownBackColor = Color.AliceBlue;
            btnPermanecer.FlatAppearance.MouseOverBackColor = Color.AliceBlue;
            btnPermanecer.FlatStyle = FlatStyle.Flat;
            btnPermanecer.Location = new Point(343, 12);
            btnPermanecer.Name = "btnPermanecer";
            btnPermanecer.Size = new Size(85, 26);
            btnPermanecer.TabIndex = 6;
            btnPermanecer.Text = "Cancelar";
            btnPermanecer.UseVisualStyleBackColor = false;
            btnPermanecer.Click += btnPermanecer_Click;
            // 
            // pbIconeAviso
            // 
            pbIconeAviso.Location = new Point(20, 20);
            pbIconeAviso.Name = "pbIconeAviso";
            pbIconeAviso.Size = new Size(32, 32);
            pbIconeAviso.TabIndex = 1;
            pbIconeAviso.TabStop = false;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.RoyalBlue;
            lblTitulo.Location = new Point(65, 20);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(201, 40);
            lblTitulo.TabIndex = 2;
            lblTitulo.Text = "Deseja salvar as alterações?\n\n";
            lblTitulo.Click += lblTitulo_Click;
            // 
            // lblSubtexto
            // 
            lblSubtexto.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtexto.Location = new Point(65, 50);
            lblSubtexto.Name = "lblSubtexto";
            lblSubtexto.Size = new Size(357, 45);
            lblSubtexto.TabIndex = 3;
            lblSubtexto.Text = "Suas alterações na lista de configurações serão perdidas se não forem salvas.\n\n";
            // 
            // frmConfirmarSalvar
            // 
            AcceptButton = btnSalvarSair;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnPermanecer;
            ClientSize = new Size(434, 141);
            Controls.Add(pbIconeAviso);
            Controls.Add(panel1);
            Controls.Add(lblSubtexto);
            Controls.Add(lblTitulo);
            Font = new Font("Segoe UI", 11F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmConfirmarSalvar";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Alterações Pendentes";
            Load += frmConfirmarSalvar_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbIconeAviso).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pbIconeAviso;
        private Label lblTitulo;
        private Label lblSubtexto;
        private Button btnSalvarSair;
        private Button btnPermanecer;
        private Button btnSairSemSalvar;
    }
}