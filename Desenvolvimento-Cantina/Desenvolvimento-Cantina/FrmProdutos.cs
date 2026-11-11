using Model;
using Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VendaDao;

namespace Desenvolvimento_Cantina
{
    public partial class FrmProdutos : Form
    {
        private readonly ProdutoService produtoService = new ProdutoService();
        private readonly VendaDAO vendaDAO = new VendaDAO();
        private List<Produto> produtos = new List<Produto>();
        private readonly List<ItemCarrinho> carrinho = new List<ItemCarrinho>();

        private int idSelecionado = 0;
        private bool atualizandoGrid = false;

        private sealed class ItemCarrinho
        {
            public Produto Produto { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }

            public decimal Subtotal
            {
                get { return PrecoUnitario * Quantidade; }
            }
        }

        public FrmProdutos()
        {
            InitializeComponent();

            CarregarProdutos();
            AtualizarCarrinho();
            TrocarPagina(false);
        }

        // NAVEGAÇÃO

        private void btnVendas_Click(object sender, EventArgs e)
        {
            TrocarPagina(false);
        }

        private void btnProdutos_Click(object sender, EventArgs e)
        {
            TrocarPagina(true);
            CarregarProdutos();
        }

        private void TrocarPagina(bool mostrarProdutos)
        {
            abas.SelectedTab = mostrarProdutos ? paginaProdutos : paginaVendas;

            btnVendas.BackColor = mostrarProdutos
                ? System.Drawing.Color.FromArgb(25, 35, 60)
                : System.Drawing.Color.FromArgb(45, 110, 220);

            btnProdutos.BackColor = mostrarProdutos
                ? System.Drawing.Color.FromArgb(45, 110, 220)
                : System.Drawing.Color.FromArgb(25, 35, 60);
        }

        // PRODUTOS

        private void CarregarProdutos()
        {
            try
            {
                atualizandoGrid = true;

                List<Produto> resultado = produtoService.Listar();
                produtos = resultado ?? new List<Produto>();

                PreencherGridProdutos(produtos);
                PreencherComboProdutos();
                AtualizarResumoEstoque();

                lblStatus.Text = "Produtos carregados: " + produtos.Count;
            }
            catch (Exception ex)
            {
                FrmMensagem.Erro(
                    this,
                    "Erro ao carregar produtos:\n" + ex.Message);
            }
            finally
            {
                atualizandoGrid = false;
            }
        }

        private void PreencherGridProdutos(IEnumerable<Produto> lista)
        {
            gridProdutos.Rows.Clear();

            foreach (Produto p in lista)
            {
                int linha = gridProdutos.Rows.Add(
                    p.id_Produto,
                    p.nomeProduto,
                    Convert.ToDecimal(p.precoProduto).ToString("C2"),
                    p.quantidadeProduto);

                gridProdutos.Rows[linha].Tag = p;
            }
        }

