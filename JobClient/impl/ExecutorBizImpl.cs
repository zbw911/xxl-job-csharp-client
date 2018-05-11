using JobClient.executor;
using JobClient.glue;
using JobClient.handler;
using JobClient.log;
using JobClient.model;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.impl
{
    public class ExecutorBizImpl
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExecutorBizImpl));

        public ReturnT<string> beat()
        {
            return ReturnT<string>.SUCCESS;
        }


        public ReturnT<String> idleBeat(int jobId)
        {

            //// isRunningOrHasQueue
            //boolean isRunningOrHasQueue = false;
            //JobThread jobThread = XxlJobExecutor.loadJobThread(jobId);
            //if (jobThread != null && jobThread.isRunningOrHasQueue())
            //{
            //    isRunningOrHasQueue = true;
            //}

            //if (isRunningOrHasQueue)
            //{
            //    return new ReturnT<String>(ReturnT.FAIL_CODE, "job thread is running or has trigger queue.");
            //}
            //return ReturnT.SUCCESS;


            throw new NotImplementedException();
        }


        public ReturnT<LogResult> log(long logDateTim, int logId, int fromLineNum)
        {
            // log filename: yyyy-MM-dd/9999.log
            //todo:这里时间戳转换应该会问题
            String logFileName = XxlJobFileAppender.makeLogFileName(new DateTime(logDateTim), logId);

            LogResult logResult = XxlJobFileAppender.readLog(logFileName, fromLineNum);
            return new ReturnT<LogResult>(logResult);
        }

        public ReturnT<String> run(TriggerParam triggerParam)
        {

            //// load old：jobHandler + jobThread
            JobThread jobThread = XxlJobExecutor.loadJobThread(triggerParam.jobId);
            IJobHandler jobHandler = jobThread != null ? jobThread.getHandler() : null;
            String removeOldReason = null;
            switch (triggerParam.glueType)
            {
                case "BEAN":
                    {
                        IJobHandler newJobHandler = XxlJobExecutor.loadJobHandler(triggerParam.executorHandler);

                        // valid old jobThread
                        if (jobThread != null && jobHandler != newJobHandler)
                        {
                            // change handler, need kill old thread
                            removeOldReason = "更换JobHandler或更换任务模式,终止旧任务线程";

                            jobThread = null;
                            jobHandler = null;
                        }

                        // valid handler
                        if (jobHandler == null)
                        {
                            jobHandler = newJobHandler;
                            if (jobHandler == null)
                            {
                                return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "job handler [" + triggerParam.executorHandler + "] not found.");
                            }
                        }
                        break;
                    }
                default:
                    {
                        return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "glueType[" + triggerParam.glueType + "] is not valid.");
                    }
            }



            //// executor block strategy
            if (jobThread != null)
            {
                throw new NotImplementedException();
                ExecutorBlockStrategyEnum blockStrategy = ExecutorBlockStrategyEnum.COVER_EARLY;
                if (ExecutorBlockStrategyEnum.DISCARD_LATER == blockStrategy)
                {
                    // discard when running
                    if (jobThread.isRunningOrHasQueue())
                    {
                        return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.DISCARD_LATER.ToString());
                    }
                }
                else if (ExecutorBlockStrategyEnum.COVER_EARLY == blockStrategy)
                {
                    // kill running jobThread
                    if (jobThread.isRunningOrHasQueue())
                    {
                        removeOldReason = "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.COVER_EARLY.ToString();

                        jobThread = null;
                    }
                }
                else
                {
                    // just queue trigger
                }
            }

            // replace thread (new or exists invalid)
            if (jobThread == null)
            {
                jobThread = XxlJobExecutor.registJobThread(triggerParam.jobId, jobHandler, removeOldReason);
            }

            // push data to queue
            ReturnT<String> pushResult = jobThread.pushTriggerQueue(triggerParam);
            return pushResult;

            //// load old：jobHandler + jobThread
            //JobThread jobThread = XxlJobExecutor.loadJobThread(triggerParam.jobId);
            //IJobHandler jobHandler = jobThread != null ? jobThread.getHandler() : null;
            //String removeOldReason = null;

            //// valid：jobHandler + jobThread
            //if (GlueTypeEnum.BEAN == GlueTypeEnum.match(triggerParam.glueType))
            //{

            //    // new jobhandler
            //    IJobHandler newJobHandler = XxlJobExecutor.loadJobHandler(triggerParam.executorHandler);

            //    // valid old jobThread
            //    if (jobThread != null && jobHandler != newJobHandler)
            //    {
            //        // change handler, need kill old thread
            //        removeOldReason = "更换JobHandler或更换任务模式,终止旧任务线程";

            //        jobThread = null;
            //        jobHandler = null;
            //    }

            //    // valid handler
            //    if (jobHandler == null)
            //    {
            //        jobHandler = newJobHandler;
            //        if (jobHandler == null)
            //        {
            //            return new ReturnT<String>(ReturnT<string>.FAIL_CODE, "job handler [" + triggerParam.executorHandler + "] not found.");
            //        }
            //    }

            //}
            //else if (GlueTypeEnum.GLUE_GROOVY == GlueTypeEnum.match(triggerParam.glueType))
            //{

            //    // valid old jobThread
            //    if (jobThread != null &&
            //            !(jobThread.getHandler() instanceof GlueJobHandler
            //            && ((GlueJobHandler)jobThread.getHandler()).getGlueUpdatetime() == triggerParam.glueUpdatetime )) {
            //        // change handler or gluesource updated, need kill old thread
            //        removeOldReason = "更新任务逻辑或更换任务模式,终止旧任务线程";

            //        jobThread = null;
            //        jobHandler = null;
            //    }

            //    // valid handler
            //    if (jobHandler == null)
            //    {
            //        try
            //        {
            //            IJobHandler originJobHandler = GlueFactory.getInstance().loadNewInstance(triggerParam.GlueSource());
            //            jobHandler = new GlueJobHandler(originJobHandler, triggerParam.GlueUpdatetime());
            //        }
            //        catch (Exception e)
            //        {
            //            logger.error(e.getMessage(), e);
            //            return new ReturnT<String>(ReturnT.FAIL_CODE, e.getMessage());
            //        }
            //    }
            //}
            //else if (GlueTypeEnum.GLUE_SHELL == GlueTypeEnum.match(triggerParam.GlueType())
            //      || GlueTypeEnum.GLUE_PYTHON == GlueTypeEnum.match(triggerParam.GlueType()))
            //{

            //    // valid old jobThread
            //    if (jobThread != null &&
            //            !(jobThread.getHandler() instanceof ScriptJobHandler
            //                && ((ScriptJobHandler)jobThread.getHandler()).getGlueUpdatetime() == triggerParam.GlueUpdatetime() )) {
            //        // change script or gluesource updated, need kill old thread
            //        removeOldReason = "更新任务逻辑或更换任务模式,终止旧任务线程";

            //        jobThread = null;
            //        jobHandler = null;
            //    }

            //    // valid handler
            //    if (jobHandler == null)
            //    {
            //        jobHandler = new ScriptJobHandler(triggerParam.JobId(), triggerParam.GlueUpdatetime(), triggerParam.GlueSource(), GlueTypeEnum.match(triggerParam.GlueType()));
            //    }
            //}
            //else
            //{
            //    return new ReturnT<String>(ReturnT.FAIL_CODE, "glueType[" + triggerParam.GlueType() + "] is not valid.");
            //}

            //// executor block strategy
            //if (jobThread != null)
            //{
            //    ExecutorBlockStrategyEnum blockStrategy = ExecutorBlockStrategyEnum.match(triggerParam.ExecutorBlockStrategy(), null);
            //    if (ExecutorBlockStrategyEnum.DISCARD_LATER == blockStrategy)
            //    {
            //        // discard when running
            //        if (jobThread.isRunningOrHasQueue())
            //        {
            //            return new ReturnT<String>(ReturnT.FAIL_CODE, "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.DISCARD_LATER.getTitle());
            //        }
            //    }
            //    else if (ExecutorBlockStrategyEnum.COVER_EARLY == blockStrategy)
            //    {
            //        // kill running jobThread
            //        if (jobThread.isRunningOrHasQueue())
            //        {
            //            removeOldReason = "阻塞处理策略-生效：" + ExecutorBlockStrategyEnum.COVER_EARLY.getTitle();

            //            jobThread = null;
            //        }
            //    }
            //    else
            //    {
            //        // just queue trigger
            //    }
            //}

            //// replace thread (new or exists invalid)
            //if (jobThread == null)
            //{
            //    jobThread = XxlJobExecutor.registJobThread(triggerParam.JobId(), jobHandler, removeOldReason);
            //}

            //// push data to queue
            //ReturnT<String> pushResult = jobThread.pushTriggerQueue(triggerParam);
            //return pushResult;
        }
    }
}
