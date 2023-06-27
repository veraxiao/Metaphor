using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ruowei1
{
    public class Server
    {
        //private DatagramSocket datagramSocket;
        public static UdpClient udpReceiver = new UdpClient(11001);
        public static UdpClient udpSender = new UdpClient(11005);
        public Boolean StartReading = false;
        public Boolean exit = true;
        public String IP = "";
        public String clientIpAddress = "";
        public static List<VR_Tag> VR_Tags { get; set; }
        public static IPAddress clientsIp { get; set; }

        public Server()
        {
        }

        public static void SendMessage(string messageBack)
        {
            byte[] outStream = Encoding.ASCII.GetBytes(messageBack);
            udpSender.Send(outStream, outStream.Length);
        }

        public static void SendTagData(string epc)
        {
            foreach(var vr_tag in VR_Tags)
            {
                if(vr_tag.EpcName == epc)
                {
                    string messageBack = Newtonsoft.Json.JsonConvert.SerializeObject(vr_tag);
                    byte[] outStream = Encoding.ASCII.GetBytes(messageBack);
                    udpSender.Send(outStream, outStream.Length);
                }
            }
        }

        public void FirstThread()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 11001);
            Task task;
            //UdpClient udp = new UdpClient(11001);

            VR_Tags = VR_Tag.ExtractFromCsvFile();

            while (exit)
            {
                try
                {
                    byte[] data = new byte[] { };

                    //data = udpClient.Receive(ref ep);
                    task = Task.Run(() => { data = udpReceiver.Receive(ref ep); });

                    while (task.IsCompleted == false)
                    {
                        if (exit == false) break;
                        Thread.Sleep(100);
                    }
                    if (exit == false) break;

                    var receivedData = data;
                    if (clientsIp != ep.Address)
                    {
                        clientsIp = ep.Address;
                        IPEndPoint endpoint = new IPEndPoint(clientsIp, 11000);
                        udpSender.Connect(clientsIp, 11000);
                    }

                    string messageFromClient = Encoding.ASCII.GetString(receivedData);

                    String messageBack = "";

                    if (messageFromClient.Contains("StartReading"))
                    {
                        StartReading = true;
                        messageBack = messageFromClient + " : true";
                    }
                    if (messageFromClient.Contains("StopReading"))
                    {
                        StartReading = false;
                        messageBack = messageFromClient + " : false";
                    }
                    if (messageFromClient.Contains("Exit"))
                    {
                        exit = false;
                        messageBack = messageFromClient + " : false";
                    }
                    if (messageFromClient.Contains("IP:"))
                    {

                        String[] list = messageFromClient.Split(':');
                        IP = list[1];
                        messageBack = "IP address changed to " + messageFromClient;
                    }

                    if (messageBack == "")
                    {
                        messageBack = "Program did not understand sended message: " + messageFromClient;
                    }
                    Console.WriteLine(messageBack);

                    SendMessage(messageBack);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception : " + ex.Message);
                }
            }

            Console.WriteLine("FirstThread finished");
        }
    }
}
