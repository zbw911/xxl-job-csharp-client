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

                //Console.WriteLine(text);
            }

            var rpcrequest = Newtonsoft.Json.JsonConvert.DeserializeObject<RpcRequest>(text);

            ExecutorBizImpl executorBizImpl = new ExecutorBizImpl();

            object invokeResult = null;

            switch (rpcrequest.methodName)
            {
                case "run":
                    {

                        invokeResult = executorBizImpl.run(Newtonsoft.Json.JsonConvert.DeserializeObject<TriggerParam>(rpcrequest.parameters[0].ToString()));
                        break;
                    }
                case "kill":
                    {
                        throw new NotImplementedException();
                        break;
                    }
                case "log":
                    {
                        invokeResult = executorBizImpl.log(long.Parse(rpcrequest.parameters[0].ToString()), int.Parse(rpcrequest.parameters[1].ToString()), int.Parse(rpcrequest.parameters[2].ToString()));

                        break;
                    }
                default:
                    throw new NotImplementedException();
                    break;
            }


            RpcResponse rpcResponse = new RpcResponse
            {
                error = null,
                result = invokeResult
            };

            ResponeHandler.ResponseHtml(obj.Response, Newtonsoft.Json.JsonConvert.SerializeObject(rpcResponse));
        }
    }
}
