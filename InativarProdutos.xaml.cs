using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CorretorEAN
{
    /// <summary>
    /// Lógica interna para InativarProdutos.xaml
    /// </summary>
    public partial class InativarProdutos : Window
    {
        public InativarProdutos()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnInativar_Click(object sender, RoutedEventArgs e)
        {
            DateTime dataInicial = dpDataInicial.DisplayDate;
            int inativados = ConexaoFirebird.InativarProdutos(dataInicial);
            MessageBox.Show("Foram inativados " + inativados + " produtos");
            Close();

        }
    }
}
