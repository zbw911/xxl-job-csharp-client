using System;

namespace ConsoleApp1
{
    public class RpcRequest
    {

        public string serverAddress { get; set; }
        public long createMillisTime { get; set; }
        public string accessToken { get; set; }
        public string className { get; set; }
        public string methodName { get; set; }
        public string[] parameterTypes { get; set; }
        public object[] parameters { get; set; }

        
        public override string ToString()
        {
            return "RpcRequest{" +
                    "serverAddress='" + serverAddress + '\'' +
                    ", createMillisTime=" + createMillisTime +
                    ", accessToken='" + accessToken + '\'' +
                    ", className='" + className + '\'' +
                    ", methodName='" + methodName + '\'' +
                    ", parameterTypes=" + (parameterTypes) +
                    ", parameters=" + (parameters) +
                    '}';
        }


    }
}
