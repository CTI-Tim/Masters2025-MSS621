using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public class Message
    {
        public bool Digital { get; set; }
        public ushort Analog { get; set; }
        public string Serial { get; set; }
    }
}

