using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System;
using System.Diagnostics;

namespace AGSML_WebApplication
{
    public class AGSML_Client
    {
        private StreamWriter swSender;
        private StreamReader srReceiver;
        private TcpClient tcpServer;
        private Thread thrMessaging;
        private IPAddress ipAddr;
        private bool Connected;

        public AGSML_Client(string ipAddress, int port)
        {
            // Parse the IP address
            // ipAddr = IPAddress.Parse(ipAddress);
            
            // Start a new TCP connections to the chat server
            tcpServer = new TcpClient();

            try
            {
                tcpServer.Connect(ipAddress, port);
                swSender = new StreamWriter(tcpServer.GetStream());


                // Start the thread for receiving messages and further communication
                thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
                thrMessaging.Start();
                Connected = true;
            }
            catch (System.Exception e2)
            {
                //  MessageBox.Show(e2.ToString());
                Debug.WriteLine(e2.ToString());
            }
        }
        private void ReceiveMessages()
        {
            // Receive the response from the server
            srReceiver = new StreamReader(tcpServer.GetStream());
            while (Connected)
            {
                String con = srReceiver.ReadLine();
                string StringMessage = HttpUtility.UrlDecode(con, System.Text.Encoding.UTF8);

                processMessage(StringMessage);



            }
        }
        private void processMessage(String p)
        {
            Debug.WriteLine(p);
        }
        private void SendMessage(String p)
        {
            if (p != "")
            {
                p = HttpUtility.UrlEncode(p, System.Text.Encoding.UTF8);
                swSender.WriteLine(p);
                swSender.Flush();

            }

        }
    }
}


