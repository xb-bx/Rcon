using Rcon.Models;
using Avalonia.Threading;
using ReactiveUI;
using System.Threading.Tasks;
namespace Rcon.ViewModels
{
    public class RemoteConsoleViewModel : ViewModelBase
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string RconPassword { get; set; }

        private string consoleText;
        public string ConsoleText
        {
            get => consoleText; 
            set
            {
                this.RaiseAndSetIfChanged(ref consoleText, value);
            }
        }

        private string command;
        public string Command
        {
            get => command; 
            set
            {
                this.RaiseAndSetIfChanged(ref command, value);
            }
        }


        private RemoteConsole remoteConsole = new RemoteConsole(null, 0, null);
        public async void Connect()
        {
            System.Console.WriteLine($"0 Connecting to {IP}:{Port}");

            remoteConsole.IP = IP;
            remoteConsole.Port = int.Parse(Port);
            remoteConsole.Password = RconPassword;
            
            if(await remoteConsole.ConnectAsync())
                ConsoleText = "Connected succesful!\n";
            else
                ConsoleText = "Unable to connect to server or incorrect rcon password!";  
        }
        public async void SendCommand(){
            var cmd = Command.Trim();
            Command = string.Empty;
            if(!string.IsNullOrWhiteSpace(cmd))
            {    
			    var resp = await remoteConsole.SendCommandAsync(cmd);
                if(!string.IsNullOrWhiteSpace(resp))
                    ConsoleText += resp + "\n";
            }
        }
    }
}