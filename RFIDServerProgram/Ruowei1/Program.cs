using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ruowei1
{
    public class Program
    {
        public static Server servendaalen;
        static void Main(string[] args)
        {
            
            servendaalen = new Server();

            Thread firstThread = new Thread(servendaalen.FirstThread);
            firstThread.Start();
            Console.WriteLine("FirstThread Started");

            Thread secondThread = new Thread(SecondThread);
            secondThread.Start();
            Console.WriteLine("SecondThread Started");

            while (servendaalen.exit)
            {
                Thread.Sleep(500);
            }
            Console.WriteLine("Program stopped");
            Console.ReadLine();
        }

        static void SecondThread()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            while (servendaalen.exit)
            {

                while (servendaalen.StartReading == false)
                {

                    Thread.Sleep(100);
                    if (servendaalen.exit == false)
                    {
                        break;
                    }
                }
                if (servendaalen.exit)
                {
                    Konehuone.ReadTags();
                }
            }
            Console.WriteLine("SecondThread thread finished");
        }
    }
}
