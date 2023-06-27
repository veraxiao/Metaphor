using System;
using System.Threading;
using ThingMagic;

namespace ReadAsync
{
    class Example
    {
        public static void EsimerkkiMetodi()
        {
            try
            {
                //Reader olio luodaan ja sille asetetaan 
                //parametriksi lukijan IP osoite
                using (Reader r = Reader.Create("192.168.0.1"))                         
                {   
                    //"Connect" metodi yrittää yhdistää lukijaan
                    r.Connect();                                                        

                    //Tämä IF lause etsii ja asettaa toiminta alueen lukijalle
                    if (Reader.Region.UNSPEC == (Reader.Region)r.ParamGet("/reader/region/id"))
                    {
                        Reader.Region[] supportedRegions = (Reader.Region[])r.ParamGet("/reader/region/supportedRegions");
                        if (supportedRegions.Length < 1)
                        {
                            throw new FAULT_INVALID_REGION_Exception();
                        }
                        r.ParamSet("/reader/region/id", supportedRegions[0]);
                    }
                    //Nämä rivit luovat "SimplePlan" olion,
                    //joka asetetaan lukijan "plan" parametriksi.
                    SimpleReadPlan plan = new SimpleReadPlan();                         
                    r.ParamSet("/reader/read/plan", plan);

                    //Metodi "TagRead" käy toteen,
                    //kun lukija havaitsee tunnisteen.
                    r.TagRead += delegate (Object sender, TagReadDataEventArgs e)      
                    {                                                                  
                         //Tähän koodataan, mitä tehdään kun tunniste havaitaan.
                    };

                    //Tämä metodi käy toteen
                    //kun lukija havaitsee virheen
                    r.ReadException += new EventHandler<ReaderExceptionEventArgs>(r_ReadException);

                    //"StartReading" metodi aloittaa lukemisen
                    r.StartReading();
                    //Lukijan annetaan lukea 10 000 millisekunttia                           
                    Thread.Sleep(10000);                                                
                    //StopReading metodi pysäyttää lukemisen
                    r.StopReading();                                                    
                }
            }
            //CATCH jos lukemisessa sattuu virhe
            catch (ReaderException re) { Console.WriteLine(re.ToString()); }
            //CATCH jos TRY lauseessa sattuu virhe
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }                  


        }
            private static void r_ReadException(object sender, ReaderExceptionEventArgs e)
            {
                Console.WriteLine("Error: " + e.ReaderException.Message);
            }
    }
}

