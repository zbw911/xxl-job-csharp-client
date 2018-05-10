using ConsoleApp1.impl;
using ConsoleApp1.model;
using ConsoleApp1.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {

            HttpServer httpServer = new HttpServer(1);
            httpServer.Start(7070);
            httpServer.ProcessRequest += HttpServer_ProcessRequest;
            //JsonTest();

            Reg();


            Console.ReadKey();
        }


        static void Reg()
        {
            var t = new Thread(x =>
            {
                while (true)
                {
                    var result = requestTo("http://localhost:8080/api", JsonUtil.Registry());

                    Console.WriteLine("注册" + result);

                    Thread.Sleep(30 * 1000);
                }

            });
            t.Start();
        }

        private static void HttpServer_ProcessRequest(HttpListenerContext obj)
        {
            var request = obj.Request;

            string text = "";
            // convert stream to string
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                text = reader.ReadToEnd();

                Console.WriteLine(text);
            }

            var rpcrequest = Newtonsoft.Json.JsonConvert.DeserializeObject<RpcRequest>(text);

            ExecutorBizImpl executorBizImpl = new ExecutorBizImpl();
            switch (rpcrequest.methodName)
            {
                case "run":
                    {

                        var result = executorBizImpl.run(Newtonsoft.Json.JsonConvert.DeserializeObject<TriggerParam>(rpcrequest.parameters[0].ToString()));
                        break;
                    }
                case "kill":
                    {

                        break;
                    }
                default:
                    break;
            }


            RpcResponse rpcResponse = new RpcResponse
            {
                error = null,
                result = ReturnT<string>.SUCCESS
            };

            ResponeHandler.ResponseHtml(obj.Response, Newtonsoft.Json.JsonConvert.SerializeObject(rpcResponse));
        }

        /*
         * 
         {"serverAddress":"127.0.0.1:7070","createMillisTime":1525927906577,"accessToken":"","className":"com.xxl.job.core.biz.ExecutorBiz","methodName":"run","parameterTypes":["com.xxl.job.core.biz.model.TriggerParam"],"parameters":[{"jobId":8,"executorHandler":"shardingJobHandler","executorParams":"","executorBlockStrategy":"SERIAL_EXECUTION","logId":188541,"logDateTim":1525927906577,"glueType":"BEAN","glueSource":"","glueUpdatetime":1525927571000,"broadcastIndex":0,"broadcastTotal":1}]}
         * 
         * */
          
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
