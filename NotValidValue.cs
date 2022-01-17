using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class NotValidValue:ApplicationException
    {
        public NotValidValue(object value) : base($"value \"{value.ToString()}\" is not valid")
        {}
    }
}
