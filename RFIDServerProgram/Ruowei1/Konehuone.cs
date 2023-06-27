using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThingMagic;

namespace Ruowei1
{
    public class Konehuone
    {
        public Konehuone() { }

        public static int ReadWritePower { get; set; } = 2300; //Configure the ReadWrite Power when necessary
        public static string ReaderIp { get; set; }
        public static bool StartReading { get; set; }

        public static void ReadTags()
        {
            using (var r = Reader.Create($"tmr:///COM3"))  //Configure the reader connection when necessary
            {

                bool conenction = ConnectToReader(r);

                if (conenction == false)
                {
                    //StartReading = false;
                    Program.servendaalen.StartReading = false;
                    return;
                }

                r.TagRead += delegate (Object sender, TagReadDataEventArgs e)
                {
                    string epc_int = e.TagReadData.EpcString;

                    if (epc_int.Length == 4 || epc_int.Length == 8)
                    {
                        _ = Task.Run(() => {
                            Console.WriteLine(epc_int + " tag sended to " + Server.clientsIp + ":11000");
                            //Console.WriteLine(e.TagReadData.ToString());
                            //Server.SendMessage(e.TagReadData.ToString());
                            Server.SendTagData(e.TagReadData.EpcString);
                        });
                    }
                };

                r.StartReading();
                Console.WriteLine("Reading started!");
                Server.SendMessage("Reading started");

                while (Program.servendaalen.StartReading == true)
                {
                    Thread.Sleep(200);
                }

                r.StopReading();
                Console.WriteLine("Reading stopped");
                Server.SendMessage("Reading stopped");
            }
        }

        static bool ConnectToReader(Reader r)
        {
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;
            bool error = false;

            Task t = Task.Run(() => {
                try
                {
                    r.Connect();
                }
                catch
                {
                    Console.WriteLine("Connection could not be established!");
                    Server.SendMessage("Connection could not be established!");
                    error = true;
                }

            }, tokenSource2.Token);



            int i = 0;

            while (i < 1)
            {
                if (t.IsCompleted == false)
                    Console.WriteLine("Connecting...");
                Server.SendMessage("Connecting...");

                Thread.Sleep(1000);
                i++;
            }

            if (t.IsCompleted == false || error)
            {
                tokenSource2.Cancel();
                Console.WriteLine("Connection Cancelled");
                Server.SendMessage("Connection Cancelled");
                //StartReading = false;
                return false;
            }



            if (Reader.Region.UNSPEC == (Reader.Region)r.ParamGet("/reader/region/id"))
            {
                Reader.Region[] supportedRegions = (Reader.Region[])r.ParamGet("/reader/region/supportedRegions");
                if (supportedRegions.Length < 1)
                {
                    throw new FAULT_INVALID_REGION_Exception();
                }
                r.ParamSet("/reader/region/id", supportedRegions[0]);
            }

            SimpleReadPlan plan = new SimpleReadPlan();
            r.ParamSet("/reader/read/plan", plan);
            r.ParamSet("/reader/radio/readPower", ReadWritePower);
            r.ParamSet("/reader/radio/writePower", ReadWritePower);
            //r.ParamSet("/reader/read/asyncOffTime", 20);


            r.ReadException += new EventHandler<ReaderExceptionEventArgs>(r_ReadException);

            return true;
        }

        private static void r_ReadException(object sender, ReaderExceptionEventArgs e)                         //-- 
        {
            Console.WriteLine("ReaderException:" + e.ReaderException.Message);
            Server.SendMessage("ReaderException:" + e.ReaderException.Message);
        }
    }
}
