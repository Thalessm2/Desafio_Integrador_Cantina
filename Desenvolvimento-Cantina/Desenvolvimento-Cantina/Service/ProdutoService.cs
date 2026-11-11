
using ProdutoDao;
using Models;
using System;
using System.Collections.Generic;

namespace Service
{
    public class ProdutoService
    {
        private readonly ProdutoDAO produtoDAO = new ProdutoDAO();

        public List<Produto> Listar()
        {
            return produtoDAO.Listar();
        }

        public void Cadastrar(Produto produto)
        {
            ValidacaoProduto(produto);
            produtoDAO.Cadastrar(produto);
        }

        public void AtualizarProdutos(Produto produto)
        {
            ValidacaoProduto(produto);

            if (produto.id_Produto <= 0)
            {
                throw new ArgumentException(
                    "Selecione um produto válido para atualizar.");
            }

            produtoDAO.AtualizarProdutos(produto);
        }

        private void ValidacaoProduto(Produto produto)
        {
            if (produto == null)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            if (string.IsNullOrWhiteSpace(produto.nomeProduto))
            {
                throw new ArgumentException(
                    "Informe o nome do produto.");
            }

            if (produto.precoProduto <= 0)
            {
                throw new ArgumentException(
                    "O preço deve ser maior que zero.");
            }

            if (produto.quantidadeProduto < 0)
            {
                throw new ArgumentException(
                    "O estoque não pode ser negativo.");
            }
        }
    }
}
