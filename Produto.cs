namespace CorretorEAN
{
    public class Produto
    {
        public string Codigo { get; set; }
        public string Ean { get; set; }
        public string Descricao { get; set; }
        public string Reduzida { get; set; }
        public string Secao { get; set; }
        public string Seccod { get; set; }
        public string Grupo { get; set; }
        public string Grpcod { get; set; }
        public string Subgrupo { get; set; }
        public string Sgrcod { get; set; }
        public string NCM { get; set; }
        public string CEST { get; set; }

        public override string ToString()
        {
            return "Codigo : " + Codigo + "\tEAN : " + Ean + "\tProduto : " + Descricao +
                "\tSecao : " + Seccod + " - " + Secao +
                "\tGrupo : " + Grpcod + " - " + Grupo +
                "\tSubGrupo : " + Sgrcod + " - " + Subgrupo +
                "\tNCM : " + NCM + "\tCEST : " + CEST;
        }
    }
}
