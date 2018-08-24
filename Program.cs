using System.Threading;
using System;

namespace agsml_client
{
    class Program
    {
        static void Main(string[] args)
        {
        
        Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
        
        Console.WriteLine("Hello World!");
        }
       static void WorkThreadFunction()
        {
            try
                {
                    // do any background work
                string ags_text = System.IO.File.ReadAllText(@"C:\Users\ThomsonSJ\Downloads\107971\E7037A_Burton SWWM_Final Factual Report AGS4.ags");
                string ags_server = "ukcrd1pc40002"; int ags_port = 1045;
                AGS_Client  ac = new AGS_Client (ags_server, ags_port);
                ac.AGS_DATA = ags_text;
                Console.WriteLine("starting AGS_Client in ThreadId:" + Thread.CurrentThread.ManagedThreadId);
                ac.start();
                
            }
            catch (Exception ex)  {
                  // log errors
             Console.WriteLine (ex.Message);
            }
        }       

    }
}
