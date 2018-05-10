using JobClient.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobClient.executor
{
    class ClientSender
    {


        public static void RegJobThread(string adminaddresses, string executorappname, string executorip, int executorPort, string accessToken)
        {
            var t = new Thread(x =>
            {
                while (true)
                {
                    var result = requestTo($"{adminaddresses}/api", RegUtil.Registry(adminaddresses, executorappname, executorip, executorPort, accessToken));

                    Console.WriteLine("注册" + result);

                    Thread.Sleep(30 * 1000);
                }

            });
            t.Start();
        }

        private static string requestTo(string url, string requestStr)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            httpWebRequest.Timeout = 30 * 1000;// timeout;
            httpWebRequest.Method = "POST";
            //    httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            Stream requestStream = null;

            try
            {
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                byte[] postData = encoding.GetBytes(requestStr);
                httpWebRequest.ContentLength = postData.Length;
                requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding("UTF-8"));
                string result = myStreamReader.ReadToEnd();
                return result;
            }
            finally
            {
                if (myStreamReader != null)
                {
                    myStreamReader.Close();
                }

                if (myResponseStream != null)
                {
                    myResponseStream.Close();
                }
                if (requestStream != null)
                {
                    requestStream.Close();
                }
            }
        }
    }
}
