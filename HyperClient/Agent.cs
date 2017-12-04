using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperClient
{
    class Agent
    {
        public Agent(Dictionary<int, string> routingTable, string port, string callSign, Form1 mainWindow)
        {
            
            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint manager = (EndPoint)managingEndPoint;
            int portNumber = Int32.Parse(port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            socket.Bind(endPoint);
            Thread keepAliveThread = new Thread(() => keepAlive(callSign));
            keepAliveThread.Start();

            while (true)
            {
                byte[] buffer = new byte[socket.SendBufferSize];
                int readBytesNumber = socket.ReceiveFrom(buffer, ref manager);
                byte[] receivedData = new byte[readBytesNumber];
                Array.Copy(buffer, receivedData, readBytesNumber);
                ManagementDataObjects.MdoForHost MdoForHost = ByteToMdoForHost(receivedData);

                string currentTime = " <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">\n";
                switch (MdoForHost.Command)
                {
                    case "Add":
                        if(routingTable.ContainsKey(MdoForHost.Label))
                        {
                            
                            mainWindow.Invoke(new Action(delegate () {
                                mainWindow.logReceivedOrder("Changed destination from " + routingTable[MdoForHost.Label] + " to " + MdoForHost.Destination + " at label " + MdoForHost.Label.ToString(), currentTime);
                                mainWindow.removeDestination(MdoForHost.Label + " -> " +routingTable[MdoForHost.Label]);
                            }));
                            routingTable[MdoForHost.Label] = MdoForHost.Destination;
                            mainWindow.Invoke(new Action(delegate () {
                                mainWindow.addDestination(MdoForHost.Label + " -> " + MdoForHost.Destination);
                            }));
                        }
                        else
                        {
                            routingTable.Add(MdoForHost.Label, MdoForHost.Destination);
                            mainWindow.Invoke(new Action(delegate () {
                                mainWindow.logReceivedOrder("Added destination " + MdoForHost.Destination + " at label " + MdoForHost.Label.ToString(), currentTime);
                                mainWindow.addDestination(MdoForHost.Label + " -> " + MdoForHost.Destination);
                            }));
                        }
                        
                        break;
                    case "Remove":
                        if (routingTable.ContainsKey( MdoForHost.Label) && routingTable[MdoForHost.Label].Equals(MdoForHost.Destination))
                        {
                            routingTable.Remove(MdoForHost.Label);

                            mainWindow.Invoke(new Action(delegate ()
                            {
                                mainWindow.logReceivedOrder("Removed destination " + MdoForHost.Destination + " at label " + MdoForHost.Label.ToString(), currentTime);
                                mainWindow.removeDestination(MdoForHost.Label + " -> " + MdoForHost.Destination);
                            }));
                        }
                            break;
                    default:
                        break;
                }
                
                
            }

        }

        static private ManagementDataObjects.MdoForHost ByteToMdoForHost(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object o = (object)bf.Deserialize(ms);
            return (ManagementDataObjects.MdoForHost)o;
        }
        private static void keepAlive(string name)
        {
            Thread.Sleep(1000);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(name));

            Socket helloes = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint helloPort = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 56000);
            while (true)
            {
                helloes.SendTo(buffer, helloPort);
                Thread.Sleep(500);
            }
        }
    }
}
