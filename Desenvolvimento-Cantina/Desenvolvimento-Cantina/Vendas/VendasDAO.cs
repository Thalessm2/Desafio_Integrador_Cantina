
using ConnectionClass;
using Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;

namespace VendaDao
{
    public class VendaDAO
    {
        public int FinalizarVenda(
            List<ItemVendaRegistro> itens,
            string formaPagamento)
        {
            if (itens == null || itens.Count == 0)
                throw new ArgumentException("O carrinho está vazio.");

            if (string.IsNullOrWhiteSpace(formaPagamento))
                throw new ArgumentException("Informe a forma de pagamento.");

            decimal total = itens.Sum(item => item.Subtotal);

            if (total <= 0)
                throw new InvalidOperationException("O total da venda é inválido.");

            var conexao = new Connection();

            using (MySqlConnection connection = conexao.CriarConexao())
            {
                connection.Open();

                using (MySqlTransaction transacao = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Baixa do estoque com validação de quantidade.
                        foreach (ItemVendaRegistro item in itens)
                        {
                            if (item.Quantidade <= 0)
                                throw new InvalidOperationException(
                                    "A quantidade deve ser maior que zero.");

                            if (item.PrecoUnitario <= 0)
                                throw new InvalidOperationException(
                                    "O preço do produto é inválido.");

                            string sqlEstoque = @"
                                UPDATE Produto
                                SET quantidadeProduto =
                                    quantidadeProduto - @quantidade
                                WHERE id_Produto = @idProduto
                                  AND quantidadeProduto >= @quantidade;";

                            using (var comando = new MySqlCommand(
                                sqlEstoque, connection, transacao))
                            {
                                comando.Parameters.AddWithValue(
                                    "@quantidade", item.Quantidade);

                                comando.Parameters.AddWithValue(
                                    "@idProduto", item.id_Produto);

                                int linhas = comando.ExecuteNonQuery();

                                if (linhas != 1)
                                {
                                    throw new InvalidOperationException(
                                        "Produto inexistente ou estoque insuficiente. " +
                                        "Atualize os produtos e tente novamente.");
                                }
                            }
                        }

                        // 2. Registra o pagamento.
                        string sqlPagamento = @"
                            INSERT INTO Pagamentos
                                (valor, statusPagamento, formaPagamento)
                            VALUES
                                (@valor, @status, @forma);

                            SELECT LAST_INSERT_ID();";

                        int idPagamento;

                        using (var comando = new MySqlCommand(
                            sqlPagamento, connection, transacao))
                        {
                            comando.Parameters.AddWithValue("@valor", total);
                            comando.Parameters.AddWithValue(
                                "@status", "Aprovado");
                            comando.Parameters.AddWithValue(
                                "@forma", formaPagamento);

                            idPagamento = Convert.ToInt32(
                                comando.ExecuteScalar());
                        }

                        // 3. Cria o pedido.
                        string sqlPedido = @"
                            INSERT INTO Pedidos
                                (data_pedido, valor_total, id_pagamento)
                            VALUES
                                (CURDATE(), @total, @idPagamento);

                            SELECT LAST_INSERT_ID();";

                        int idPedido;

                        using (var comando = new MySqlCommand(
                            sqlPedido, connection, transacao))
                        {
                            comando.Parameters.AddWithValue("@total", total);
                            comando.Parameters.AddWithValue(
                                "@idPagamento", idPagamento);

                            idPedido = Convert.ToInt32(
                                comando.ExecuteScalar());
                        }

                        // 4. Registra os itens do pedido.
                        string sqlItem = @"
                            INSERT INTO ItemPedido
                                (ID_Pedidos, id_Produto,
                                 Quantidade, preco_unitario)
                            VALUES
                                (@pedido, @produto, @quantidade, @preco);";

                        foreach (ItemVendaRegistro item in itens)
                        {
                            using (var comando = new MySqlCommand(
                                sqlItem, connection, transacao))
                            {
                                comando.Parameters.AddWithValue(
                                    "@pedido", idPedido);

                                comando.Parameters.AddWithValue(
                                    "@produto", item.id_Produto);

                                comando.Parameters.AddWithValue(
                                    "@quantidade", item.Quantidade);

                                comando.Parameters.AddWithValue(
                                    "@preco", item.PrecoUnitario);

                                comando.ExecuteNonQuery();
                            }
                        }

                        // Confirma todas as operações juntas.
                        transacao.Commit();

                        return idPedido;
                    }
                    catch
                    {
                        transacao.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
