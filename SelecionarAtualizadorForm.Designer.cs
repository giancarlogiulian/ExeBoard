using System.Windows.Forms;

namespace ExeBoard
{
    partial class frmAtualizador
    {
        private System.ComponentModel.IContainer components = null;

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
            lbAtualizador = new ListBox();
            btnAbrir = new Button();
            SuspendLayout();
            // 
            // lbAtualizador
            // 
            lbAtualizador.FormattingEnabled = true;
            lbAtualizador.ItemHeight = 15;
            lbAtualizador.Location = new Point(12, 12);
            lbAtualizador.Name = "lbAtualizador";
            lbAtualizador.Size = new Size(291, 94);
            lbAtualizador.TabIndex = 0;
            // 
            // btnAbrir
            // 
            btnAbrir.Location = new Point(309, 12);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(75, 23);
            btnAbrir.TabIndex = 1;
            btnAbrir.Text = "Abrir";
            btnAbrir.UseVisualStyleBackColor = true;
            btnAbrir.Click += btnAbrir_Click;
            // 
            // frmAtualizador
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 143);
            Controls.Add(btnAbrir);
            Controls.Add(lbAtualizador);
            Name = "frmAtualizador";
            Text = "Selecionar o programa atualizador";
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbAtualizador;
        private Button btnAbrir;
    }
}