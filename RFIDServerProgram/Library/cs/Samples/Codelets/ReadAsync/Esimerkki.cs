using System;
using System.Threading;
using ThingMagic;

namespace ReadAsync
{
    class Esimerkki
    {
        public static void EsimerkkiMetodi()
        {
            try
            {
                //Reader olio luodaan ja sille asetetaan 
                //parametriksi lukijan IP osoite
                using (Reader r = Reader.Create("192.168.0.1"))                         //Reader olio luodaan ja sille
                {                                                                       //asetetaan parametriksi lukijan IP osoite

                    r.Connect();                                                        //"Connect" metodi yrittää yhdistää lukijaan

                    if (Reader.Region.UNSPEC == (Reader.Region)r.ParamGet("/reader/region/id"))                             //Tämä IF lause etsii ja asettaa
                    {                                                                                                       //toiminta alueen lukijalle.
                        Reader.Region[] supportedRegions = (Reader.Region[])r.ParamGet("/reader/region/supportedRegions");  
                        if (supportedRegions.Length < 1)
                        {
                            throw new FAULT_INVALID_REGION_Exception();
                        }
                        r.ParamSet("/reader/region/id", supportedRegions[0]);
                    }
  
                    SimpleReadPlan plan = new SimpleReadPlan();                         //Nämä rivit luovat "SimplePlan" olion,
                    r.ParamSet("/reader/read/plan", plan);                              //joka asetetaan lukijan "plan" parametriksi.

                    r.TagRead += delegate (Object sender, TagReadDataEventArgs e)       //Metodi "TagRead" käy toteen,
                    {                                                                   //kun lukija havaitsee tunnisteen
                        
                    };

                    r.StartReading();                                                   //"StartReading" metodi aloittaa lukemisen
                                                
                    Thread.Sleep(10000);                                                //Lukijan annetaan lukea 10 000 millisekunttia 
                    r.StopReading();                                                    //StopReading metodi pysäyttää lukemisen
                }
            }
            catch (ReaderException re) { Console.WriteLine(re.ToString()); }            //CATCH jos lukemisessa sattuu virhe
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }                  //CATCH jos TRY lauseessa sattuu virhe
        }
    }
}
