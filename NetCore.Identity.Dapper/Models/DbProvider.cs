using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Identity.Dapper.Models
{
    public class DbProvider
    {
        public string DbSchema { get; set; } = "[dbo]";

        public string ConnectionString { get; set; }
    }
}
