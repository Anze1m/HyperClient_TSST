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
    public class Sender
    {
        Socket socket;
        byte gate;
        Form1 mainWindow;
        bool sendingAllowance;
        public Sender(string destinationPort, byte gate)
        {

            this.gate = gate;
            int port;
            if (Int32.TryParse(destinationPort, out port))
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                IPAddress iPAddress = iPHostEntry.AddressList[0];
                socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEnd = new IPEndPoint(iPAddress, port);
                while (true)
                {
                    try
                    {
                        socket.Connect(ipEnd);
                        break;
                    }
                    catch (SocketException ex)
                    {

                    }
                }


            }
        }
        public void send(string message, string label, int numberOfPacket, int sendingInterval, bool randomization)
        {
            sendingAllowance = true;

            
            int[] labels = new int[1];
            labels[0] = Int32.Parse(label);
            
                if (randomization)
                {
                    int randomByte;
                    int miliseconds = sendingInterval;
                    int number = numberOfPacket;
                    for (int i = 0; i < number; i++)
                    {
                        if (!sendingAllowance)
                        {
                            break;
                        }
                        string stringToSend = message;
                        Random randomNumber = new Random();
                        int multiplication = randomNumber.Next(0, 4);
                        for (int j = 0; j < multiplication; j++)
                            stringToSend = stringToSend + ";" + message;
                        stringToSend = stringToSend + "(" + (multiplication + 1) + ")";
                        MPLS.MPLSPacket[] packet = new MPLS.MPLSPacket[1];
                        packet[0] = new MPLS.MPLSPacket(labels, stringToSend);
                        MPLS.AggregatePacket aggregate = new MPLS.AggregatePacket(packet);
                        MPLS.BinaryWrapper messageToSend = MPLS.MPLSMethods.Serialize(aggregate);
                        messageToSend.interfaceId = gate;
                        randomByte = randomNumber.Next(0, 255);
                        messageToSend.randomNumber = Byte.Parse(randomByte.ToString());
                        socket.Send(messageToSend.HeaderPlusData());
                        string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                        mainWindow.Invoke(new Action(delegate () {
                            mainWindow.logSentPacket(stringToSend, currentTime);
                        }));
                        Thread.Sleep(miliseconds);
                    }
                }
                else
                {
                    MPLS.MPLSPacket[] packet = new MPLS.MPLSPacket[1];
                    packet[0] = new MPLS.MPLSPacket(labels, message);
                    MPLS.AggregatePacket aggregate = new MPLS.AggregatePacket(packet);
                    MPLS.BinaryWrapper messageToSend = MPLS.MPLSMethods.Serialize(aggregate);
                    messageToSend.interfaceId = gate;
                    Random random = new Random();
                    int randomByte;
                    int miliseconds = sendingInterval;
                    int number = numberOfPacket;
                    for (int i = 0; i < number; i++)
                    {
                        if (!sendingAllowance)
                        {
                            break;
                        }
                        randomByte = random.Next(0, 255);
                        messageToSend.randomNumber = Byte.Parse(randomByte.ToString());
                        socket.Send(messageToSend.HeaderPlusData());
                        string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                        mainWindow.Invoke(new Action(delegate () {
                            mainWindow.logSentPacket(message, currentTime);
                        }));
                        Thread.Sleep(miliseconds);
                    }
                }

            
        }

        public void setConnectionToLogViewer(Form1 window)
        {
            this.mainWindow = window;
        }

        public void stopSending()
        {
            sendingAllowance = false;
        }
        
    }

    
}
