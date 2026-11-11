using System;
using System.Drawing;
using System.Windows.Forms;

namespace Desenvolvimento_Cantina
{
    partial class FrmProdutos
    {
        private System.ComponentModel.IContainer components = null;

        // ===== CAMPOS (mesmos nomes de antes, o code-behind continua funcionando) =====
        private Panel pnlMenu, pnlCabecalho;
        private Label lblTitulo, lblSubtitulo;
        private Button btnVendas, btnProdutos;

        private TabControl abas;
        private TabPage paginaVendas, paginaProdutos;

        private ComboBox cmbProdutoVenda, cmbFormaPagamento;
        private NumericUpDown numQuantidadeVenda, numValorRecebido;
        private DataGridView gridCarrinho;
        private Label lblTotalVenda, lblFormaPagamento;
        private Label lblValorRecebido, lblTroco;
        private Button btnAdicionarCarrinho, btnRemoverCarrinho, btnFinalizarVenda;

        private TextBox txtNome, txtPesquisa;
        private NumericUpDown txtPreco, txtQuantidade;
        private DataGridView gridProdutos;
        private Button btnCadastrar, btnAtualizar, btnLimpar, btnRecarregar;
        private Label lblStatus, lblTotalProdutos, lblTotalEstoque, lblEstoqueZero;

        // ===== CORES =====
        private static readonly Color CorFundo = Color.FromArgb(245, 247, 250);
        private static readonly Color CorMenu = Color.FromArgb(25, 35, 60);

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Text = "Sistema de Cantina";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1200, 750);
            MinimumSize = new Size(1000, 650);
            BackColor = CorFundo;
            Font = new Font("Segoe UI", 10F);

            ConstruirMenu();
            ConstruirCabecalho();
            ConstruirAbas();
            ConstruirPaginaVendas();
            ConstruirPaginaProdutos();

            // Ordem importa no Dock: o controle Fill (abas) precisa ser o primeiro da coleção.
            Controls.Add(abas);
            Controls.Add(pnlCabecalho);
            Controls.Add(pnlMenu);

            // O índice inicial dispara o evento de pagamento.
            if (cmbFormaPagamento.Items.Count > 0)
                cmbFormaPagamento.SelectedIndex = 0;

