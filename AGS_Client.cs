using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Net;
using System.Text;

namespace agsml_client
{
    public class AGS_Client
    {
        String ags_data;
        String xml_data;
        StreamWriter s_out;
        StreamReader s_in;
       
        enumStatus status = enumStatus.AGSEmpty;
        TcpClient socket;

        const String AGS_START= "[AGS_START]";
        const String AGS_END = "[AGS_END]";
        const int MAX_BUFFER_SIZE = 1048;

        const String XML_START= "[XML_START]";
        const String XML_END = "[XML_END]";

        enum enumStatus
        {
            AGSEmpty,
            AGSReceived,
            AGSSaved,
            AGSSent,
            XMLReceived,
            XMLSaved,
        }

        public AGS_Client(String ags_server, int port)
        {
          //  socket = ConnectSocket(ags_server, port);
        try {

            socket = new TcpClient();
            socket.Connect(ags_server, port);

            Stream networkStream = socket.GetStream();

            s_in = new StreamReader(networkStream);
            s_out = new StreamWriter(networkStream) { NewLine = "\r\n", AutoFlush = true };

            Console.WriteLine ("Connected to " + ags_server + ":" + port);

        } catch (Exception e){
         Console.WriteLine(e.Message);
        }

        }
        
         public String AGS_DATA {
            get {
            return ags_data;
            }
            set {
            ags_data = value;
            
            if (ags_data.Length > 0) {
              status = enumStatus.AGSReceived;
            } else {
              status = enumStatus.AGSEmpty;  
            }
            
            }
        }
        public String XML_DATA
        {
            get
            {
                return xml_data;
            }
            set
            {
                xml_data = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void sendAGS() {
           try {

                StringBuilder sb = new StringBuilder();
                sb.Append(AGS_START);
                sb.AppendLine();
                sb.Append(ags_data);
                sb.AppendLine();
                sb.Append(AGS_END);

                String wrapped_ags_data = sb.ToString();

                TextReader tr = new StringReader(wrapped_ags_data);

                String line = "";

                while ((line = tr.ReadLine()) != null) {
                    s_out.Write(line);
                    s_out.WriteLine();
                }

                s_out.Flush();

                status = enumStatus.AGSSent;

            } catch (Exception e)  {
                Console.WriteLine (e.Message);
            }
        }
        private void saveAGS() {
            // Write ags_data to database
            status = enumStatus.AGSSaved;
        }
        private void readXML() {
            // recieve xml data from ags_server 
            // https://stackoverflow.com/questions/5867227/convert-streamreader-to-byte

            try {
                Boolean IsXMLData = false;
                int read;
                byte[] buffer = new byte[MAX_BUFFER_SIZE];
                // MemoryStream ms = new MemoryStream();
                StringBuilder sb =  null; 
                while ((read = s_in.BaseStream.Read(buffer, 0, MAX_BUFFER_SIZE)) > 0) {
                        // ms.Write(buffer, 0, read);
                        string s = System.Text.Encoding.UTF8.GetString(buffer, 0, read);
                        if (IsXMLData==false) {
                            if (s.IndexOf(XML_START)==0) {
                                IsXMLData=true;
                                sb =  new StringBuilder();
                                sb.Append(s.Substring(XML_START.Length));
                            }
                        else {
                            if (s.IndexOf(XML_END)>0) {
                              sb.Append(s.Substring(0, s.Length - XML_END.Length));  
                            } 
                            else {
                              sb.Append(s);  
                            }
                        }
                        }

                        
                    }

                //    xml_data = Encoding.UTF8.GetString(ms.ToArray());
                
                xml_data = sb.ToString();
                
                status = enumStatus.XMLReceived;

            } catch (Exception e) {
                Console.WriteLine (e.Message);
            }
            
        }
        private void saveXML() {
            // Write xmldata to database
            status = enumStatus.XMLSaved;

        }


        public void start() {

            while (true) {
                if (status == AGS_Client.enumStatus.AGSReceived) {
                    saveAGS();
                }

                if (status == AGS_Client.enumStatus.AGSSaved) {
                    sendAGS();
                }

                if (status == AGS_Client.enumStatus.AGSSent) {
                    readXML();
                }

                if (status == AGS_Client.enumStatus.XMLReceived) {
                    saveXML();
                }

                if (status == AGS_Client.enumStatus.XMLSaved) {
                    break;
                }
                
            } ;
        }



}
}