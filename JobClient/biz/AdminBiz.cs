using JobClient.model;
using JobClient.utils;
using log4net;
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
        private static ILog logger = Log4netManager.GetLogger(typeof(AdminBiz));
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
            //RpcRequest{serverAddress='http://127.0.0.1:8080/api', createMillisTime=1526018505399, accessToken='', className='com.xxl.job.core.biz.AdminBiz', methodName='callback', parameterTypes=[interface java.util.List], parameters=[[{logId=188593, executeResult={code=200, msg=null, content=null}}]]}
            //RpcRequest{"serverAddress":"http://localhost:8080/api","createMillisTime":1526022306105,"accessToken":null,"className":"com.xxl.job.core.biz.AdminBiz","methodName":"callback","parameterTypes":["interface java.util.List"],"parameters":[[{"logId":188605,"executeResult":{"code":200,"msg":null,"content":null}}]]}
            return PostPackage("callback", new[] { "java.util.List" }, new[] { callbackParamList });
        }



        public ReturnT<String> registry(RegistryParam registryParam)
        {
            return PostPackage("registry", new[] { "com.xxl.job.core.biz.model.RegistryParam" }, new[] { registryParam });
        }




        public ReturnT<String> registryRemove(RegistryParam registryParam)
        {
            return PostPackage("registryRemove", new[] { "com.xxl.job.core.biz.model.RegistryParam" }, new[] { registryParam });
        }



        public ReturnT<String> triggerJob(int jobId)
        {
            throw new NotImplementedException();
        }

        #region  Http Send Package functions 

        private ReturnT<String> PostPackage(string methodName, string[] parameterTypes, object[] parameters)
        {
            var str = createPackage(methodName, parameterTypes, parameters);

            var result = requestTo(address + MAPPING, str);

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<RpcResponse>(result);

            if (response == null)
            {
                logger.Error(">>>>>>>>>>> xxl-rpc netty response not found.");
                throw new Exception(">>>>>>>>>>> xxl-rpc netty response not found.");
            }
            if (response.isError())
            {
                throw new Exception(response.error);
            }
            else
            {
                var r1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnT<String>>(response.result.ToString());

                return r1;
            }
        }

        private string createPackage(string methodName, string[] parameterTypes, object[] parameters)
        {
            //RegistryParam registryParam = new RegistryParam
            //{
            //    registGroup = "EXECUTOR",
            //    registryKey = executorappname,
            //    registryValue = $"{executorip}:{executorPort}"
            //};

            RpcRequest rpcRequest = new RpcRequest
            {
                accessToken = accessToken,
                className = "com.xxl.job.core.biz.AdminBiz",
                createMillisTime = TimeUtil.CurrentTimeMillis(),
                methodName = methodName,// "registry",
                parameters = parameters,
                parameterTypes = parameterTypes,
                serverAddress = this.address + MAPPING,
            };

            var str = Newtonsoft.Json.JsonConvert.SerializeObject(rpcRequest);

            return str;
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
        #endregion
    }
}
