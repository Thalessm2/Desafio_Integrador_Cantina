using ConnectionClass;


namespace Models;

public class Produto
{
    public int id_Produto {  get; set; }
    public string nomeProduto { get; set; } = string.Empty;
    public decimal precoProduto { get; set; }
    public int quantidadeProduto { get; set; }

}
