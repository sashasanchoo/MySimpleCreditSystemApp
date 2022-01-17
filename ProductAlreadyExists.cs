using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class ProductAlreadyExists:ApplicationException
    {
        public ProductAlreadyExists(string name) : base($"The product with name \"{name}\" already exist")
        {}
    }
}
