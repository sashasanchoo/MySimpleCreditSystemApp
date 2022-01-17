using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class ProductDoesntExist:ApplicationException
    {
        public ProductDoesntExist(string name) : base($"The product with name \"{name}\" doesn't exist")
        { }
    }
}
