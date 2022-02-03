using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CacheServerApp
{
    class Program
    {
        public static Dictionary<string, string> Dict { get; set; } = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            Dict.Add("perviz", "36127");
            Dict.Add("tofiq", "120356");

            var EndPoint = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 555);
            var listener = new TcpListener(EndPoint);

            listener.Start(10);
            

            while (true)
            {
                do
                {
                    var client = listener.AcceptTcpClient();
                    using (var stream = client.GetStream())
                    {
                        var br = new BinaryReader(stream);
                        var bw = new BinaryWriter(stream);

                        var temp = br.ReadString().Split(' ');
                        if (temp[0] == "get")
                        {
                            if (Dict.ContainsKey(temp[1])) { bw.Write(Dict[temp[1]]); Console.WriteLine($"{temp[1]}'s Data is found!"); }
                            else { bw.Write("null"); Console.WriteLine($"{temp[1]}'s Data is not found!"); }
                        }
                        if (temp[0] == "put")
                        {
                            Dict.Add(temp[1], temp[2]);
                            Console.WriteLine($"{temp[1]}'s Data Succesfully Add!");
                            bw.Write(temp[2]);
                        }
                    }


                } while (true);

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
