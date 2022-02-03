using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        public static TcpClient Client { get; set; }
        public static Dictionary<string, int> Dict { get; set; } = new Dictionary<string, int>();
        public static StringBuilder stringBuilder { get; set; }

        static void Main(string[] args)
        {
            // Null Database add in project

            //var connection = new SQLiteConnection("Data Source=TwiterDB.db ");
            //if (!File.Exists("./TwiterDB.db"))
            //{
            //    System.Console.WriteLine();
            //    SQLiteConnection.CreateFile("TwiterDB.db");
            //}


            var EndPoint = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 444);
            var listener = new TcpListener(EndPoint);

            listener.Start(10);

            try
            {
                while (true)
                {
                    Task.Run(() =>
                    {
                        do
                        {
                            Client = listener.AcceptTcpClient();

                            DataRequest();
                        } while (true);
                    });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           

        }

        static string DataRequestSend_CacheServer(StringBuilder request)
        {
            var EndPoint = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 555);
            var client = new TcpClient();
            client.Connect(EndPoint);

            
            using (var stream1 = client.GetStream())
            {
                var bw = new BinaryWriter(stream1);
                var br = new BinaryReader(stream1);

                Console.WriteLine("Data send Cache Server !!");
                bw.Write(Convert.ToString(request));
                return br.ReadString();
            }
        }

        

        static void DataRequest()
        {
            using (var stream = Client.GetStream())
            {
                var br = new BinaryReader(stream);
                var bw = new BinaryWriter(stream);

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=TwiterDB.db "))
                {
                    connection.Open();
                    string name = br.ReadString();

                    string request = @"Select * from Users Where Name = @name";
                    var cmd = new SQLiteCommand(request, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    
                    
                    // send Cache server
                    stringBuilder = new StringBuilder();
                    var response=DataRequestSend_CacheServer(stringBuilder.Append("get").Append(" ").Append(name));

                    if (response == "null")
                    {
                        if (!Dict.ContainsKey(name))
                        {
                            Dict.Add(name, 1);
                            // response database and send response to client
                            SQLiteDataReader rdr = cmd.ExecuteReader();
                            Console.WriteLine("Data send Database !!");
                            while (rdr.Read())
                            {
                                Console.WriteLine("Data is found");
                                bw.Write(Convert.ToString(rdr.GetInt32(2)));
                            }
                        }
                        else if (Dict.ContainsKey(name))
                        {
                            if (Dict[name] < 2) 
                            { 
                                Dict[name] += 1;
                                // response database and send response to client
                                SQLiteDataReader rdr = cmd.ExecuteReader();
                                Console.WriteLine("Data send Database !!");
                                while (rdr.Read())
                                {
                                    Console.WriteLine("Data is found ");
                                    bw.Write(Convert.ToString(rdr.GetInt32(2)));
                                }
                            }
                            else if (Dict[name] == 2)
                            {
                                // response database and send response to client
                                SQLiteDataReader rdr = cmd.ExecuteReader();
                                Console.WriteLine("Data send Database !!");
                                while (rdr.Read())
                                {
                                    Console.WriteLine("Data is found ");

                                    //Put Data in Cache server
                                    stringBuilder = new StringBuilder();
                                    Console.WriteLine("Data Request count more than 2");
                                    var response1=DataRequestSend_CacheServer(stringBuilder.Append("put").Append(" ").Append(name).Append(" ").Append(Convert.ToString(rdr.GetInt32(2))));
                                    bw.Write(response1);
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        bw.Write(response);
                    }

                }
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
