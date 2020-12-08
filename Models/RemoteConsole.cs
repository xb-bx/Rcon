

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Rcon.Models
{
    public class RemoteConsole
    {
        public RemoteConsole(string iP, int port, string password)
        {
            IP = iP;
            Port = port;
            Password = password;
            Client = new UdpClient();
            Client.Client.SendTimeout = 5000;
            Client.Client.ReceiveTimeout = 5000;
        }

        /// <summary>
        /// Server's ip address
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Server's port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Server's rcon password
        /// </summary>
        public string Password { get; set; }

        private string challenge;

        public UdpClient Client { get; set; }

        private System.Net.IPEndPoint endp;
        public void AcceptChallenge()
        {
            var ch = CreateCommand("challenge rcon\n");
            Client.Send(ch, ch.Length);
            var rec = Client.Receive(ref endp);
            var sb = new StringBuilder();
            foreach (var item in Encoding.ASCII.GetString(rec))
            {
                if (char.IsDigit(item)) sb.Append(item);
            }
            challenge = sb.ToString();

        }
        public bool Connect()
        {
            Console.WriteLine($"Connecting to {IP}:{Port}");
            Client.Close();
            Client = new UdpClient();
            try
            {
                Client.Connect(IP, Port);
                Client.Client.ReceiveTimeout = 1000;
                Client.Client.SendTimeout = 1000;
                if (SendCommand("console").Contains("console"))
                {
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public Task<bool> ConnectAsync() => Task.Run(Connect);

        public string SendCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(challenge))
            {
                AcceptChallenge();
            }
            var cmdStr = $"rcon \"{challenge}\" {Password} {command}\n";
            var commandBytes = CreateCommand(cmdStr);
            Client.Send(commandBytes, commandBytes.Length);
            var sb = new StringBuilder();

            try
            {
                do
                {
                    var response = Client.Receive(ref endp);
                    Console.WriteLine($"{endp.Address}:{endp.Port}");
                    foreach (var item in response)
                    {
                        if (item <= 127)
                        {
                            sb.Append((char)item);
                        }
                    }
                    System.Threading.Thread.Sleep(200);
                }
                while (Client.Available > 0);

            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
            {
            } 
            var res = sb.ToString();
            return res.Length == 0 ? "Unable to get response from server" : res.Substring(1);
        }
		
		public Task<string> SendCommandAsync(string command) => Task.Run(() => SendCommand(command));

        private byte[] CreateCommand(string command)
        {
            var res = new byte[command.Length + 4];
            Encoding.ASCII.GetBytes(command, 0, command.Length, res, 4);
            res[0] = res[1] = res[2] = res[3] = 255;
            return res;
        }

    }
}