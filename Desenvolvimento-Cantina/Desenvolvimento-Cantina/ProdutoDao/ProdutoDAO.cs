
using ConnectionClass;
using Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ProdutoDao
{
    public class ProdutoDAO
    {
        public List<Produto> Listar()
        {
            var produtos = new List<Produto>();
            var conexao = new Connection();

            using (MySqlConnection connection = conexao.CriarConexao())
            {
                connection.Open();

                string sql = @"
                    SELECT id_Produto,
                           nomeProduto,
                           precoProduto,
                           quantidadeProduto,
                           PesoProduto
                    FROM Produto;";

                using var comando = new MySqlCommand(sql, connection);
                using var reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    var produto = new Produto
                    {
                        id_Produto = reader.GetInt32("id_Produto"),
                        nomeProduto = reader.GetString("nomeProduto"),
                        precoProduto = reader.GetDecimal("precoProduto"),
                        quantidadeProduto = reader.GetInt32("quantidadeProduto")
                    };

                    produtos.Add(produto);
                }
            }

            return produtos;
        }

        public void Cadastrar(Produto produto)
        {
            var conexao = new Connection();

            using (MySqlConnection connection = conexao.CriarConexao())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Produto
                        (nomeProduto, precoProduto, quantidadeProduto)
                    VALUES
                        (@nomeProduto, @precoProduto, @quantidadeProduto);";

                using var comando = new MySqlCommand(sql, connection);

                comando.Parameters.AddWithValue(
                    "@nomeProduto", produto.nomeProduto);

                comando.Parameters.AddWithValue(
                    "@precoProduto", produto.precoProduto);

                comando.Parameters.AddWithValue(
                    "@quantidadeProduto", produto.quantidadeProduto);

                comando.ExecuteNonQuery();
            }
        }

        public void AtualizarProdutos(Produto produto)
        {
            var conexao = new Connection();

            using (MySqlConnection connection = conexao.CriarConexao())
            {
                connection.Open();

                string sql = @"
                    UPDATE Produto
                    SET nomeProduto = @nomeProduto,
                        precoProduto = @precoProduto,
                        quantidadeProduto = @quantidadeProduto
                    WHERE id_Produto = @id_Produto;";

                using var comando = new MySqlCommand(sql, connection);

                comando.Parameters.AddWithValue(
                    "@id_Produto", produto.id_Produto);

                comando.Parameters.AddWithValue(
                    "@nomeProduto", produto.nomeProduto);

                comando.Parameters.AddWithValue(
                    "@precoProduto", produto.precoProduto);

                comando.Parameters.AddWithValue(
                    "@quantidadeProduto", produto.quantidadeProduto);

                int linhasAfetadas = comando.ExecuteNonQuery();

                if (linhasAfetadas == 0)
                {
                    throw new InvalidOperationException(
                        "Produto não encontrado ou nenhuma alteração realizada.");
                }
            }
        }
    }
}
