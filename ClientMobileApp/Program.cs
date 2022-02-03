using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientMobileApp
{
    class Program
    {
        public static TcpClient Client { get; set; }
        static void Main(string[] args)
        {


            
            Client = new TcpClient();
            Client.Connect(GetIpAddress(), 444);


            using (var stream = Client.GetStream())
            {
                var bw = new BinaryWriter(stream);
                var br = new BinaryReader(stream);


                Console.Write("Search name: ");
                var name = Console.ReadLine();


                bw.Write(name);
                Console.WriteLine($"Like count:{br.ReadString()}");

            }
        }
        static string GetIpAddress()
        {
            IPHostEntry host;
            string localhost = "?";
            host = Dns.GetHostEntry(Dns.GetHostName()); // return hostname

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localhost = ip.ToString();
                }
            }
            return localhost;
        }
    }
}
