using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.utils
{
    public class JsonUtil
    {

        public static string Registry()
        {

            /*
             {
   "serverAddress": "http://127.0.0.1:8080/api",
   "createMillisTime": 1525923077142,
   "accessToken": "",
   "className": "com.xxl.job.core.biz.AdminBiz",
   "methodName": "registry",
   "parameterTypes": [
      "com.xxl.job.core.biz.model.RegistryParam"
   ],
   "parameters": [
      {
         "registGroup": "EXECUTOR",
         "registryKey": "xxl-job-executor-sample1",
         "registryValue": "172.30.128.33:62347"
      }
   ]
}
             */


            RegistryParam registryParam = new RegistryParam
            {
                registGroup = "EXECUTOR",
                registryKey = "windows-job",
                registryValue = "127.0.0.1:7070"
            };

            RpcRequest rpcRequest = new RpcRequest
            {
                accessToken = null,
                className = "com.xxl.job.core.biz.AdminBiz",
                createMillisTime = TimeUtil.CurrentTimeMillis(),
                methodName = "registry",
                parameters = new object[] { registryParam },
                parameterTypes = new[] { "com.xxl.job.core.biz.model.RegistryParam" },
                serverAddress = "http://127.0.0.1:8080/api",
            };

            var str = Newtonsoft.Json.JsonConvert.SerializeObject(rpcRequest);

            //Console.WriteLine(str);

            return str;
        }
    }
}
