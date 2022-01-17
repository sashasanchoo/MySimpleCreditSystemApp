using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class WrongData:ApplicationException
    {
        public WrongData(string name) : base($"Entered date \"{name}\" is not valid")
        {}
    }
}
