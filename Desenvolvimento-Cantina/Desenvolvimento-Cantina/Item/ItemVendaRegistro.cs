namespace Model
{
    public class ItemVendaRegistro
    {
        public int id_Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        public decimal Subtotal
        {
            get { return PrecoUnitario * Quantidade; }
        }
    }
}
