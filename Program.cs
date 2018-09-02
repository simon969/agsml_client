using System.Threading;
using System;

namespace agsml_client 

{
    class Program

    {
        static void Main(string[] args)
        {

        Thread[] array = new Thread[2];

        array[0] = new Thread(new ThreadStart(WorkThreadFunction0));
        array[0].Start();
        
        array[1] = new Thread(new ThreadStart(WorkThreadFunction1));
        array[1].Start();
        
        Console.WriteLine("Exiting Main()");
        

        }

        static void AGS_Client (String ags_server,int ags_port, String ags_fileIN, String xml_fileOUT) {
            try
                {
                AGS_Client  ac = new AGS_Client (ags_server, ags_port);
                ac.fileNameIN_AGS = ags_fileIN;
                ac.fileNameOUT_XML = xml_fileOUT;
                Console.WriteLine("starting AGS_Client in ThreadId:" + Thread.CurrentThread.ManagedThreadId);
                ac.start();
                 }
            catch (Exception ex)  {
                  // log errors
             Console.WriteLine (ex.Message);
            }
        }
       static void WorkThreadFunction0() {
                    AGS_Client ("localhost", 1045,
                    @"C:\Users\ThomsonSJ\Downloads\107971\E7037A_Burton SWWM_Final Factual Report AGS4_NOERES.ags",
                    @"C:\Users\ThomsonSJ\Downloads\107971\E7037A_Burton SWWM_Final Factual Report AGS4_NOERES.xml");
        }       

       static void WorkThreadFunction1() {
                    AGS_Client ("localhost", 1045,
                    @"C:\Users\ThomsonSJ\Documents\NetBeansProjects\AGSML_examples\ags\BHDISK1.AGS",
                    @"C:\Users\ThomsonSJ\Documents\NetBeansProjects\AGSML_examples\agsml\BHDISK1.xml");
        } 
    }
}
