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
        Socket send;

        public CPCC(int CPCCport, int NCCport)
        {
            this.CPCCport = CPCCport;
            this.NCCport = NCCport;
        }
        public void setConnectionToLogViewer(Form1 mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void sendCallRequest(string callSign, string receiverName, int label, int capacity)
        {
            CPCCcommunications.CallRequest callRequest = new CPCCcommunications.CallRequest(callSign, receiverName, CPCCport, label, capacity, 0);

            byte[] sendbuf = Communications.Serialization.Serialize(callRequest);
            send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            send.SendTo(sendbuf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), NCCport));

            string stringToLog = "CallRequest was sent (D:" + receiverName +" L:"+ label + " C:" + capacity + ")";

            string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
            mainWindow.Invoke(new Action(delegate () {
                mainWindow.logSentOrder(stringToLog, currentTime);
            }));
        }

        public void sendHangUpRequest(string callSign, int label)
        {
            CPCCcommunications.HangUpRequest hangUpRequest = new CPCCcommunications.HangUpRequest(callSign, CPCCport, label);

            byte[] sendbuf = Communications.Serialization.Serialize(hangUpRequest);
            send = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            send.SendTo(sendbuf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), NCCport));

            string stringToLog = "CallRequest was sent (L:" + label + ")";

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
                string stringToLog = "";
                Communications.Message message = Communications.Serialization.Deserialize(receivedData);
                if(message.messageType.Equals("ConnectionDown"))
                {
                    CPCCcommunications.ConnectionDown connectionDown = (CPCCcommunications.ConnectionDown)message;
                    stringToLog = "Connection at label " + connectionDown.label + " is down";
                }
                else if(message.messageType.Equals("ConnectionReady"))
                {
                    CPCCcommunications.ConnectionReady connectionReady = (CPCCcommunications.ConnectionReady)message;
                    if(connectionReady.ready)
                    {
                        stringToLog = "Connection at label " + connectionReady.label + " established";
                    }
                    else
                    {
                        stringToLog = "Connection at label " + connectionReady.label + " failed";
                    }
                }
                else if (message.messageType.Equals("ConnectionReady"))
                {
                    CPCCcommunications.CallNotification callNotification = (CPCCcommunications.CallNotification)message;
                    if (callNotification.ready)
                    {
                        stringToLog = "Incoming call from " + callNotification.senderName;
                    }
                    else
                    {
                        stringToLog = "Connection with " + callNotification.senderName + " expired";
                    }
                }

                    string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                mainWindow.Invoke(new Action(delegate () {
                    mainWindow.logReceivedOrder(stringToLog, currentTime);
                }));
            }
        }
    }
}
