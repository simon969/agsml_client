using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Net;
using System.Text;
using System.Data.SqlClient;

namespace agsml_client
{
    public class AGS_Client
    {
        String ags_data = "";
        String xml_data = "";
        StreamWriter s_out = null;
        StreamReader s_in =  null;
        String db_connect = "";
        String db_updateAGS = "";
        String db_selectAGS = "";
        String db_paramNameAGS = "@ags_data";
        String db_updateXML = "";
        String db_paramNameXML = "@xml_data";
        String ags_fileNameIN = "";
        String ags_fileNameOUT = "";
        String xml_fileNameOUT = "";
        enumStatus status = enumStatus.AGSEmpty;
        TcpClient socket = null;

        const String AGS_START= "[ags_start]";
        const String AGS_END = "[ags_end]";
        const int MAX_BUFFER_SIZE = 1048;

        const String XML_START= "[xml_start]";
        const String XML_END = "[xml_end]";
 
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
        
        public void CloseConnections() {
            s_in.Close();
            s_out.Close();
        }
         public String data_AGS {
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
        public String data_XML
        {
            get
            {
                return xml_data;
            }
        }

        public String fileNameIN_AGS {
            set {
                ags_fileNameIN = value;
            }
        }
        public String fileNameOUT_XML {
            set {
                xml_fileNameOUT = value;
            }
        }
       public String fileNameOUT_AGS {
            set {
                ags_fileNameOUT = value;
            }
        }
        public void setDbConnect(String connect) {
          db_connect = connect;
        }
        public void setDbSelect_AGS(String select) {
            db_selectAGS = select;
        }
        public void setDbStatement_AGS(String statement, optional String paramName ="@p1") {
          db_updateAGS = statement;
          db_paramNameAGS = paramName;
        }
        public void setDbStatement_XML(String statement, otional String paramName="@p1") {
          db_updateXML = statement;
          db_paramNameXML = paramName;
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
            saveFileAGS(); 
            saveDatabaseAGS();
            status = enumStatus.AGSSaved;
        }

        private void readAGS() {
            if (ags_fileNameIN.Length > 0 && ags_data.Length==0) {
                readFileAGS();
            }    
            
            if (db_selectAGS.Length > 0 && ags_data.Length==0){
                readDatabaseAGS();
            }
            if (ags_data.Length>=0) {
                status = AGS_Client.enumStatus.AGSReceived;
            }
            
        }
        private void readFileAGS() {
        
        if (ags_fileNameIN.Length == 0) {
            return;
        }
                using (System.IO.StreamReader file = 
                new System.IO.StreamReader(ags_fileNameIN))
                    {
                        ags_data = file.ReadToEnd();
                    }  
        }
        private void readDatabaseAGS() {
        if (db_connect.Length == 0 || db_selectAGS.Length==0) {
            Console.WriteLine ("db_connect:" + db_connect);
            Console.WriteLine ("db_selectAGS:" + db_selectAGS);
            Console.WriteLine ("Insufficient parameters for readDatabaseAGS");
            return;
        }    
        
            using (SqlConnection db = new SqlConnection(db_connect)) {
                using (SqlCommand cmd = new SqlCommand(db_selectAGS, db)) {
                    //SqlCommand s = new SqlCommand("select * from foo where a=@xml", db);
                    //    cmd.Parameters.Add(db_paramNameXML, SqlDbType.Xml);
                        cmd.Parameters.Add(db_paramNameXML, SqlDbType.VarChar,1024*4);
                        cmd.Parameters[db_paramNameXML].Value = xml_data;
                    try {
                        db.Open();
                        cmd.ExecuteNonQuery();
                    } catch (SqlException e) {
                        Console.WriteLine (e.Message);
                    }
                }
            }    
        }
        private void saveFileAGS(){
        
        if (ags_fileNameOUT.Length == 0) {
            return;
        }
        
            using (System.IO.StreamWriter file = 
                new System.IO.StreamWriter(ags_fileNameOUT))
                    {
                        file.Write(ags_data);
                        file.Flush();
                    }
        }
        
        private void saveFileXML() {
        
        if (xml_fileNameOUT.Length == 0) {
            return;
        }
        
            using (System.IO.StreamWriter file = 
                new System.IO.StreamWriter(xml_fileNameOUT))
                    {
                        file.Write(xml_data);
                        file.Flush();
                    }
        }
        
        public void setDbConnect(String connect, 
                                 String statementXML, 
                                 String paramNameXML,
                                 String statementAGS,
                                 String paramNameAGS) {
           setDbConnect(connect);
           setDbStatement_AGS(statementAGS,paramNameAGS);
           setDbStatement_XML(statementXML,paramNameXML);          
        } 
        private void saveDatabaseXML() {

        if (db_connect.Length == 0 || db_updateXML.Length==0 || db_paramNameXML.Length==0) {
            Console.WriteLine ("db_connect:" + db_connect);
            Console.WriteLine ("db_statementXML:" + db_updateAGS);
            Console.WriteLine ("db_paramNameXML:" + db_paramNameAGS);
            Console.WriteLine ("Insufficient parameters for saveDatabaseXML");
            return;
        }    
     
            using (SqlConnection db = new SqlConnection(db_connect)) {
                using (SqlCommand cmd = new SqlCommand(db_updateXML, db)) {
                    //SqlCommand s = new SqlCommand("select * from foo where a=@xml", db);
                    //    cmd.Parameters.Add(db_paramNameXML, SqlDbType.Xml);
                        cmd.Parameters.Add(db_paramNameXML, SqlDbType.VarChar,1024*4);
                        cmd.Parameters[db_paramNameXML].Value = xml_data;
                    try {
                        db.Open();
                        cmd.ExecuteNonQuery();
                    } catch (SqlException e) {
                        Console.WriteLine (e.Message);
                    }
                }
            }
        }
        private void saveDatabaseAGS() {

        if (db_connect.Length == 0 || db_updateAGS.Length==0 || db_paramNameAGS.Length==0) {
            Console.WriteLine ("db_connect:" + db_connect);
            Console.WriteLine ("db_updateAGS:" + db_updateAGS);
            Console.WriteLine ("db_paramNameAGS:" + db_paramNameAGS);
            Console.WriteLine ("Insufficient parameters for saveDatabaseAGS");
            return;
        }    
     
            using (SqlConnection db = new SqlConnection(db_connect)) {
                using (SqlCommand cmd = new SqlCommand(db_updateAGS, db)) {
                    //SqlCommand s = new SqlCommand("select * from foo where a=@xml", db);
                    //    cmd.Parameters.Add(db_paramNameXML, SqlDbType.Xml);
                        cmd.Parameters.Add(db_paramNameAGS, SqlDbType.VarChar,1024*4);
                        cmd.Parameters[db_paramNameAGS].Value = ags_data;
                    try {
                        db.Open();
                        cmd.ExecuteNonQuery();
                    } catch (SqlException e) {
                        Console.WriteLine (e.Message);
                    }
                }
            }
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
                                sb.Append(s.Substring(XML_START.Length + 1));
                            }
                        } else {
                            if (s.IndexOf(XML_END)>0) {
                              sb.Append(s.Substring(0, s.Length - (XML_END.Length+1)));
                              break; 
                            } 
                            else {
                              sb.Append(s);  
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
            saveFileXML();
            saveDatabaseXML();
            status = enumStatus.XMLSaved;

        }


        public void start() {

            while (true) {
                
                if (status == AGS_Client.enumStatus.AGSEmpty) {
                    readAGS();
                }
                
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
                    CloseConnections();
                    break;
                }
                
            } ;
        }



}
}