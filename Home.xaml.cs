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
            btTransferir.IsEnabled = false;
            CarregaLV();
            lvOrigem.ItemsSource = Origem;
            lvDestino.ItemsSource = Destino;
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

        private void CarregaLV()
        {
            Destino = ConexaoFirebird.GetListProdutosModel();
            Origem = ConexaoFirebird.GetListProdutosSysPDV(Destino);
            Destino = Destino.Intersect(Origem, new ProdutoEanComparer()).ToList();
        }

        #endregion

        #region Events

        private void Lbx1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer _listboxScrollViewer1 = GetDescendantByType(lvOrigem, typeof(ScrollViewer)) as ScrollViewer;
            ScrollViewer _listboxScrollViewer2 = GetDescendantByType(lvDestino, typeof(ScrollViewer)) as ScrollViewer;
            _listboxScrollViewer2.ScrollToVerticalOffset(_listboxScrollViewer1.VerticalOffset);
        }

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

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Concluido com Sucesso");
            EnableAll();
            pgBar_Produtos.Value = 0;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBar_Produtos.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime currrentWork = DateTime.Now;
            var worker = sender as BackgroundWorker;
            try
            {
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


        private void Worker_RunWorkerCompleted_Secao(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Worker_ProgressChanged_Secao(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Worker_DoWork_Secoes(object sender, DoWorkEventArgs e)
        {
            try
            {
                if(ConexaoFirebird.DeleteSecoesSysPDV())
                    ConexaoFirebird.CreateSecoesSysPDV();
            }
            catch (Exception error) {
                MessageBox.Show(error.Message);
            }
        }

        #endregion
    }
}
