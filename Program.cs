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

        static void runAGS_Client (String ags_server,int ags_port, String ags_fileIN, String xml_fileOUT) {
            try
                {
                AGS_Client  ac = new AGS_Client (ags_server, ags_port);
                ac.fileNameIN_AGS = ags_fileIN;
                ac.fileNameOUT_XML = xml_fileOUT;
                ac.setDbConnect = "server=UKLON3SQ033\CORPDB_2012;Database=gedata;Trusted_Connection=True;"
                ac.setDbStatement_AGS ("insert into ge_data (projectid,ags_data) values (101, @p1")
                ac.setDbStatement_XML ("insert ge_data (projectid,xml_data) values (101, @p1")
                Console.WriteLine("starting AGS_Client in ThreadId:" + Thread.CurrentThread.ManagedThreadId);
                ac.start();
                 }
            catch (Exception ex)  {
                  // log errors
             Console.WriteLine (ex.Message);
            }
        }
       static void WorkThreadFunction0() {
                    runAGS_Client ("localhost", 1045,
                    @"C:\Users\ThomsonSJ\Downloads\107971\E7037A_Burton SWWM_Final Factual Report AGS4_NOERES.ags",
                    @"C:\Users\ThomsonSJ\Downloads\107971\E7037A_Burton SWWM_Final Factual Report AGS4_NOERES.xml");
        }       

       static void WorkThreadFunction1() {
                    runAGS_Client ("localhost", 1045,
                    @"C:\Users\ThomsonSJ\Documents\NetBeansProjects\AGSML_examples\ags\BHDISK1.AGS",
                    @"C:\Users\ThomsonSJ\Documents\NetBeansProjects\AGSML_examples\agsml\BHDISK1.xml");
        } 
    }
}
