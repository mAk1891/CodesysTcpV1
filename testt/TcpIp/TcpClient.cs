using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;

namespace testt.NewFolder
{
    // esta classe tem o tal nome SimpleTcpClient. 
    public static class TcpClient
    {
        public static bool IsConnected;
        //TcpIp Client
        public static void InitTcp() 
        {
            // instantiate

        }
        public static void StartConnection() 
        { 
            //ele faz tipo isto. Mas um pouco mais complexo
            IsConnected = true;
        }
        public static void StopConnection() 
        { 
        }


        public static void Metodo()
        {
            Console.WriteLine("sei la");
        }
    }
}
