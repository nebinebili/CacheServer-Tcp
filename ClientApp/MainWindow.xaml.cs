using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientWebApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TcpClient Client { get; set; }
        public MainWindow()
        {
            InitializeComponent();

        }

        string GetIpAddress()
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

        private async void btn_search_Click(object sender, RoutedEventArgs e)
        {

            Client = new TcpClient();
            Client.Connect(await Task.Run(() => GetIpAddress()), 444);
            try
            {
                using (var stream = Client.GetStream())
                {
                    var bw = new BinaryWriter(stream);
                    var br = new BinaryReader(stream);
                    bw.Write(txb_name.Text);
                    txb_likecount.Text = br.ReadString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