            ResumeLayout(true);
            WindowState = FormWindowState.Maximized;
        }

        // =====================================================================
        //  MENU LATERAL
        // =====================================================================
        private void ConstruirMenu()
        {
            pnlMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = CorMenu
            };

            Label lblMarca = new Label
            {
                Text = "CANTINA",
                Dock = DockStyle.Top,
                Height = 100,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnVendas = CriarBotaoMenu("Vendas");
            btnVendas.Click += btnVendas_Click;

            btnProdutos = CriarBotaoMenu("Produtos");
            btnProdutos.Click += btnProdutos_Click;

            // Ordem inversa: o último adicionado fica no topo.
            pnlMenu.Controls.Add(btnProdutos);
            pnlMenu.Controls.Add(btnVendas);
            pnlMenu.Controls.Add(lblMarca);
        }

        // =====================================================================
        //  CABEÇALHO
        // =====================================================================
        private void ConstruirCabecalho()
        {
            pnlCabecalho = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White
            };

            lblTitulo = new Label
            {
                Text = "Sistema de Cantina",
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                Location = new Point(25, 10),
                AutoSize = true
            };

            lblSubtitulo = new Label
            {
                Text = "Controle de produtos, estoque e vendas",
                Location = new Point(28, 55),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            pnlCabecalho.Controls.Add(lblTitulo);
            pnlCabecalho.Controls.Add(lblSubtitulo);
        }

        // =====================================================================
        //  ABAS (sem a faixa de títulos)
        // =====================================================================
        private void ConstruirAbas()
        {
            abas = new AbasSemCabecalho
            {
                Dock = DockStyle.Fill,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed
            };

            paginaVendas = new TabPage("Vendas")
            {
                UseVisualStyleBackColor = false,
                BackColor = CorFundo,
                Padding = new Padding(20)
            };

            paginaProdutos = new TabPage("Produtos")
            {
                UseVisualStyleBackColor = false,
                BackColor = CorFundo,
                Padding = new Padding(20)
            };

            abas.TabPages.Add(paginaVendas);
            abas.TabPages.Add(paginaProdutos);
        }

        // =====================================================================
        //  PÁGINA DE VENDAS
        // =====================================================================
        private void ConstruirPaginaVendas()
        {
            // Layout: [ cartão esquerdo | espaço | cartão direito (pagamento) ]
            TableLayoutPanel layout = CriarLayoutDuasColunas(380);

            // ---------- CARTÃO ESQUERDO ----------
            Panel cartaoEsq = CriarCartao();

            Label lblVendaTitulo = CriarRotulo("Nova venda");
            lblVendaTitulo.Dock = DockStyle.Top;
            lblVendaTitulo.Height = 55;
            lblVendaTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);

            // Linha de seleção: produto | quantidade | botão
            TableLayoutPanel tlpSelecao = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 85,
                ColumnCount = 3,
                RowCount = 2,
                Margin = Padding.Empty
            };
            tlpSelecao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpSelecao.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tlpSelecao.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230F));
            tlpSelecao.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tlpSelecao.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            Label lblProduto = CriarRotuloCampo("Produto");
            Label lblQuantidadeVenda = CriarRotuloCampo("Quantidade");

            cmbProdutoVenda = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 15, 0)
            };

            numQuantidadeVenda = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 100000,
                Value = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 15, 0)
            };

            btnAdicionarCarrinho = CriarBotao("Adicionar ao carrinho", Color.SeaGreen);
            btnAdicionarCarrinho.Height = 34;
            btnAdicionarCarrinho.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnAdicionarCarrinho.Margin = Padding.Empty;
            btnAdicionarCarrinho.Click += BtnAdicionarCarrinho_Click;

            tlpSelecao.Controls.Add(lblProduto, 0, 0);
            tlpSelecao.Controls.Add(lblQuantidadeVenda, 1, 0);
            tlpSelecao.Controls.Add(cmbProdutoVenda, 0, 1);
            tlpSelecao.Controls.Add(numQuantidadeVenda, 1, 1);
            tlpSelecao.Controls.Add(btnAdicionarCarrinho, 2, 1);

            // Rodapé com o botão de remover
            Panel pnlRodape = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };

            btnRemoverCarrinho = CriarBotao("Remover item", Color.Firebrick);
            btnRemoverCarrinho.Size = new Size(160, 40);
            btnRemoverCarrinho.Location = new Point(0, 15);
            btnRemoverCarrinho.Click += BtnRemoverCarrinho_Click;
            pnlRodape.Controls.Add(btnRemoverCarrinho);

            // Grid do carrinho
            gridCarrinho = new DataGridView();
            gridCarrinho.Columns.Add("Produto", "Produto");
            gridCarrinho.Columns.Add("Quantidade", "Quantidade");
            gridCarrinho.Columns.Add("Preco", "Preço unitário");
            gridCarrinho.Columns.Add("Subtotal", "Subtotal");
            EstilizarGrid(gridCarrinho);

            // Ordem de adição: Fill primeiro, depois os docks (último adicionado = mais externo).
            cartaoEsq.Controls.Add(gridCarrinho);
            cartaoEsq.Controls.Add(pnlRodape);
            cartaoEsq.Controls.Add(tlpSelecao);
            cartaoEsq.Controls.Add(lblVendaTitulo);

            // ---------- CARTÃO DIREITO (PAGAMENTO) ----------
            Panel cartaoDir = CriarCartao();
            TableLayoutPanel colPag = CriarColuna();

            Label lblPagTitulo = CriarRotulo("Pagamento");
            lblPagTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            lblTotalVenda = CriarRotulo("Total da compra: R$ 0,00");
            lblTotalVenda.Font = new Font("Segoe UI", 15F, FontStyle.Bold);

            lblFormaPagamento = CriarRotuloCampo("Forma de pagamento");

            cmbFormaPagamento = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 0, 0)
            };
            cmbFormaPagamento.Items.AddRange(new object[]
            {
                "Dinheiro", "Pix", "Cartão de débito", "Cartão de crédito"
            });
            cmbFormaPagamento.SelectedIndexChanged += cmbFormaPagamento_SelectedIndexChanged;

            lblValorRecebido = CriarRotuloCampo("Valor recebido (R$)");

            numValorRecebido = new NumericUpDown
            {
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000,
                ThousandsSeparator = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 0, 0)
            };
            numValorRecebido.ValueChanged += numValorRecebido_ValueChanged;

            lblTroco = CriarRotulo("Troco: R$ 0,00");
            lblTroco.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnFinalizarVenda = CriarBotao("Finalizar compra", Color.SeaGreen);
            btnFinalizarVenda.Height = 45;
            btnFinalizarVenda.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFinalizarVenda.Margin = new Padding(0, 10, 0, 0);
            btnFinalizarVenda.Click += BtnFinalizarVenda_Click;

            AdicionarLinha(colPag, lblPagTitulo, 40);
            AdicionarLinha(colPag, lblTotalVenda, 70);
            AdicionarLinha(colPag, lblFormaPagamento, 30);
            AdicionarLinha(colPag, cmbFormaPagamento, 42);
            AdicionarLinha(colPag, lblValorRecebido, 30);
            AdicionarLinha(colPag, numValorRecebido, 42);
            AdicionarLinha(colPag, lblTroco, 45);
            AdicionarLinha(colPag, btnFinalizarVenda, 60);
            AdicionarLinha(colPag, null, 0); // espaço restante

            cartaoDir.Controls.Add(colPag);

            layout.Controls.Add(cartaoEsq, 0, 0);
            layout.Controls.Add(cartaoDir, 2, 0);
            paginaVendas.Controls.Add(layout);
        }

        // =====================================================================
        //  PÁGINA DE PRODUTOS
        // =====================================================================
        private void ConstruirPaginaProdutos()
        {
            TableLayoutPanel layout = CriarLayoutDuasColunas(320);

            // ---------- CARTÃO: CADASTRO ----------
            Panel pnlDados = CriarCartao();

            TableLayoutPanel tlpCampos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                Margin = Padding.Empty
            };
            tlpCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tlpCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tlpCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tlpCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlpCampos.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));

            Label lblDados = CriarRotulo("Cadastro de produto");
            lblDados.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            Label lblNome = CriarRotuloCampo("Nome do produto");
            Label lblPreco = CriarRotuloCampo("Preço (R$)");
            Label lblQuantidade = CriarRotuloCampo("Quantidade");

            txtNome = new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 15, 0)
            };

            txtPreco = new NumericUpDown
            {
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 15, 0)
            };

            txtQuantidade = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 1000000,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 2, 0, 0)
            };

            FlowLayoutPanel flwBotoes = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            btnCadastrar = CriarBotao("Cadastrar", Color.SeaGreen);
            btnCadastrar.Size = new Size(130, 38);
            btnCadastrar.Margin = new Padding(0, 10, 10, 0);
            btnCadastrar.Click += BtnCadastrar_Click;

            btnAtualizar = CriarBotao("Atualizar", Color.RoyalBlue);
            btnAtualizar.Size = new Size(130, 38);
            btnAtualizar.Margin = new Padding(0, 10, 10, 0);
            btnAtualizar.Click += BtnAtualizar_Click;
            btnAtualizar.Enabled = false;

            btnLimpar = CriarBotao("Limpar", Color.Gray);
            btnLimpar.Size = new Size(110, 38);
            btnLimpar.Margin = new Padding(0, 10, 0, 0);
            btnLimpar.Click += (s, e) => LimparCampos();

            flwBotoes.Controls.Add(btnCadastrar);
            flwBotoes.Controls.Add(btnAtualizar);
            flwBotoes.Controls.Add(btnLimpar);

            tlpCampos.Controls.Add(lblDados, 0, 0);
            tlpCampos.SetColumnSpan(lblDados, 3);
            tlpCampos.Controls.Add(lblNome, 0, 1);
            tlpCampos.Controls.Add(lblPreco, 1, 1);
            tlpCampos.Controls.Add(lblQuantidade, 2, 1);
            tlpCampos.Controls.Add(txtNome, 0, 2);
            tlpCampos.Controls.Add(txtPreco, 1, 2);
            tlpCampos.Controls.Add(txtQuantidade, 2, 2);
            tlpCampos.Controls.Add(flwBotoes, 0, 3);
            tlpCampos.SetColumnSpan(flwBotoes, 3);

            pnlDados.Controls.Add(tlpCampos);

            // ---------- CARTÃO: LISTA ----------
            Panel pnlLista = CriarCartao();

            Label lblLista = CriarRotulo("Produtos cadastrados");
            lblLista.Dock = DockStyle.Top;
            lblLista.Height = 38;
            lblLista.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            Panel pnlBusca = new Panel
            {
                Dock = DockStyle.Top,
                Height = 62
            };

            Label lblPesquisa = new Label
            {
                Text = "Pesquisar produto",
                Location = new Point(0, 0),
                AutoSize = true,
                ForeColor = Color.DimGray
            };

            txtPesquisa = new TextBox
            {
                Location = new Point(0, 25),
                Width = 350
            };
            txtPesquisa.TextChanged += TxtPesquisa_TextChanged;

            btnRecarregar = CriarBotao("Recarregar", Color.Gray);
            btnRecarregar.Size = new Size(130, 30);
            btnRecarregar.Location = new Point(365, 23);
            btnRecarregar.Click += (s, e) => CarregarProdutos();

            pnlBusca.Controls.Add(lblPesquisa);
            pnlBusca.Controls.Add(txtPesquisa);
            pnlBusca.Controls.Add(btnRecarregar);

            gridProdutos = new DataGridView();
            gridProdutos.Columns.Add("Id", "Código");
            gridProdutos.Columns.Add("Nome", "Produto");
            gridProdutos.Columns.Add("Preco", "Preço");
            gridProdutos.Columns.Add("Quantidade", "Estoque");
            EstilizarGrid(gridProdutos);
            gridProdutos.SelectionChanged += GridProdutos_SelectionChanged;

            pnlLista.Controls.Add(gridProdutos);
            pnlLista.Controls.Add(pnlBusca);
            pnlLista.Controls.Add(lblLista);

            // ---------- CARTÃO: RESUMO ----------
            Panel pnlResumo = CriarCartao();
            TableLayoutPanel colResumo = CriarColuna();

            Label lblResumo = CriarRotulo("Resumo do estoque");
            lblResumo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            lblTotalProdutos = CriarRotulo("");
            lblTotalEstoque = CriarRotulo("");
            lblEstoqueZero = CriarRotulo("");

            AdicionarLinha(colResumo, lblResumo, 40);
            AdicionarLinha(colResumo, lblTotalProdutos, 38);
            AdicionarLinha(colResumo, lblTotalEstoque, 38);
            AdicionarLinha(colResumo, lblEstoqueZero, 38);
            pnlResumo.Controls.Add(colResumo);

            // ---------- CARTÃO: INFORMAÇÕES ----------
            Panel pnlInformacoes = CriarCartao();
            TableLayoutPanel colInfo = CriarColuna();

            Label lblInfoTitulo = CriarRotulo("Informações");
            lblInfoTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            lblStatus = CriarRotulo("Sistema pronto.");
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.TextAlign = ContentAlignment.TopLeft;

            AdicionarLinha(colInfo, lblInfoTitulo, 40);
            AdicionarLinha(colInfo, lblStatus, 0); // ocupa o restante
            pnlInformacoes.Controls.Add(colInfo);

            // ---------- MONTAGEM DAS DUAS COLUNAS ----------
            TableLayoutPanel colEsq = CriarColuna();
            AdicionarLinha(colEsq, pnlDados, 220);
            AdicionarLinha(colEsq, null, 20);
            AdicionarLinha(colEsq, pnlLista, 0);

            TableLayoutPanel colDir = CriarColuna();
            AdicionarLinha(colDir, pnlResumo, 200);
            AdicionarLinha(colDir, null, 20);
            AdicionarLinha(colDir, pnlInformacoes, 170);
            AdicionarLinha(colDir, null, 0);

            layout.Controls.Add(colEsq, 0, 0);
            layout.Controls.Add(colDir, 2, 0);
            paginaProdutos.Controls.Add(layout);
        }

        // =====================================================================
        //  HELPERS DE LAYOUT
        // =====================================================================

        /// <summary>Duas colunas: conteúdo flexível | espaço de 20px | coluna fixa à direita.</summary>
        private TableLayoutPanel CriarLayoutDuasColunas(int larguraDireita)
        {
            TableLayoutPanel t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, larguraDireita));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            return t;
        }

        /// <summary>Tabela de uma coluna onde as linhas são adicionadas com AdicionarLinha.</summary>
        private TableLayoutPanel CriarColuna()
        {
            TableLayoutPanel t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 0,
                Margin = Padding.Empty
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            return t;
        }

        /// <summary>Adiciona uma linha. altura &lt;= 0 significa "ocupar o espaço que sobrar".</summary>
        private void AdicionarLinha(TableLayoutPanel tabela, Control controle, int altura)
        {
            int linha = tabela.RowStyles.Count;
            tabela.RowStyles.Add(altura > 0
                ? new RowStyle(SizeType.Absolute, altura)
                : new RowStyle(SizeType.Percent, 100F));
            tabela.RowCount = linha + 1;

            if (controle != null)
                tabela.Controls.Add(controle, 0, linha);
        }

        private Panel CriarCartao()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Margin = Padding.Empty
            };
        }

        private void EstilizarGrid(DataGridView g)
        {
            g.Dock = DockStyle.Fill;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 38;
            g.ColumnHeadersDefaultCellStyle.BackColor = CorMenu;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = CorMenu;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 250);
            g.DefaultCellStyle.SelectionForeColor = Color.Black;
            g.RowTemplate.Height = 32;
        }

        // =====================================================================
        //  HELPERS DE CONTROLES
        // =====================================================================
        private Button CriarBotaoMenu(string texto)
        {
            Button b = new Button
            {
                Text = "   " + texto,
                Dock = DockStyle.Top,
                Height = 55,
                FlatStyle = FlatStyle.Flat,
                BackColor = CorMenu,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 60, 100);
            b.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 80, 130);
            return b;
        }

        private Button CriarBotao(string texto, Color cor)
        {
            Button b = new Button
            {
                Text = texto,
                BackColor = cor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        /// <summary>Rótulo que preenche a célula, alinhado à esquerda e centralizado na vertical.</summary>
        private Label CriarRotulo(string texto)
        {
            return new Label
            {
                Text = texto,
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
        }

        /// <summary>Rótulo pequeno que fica logo acima de um campo (alinhado embaixo).</summary>
        private Label CriarRotuloCampo(string texto)
        {
            Label l = CriarRotulo(texto);
            l.TextAlign = ContentAlignment.BottomLeft;
            return l;
        }
    }

    /// <summary>
    /// TabControl sem a faixa das abas: a página ocupa a área inteira
    /// e a troca de aba é feita pelos botões do menu lateral.
    /// </summary>
    internal class AbasSemCabecalho : TabControl
    {
        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT && !DesignMode)
            {
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }
    }
}