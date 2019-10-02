using System;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Linq;

namespace CorretorEAN
{
    internal static class ConexaoFirebird
    {
        private readonly static FbConnection connectionSysPDV = new FbConnection(@"DataSource=localhost; Database=C:\SysPDV\SysPDV_SRV.fdb; User=sysdba; Password=masterkey");
        private readonly static FbConnection connectionModel = new FbConnection(@"DataSource=localhost; Database=C:\SysPDV\BaseModel.fdb; User=sysdba; Password=masterkey");

        public static List<Produto> GetListProdutosSysPDV()
        {
            List<Produto> produtos = new List<Produto>();
            try
            {
                connectionSysPDV.Open();
                FbCommand fbCommand = new FbCommand(@"select P.PROCOD as CODIGO, PA.PROCODAUX as EAN, P.PRODES as DESCRICAO, P.PRODESRDZ as REDUZIDA, " +
                    "S.SECDES as SECAO, P.SECCOD as SECCOD, G.GRPDES as GRUPO, P.GRPCOD as GRPCOD, SG.SGRDES as SUBGRUPO, P.SGRCOD as SGRCOD, P.PRONCM as NCM, " +
                    "P.PROCEST as CEST from PRODUTO P left outer join PRODUTOAUX PA on PA.PROCOD=P.PROCOD " +
                    "left outer join SECAO S on S.SECCOD=P.SECCOD " +
                    "left outer join GRUPO G on G.SECCOD = P.SECCOD and G.GRPCOD = P.GRPCOD " +
                    "left outer join SUBGRUPO SG on SG.SECCOD = P.SECCOD and SG.GRPCOD = P.GRPCOD AND SG.SGRCOD = P.SGRCOD order by PA.PROCODAUX", connectionSysPDV);
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
                                Sgrcod = dataTable.Rows[i]["SGRCOD"].ToString(),
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
                connectionSysPDV.Close();
            }
            return produtos;
        }

