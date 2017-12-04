using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyperClient
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string callSign;
            string destinationPort;
            string listeningPort;
            string agentPort;
            byte gate;
            if (args.Length > 0)
            {
                callSign = args[0];
                destinationPort = args[1];
                listeningPort = args[2];
                gate = Byte.Parse(args[3]);
                agentPort = args[4];
            }
            else
            {
                callSign = "H.1";
                destinationPort = "7711";
                listeningPort = "7712";
                gate = Byte.Parse("1");
                agentPort = "56001";
            }
            Dictionary< int, string> routingTable = new Dictionary<int, string>();
            Sender sender = new Sender(destinationPort, gate);
            Form1 mainWindow = new Form1(sender, callSign);
            mainWindow.allowSenderToLog();
            Thread agentThread = new Thread(() => new Agent(routingTable, agentPort, callSign, mainWindow));
            agentThread.Start();
            Receiver receiver = new Receiver(listeningPort, mainWindow);
            Application.EnableVisualStyles();
            Thread recThread = new Thread(() => receiver.receive());
            recThread.Start();
            Application.Run(mainWindow);
            Environment.Exit(0);
        }
    }
}
