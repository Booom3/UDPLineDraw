using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    abstract class NetworkObject
    {
        public int NetworkID { get; private set; }
        public static string networkNameIdentifier { get; }
        public NetworkObject (int networkID)
        {
            NetworkID = networkID;
        }
    }
}
