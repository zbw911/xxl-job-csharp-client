using System;
using System.Collections.Concurrent;
using System.Threading;
using JobClient.handler;
using JobClient.log;
using JobClient.model;
using JobClient.utils;
using log4net;

namespace JobClient.executor
{
    public class JobThread
    {
        private static ILog logger = Log4netManager.GetLogger(typeof(JobThread));
        private int jobId;
        private IJobHandler handler;

        //BlockingCollection<TriggerParam> triggerQueue;
        ConcurrentQueue<TriggerParam> triggerQueue = new ConcurrentQueue<TriggerParam>();
        //private LinkedBlockingQueue<TriggerParam> triggerQueue;
        private ConcurrentHashSet<int> triggerLogIdSet;     // avoid repeat trigger for the same TRIGGER_LOG_ID


        private bool toStop1 = false;
        private String stopReason;

        private bool running = false;    // if running job
        private int idleTimes = 0;			// idel times

        public JobThread(int jobId, IJobHandler handler)
        {
            this.jobId = jobId;
            this.handler = handler;

            this.triggerQueue = new ConcurrentQueue<TriggerParam>();// new BlockingCollection<TriggerParam>();
            this.triggerLogIdSet = new ConcurrentHashSet<int>();
        }
        public ReturnT<String> pushTriggerQueue(TriggerParam triggerParam)
        {
            // avoid repeat
            if (triggerLogIdSet.Contains(triggerParam.logId))
            {
                logger.Debug(string.Format("repeate trigger job, logId:{0}", triggerParam.logId));
                return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "repeate trigger job, logId:" + triggerParam.logId);
            }

            triggerLogIdSet.Add(triggerParam.logId);
            triggerQueue.Enqueue(triggerParam);
            return ReturnT<string>.SUCCESS;
        }
        internal void start()
        {

            var thread = new Thread(x =>
            {
                innerStart();
            }
            );

            thread.Start();


        }


        void innerStart()
        {
            while (!toStop1)
            {
                running = false;
                idleTimes++;
                try
                {
                    // to check toStop signal, we need cycle, so wo cannot use queue.take(), instand of poll(timeout)
                    var result = triggerQueue.TryDequeue(out TriggerParam triggerParam);

                    if (!result || triggerParam == null)
                    {
                        Thread.Sleep(3 * 1000);
                    }

                    if (triggerParam != null)
                    {
                        running = true;
                        idleTimes = 0;
                        triggerLogIdSet.Remove(triggerParam.logId);

                        // parse param
                        string[] handlerParams = (triggerParam.executorParams != null && triggerParam.executorParams.Trim().Length > 0)
                                ? triggerParam.executorParams.Split(',') : null;

                        // handle job
                        ReturnT<String> executeResult = null;
                        try
                        {
                            // log filename: yyyy-MM-dd/9999.log
                            String logFileName = XxlJobFileAppender.makeLogFileName(TimeUtil.ToTime(triggerParam.logDateTim), triggerParam.logId);

                            XxlJobFileAppender.contextHolder.Value = (logFileName);
                            ShardingUtil.setShardingVo(new ShardingUtil.ShardingVO(triggerParam.broadcastIndex, triggerParam.broadcastTotal));
                            XxlJobLogger.log("<br>----------- xxl-job job execute start -----------<br>----------- Params:" + string.Join(",", handlerParams));

                            executeResult = handler.execute(handlerParams);
                            if (executeResult == null)
                            {
                                executeResult = ReturnT<string>.FAIL;
                            }

                            XxlJobLogger.log("<br>----------- xxl-job job execute end(finish) -----------<br>----------- ReturnT:" + executeResult);
                        }
                        catch (Exception e)
                        {
                            if (toStop1)
                            {
                                XxlJobLogger.log("<br>----------- JobThread toStop, stopReason:" + stopReason);
                            }

                            //StringWriter stringWriter = new StringWriter();
                            //e.printStackTrace(new PrintWriter(stringWriter));
                            String errorMsg = e.ToString();
                            executeResult = new ReturnT<String>(ReturnT<string>.FAIL_CODE, errorMsg);

                            XxlJobLogger.log("<br>----------- JobThread Exception:" + errorMsg + "<br>----------- xxl-job job execute end(error) -----------");
                        }

                        // callback handler info
                        if (!toStop1)
                        {
                            // commonm
                            TriggerCallbackThread.pushCallBack(new HandleCallbackParam(triggerParam.logId, executeResult));
                        }
                        else
                        {
                            // is killed
                            ReturnT<String> stopResult = new ReturnT<String>(ReturnT<string>.FAIL_CODE, stopReason + " [业务运行中，被强制终止]");
                            TriggerCallbackThread.pushCallBack(new HandleCallbackParam(triggerParam.logId, stopResult));
                        }
                    }
                    else
                    {
                        if (idleTimes > 30)
                        {
                            XxlJobExecutor.removeJobThread(jobId, "excutor idel times over limit.");
                        }
                    }
                }
                catch (Exception e)
                {
                    if (toStop1)
                    {
                        XxlJobLogger.log("<br>----------- xxl-job toStop, stopReason:" + stopReason);
                    }


                    String errorMsg = e.ToString();
                    XxlJobLogger.log("----------- xxl-job JobThread Exception:" + errorMsg);
                }
            }

            // callback trigger request in queue
            while (triggerQueue != null && triggerQueue.Count > 0)
            {
                triggerQueue.TryDequeue(out TriggerParam triggerParam);
                if (triggerParam != null)
                {
                    // is killed
                    ReturnT<String> stopResult = new ReturnT<String>(ReturnT<string>.FAIL_CODE, stopReason + " [任务尚未执行，在调度队列中被终止]");
                    TriggerCallbackThread.pushCallBack(new HandleCallbackParam(triggerParam.logId, stopResult));
                }
            }

            logger.Info(string.Format(">>>>>>>>>>>> xxl-job JobThread stoped, hashCode:{0}", Thread.CurrentThread));
        }


        /**
 * is running job
 * @return
 */
        public bool isRunningOrHasQueue()
        {
            return running || triggerQueue.Count > 0;
        }

        internal void toStop(string stopReason)
        {
            /**
		 * Thread.interrupt只支持终止线程的阻塞状态(wait、join、sleep)，
		 * 在阻塞出抛出InterruptedException异常,但是并不会终止运行的线程本身；
		 * 所以需要注意，此处彻底销毁本线程，需要通过共享变量方式；
		 */
            this.toStop1 = true;
            this.stopReason = stopReason;
        }

        public IJobHandler getHandler()
        {
            return handler;
        }
    }
}