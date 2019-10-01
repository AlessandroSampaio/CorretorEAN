using System;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data;

namespace CorretorEAN
{
    internal static class ConexaoFirebird
    {
        private readonly static FbConnection connectionSysPDV = new FbConnection(@"DataSource=localhost; Database=C:\SysPDV\SysPDV_SRV.fdb; User=sysdba; Password=masterkey");
        private readonly static FbConnection connectionModel = new FbConnection(@"DataSource=localhost; Database=C:\SysPDV\baseDirectory.fdb; User=sysdba; Password=masterkey");

        public static List<Produto> GetListProdutosSysPDV(int opt)
        {
            FbConnection connection;
            if(opt == 1)
            {
                connection = connectionSysPDV;
            }
            else
            {
                connection = connectionModel;
            }
            List<Produto> produtos = new List<Produto>();
            try
            {
                connection.Open();
                FbCommand fbCommand = new FbCommand(@"select P.PROCOD as CODIGO, PA.PROCODAUX as EAN, P.PRODES as DESCRICAO, P.PRODESRDZ as REDUZIDA, "+
                    "S.SECDES as SECAO, P.SECCOD as SECCOD, G.GRPDES as GRUPO, P.GRPCOD as GRPCOD, SG.SGRDES as SUBGRUPO, P.SGRCOD as SUBGRUPO, P.PRONCM as NCM, "+
                    "P.PROCEST as CEST from PRODUTO P left outer join PRODUTOAUX PA on PA.PROCOD=P.PROCOD " +
                    "left outer join SECAO S on S.SECCOD=P.SECCOD "+
                    "left outer join GRUPO G on G.SECCOD = P.SECCOD and G.GRPCOD = P.GRPCOD "+
                    "left outer join SUBGRUPO SG on SG.SECCOD = P.SECCOD and SG.GRPCOD = P.GRPCOD AND SG.SGRCOD = P.SGRCOD order by prodes", connectionSysPDV);
                using (FbDataAdapter fbDataAdapter = new FbDataAdapter(fbCommand))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        fbDataAdapter.Fill(dataTable);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            produtos.Add(new Produto()
                            {
                                Codigo = dataTable.Rows[i]["CODIGO"].ToString(),
                                Ean = dataTable.Rows[i]["EAN"].ToString(),
                                Descricao = dataTable.Rows[i]["DESCRICAO"].ToString(),
                                Reduzida = dataTable.Rows[i]["REDUZIDA"].ToString(),
                                Secao = dataTable.Rows[i]["SECAO"].ToString(),
                                Seccod = dataTable.Rows[i]["SECCOD"].ToString(),
                                Grupo = dataTable.Rows[i]["GRUPO"].ToString(),
                                Grpcod = dataTable.Rows[i]["GRPCOD"].ToString(),
                                Subgrupo = dataTable.Rows[i]["SUBGRUPO"].ToString(),
                                Sgrcod = dataTable.Rows[i]["REDUZIDA"].ToString(),
                                NCM = dataTable.Rows[i]["NCM"].ToString(),
                                CEST = dataTable.Rows[i]["CEST"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (FbException erro)
            {
                throw erro;
            }
            finally
            {
                connection.Close();
            }
            return produtos;
        }

        
        public static int UpdateProdutosSysPDV(List<Produto> produtos, string secao, string grupo, string subgrupo, string ncm, string cest)
        {
            int counter = 0;
            if(secao.Equals("") || grupo.Equals("") || subgrupo.Equals(""))
            {
                throw new ArgumentNullException("Campos obrigatórios não foram preenchidos!");
            }
            try
            {
                connectionSysPDV.Open();
                for (int i = 0; i < produtos.Count; i++)
                {
                    FbCommand command = new FbCommand("update produto set seccod=@seccod, grpcod=@grpcod, sgrcod=@sgrcod, proncm=@proncm," +
                        " gencodigo=@gencodigo, procest=@procest where procod=@procod", connectionSysPDV);
                    command.Parameters.Add("seccod", FbDbType.VarChar).Value = secao;
                    command.Parameters.Add("grpcod", FbDbType.VarChar).Value = grupo;
                    command.Parameters.Add("sgrcod", FbDbType.VarChar).Value = subgrupo;
                    command.Parameters.Add("proncm", FbDbType.VarChar).Value = ncm == "" ? produtos[i].NCM : ncm;
                    command.Parameters.Add("gencodigo", FbDbType.VarChar).Value = ncm == "" ? produtos[i].NCM == "" ? "": produtos[i].NCM.Substring(0, 2) : ncm.Substring(0,2);
                    command.Parameters.Add("procest", FbDbType.VarChar).Value = cest == "" ? produtos[i].CEST : cest;
                    command.Parameters.Add("procod", FbDbType.VarChar).Value = produtos[i].Codigo;

                    command.ExecuteScalar();
                    counter++;
                }
            }
            catch (FbException error)
            {
                throw error;
            }
            finally
            {
                connectionSysPDV.Close();
            }
            return counter;
        }

    }
}
