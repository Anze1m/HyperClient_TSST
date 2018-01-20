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
            int CPCCport;
            int NCCport;
            string name;
            byte gate;
            if (args.Length > 0)
            {
                name = args[0];
                callSign = args[1];
                destinationPort = args[2];
                listeningPort = args[3];
                gate = Byte.Parse(args[4]);
                CPCCport = Int32.Parse(args[5]);
                NCCport = Int32.Parse(args[6]);
            }
            else
            {
                name = "Abacki";
                callSign = "H.2";
                destinationPort = "7711";
                listeningPort = "7712";
                gate = Byte.Parse("1");
                CPCCport = 56001;
                NCCport = 56002;
            }
            Sender sender = new Sender(destinationPort, gate);
            CPCC cpcc = new CPCC(CPCCport, NCCport);

            Form1 mainWindow = new Form1(sender, cpcc, name);
            mainWindow.allowSenderToLog();
            mainWindow.allowCPCCToLog();

            Thread cpccThread = new Thread(() => cpcc.listen());
            cpccThread.Start();
                       
            Receiver receiver = new Receiver(listeningPort, mainWindow);
            Application.EnableVisualStyles();
            Thread recThread = new Thread(() => receiver.receive());
            recThread.Start();
            Application.Run(mainWindow);
            Environment.Exit(0);
        }
    }
}
