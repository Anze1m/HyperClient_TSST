using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperClient
{
    class Receiver
    {
        Socket socket;
        Form1 mainWindow;

        public Receiver(string listeningPort, Form1 mainWindow)
        {
            this.mainWindow = mainWindow;
            int port;

            if (Int32.TryParse(listeningPort, out port))
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                IPAddress iPAddress = iPHostEntry.AddressList[0];
                socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEnd = new IPEndPoint(iPAddress, port);
                socket.Bind(ipEnd);
                socket.Listen(0);
                socket = socket.Accept();
            }
        }
        

        public void receive()
        {
            while (true)
            {
                byte[] Buffer = new byte[socket.SendBufferSize];
                int readByte = socket.Receive(Buffer);
                byte[] receivedData = new byte[readByte];
                Array.Copy(Buffer, receivedData, readByte);
                MPLS.BinaryWrapper receivedMessage = new MPLS.BinaryWrapper(receivedData, true);
                MPLS.AggregatePacket receivedAggregate = MPLS.MPLSMethods.Deserialize(receivedMessage);
                int numberOfPackets = receivedAggregate.packets.Length;
                string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                for (int i = 0; i<numberOfPackets; i++)
                {
                    if (receivedAggregate.packets[i].labels.Last() == 0)
                        continue;
                    string receivedString = receivedAggregate.packets[i].data;
                    mainWindow.Invoke(new Action(delegate () {
                        mainWindow.logReceivedPacket(receivedString, currentTime);
                    }));
                }
            }
        }
    }
}
