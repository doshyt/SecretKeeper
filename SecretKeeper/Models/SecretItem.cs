using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretKeeper.Models
{
    public class SecretItem
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string Value { get; set; }
    }
}
