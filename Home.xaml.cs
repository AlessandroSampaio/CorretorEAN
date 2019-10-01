﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorretorEAN
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
            CarregaLV();
        }

        private void CarregaLV()
        {
            lvOrigem.ItemsSource = ConexaoFirebird.GetListProdutosSysPDV(1);
            lvDestino.ItemsSource = ConexaoFirebird.GetListProdutosSysPDV(0);
        }

        private void BtTransferir_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
