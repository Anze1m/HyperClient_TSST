using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HyperClient
{
    public class CPCC
    {
        Form1 mainWindow;
        int CPCCport;
        int NCCport;

        public CPCC(int CPCCport, int NCCport)
        {
            this.CPCCport = CPCCport;
            this.NCCport = NCCport;
        }
        public void setConnectionToLogViewer(Form1 mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void sendCallRequest()
        {
            //stworzenie na podstawie przekazanych argumentów requesta i wysłąnie na NCCport
            string stringToLog = "";
            //stworzenie stringa do zalogowania w zależności od zawartości komunikatu

            string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
            mainWindow.Invoke(new Action(delegate () {
                mainWindow.logSentOrder(stringToLog, currentTime);
            }));
        }

        public void sendHangUpRequest()
        {
            //stworzenie na podstawie przekazanych argumentów requesta i wysłąnie na NCCport
            string stringToLog = "";
            //stworzenie stringa do zalogowania w zależności od zawartości komunikatu

            string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
            mainWindow.Invoke(new Action(delegate () {
                mainWindow.logSentOrder(stringToLog, currentTime);
            }));
        }
        public void listen()
        {
            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint manager = (EndPoint)managingEndPoint;
            Socket orderingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), CPCCport);
            orderingSocket.Bind(endPoint);
            while (true)
            {
                byte[] buffer = new byte[orderingSocket.SendBufferSize];
                int readBytesNumber = orderingSocket.ReceiveFrom(buffer, ref manager);
                byte[] receivedData = new byte[readBytesNumber];
                Array.Copy(buffer, receivedData, readBytesNumber);

                //deserializacja receivedData
                string stringToLog = "";
                //stworzenie stringa do zalogowania w zależności od zawartości komunikatu

                string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                mainWindow.Invoke(new Action(delegate () {
                    mainWindow.logReceivedOrder(stringToLog, currentTime);
                }));
            }
        }
    }
}
