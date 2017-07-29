using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopClient.Models
{
    class EncodingItem
    {
        public Encoding Encoding { get; set; }
        public override string ToString() => Encoding.EncodingName;
    }
}
