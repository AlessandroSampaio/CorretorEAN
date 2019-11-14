using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CorretorEAN
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class Home : Window
    {
        public List<Produto> Origem { get; set; }
        public List<Produto> Destino { get; set; }
        public bool Descricao { get; set; }
        public bool Reduzida { get; set; }
        public bool Classificacao { get; set; }
        public bool NCM { get; set; }

        public Home()
        {
            InitializeComponent();
            try
            {
                btTransferir.IsEnabled = false;
                CarregaLV();
                lvOrigem.ItemsSource = Origem;
                lvDestino.ItemsSource = Destino;
                
            }catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }


        #region Util

        public Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        /// <summary>
        /// Ativa ou inativa todos os controles da windows
        /// </summary>
        public void EnableAll()
        {
            ckbClassificacao.IsEnabled = !ckbClassificacao.IsEnabled;
            ckbDescricao.IsEnabled = !ckbDescricao.IsEnabled;
            ckbDescricaoReduzida.IsEnabled = !ckbDescricaoReduzida.IsEnabled;
            ckbNCM.IsEnabled = !ckbNCM.IsEnabled;
            lvOrigem.IsEnabled = !lvOrigem.IsEnabled;
            lvDestino.IsEnabled = !lvDestino.IsEnabled;
            btTransferir.IsEnabled = !btTransferir.IsEnabled;
        }

        /// <summary>
        /// Popula as ListViews com os dados de origem e destino utilizando Intercept()
        /// </summary>
        private void CarregaLV()
        {
            Destino = ConexaoFirebird.GetListProdutosModel();
            Origem = ConexaoFirebird.GetListProdutosSysPDV(Destino);
            Destino = Destino.Intersect(Origem, new ProdutoEanComparer()).ToList();
        }

        #endregion

        #region Events
        /// <summary>
        /// Evento para sincronizar as ListViews
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lbx1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer _listboxScrollViewer1 = GetDescendantByType(lvOrigem, typeof(ScrollViewer)) as ScrollViewer;
            ScrollViewer _listboxScrollViewer2 = GetDescendantByType(lvDestino, typeof(ScrollViewer)) as ScrollViewer;
            _listboxScrollViewer2.ScrollToVerticalOffset(_listboxScrollViewer1.VerticalOffset);
        }

        /// <summary>
        /// Inicia o processo de atualização de cadastro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtTransferir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            if (Classificacao)
            {
                result = MessageBox.Show("Você marcou a opção \"CLASSIFICAO\" o que acarretará na substituição de todo o cadastro referente a SEÇAO/GRUPO/SUBGRUPO. Deseja prosseguir mesmo assim? ", "", MessageBoxButton.YesNo);
            }
            else
            {
                result = MessageBox.Show("Deseja prosseguir com as alterações? ", "", MessageBoxButton.YesNo);
            }
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Todos os produtos com referência não localizada serão movidos para a Seção 99!");
                EnableAll();
                if (Classificacao)
                {
                    using (BackgroundWorker worker = new BackgroundWorker())
                    {
                        worker.WorkerReportsProgress = true;
                        worker.ProgressChanged += Worker_ProgressChanged_Secao;
                        worker.DoWork += Worker_DoWork_Secoes;
                        worker.RunWorkerCompleted += Worker_RunWorkerCompleted_Secao;
                        worker.RunWorkerAsync();
                    }
                }
                else
                {
                    AtualizarProdutos();
                }
            }
        }

        /// <summary>
        /// Inativa produtos sem movimentacao a partir de determinado periodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInativarProdutos_Click(object sender, RoutedEventArgs e)
        {
            InativarProdutos inativarProdutos = new InativarProdutos();
            inativarProdutos.ShowDialog();
        }

        /// <summary>
        /// Gerar custos ficticios com base em uma margem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGerarCustos_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Utilizado para popular as variaveis globais.
        /// Ativa e inativa o botão de transferencia caso uma checkbox esteja marcada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkbFilters_Checked(object sender, RoutedEventArgs e)
        {
            Descricao = ckbDescricao.IsChecked.GetValueOrDefault(false);
            Reduzida = ckbDescricaoReduzida.IsChecked.GetValueOrDefault(false);
            Classificacao = ckbClassificacao.IsChecked.GetValueOrDefault(false);
            NCM = ckbNCM.IsChecked.GetValueOrDefault(false);
            if (Descricao || Reduzida || Classificacao || NCM)
            {
                btTransferir.IsEnabled = true;
            }
            else
            {
                btTransferir.IsEnabled = false;
            }
        }

        #endregion

        #region Workers

        /// <summary>
        /// Gera um BackGroundWorker para control o progresso na atualização dos produtos
        /// </summary>
        private void AtualizarProdutos()
        {
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Ação realizada no fim do BackGroundWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Concluido com Sucesso");
            EnableAll();
            pgBar_Produtos.Value = 0;
            lbProgress.Content = "";
        }

        /// <summary>
        /// Controle de progresso da execução do BackGroundWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBar_Produtos.Value = e.ProgressPercentage;
            lbProgress.Content = "Movendo Produtos : " + e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// Realiza a alteração dos itens listados na ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime currrentWork = DateTime.Now;
            var worker = sender as BackgroundWorker;
            try
            {
                //Capturando Lista de produtos sem correspondencia na model
               
                for (int i = 0; i < Origem.Count; i++)
                {
                    if (Origem[i].Ean.Equals(Destino[i].Ean))
                    {
                        try
                        {
                            ConexaoFirebird.UpdateProdutosSysPDV(Origem[i].Codigo, Destino[i], Descricao, Reduzida, Classificacao, NCM);
                        }
                        catch (InvalidOperationException fbError)
                        {
                            MessageBox.Show(fbError.Message);
                            Log.AppendFile(Origem[i], currrentWork);
                        }
                        worker.ReportProgress((100 * i / Origem.Count));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Inicia as alterações nos produtos no fim do BackGroundWorker das seções
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_RunWorkerCompleted_Secao(object sender, RunWorkerCompletedEventArgs e)
        {
            AtualizarProdutos();
        }

        /// <summary>
        /// Controle de progresso da execução do BackGroundWorker das Seções
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_ProgressChanged_Secao(object sender, ProgressChangedEventArgs e)
        {
            pgBar_Secao.Value = e.ProgressPercentage;
            if(e.ProgressPercentage < 33)
            {
                lbProgress.Content = "CRIANDO SEÇÕES";
            }else if(e.ProgressPercentage < 66)
            {
                lbProgress.Content = "CRIANDO GRUPOS";
            }else if(e.ProgressPercentage < 99)
            {
                lbProgress.Content = "CRIANDO SUBGRUPOS";
            }
            else
            {
                lbProgress.Content = "MOVENDO PRODUTOS SEM REFERENCIAS";
            }
        }

        /// <summary>
        /// Realiza a remoção e a criação dos itens de classificação de um produto.
        /// Seção/Grupo/SubGrupo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_DoWork_Secoes(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            try
            {
                if(ConexaoFirebird.DeleteSecoesSysPDV())
                    ConexaoFirebird.CreateSecaoSysPDV();
                worker.ReportProgress(33);
                ConexaoFirebird.CreateGrupoSysPDV();
                worker.ReportProgress(66);
                ConexaoFirebird.CreateSubGrupoSysPDV();
                worker.ReportProgress(99);
                ConexaoFirebird.UpdateSecaoProdutosSysPDV();                
            }
            catch (Exception error) {
                MessageBox.Show(error.Message);
            }
            finally
            {
                worker.ReportProgress(100);
            }
        }

        #endregion

       
    }
}
