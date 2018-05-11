using JobClient.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.biz
{
    public class AdminBiz
    {
        public static string MAPPING = "/api";
        private string address;
        private string accessToken;

        public AdminBiz(string address, string accessToken)
        {
            this.address = address;
            this.accessToken = accessToken;
        }


        public ReturnT<String> callback(List<HandleCallbackParam> callbackParamList)
        {
            throw new NotImplementedException();
        }



        public ReturnT<String> registry(RegistryParam registryParam)
        {
            throw new NotImplementedException();
        }


        public ReturnT<String> registryRemove(RegistryParam registryParam)
        {
            throw new NotImplementedException();
        }



        public ReturnT<String> triggerJob(int jobId)
        {
            throw new NotImplementedException();
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
