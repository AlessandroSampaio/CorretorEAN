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
        private readonly static FbConnection connectionModel = new FbConnection(@"DataSource=localhost; Database=" + AppDomain.CurrentDomain.BaseDirectory + @"BaseModel.fdb; User=sysdba; Password=masterkey");

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

        public static bool UpdateProdutosSysPDV(string procod, Produto produto, bool descr, bool reduz, bool classif, bool ncm)
        {
            if (produto.Seccod.Equals(""))
            {
                throw new ArgumentNullException("Campos obrigatórios não foram preenchidos!");
            }
            try
            {
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("update produto set ", connectionSysPDV);
                if (!produto.Secao.Equals(string.Empty) && classif)
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "seccod=@seccod, grpcod=@grpcod, sgrcod=@sgrcod ";
                    command.Parameters.Add("seccod", FbDbType.VarChar).Value = produto.Seccod;
                    command.Parameters.Add("grpcod", FbDbType.VarChar).Value = produto.Grpcod == "" ? "000" : produto.Grpcod;
                    command.Parameters.Add("sgrcod", FbDbType.VarChar).Value = produto.Sgrcod == "" ? "000" : produto.Sgrcod;
                }
                if (!produto.NCM.Equals(string.Empty) && ncm)
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "proncm=@proncm, gencodigo=@gencodigo, procest=@procest ";
                    command.Parameters.Add("proncm", FbDbType.VarChar).Value = produto.NCM;
                    command.Parameters.Add("gencodigo", FbDbType.VarChar).Value = produto.NCM.Substring(0, 2);
                    command.Parameters.Add("procest", FbDbType.VarChar).Value = produto.CEST;

                }
                if (!produto.Descricao.Equals(string.Empty) && descr)
                {
                    if (!command.CommandText.Equals("update produto set "))
                    {
                        command.CommandText += ", ";
                    }
                    command.CommandText += "prodes=@prodes ";
                    command.Parameters.Add("prodes", FbDbType.VarChar).Value = produto.Descricao;
                }
                if (!produto.Reduzida.Equals(string.Empty) && reduz)
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
                command.Dispose();

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

        public static int UpdateSecaoProdutosSysPDV(List<Produto> produtos, string seccod)
        {
            int counter = 0;
            if (seccod.Equals(""))
            {
                throw new ArgumentNullException("Campos obrigatórios não foram preenchidos!");
            }
            try
            {
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("update produto set seccod=@seccod, grpcod='000', sgrcod='000' where procod=@procod", connectionSysPDV);
                command.Parameters.Add("seccod", FbDbType.VarChar).Value = "";
                command.Parameters.Add("procod", FbDbType.VarChar).Value = "";
                for (int i = 0; i < produtos.Count; i++)
                {
                    command.Parameters["seccod"].Value = seccod;
                    command.Parameters["procod"].Value = produtos[i].Codigo;
                    command.ExecuteScalar();
                    counter++;
                }
                command.Dispose();
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

        public static bool DeleteSecoesSysPDV()
        {
            try
            {
                connectionSysPDV.Open();
                using (FbCommand command = new FbCommand("delete from subgrupo", connectionSysPDV))
                {
                    command.ExecuteNonQuery();
                    command.CommandText = "delete from grupo";
                    command.ExecuteNonQuery();
                    command.CommandText = "delete from secao";
                    command.ExecuteNonQuery();
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
            return true;
        }

        public static bool CreateSecaoSysPDV()
        {
            try
            {
                connectionModel.Open();
                using (FbCommand commandModel = new FbCommand("select secao_id, descricao from secao", connectionModel))
                {
                    using (FbDataAdapter fbData = new FbDataAdapter(commandModel))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            fbData.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                connectionSysPDV.Open();
                                using (FbCommand insertSecaoSysPDV = new FbCommand("insert into secao (seccod, secdes) values (@seccod, @secdes)", connectionSysPDV))
                                {
                                    insertSecaoSysPDV.Parameters.Add("seccod", FbDbType.Char).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("secdes", FbDbType.VarChar).Value = "";
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        insertSecaoSysPDV.Parameters["seccod"].Value = dt.Rows[i]["secao_id"].ToString();
                                        insertSecaoSysPDV.Parameters["secdes"].Value = dt.Rows[i]["descricao"].ToString();
                                        insertSecaoSysPDV.ExecuteNonQuery();
                                    }
                                    insertSecaoSysPDV.Parameters["seccod"].Value = "99";
                                    insertSecaoSysPDV.Parameters["secdes"].Value = "PRODUTOS NAO LOCALIZADOS";
                                    insertSecaoSysPDV.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (FbException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                throw error;
            }
            finally
            {
                connectionModel.Close();
                connectionSysPDV.Close();
            }
            return true;
        }

        public static bool CreateGrupoSysPDV()
        {
            try
            {
                connectionModel.Open();
                using (FbCommand commandModel = new FbCommand("select secao_id, grupo_id, descricao from grupo", connectionModel))
                {
                    using (FbDataAdapter fbData = new FbDataAdapter(commandModel))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            fbData.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                connectionSysPDV.Open();
                                using (FbCommand insertSecaoSysPDV = new FbCommand("insert into grupo (seccod, grpcod, grpdes) values (@seccod, @grpcod,  @grpdes)", connectionSysPDV))
                                {
                                    insertSecaoSysPDV.Parameters.Add("seccod", FbDbType.Char).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("grpcod", FbDbType.VarChar).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("grpdes", FbDbType.VarChar).Value = "";
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        insertSecaoSysPDV.Parameters["seccod"].Value = dt.Rows[i]["secao_id"].ToString();
                                        insertSecaoSysPDV.Parameters["grpcod"].Value = dt.Rows[i]["grupo_id"].ToString();
                                        insertSecaoSysPDV.Parameters["grpdes"].Value = dt.Rows[i]["descricao"].ToString();
                                        insertSecaoSysPDV.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (FbException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                throw error;
            }
            finally
            {
                connectionModel.Close();
                connectionSysPDV.Close();
            }
            return true;
        }

        public static bool CreateSubGrupoSysPDV()
        {
            try
            {
                connectionModel.Open();
                using (FbCommand commandModel = new FbCommand("select secao_id, grupo_id, subgrupo_id, descricao from subgrupo", connectionModel))
                {
                    using (FbDataAdapter fbData = new FbDataAdapter(commandModel))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            fbData.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                connectionSysPDV.Open();
                                using (FbCommand insertSecaoSysPDV = new FbCommand("insert into subgrupo (seccod, grpcod, sgrcod, sgrdes) values (@seccod, @grpcod, @sgrcod, @sgrdes)", connectionSysPDV))
                                {
                                    insertSecaoSysPDV.Parameters.Add("seccod", FbDbType.Char).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("grpcod", FbDbType.VarChar).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("sgrcod", FbDbType.VarChar).Value = "";
                                    insertSecaoSysPDV.Parameters.Add("sgrdes", FbDbType.VarChar).Value = "";
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        insertSecaoSysPDV.Parameters["seccod"].Value = dt.Rows[i]["secao_id"].ToString();
                                        insertSecaoSysPDV.Parameters["grpcod"].Value = dt.Rows[i]["grupo_id"].ToString();
                                        insertSecaoSysPDV.Parameters["sgrcod"].Value = dt.Rows[i]["subgrupo_id"].ToString();
                                        insertSecaoSysPDV.Parameters["sgrdes"].Value = dt.Rows[i]["descricao"].ToString();
                                        insertSecaoSysPDV.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (FbException error)
            {
                throw error;
            }
            catch (Exception error)
            {
                throw error;
            }
            finally
            {
                connectionModel.Close();
                connectionSysPDV.Close();
            }
            return true;
        }
    }
}