        public static List<Produto> GetListProdutosSysPDV(List<Produto> products)
        {
            List<Produto> produtos = new List<Produto>();
            try
            {
                connectionSysPDV.Open();
                FbCommand fbCommand = new FbCommand(@"select P.PROCOD as CODIGO, PA.PROCODAUX as EAN, P.PRODES as DESCRICAO, P.PRODESRDZ as REDUZIDA, " +
                    "S.SECDES as SECAO, P.SECCOD as SECCOD, G.GRPDES as GRUPO, P.GRPCOD as GRPCOD, SG.SGRDES as SUBGRUPO, P.SGRCOD as SGRCOD, P.PRONCM as NCM, " +
                    "P.PROCEST as CEST from PRODUTO P left outer join PRODUTOAUX PA on PA.PROCOD=P.PROCOD " +
                    "left outer join SECAO S on S.SECCOD=P.SECCOD " +
                    "left outer join GRUPO G on G.SECCOD = P.SECCOD and G.GRPCOD = P.GRPCOD " +
                    "left outer join SUBGRUPO SG on SG.SECCOD = P.SECCOD and SG.GRPCOD = P.GRPCOD AND SG.SGRCOD = P.SGRCOD " +
                    "order by PA.PROCODAUX", connectionSysPDV);
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
                                Ean = dataTable.Rows[i]["EAN"].ToString().Trim(' '),
                                Descricao = dataTable.Rows[i]["DESCRICAO"].ToString(),
                                Reduzida = dataTable.Rows[i]["REDUZIDA"].ToString(),
                                Secao = dataTable.Rows[i]["SECAO"].ToString(),
                                Seccod = dataTable.Rows[i]["SECCOD"].ToString(),
                                Grupo = dataTable.Rows[i]["GRUPO"].ToString(),
                                Grpcod = dataTable.Rows[i]["GRPCOD"].ToString(),
                                Subgrupo = dataTable.Rows[i]["SUBGRUPO"].ToString(),
                                Sgrcod = dataTable.Rows[i]["SGRCOD"].ToString(),
                                NCM = dataTable.Rows[i]["NCM"].ToString(),
                                CEST = dataTable.Rows[i]["CEST"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                connectionSysPDV.Close();
            }
            return produtos.Intersect(products, new ProdutoEanComparer()).ToList();
        }

        public static List<Produto> GetListProdutosModel()
        {
            List<Produto> produtos = new List<Produto>();
            try
            {
                connectionModel.Open();
                FbCommand fbCommand = new FbCommand(@"select P.CODIGO as EAN, P.DESCRICAO as DESCRICAO, P.REDUZIDA as REDUZIDA, " +
                    "S.DESCRICAO as SECAO, P.SECAO_ID as SECCOD, G.DESCRICAO as GRUPO, P.GRUPO_ID as GRPCOD, SG.DESCRICAO as SUBGRUPO, P.SUBGRUPO_ID as SGRCOD, P.NCM as NCM, " +
                    "P.CEST as CEST from PRODUCTS P " +
                    "left outer join SECAO S on S.SECAO_ID = P.SECAO_ID " +
                    "left outer join GRUPO G on G.SECAO_ID = P.SECAO_ID and G.GRUPO_ID = P.GRUPO_ID " +
                    "left outer join SUBGRUPO SG on SG.SECAO_ID = P.SECAO_ID and SG.GRUPO_ID = P.GRUPO_ID AND SG.SUBGRUPO_ID = P.SUBGRUPO_ID order by P.CODIGO", connectionModel);
                using (FbDataAdapter fbDataAdapter = new FbDataAdapter(fbCommand))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        fbDataAdapter.Fill(dataTable);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            produtos.Add(new Produto()
                            {
                                Ean = dataTable.Rows[i]["EAN"].ToString(),
                                Descricao = dataTable.Rows[i]["DESCRICAO"].ToString(),
                                Reduzida = dataTable.Rows[i]["REDUZIDA"].ToString(),
                                Secao = dataTable.Rows[i]["SECAO"].ToString(),
                                Seccod = dataTable.Rows[i]["SECCOD"].ToString(),
                                Grupo = dataTable.Rows[i]["GRUPO"].ToString(),
                                Grpcod = dataTable.Rows[i]["GRPCOD"].ToString(),
                                Subgrupo = dataTable.Rows[i]["SUBGRUPO"].ToString(),
                                Sgrcod = dataTable.Rows[i]["SGRCOD"].ToString(),
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
                connectionModel.Close();
            }
            return produtos;
        }

        public static bool UpdateProdutosSysPDV(string procod, Produto produto)
        {
            if (produto.Seccod.Equals(""))
            {
                throw new ArgumentNullException("Campos obrigatórios não foram preenchidos!");
            }
            try
            {
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("update produto set ", connectionSysPDV);
                if (!produto.Secao.Equals(string.Empty))
                {
                    if(!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "seccod=@seccod, grpcod=@grpcod, sgrcod=@sgrcod ";
                    command.Parameters.Add("seccod", FbDbType.VarChar).Value = produto.Seccod;
                    command.Parameters.Add("grpcod", FbDbType.VarChar).Value = produto.Grpcod == "" ? "000" : produto.Grpcod;
                    command.Parameters.Add("sgrcod", FbDbType.VarChar).Value = produto.Sgrcod == "" ? "000" : produto.Sgrcod;
                }
                if (!produto.NCM.Equals(string.Empty))
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "proncm=@proncm, gencodigo=@gencodigo, procest=@procest ";
                    command.Parameters.Add("proncm", FbDbType.VarChar).Value = produto.NCM;
                    command.Parameters.Add("gencodigo", FbDbType.VarChar).Value = produto.NCM.Substring(0,2) ;
                    command.Parameters.Add("procest", FbDbType.VarChar).Value = produto.CEST;

                }
                if (!produto.Descricao.Equals(string.Empty))
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "prodes=@prodes ";
                    command.Parameters.Add("prodes", FbDbType.VarChar).Value = produto.Descricao;
                }
                if (!produto.Reduzida.Equals(string.Empty))
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "prodesrdz=@prodesrdz ";
                    command.Parameters.Add("prodesrdz", FbDbType.VarChar).Value = produto.Reduzida;
                }
                command.CommandText += "where procod=@procod";
                command.Parameters.Add("procod", FbDbType.VarChar).Value = procod;
                
                command.ExecuteScalar();
            }
            catch (FbException error)
            {
                if (error.Message.ToLower().Contains("truncation"))
                {
                    throw new InvalidOperationException("Arithmethic Truncation Exception no Produto :" + procod);
                }
                throw error;
            }
            finally
            {
                connectionSysPDV.Close();
            }
            return true;
        }

    }
}
