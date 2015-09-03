using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.FtpClient;

namespace FTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FTpInfo ftpInfo = new FTpInfo();
            ////TestFTPServer(ftpInfo);
            ////UploadFromLocalStorage(ftpInfo);
            ////UploadFromStream(ftpInfo);
            UploadFromDicStream(ftpInfo);
        }

        public static void TestFTPServer(FTpInfo ftpInfo)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpInfo.Host);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.PassWord);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            List<string> result = new List<string>();

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        public static void UploadFromLocalStorage(FTpInfo ftpInfo)
        {
            var fileName = Path.GetFileName(ftpInfo.FileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftpInfo.Host, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.PassWord);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            using (var fileStream = File.OpenRead(ftpInfo.FileName))
            {
                using (var requestStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(requestStream);
                    requestStream.Close();
                }
            }

            var response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine("Upload done: {0}", response.StatusDescription);
            response.Close();
        }

        public static void UploadFromStream(FTpInfo ftpInfo)
        {
            var fileName = Path.GetFileName(ftpInfo.FileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftpInfo.Host, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.PassWord);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            Stream stream = File.OpenRead(ftpInfo.FileName);

            using (var requestStream = request.GetRequestStream())
            {
                stream.CopyTo(requestStream);
                requestStream.Close();
            }

            var response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine("Upload done: {0}", response.StatusDescription);
            response.Close();
        }

        public static void UploadFromDicStream(FTpInfo ftpInfo)
        {
            var fileName = Path.GetFileName(ftpInfo.FileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftpInfo.Host, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpInfo.UserName, ftpInfo.PassWord);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false; 
          
            Stream stream = GetStream();           
            using (var requestStream = request.GetRequestStream())
            {
                stream.CopyTo(requestStream);
                requestStream.Close();
            }

            var response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine("Upload done: {0}", response.StatusDescription);
            response.Close();
        }

        private static Stream GetStream()
        {
            string[] fieldId = new[] { "UserName", "FullName", "Department", "RManager", "CourseName", "Country" };
            var dicList = GetDictionay();

            StringWriter sw = GetStreamData(fieldId, dicList, "Test.csv");
            byte[] byteArray = Encoding.UTF8.GetBytes(sw.ToString());
            Stream stream = new MemoryStream(byteArray);
            return stream;
        }

        public static List<Dictionary<string, string>> GetDictionay()
        {
            List<Dictionary<string, string>> ldict = new List<Dictionary<string, string>>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UserName", "Rachuri");
            dict.Add("FullName", "Rachuri Sreedhar");
            dict.Add("Department", "Tech");
            dict.Add("RManager", "Anto V");
            dict.Add("CourseName", "Asp.Net MVC");            
            dict.Add("Country", "India");
            ldict.Add(dict);
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            dict1.Add("UserName", "karthik Sp");
            dict1.Add("FullName", "karthik Sp");
            dict1.Add("Department", "Tech");
            dict1.Add("RManager", "Anto V");
            dict1.Add("CourseName", "Asp.Net MVC");
            dict1.Add("Country", "India");
            ldict.Add(dict1);
            return ldict;
        }

        private static StringWriter GetStreamData(string[] fieldId, List<Dictionary<string, string>> fields, string reportname)
        {
            MemoryStream stream = new MemoryStream();
            int colCount = !(fieldId == null) ? fieldId.Length : 0;
            StringWriter sw = new StringWriter();
            StringBuilder builder = new StringBuilder();
            foreach (var field in fieldId)
            {
                builder.Append(field).Append(",");
            }

            sw.WriteLine(builder);
            foreach (var field in fields)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var value in field)
                {
                    foreach (var col in fieldId)
                    {
                        if (col == value.Key)
                        {
                            if (value.Key.Equals("Name[String_Mandatory]") || value.Key.Equals("Location[Mandatory]"))
                            {
                                if (value.Value != null)
                                {
                                    if (value.Value.Contains(","))
                                    {
                                        string v = value.Value;
                                        if (value.Value.Contains("\""))
                                        {
                                            sb.Append(v).Append(",");
                                        }
                                        else
                                        {
                                            v = "\"" + v + "\"";
                                            sb.Append(v).Append(",");
                                        }
                                    }
                                    else
                                    {
                                        sb.Append(value.Value).Append(",");
                                    }
                                }
                                else
                                {
                                    sb.Append(value.Value).Append(",");
                                }
                            }
                            else
                            {
                                sb.Append(value.Value).Append(",");
                            }
                        }
                    }
                }

                sw.WriteLine(sb);
            }

            return sw;
        }
    }

    class FTpInfo
    {
        public string UserName { get { return "rachuri"; } }

        public string PassWord { get { return "rachuri"; } }

        public string Host { get { return "ftp://localhost"; } }

        public string FileName { get { return @"C:\Users\sreedhar.r\Downloads\ClassDeliveryReport.csv"; ; } }
    }
}
