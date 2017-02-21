using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamergotchi.Entities
{
    public class Quote
    {
        public string text { get; set; }
        public string sound { get; set; }
        public string callout { get; set; }
        public string group { get; set; }
        public string _id { get; set; }
        public List<string> replaces { get; set; }
    }
}
