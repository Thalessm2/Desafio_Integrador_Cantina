using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Desenvolvimento_Cantina
{
    public enum TipoMensagem
    {
        Informacao,
        Sucesso,
        Aviso,
        Erro
    }

    /// <summary>
    /// Janela de mensagem no visual do sistema (substitui o MessageBox).
    /// Uso: FrmMensagem.Sucesso(this, "Venda registrada!");
    ///      FrmMensagem.Erro(this, "Não foi possível salvar.");
    /// </summary>
    public class FrmMensagem : Form
    {
        // ---------- ATALHOS ----------
        public static void Sucesso(IWin32Window dono, string mensagem, string titulo = null)
        {
            Mostrar(dono, mensagem, TipoMensagem.Sucesso, titulo);
        }

        public static void Erro(IWin32Window dono, string mensagem, string titulo = null)
        {
            Mostrar(dono, mensagem, TipoMensagem.Erro, titulo);
        }

        public static void Aviso(IWin32Window dono, string mensagem, string titulo = null)
        {
            Mostrar(dono, mensagem, TipoMensagem.Aviso, titulo);
        }

        public static void Informacao(IWin32Window dono, string mensagem, string titulo = null)
        {
            Mostrar(dono, mensagem, TipoMensagem.Informacao, titulo);
        }

        public static void Mostrar(IWin32Window dono, string mensagem, TipoMensagem tipo, string titulo = null)
        {
            using (FrmMensagem f = new FrmMensagem(mensagem, tipo, titulo))
            {
                f.ShowDialog(dono);
            }
        }

        // ---------- JANELA ----------
        private FrmMensagem(string mensagem, TipoMensagem tipo, string titulo)
        {
            Color cor = Color.RoyalBlue;
            string glifo = "i";
            string fonteGlifo = "Segoe UI";
            string tituloPadrao = "Informação";

            switch (tipo)
            {
                case TipoMensagem.Sucesso:
                    cor = Color.SeaGreen;
                    glifo = "\u2713";
                    fonteGlifo = "Segoe UI Symbol";
                    tituloPadrao = "Sucesso";
                    break;
                case TipoMensagem.Erro:
                    cor = Color.Firebrick;
                    glifo = "\u2715";
                    fonteGlifo = "Segoe UI Symbol";
                    tituloPadrao = "Erro";
                    break;
                case TipoMensagem.Aviso:
                    cor = Color.DarkOrange;
                    glifo = "!";
                    fonteGlifo = "Segoe UI";
                    tituloPadrao = "Atenção";
                    break;
            }

            if (mensagem == null)
                mensagem = "";

            SuspendLayout();

            Text = string.IsNullOrWhiteSpace(titulo) ? tituloPadrao : titulo;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Font = new Font("Segoe UI", 10F);
            BackColor = cor;                       // vira uma borda fina de 1px
            Padding = new Padding(1);

            const int largura = 440;
            int larguraTexto = largura - 2 - 48;

            Size medida = TextRenderer.MeasureText(
                mensagem,
                Font,
                new Size(larguraTexto, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

            int alturaTexto = Math.Min(Math.Max(24, medida.Height + 4), 300);

            ClientSize = new Size(largura, 2 + 70 + 24 + alturaTexto + 24 + 70);

            Panel corpo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // ---------- CABEÇALHO ----------
            Panel cabecalho = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = cor
            };

            Label lblTitulo = new Label
            {
                Text = Text,
                Dock = DockStyle.Fill,
                BackColor = cor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Panel pnlIcone = new Panel
            {
                Dock = DockStyle.Left,
                Width = 88,
                BackColor = cor
            };
            pnlIcone.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle circulo = new Rectangle(24, 15, 40, 40);

                using (SolidBrush pincel = new SolidBrush(Color.White))
                    e.Graphics.FillEllipse(pincel, circulo);

                using (Font fonte = new Font(fonteGlifo, 17F, FontStyle.Bold))
                    TextRenderer.DrawText(
                        e.Graphics, glifo, fonte, circulo, cor,
                        TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter |
                        TextFormatFlags.NoPadding);
            };

            cabecalho.Controls.Add(lblTitulo);   // Fill primeiro
            cabecalho.Controls.Add(pnlIcone);    // depois o Left

            // ---------- TEXTO ----------
            Panel areaTexto = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            Label lblMensagem = new Label
            {
                Text = mensagem,
                AutoSize = true,
                MaximumSize = new Size(larguraTexto, 0),
                Location = new Point(24, 24),
                ForeColor = Color.FromArgb(40, 45, 60),
                UseMnemonic = false
            };
            areaTexto.Controls.Add(lblMensagem);

            // ---------- RODAPÉ ----------
            Panel rodape = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(24, 14, 24, 16)
            };

            Button btnOk = new Button
            {
                Text = "OK",
                Dock = DockStyle.Right,
                Width = 120,
                BackColor = cor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;
            rodape.Controls.Add(btnOk);

            AcceptButton = btnOk;
            CancelButton = btnOk;

            // Ordem: o Fill primeiro, depois os Dock.
            corpo.Controls.Add(areaTexto);
            corpo.Controls.Add(rodape);
            corpo.Controls.Add(cabecalho);
            Controls.Add(corpo);

            ResumeLayout(true);
        }
    }
}