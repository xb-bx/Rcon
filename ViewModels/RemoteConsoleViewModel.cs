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


        private RemoteConsole remoteConsole = new RemoteConsole(null, 9, null);
        public void Connect()
        {
            remoteConsole.IP = IP;
            remoteConsole.Port = int.Parse(Port);
            remoteConsole.Password = RconPassword;
            Task.Run(() => 
            {
                if(remoteConsole.Connect())
                    ConsoleText = "Connected succesful!";
                else
                    ConsoleText = "Unable to connect to server or incorrect rcon password!"; 
            });
        }
        public void SendCommand(){
            var cmd = Command.Trim();
            Command = string.Empty;
            if(!string.IsNullOrWhiteSpace(cmd))
            {			
                new Task(() => 
                {    
			        var resp = remoteConsole.SendCommand(cmd);
                    
                    ConsoleText += resp + "\n";
                }).Start();
            }
        }
    }
}