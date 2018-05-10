using JobClient.impl;
using JobClient.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.executor
{
    public class ServerDispose
    {
        public static int Start(int port)
        {
            HttpServer httpServer = new HttpServer(1);
            var newPort = httpServer.Start(port);
            httpServer.ProcessRequest += HttpServer_ProcessRequest;

            return newPort;
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
    }
}
