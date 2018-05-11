using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JobClient.biz;
using JobClient.model;
using JobClient.utils;
using log4net;

namespace JobClient.executor
{
    internal class TriggerCallbackThread
    {
        private static ILog logger = Log4netManager.GetLogger(typeof(TriggerCallbackThread));



        private static TriggerCallbackThread instance = new TriggerCallbackThread();
        public static TriggerCallbackThread getInstance()
        {
            return instance;
        }

        /**
         * job results callback queue
         */
        private BlockingCollection<HandleCallbackParam> callBackQueue = new BlockingCollection<HandleCallbackParam>();
        public static void pushCallBack(HandleCallbackParam callback)
        {
            getInstance().callBackQueue.Add(callback);
            logger.Debug(string.Format(">>>>>>>>>>> xxl-job, push callback request, logId:{0}", callback.logId));
        }

        /**
         * callback thread
         */
        private Thread triggerCallbackThread;
        private volatile bool toStop1 = false;
        public void start()
        {

            // valid
            if (XxlJobExecutor.getAdminBizList() == null)
            {
                logger.WarnFormat(">>>>>>>>>>>> xxl-job, executor callback config fail, adminAddresses is null.");
                return;
            }

            triggerCallbackThread = new Thread(() =>
            {
                // normal callback
                while (!toStop1)
                {
                    try
                    {
                        getInstance().callBackQueue.TryTake(out HandleCallbackParam callback);
                        if (callback != null)
                        {

                            // callback list param
                            List<HandleCallbackParam> callbackParamList = new List<HandleCallbackParam>();
                            var arrays = getInstance().callBackQueue.ToArray();
                            callbackParamList.AddRange(arrays);
                            callbackParamList.Add(callback);

                            // callback, will retry if error
                            if (callbackParamList != null && callbackParamList.Count > 0)
                            {
                                doCallback(callbackParamList);
                            }
                        }
                        if (callback == null)
                            Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message, e);
                    }
                }

                // last callback
                try
                {
                    List<HandleCallbackParam> callbackParamList = new List<HandleCallbackParam>();
                    //int drainToNum = getInstance().callBackQueue.drainTo(callbackParamList);
                    callbackParamList.AddRange(getInstance().callBackQueue.ToArray());
                    if (callbackParamList != null && callbackParamList.Count > 0)
                    {
                        doCallback(callbackParamList);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message, e);
                }
                logger.WarnFormat(">>>>>>>>>>>> xxl-job, executor callback thread destory.");


            });
            triggerCallbackThread.IsBackground = (true);
            triggerCallbackThread.Name = ("triggerCallbackThread");
            triggerCallbackThread.Start();
        }
        public void toStop()
        {
            toStop1 = true;
            // interrupt and wait
            triggerCallbackThread.Interrupt();
            try
            {
                triggerCallbackThread.Join();
            }
            catch (ThreadInterruptedException e)
            {
                logger.Error(e.Message, e);
            }
        }

        /**
         * do callback, will retry if error
         * @param callbackParamList
         */
        private void doCallback(List<HandleCallbackParam> callbackParamList)
        {
            // callback, will retry if error
            foreach (AdminBiz adminBiz in XxlJobExecutor.getAdminBizList())
            {
                try
                {
                    ReturnT<String> callbackResult = adminBiz.callback(callbackParamList);
                    if (callbackResult != null && ReturnT<string>.SUCCESS_CODE == callbackResult.code)
                    {
                        callbackResult = ReturnT<string>.SUCCESS;
                        logger.Info(string.Format(">>>>>>>>>>> xxl-job callback success, callbackParamList:{0}, callbackResult:{1}", callbackParamList, callbackResult));
                        break;
                    }
                    else
                    {
                        logger.Info(string.Format(">>>>>>>>>>> xxl-job callback fail, callbackParamList:{0}, callbackResult:{1}", callbackParamList, callbackResult));
                    }
                }
                catch (Exception e)
                {
                    logger.Error(string.Format(">>>>>>>>>>> xxl-job callback error, callbackParamList：{0}", callbackParamList), e);
                    //getInstance().callBackQueue.addAll(callbackParamList);
                }
            }
        }
    }
}