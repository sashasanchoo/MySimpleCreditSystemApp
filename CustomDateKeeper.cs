using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class CustomDateKeeper
    {
        public DateTime UsersDate { get; set; }
        public CustomDateKeeper(DateTime usersDate)
        {
            this.UsersDate = usersDate;
        }
    }
}
