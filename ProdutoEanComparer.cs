using System;
using System.Collections.Generic;

namespace CorretorEAN
{
    class ProdutoEanComparer : IEqualityComparer<Produto>
    {
        public bool Equals(Produto x, Produto y)
        {
            if (string.Equals(x.Ean, y.Ean, StringComparison.InvariantCulture))
            {
                return true;
            }
            return false;

        }

        public int GetHashCode(Produto obj)
        {
            return obj.Ean.GetHashCode();
        }
    }
}
