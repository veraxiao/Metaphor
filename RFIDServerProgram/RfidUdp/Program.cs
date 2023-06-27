using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RfidUdp
{
    class Program
    {
        static Thread UdpServerThread;
        static bool Running = true;

        static void Main(string[] args)
        {
            UdpServerThread = new Thread(new ThreadStart(UdpServer));
            UdpServerThread.Start();

            UdpSender();

            Console.ReadKey();
        }



        static void UdpSender()
        {
            UdpClient client = new UdpClient(58191);

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11001);
            client.Connect(ep);

            while (Running)
            {
                string a = Console.ReadLine();

                if (a == "Close")
                {
                    Running = false;
                    break;
                }

                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(a);
                client.Send(outStream, outStream.Length);
            }
        }

        static void UdpServer()
        {
            UdpClient udpServer = new UdpClient(11000);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);

            while (Running)
            {
                byte[] data = new byte[] { };
                Task task = Task.Run(() => { data = udpServer.Receive(ref remoteEP); });

                while (task.IsCompleted == false)
                {
                    Thread.Sleep(10);
                    if (Running == false)
                    {
                        Console.WriteLine("Cancel task!");
                        break;
                    }
                }

                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine(DateTime.Now.Millisecond.ToString() + " receive data from " + remoteEP.ToString() + " " + message);

            }

        }

    }
}
