using System;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data;

namespace CorretorEAN
{
    internal static class ConexaoFirebird
    {
        private readonly static FbConnection connectionSysPDV = new FbConnection(@"DataSource=localhost; Database=C:\SysPDV\SysPDV_SRV.fdb; User=sysdba; Password=masterkey");

        public static List<Produto> GetListProdutosSysPDV(string seccod, string grpcod, string sgrcod, string prodes)
        {
            List<Produto> produtos = new List<Produto>();
            try
            {
                connectionSysPDV.Open();
                FbCommand fbCommand = new FbCommand(@"select P.PROCOD as CODIGO, PA.PROCODAUX as EAN, P.PRODES as DESCRICAO, P.PRODESRDZ as REDUZIDA, "+
                    "S.SECDES as SECAO, P.SECCOD as SECCOD, G.GRPDES as GRUPO, P.GRPCOD as GRPCOD, SG.SGRDES as SUBGRUPO, P.SGRCOD as SUBGRUPO, P.PRONCM as NCM, "+
                    "P.PROCEST as CEST from PRODUTO P left outer join PRODUTOAUX PA on PA.PROCOD=P.PROCOD " +
                    "left outer join SECAO S on S.SECCOD=P.SECCOD "+
                    "left outer join GRUPO G on G.SECCOD = P.SECCOD and G.GRPCOD = P.GRPCOD "+
                    "left outer join SUBGRUPO SG on SG.SECCOD = P.SECCOD and SG.GRPCOD = P.GRPCOD AND SG.SGRCOD = P.SGRCOD" +
                    "where p.seccod like @seccod and p.grpcod like @grpcod and p.sgrcod like @sgrcod and p.prodes like @prodes order by prodes", connectionSysPDV);
                fbCommand.Parameters.Add("@seccod", FbDbType.VarChar).Value = seccod == "00" ? "%" : seccod;
                fbCommand.Parameters.Add("@grpcod", FbDbType.VarChar).Value = grpcod == "000" ? "%" : grpcod;
                fbCommand.Parameters.Add("@sgrcod", FbDbType.VarChar).Value = sgrcod == "000" ? "%" : sgrcod;
                fbCommand.Parameters.Add("@prodes", FbDbType.VarChar).Value = prodes.ToUpperInvariant() == "" ? "%" : prodes.ToUpperInvariant();
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
                connectionSysPDV.Close();
            }
            return produtos;
        }

        public static List<string> GetSecaoSysPDV()
        {
            List<string> secoes = new List<string>();
            try
            {
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("select seccod || ' - ' || secdes as secao from secao order by seccod", connectionSysPDV);
                using (FbDataAdapter fbDataAdapter = new FbDataAdapter(command))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        fbDataAdapter.Fill(dataTable);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            secoes.Add(dataTable.Rows[i]["secao"].ToString());
                        }
                    }
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

            return secoes;
        }

        public static List<string> GetGrupoSysPDV(string seccod)
        {
            List<string> grupos = new List<string>();
            try
            {
                grupos.Add("000 - PADRAO");
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("select grpcod || ' - ' || grpdes as grupo from grupo where seccod=@seccod order by grpcod", connectionSysPDV);
                command.Parameters.Add("seccod", FbDbType.Char).Value = seccod;
                using (FbDataAdapter fbDataAdapter = new FbDataAdapter(command))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        fbDataAdapter.Fill(dataTable);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            grupos.Add(dataTable.Rows[i]["grupo"].ToString());
                        }
                    }
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

            return grupos;
        }

        public static List<string> GetSubGrupoSysPDV(string seccod, string grpcod)
        {
            List<string> subGrupos = new List<string>();
            try
            {
                subGrupos.Add("000 - PADRAO");
                connectionSysPDV.Open();
                FbCommand command = new FbCommand("select sgrcod || ' - ' || sgrdes as subgrupo from subgrupo where seccod=@seccod and grpcod=@grpcod order by sgrcod", connectionSysPDV);
                command.Parameters.Add("seccod", FbDbType.Char).Value = seccod;
                command.Parameters.Add("grpcod", FbDbType.Char).Value = grpcod;
                using (FbDataAdapter fbDataAdapter = new FbDataAdapter(command))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        fbDataAdapter.Fill(dataTable);
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            subGrupos.Add(dataTable.Rows[i]["subgrupo"].ToString());
                        }
                    }
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

            return subGrupos;
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
                    command.Parameters.Add("procod", FbDbType.VarChar).Value = produtos[i].CODIGO;

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
