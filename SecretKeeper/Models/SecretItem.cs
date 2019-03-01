using System;

namespace SecretKeeper.Models
{
    public class SecretItem
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string Value { get; set; }
        public string TimeToLive { get; set; }
        public DateTime ExpiredBy { get; set; }
     }
}
