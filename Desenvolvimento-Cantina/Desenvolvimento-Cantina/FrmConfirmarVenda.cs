using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Desenvolvimento_Cantina
{
    /// <summary>
    /// Janela de confirmação de venda, no mesmo visual do sistema.
    /// Uso: if (!FrmConfirmarVenda.Mostrar(this, total, forma, recebido, troco)) return;
    /// </summary>
    public class FrmConfirmarVenda : Form
    {
        private static readonly CultureInfo Br = new CultureInfo("pt-BR");
        private static readonly Color CorMenu = Color.FromArgb(25, 35, 60);
        private static readonly Color CorTexto = Color.FromArgb(40, 45, 60);
        private static readonly Color CorSuave = Color.FromArgb(120, 128, 145);

        /// <summary>Mostra a janela. Retorna true se o usuário confirmou.</summary>
        /// <param name="valorRecebido">Passe null se não for pagamento em dinheiro.</param>
        /// <param name="troco">Passe null se não for pagamento em dinheiro.</param>
        public static bool Mostrar(IWin32Window dono, decimal total, string formaPagamento,
                                   decimal? valorRecebido = null, decimal? troco = null)
        {
            using (FrmConfirmarVenda f = new FrmConfirmarVenda(total, formaPagamento, valorRecebido, troco))
            {
                return f.ShowDialog(dono) == DialogResult.OK;
            }
        }

        private FrmConfirmarVenda(decimal total, string formaPagamento, decimal? valorRecebido, decimal? troco)
        {
            int linhasDetalhe = 1 + (valorRecebido.HasValue ? 1 : 0) + (troco.HasValue ? 1 : 0);

            SuspendLayout();

            Text = "Finalizar venda";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            KeyPreview = true;
            Font = new Font("Segoe UI", 10F);
            BackColor = CorMenu;                 // aparece como uma borda fina de 1px
            Padding = new Padding(1);
            ClientSize = new Size(420, 2 + 64 + 16 + 106 + 34 * linhasDetalhe + 84 + 10);

            Panel corpo = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // ---------- CABEÇALHO ----------
            Panel cabecalho = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = CorMenu };
            Label lblTitulo = new Label
            {
                Text = "Confirmar venda",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0)
            };
            cabecalho.Controls.Add(lblTitulo);

            // ---------- DETALHES ----------
            Panel conteudo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 0)
            };

            TableLayoutPanel tabela = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = Padding.Empty
            };
            tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tabela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            AdicionarLinha(tabela, 28,
                Rotulo("Total da compra", new Font("Segoe UI", 10F), CorSuave, ContentAlignment.BottomLeft), null);

            AdicionarLinha(tabela, 58,
                Rotulo(total.ToString("C2", Br), new Font("Segoe UI", 26F, FontStyle.Bold),
                       Color.SeaGreen, ContentAlignment.MiddleLeft), null);

            Panel linha = new Panel
            {
                Height = 1,
                BackColor = Color.FromArgb(225, 229, 235),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = Padding.Empty
            };
            AdicionarLinha(tabela, 20, linha, null);

            Font fonteValor = new Font("Segoe UI", 10.5F, FontStyle.Bold);

            AdicionarLinha(tabela, 34,
                Rotulo("Pagamento", Font, CorSuave, ContentAlignment.MiddleLeft),
                Rotulo(formaPagamento, fonteValor, CorTexto, ContentAlignment.MiddleRight));

            if (valorRecebido.HasValue)
            {
                AdicionarLinha(tabela, 34,
                    Rotulo("Valor recebido", Font, CorSuave, ContentAlignment.MiddleLeft),
                    Rotulo(valorRecebido.Value.ToString("C2", Br), fonteValor, CorTexto, ContentAlignment.MiddleRight));
            }

            if (troco.HasValue)
            {
                AdicionarLinha(tabela, 34,
                    Rotulo("Troco", Font, CorSuave, ContentAlignment.MiddleLeft),
                    Rotulo(troco.Value.ToString("C2", Br), fonteValor, Color.RoyalBlue, ContentAlignment.MiddleRight));
            }

            conteudo.Controls.Add(tabela);

            // ---------- BOTÕES ----------
            Panel rodape = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 84,
                Padding = new Padding(24, 10, 24, 24)
            };

            TableLayoutPanel botoes = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };
            botoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            botoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            botoes.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Button btnCancelar = CriarBotao("Cancelar", Color.FromArgb(235, 238, 243), CorTexto);
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Margin = new Padding(0, 0, 6, 0);

            Button btnConfirmar = CriarBotao("Confirmar venda", Color.SeaGreen, Color.White);
            btnConfirmar.DialogResult = DialogResult.OK;
            btnConfirmar.Margin = new Padding(6, 0, 0, 0);

            botoes.Controls.Add(btnCancelar, 0, 0);
            botoes.Controls.Add(btnConfirmar, 1, 0);
            rodape.Controls.Add(botoes);

            AcceptButton = btnConfirmar;   // Enter confirma
            CancelButton = btnCancelar;    // Esc cancela

            // Ordem: o Fill primeiro, depois os Dock.
            corpo.Controls.Add(conteudo);
            corpo.Controls.Add(rodape);
            corpo.Controls.Add(cabecalho);
            Controls.Add(corpo);

            ResumeLayout(true);
        }

        private static void AdicionarLinha(TableLayoutPanel t, int altura, Control esquerda, Control direita)
        {
            int linha = t.RowStyles.Count;
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, altura));
            t.RowCount = linha + 1;

            t.Controls.Add(esquerda, 0, linha);
            if (direita == null)
            {
                t.SetColumnSpan(esquerda, 2);
            }
            else
            {
                t.Controls.Add(direita, 1, linha);
            }
        }

        private static Label Rotulo(string texto, Font fonte, Color cor, ContentAlignment alinhamento)
        {
            return new Label
            {
                Text = texto,
                Font = fonte,
                ForeColor = cor,
                TextAlign = alinhamento,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Margin = Padding.Empty
            };
        }

        private static Button CriarBotao(string texto, Color fundo, Color letra)
        {
            Button b = new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                BackColor = fundo,
                ForeColor = letra,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}
