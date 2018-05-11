using JobClient.biz;
using JobClient.enums;
using JobClient.model;
using JobClient.utils;
using log4net;
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
    class ExecutorRegistryThread
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExecutorRegistryThread));

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


        private static ExecutorRegistryThread instance = new ExecutorRegistryThread();
        private Thread registryThread;
        private volatile bool toStop1 = false;

        public static ExecutorRegistryThread getInstance()
        {
            return instance;
        }

        public void start(int port, String ip, String appName)
        {

            // valid
            if (appName == null || appName.Trim().Length == 0)
            {
                logger.Warn(">>>>>>>>>>>> xxl-job, executor registry config fail, appName is null.");
                return;
            }
            if (XxlJobExecutor.getAdminBizList() == null)
            {
                logger.Warn(">>>>>>>>>>>> xxl-job, executor registry config fail, adminAddresses is null.");
                return;
            }

            // executor address (generate addredd = ip:port)
            String executorAddress;
            if (ip != null && ip.Trim().Length > 0)
            {
                executorAddress = ip.Trim() + (":") + port;
            }
            else
            {
                executorAddress = IpUtil.getIpPort(port);
            }

            registryThread = new Thread(
                () =>
{
    // registry
    while (!toStop1)
    {
        try
        {
            RegistryParam registryParam = new RegistryParam(RegistryConfig.RegistType.EXECUTOR.ToString(), appName, executorAddress);
            foreach (AdminBiz adminBiz in XxlJobExecutor.getAdminBizList())
            {
                try
                {
                    Object registry = adminBiz.registry(registryParam);
                    ReturnT<String> registryResult = adminBiz.registry(registryParam);
                    if (registryResult != null && ReturnT<string>.SUCCESS_CODE == registryResult.code)
                    {
                        registryResult = ReturnT<string>.SUCCESS;
                        logger.Info(string.Format(">>>>>>>>>>> xxl-job registry success, registryParam:{0}, registryResult:{1}", registryParam, registryResult));
                        break;
                    }
                    else
                    {
                        logger.Info(string.Format(">>>>>>>>>>> xxl-job registry fail, registryParam:{0}, registryResult:{1}", registryParam, registryResult));
                    }
                }
                catch (Exception e)
                {
                    logger.Info($">>>>>>>>>>> xxl-job registry error, registryParam:{registryParam}", e);
                }
            }
        }
        catch (Exception e)
        {
            logger.Error(e.Message, e);
        }

        Thread.Sleep(RegistryConfig.BEAT_TIMEOUT * 1000);

    }

    // registry remove
    try
    {
        RegistryParam registryParam = new RegistryParam(RegistryConfig.RegistType.EXECUTOR.ToString(), appName, executorAddress);
        foreach (AdminBiz adminBiz in XxlJobExecutor.getAdminBizList())
        {
            try
            {
                ReturnT<String> registryResult = adminBiz.registryRemove(registryParam);
                if (registryResult != null && ReturnT<string>.SUCCESS_CODE == registryResult.code)
                {
                    registryResult = ReturnT<string>.SUCCESS;
                    logger.InfoFormat(">>>>>>>>>>> xxl-job registry-remove success, registryParam:{}, registryResult:{}", new Object[] { registryParam, registryResult });
                    break;
                }
                else
                {
                    logger.InfoFormat(">>>>>>>>>>> xxl-job registry-remove fail, registryParam:{}, registryResult:{}", new Object[] { registryParam, registryResult });
                }
            }
            catch (Exception e)
            {
                logger.InfoFormat(">>>>>>>>>>> xxl-job registry-remove error, registryParam:{}", registryParam, e);
            }

        }
    }
    catch (Exception e)
    {
        logger.Error(e.Message, e);
    }
    logger.Warn(">>>>>>>>>>>> xxl-job, executor registry thread destory.");

}
         );
            registryThread.IsBackground = (true);
            registryThread.Start();
        }

        public void toStop()
        {
            toStop1 = true;
            // interrupt and wait
            registryThread.Interrupt();
            try
            {
                registryThread.Join();
            }
            catch (ThreadInterruptedException e)
            {
                logger.Error(e.Message, e);
            }
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
