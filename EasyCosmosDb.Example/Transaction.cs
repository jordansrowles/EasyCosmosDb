using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCosmosDb.Example
{
    public class Transaction : BaseCosmosModel
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public double Amount { get; set; }
    }
}
