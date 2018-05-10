using JobClient.handler;
using JobClient.handler.attribute;
using JobClient.log;
using JobClient.utils;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.executor
{
    public class XxlJobExecutor
    {
        private static ILog logger = LogManager.GetLogger(typeof(XxlJobExecutor));

        // ---------------------- param ----------------------
        private String Localip;
        private int localport = 0;
        private String appName;
        private String adminAddresses;
        private String accessToken;
        private String logPath;

        public void setLocalIp(String ip)
        {
            this.Localip = ip;
        }
        public void setPort(int port)
        {
            this.localport = port;
        }
        public void setAppName(String appName)
        {
            this.appName = appName;
        }
        public void setAdminAddresses(String adminAddresses)
        {
            this.adminAddresses = adminAddresses;
        }
        public void setAccessToken(String accessToken)
        {
            this.accessToken = accessToken;
        }
        public void setLogPath(String logPath)
        {
            this.logPath = logPath;
        }

        public void start()
        {
            // init admin-client
            //initAdminBizList(adminAddresses, accessToken);

            // init executor-jobHandlerRepository

            initJobHandlerRepository();


            // init logpath
            if (logPath != null && logPath.Trim().Length > 0)
            {
                XxlJobFileAppender.logPath = logPath;
            }

            // init executor-server
            initExecutorServer(localport, Localip, appName, accessToken);
        }

        private void initExecutorServer(int port, string ip, string appName, string accessToken)
        {
            port = ServerDispose.Start(port);

            ClientSender.RegJobThread(adminAddresses, appName, ip, port, accessToken);

        }

        public void destroy()
        {
            // destory JobThreadRepository
            if (JobThreadRepository.Count > 0)
            {
                foreach (var item in JobThreadRepository)
                {
                    removeJobThread(item.Key, "Web容器销毁终止");
                }
                JobThreadRepository.Clear();
            }

            // destory executor-server
            stopExecutorServer();
        }

        private void stopExecutorServer()
        {
            throw new NotImplementedException();
        }


        #region  Handler
        // ---------------------- job handler repository ----------------------
        private static ConcurrentDictionary<String, IJobHandler> jobHandlerRepository = new ConcurrentDictionary<String, IJobHandler>();
        public static IJobHandler registJobHandler(String name, IJobHandler jobHandler)
        {
            logger.InfoFormat(">>>>>>>>>>> xxl-job register jobhandler success, name:{}, jobHandler:{}", name, jobHandler);
            return jobHandlerRepository.GetOrAdd(name, jobHandler);
        }
        private static void initJobHandlerRepository()
        {

            var list = AssemblyManager.GetTypeInstances<IJobHandler>();

            foreach (var item in list)
            {
                var itemtype = item.GetType();
                var attr = itemtype.GetCustomAttributes(typeof(JobHanderAttribute), true).FirstOrDefault() as JobHanderAttribute;

                var name = itemtype.Name;
                if (attr != null)
                {
                    name = attr.Name;
                }
                if (loadJobHandler(name) != null)
                {
                    throw new Exception("xxl-job jobhandler naming conflicts.");
                }

                registJobHandler(name, item);
            }
        }

        public static IJobHandler loadJobHandler(String name)
        {
            jobHandlerRepository.TryGetValue(name, out IJobHandler jobHandler);

            return jobHandler;
        }
        #endregion


        #region Jobs

        private static ConcurrentDictionary<int, JobThread> JobThreadRepository = new ConcurrentDictionary<int, JobThread>();
        public static JobThread registJobThread(int jobId, IJobHandler handler, String removeOldReason)
        {
            JobThread newJobThread = new JobThread(jobId, handler);
            newJobThread.start();
            logger.InfoFormat(">>>>>>>>>>> xxl-job regist JobThread success, jobId:{}, handler:{}", new Object[] { jobId, handler });

            JobThread oldJobThread = JobThreadRepository.GetOrAdd(jobId, newJobThread);  // putIfAbsent | oh my god, map's put method return the old value!!!
            if (oldJobThread != null)
            {
                oldJobThread.toStop(removeOldReason);
                //oldJobThread.interrupt();
            }

            return newJobThread;
        }
        public static void removeJobThread(int jobId, String removeOldReason)
        {
            JobThreadRepository.TryRemove(jobId, out JobThread oldJobThread);
            if (oldJobThread != null)
            {
                oldJobThread.toStop(removeOldReason);

            }
        }
        public static JobThread loadJobThread(int jobId)
        {
            JobThreadRepository.TryGetValue(jobId, out JobThread jobThread);
            return jobThread;
        }

        #endregion
        public static void TestInner()
        {
            initJobHandlerRepository();
        }


    }
}