        private void PreencherComboProdutos()
        {
            int idAnterior = 0;

            Produto selecionado = cmbProdutoVenda.SelectedItem as Produto;
            if (selecionado != null)
                idAnterior = selecionado.id_Produto;

            cmbProdutoVenda.DataSource = null;
            cmbProdutoVenda.DisplayMember = "nomeProduto";

            List<Produto> disponiveis = produtos
                .Where(p => p.quantidadeProduto > 0)
                .OrderBy(p => p.nomeProduto)
                .ToList();

            cmbProdutoVenda.DataSource = disponiveis;

            if (idAnterior > 0)
            {
                for (int i = 0; i < disponiveis.Count; i++)
                {
                    if (disponiveis[i].id_Produto == idAnterior)
                    {
                        cmbProdutoVenda.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void AtualizarResumoEstoque()
        {
            lblTotalProdutos.Text =
                "Produtos cadastrados: " + produtos.Count;

            lblTotalEstoque.Text =
                "Unidades em estoque: " +
                produtos.Sum(p => p.quantidadeProduto);

            lblEstoqueZero.Text =
                "Sem estoque: " +
                produtos.Count(p => p.quantidadeProduto <= 0);
        }

        private bool ValidarProduto()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                FrmMensagem.Aviso(this, "Informe o nome do produto.");
                txtNome.Focus();
                return false;
            }

            if (txtPreco.Value <= 0)
            {
                FrmMensagem.Aviso(this, "O preço deve ser maior que zero.");
                txtPreco.Focus();
                return false;
            }

            return true;
        }

        private Produto LerFormulario()
        {
            return new Produto
            {
                id_Produto = idSelecionado,
                nomeProduto = txtNome.Text.Trim(),
                precoProduto = txtPreco.Value,
                quantidadeProduto = Convert.ToInt32(txtQuantidade.Value)
            };
        }

        private void BtnCadastrar_Click(object sender, EventArgs e)
        {
            if (!ValidarProduto())
                return;

            try
            {
                Produto produto = LerFormulario();
                produto.id_Produto = 0;

                produtoService.Cadastrar(produto);

                FrmMensagem.Sucesso(this, "Produto cadastrado com sucesso!", "Cadastro");

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                FrmMensagem.Erro(
                    this,
                    "Erro ao cadastrar produto:\n" + ex.Message);
            }
        }

        private void BtnAtualizar_Click(object sender, EventArgs e)
        {
            if (idSelecionado <= 0)
            {
                FrmMensagem.Aviso(this, "Selecione um produto na tabela.");
                return;
            }

            if (!ValidarProduto())
                return;

            try
            {
                Produto produto = LerFormulario();
                produtoService.AtualizarProdutos(produto);

                FrmMensagem.Sucesso(this, "Produto atualizado com sucesso!", "Atualização");

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                FrmMensagem.Erro(
                    this,
                    "Erro ao atualizar produto:\n" + ex.Message);
            }
        }

        private void LimparCampos()
        {
            idSelecionado = 0;

            txtNome.Clear();
            txtPreco.Value = 0;
            txtQuantidade.Value = 0;

            gridProdutos.ClearSelection();

            btnCadastrar.Enabled = true;
            btnAtualizar.Enabled = false;
        }

        private void TxtPesquisa_TextChanged(object sender, EventArgs e)
        {
            string termo = txtPesquisa.Text.Trim();

            IEnumerable<Produto> filtrados = produtos.Where(p =>
                p.nomeProduto.IndexOf(
                    termo, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.id_Produto.ToString().Contains(termo));

            atualizandoGrid = true;
            PreencherGridProdutos(filtrados);
            atualizandoGrid = false;
        }

        private void GridProdutos_SelectionChanged(object sender, EventArgs e)
        {
            if (atualizandoGrid || gridProdutos.CurrentRow == null)
                return;

            Produto produto = gridProdutos.CurrentRow.Tag as Produto;

            if (produto == null)
                return;

            idSelecionado = produto.id_Produto;
            txtNome.Text = produto.nomeProduto;

            txtPreco.Value = Math.Max(
                txtPreco.Minimum,
                Math.Min(txtPreco.Maximum,
                    Convert.ToDecimal(produto.precoProduto)));

            txtQuantidade.Value = Math.Max(
                txtQuantidade.Minimum,
                Math.Min(txtQuantidade.Maximum,
                    Convert.ToDecimal(produto.quantidadeProduto)));

            btnCadastrar.Enabled = false;
            btnAtualizar.Enabled = true;
        }

        // CARRINHO

        private void BtnAdicionarCarrinho_Click(object sender, EventArgs e)
        {
            Produto produto = cmbProdutoVenda.SelectedItem as Produto;

            if (produto == null)
            {
                FrmMensagem.Aviso(this, "Selecione um produto com estoque disponível.");
                return;
            }

            int quantidade = Convert.ToInt32(numQuantidadeVenda.Value);

            ItemCarrinho existente = carrinho.FirstOrDefault(
                item => item.Produto.id_Produto == produto.id_Produto);

            int quantidadeNoCarrinho =
                existente == null ? 0 : existente.Quantidade;

            if (quantidade + quantidadeNoCarrinho > produto.quantidadeProduto)
            {
                FrmMensagem.Aviso(
                    this,
                    "Estoque insuficiente. Disponível: " +
                    (produto.quantidadeProduto - quantidadeNoCarrinho),
                    "Estoque");
                return;
            }

            if (existente == null)
            {
                carrinho.Add(new ItemCarrinho
                {
                    Produto = produto,
                    Quantidade = quantidade,
                    PrecoUnitario = Convert.ToDecimal(produto.precoProduto)
                });
            }
            else
            {
                existente.Quantidade += quantidade;
            }

            AtualizarCarrinho();
            numQuantidadeVenda.Value = 1;
        }

        private void BtnRemoverCarrinho_Click(object sender, EventArgs e)
        {
            if (gridCarrinho.CurrentRow == null)
            {
                FrmMensagem.Aviso(this, "Selecione um item do carrinho.");
                return;
            }

            int indice = gridCarrinho.CurrentRow.Index;

            if (indice >= 0 && indice < carrinho.Count)
            {
                carrinho.RemoveAt(indice);
                AtualizarCarrinho();
            }
        }

        private void AtualizarCarrinho()
        {
            gridCarrinho.Rows.Clear();

            foreach (ItemCarrinho item in carrinho)
            {
                gridCarrinho.Rows.Add(
                    item.Produto.nomeProduto,
                    item.Quantidade,
                    item.PrecoUnitario.ToString("C2"),
                    item.Subtotal.ToString("C2"));
            }

            lblTotalVenda.Text =
                "Total da compra: " + CalcularTotal().ToString("C2");

            CalcularTroco();
        }

        private decimal CalcularTotal()
        {
            return carrinho.Sum(item => item.Subtotal);
        }

        // PAGAMENTO

        private void cmbFormaPagamento_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            bool dinheiro = cmbFormaPagamento.SelectedItem != null &&
                cmbFormaPagamento.SelectedItem.ToString() == "Dinheiro";

            lblValorRecebido.Visible = dinheiro;
            numValorRecebido.Visible = dinheiro;
            lblTroco.Visible = dinheiro;

            CalcularTroco();
        }

        private void numValorRecebido_ValueChanged(
            object sender, EventArgs e)
        {
            CalcularTroco();
        }

        private void CalcularTroco()
        {
            if (cmbFormaPagamento == null ||
                numValorRecebido == null ||
                lblTroco == null)
                return;

            if (cmbFormaPagamento.SelectedItem == null)
            {
                lblTroco.Text = "Selecione o pagamento";
                return;
            }

            if (cmbFormaPagamento.SelectedItem.ToString() != "Dinheiro")
            {
                lblTroco.Text = "Pagamento selecionado";
                return;
            }

            decimal total = CalcularTotal();
            decimal recebido = numValorRecebido.Value;

            if (recebido < total)
            {
                lblTroco.Text = "Valor insuficiente";
            }
            else
            {
                lblTroco.Text = "Troco: " + (recebido - total).ToString("C2");
            }
        }


        private void BtnFinalizarVenda_Click(object sender, EventArgs e)
        {
            if (carrinho.Count == 0)
            {
                FrmMensagem.Aviso(this, "Adicione produtos ao carrinho.");
                return;
            }

            if (cmbFormaPagamento.SelectedItem == null)
            {
                FrmMensagem.Aviso(this, "Selecione a forma de pagamento.");
                return;
            }

            string pagamento = cmbFormaPagamento.SelectedItem.ToString();
            decimal total = CalcularTotal();

            if (pagamento == "Dinheiro" &&
                numValorRecebido.Value < total)
            {
                FrmMensagem.Aviso(this, "O valor recebido é insuficiente.");
                return;
            }

            decimal troco = pagamento == "Dinheiro"
                ? numValorRecebido.Value - total
                : 0m;

            // ===== CONFIRMAÇÃO (janela personalizada) =====
            bool pagouEmDinheiro = pagamento == "Dinheiro";

            bool confirmou = FrmConfirmarVenda.Mostrar(
                this,
                total,
                pagamento,
                pagouEmDinheiro ? numValorRecebido.Value : (decimal?)null,
                pagouEmDinheiro ? troco : (decimal?)null);

            if (!confirmou)
                return;
            // ===============================================

            try
            {
                List<ItemVendaRegistro> itens = carrinho
                    .Select(item => new ItemVendaRegistro
                    {
                        id_Produto = item.Produto.id_Produto,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario
                    })
                    .ToList();

                int idPedido = vendaDAO.FinalizarVenda(itens, pagamento);

                carrinho.Clear();
                numValorRecebido.Value = 0;

                CarregarProdutos();
                AtualizarCarrinho();

                FrmMensagem.Sucesso(
                    this,
                    "Venda registrada com sucesso!\n\n" +
                    "Pedido: " + idPedido + "\n" +
                    "Total: " + total.ToString("C2") + "\n" +
                    "Pagamento: " + pagamento +
                    (pagamento == "Dinheiro"
                        ? "\nTroco: " + troco.ToString("C2")
                        : ""),
                    "Venda concluída");
            }
            catch (Exception ex)
            {
                FrmMensagem.Erro(
                    this,
                    "Não foi possível finalizar a venda.\n\n" + ex.Message);

                CarregarProdutos();
            }
        }


        private void btnRecarregar_Click(object sender, EventArgs e)
        {
            CarregarProdutos();
        }
    }
}