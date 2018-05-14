
using JobClient;
using JobClient.executor;
using JobClient.impl;
using JobClient.model;
using JobClient.utils;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {



            XxlJobExecutor xxlJobExecutor = new XxlJobExecutor();
            xxlJobExecutor.setLocalIp("127.0.0.1");
            xxlJobExecutor.setAppName("windows-job");
            //xxlJobExecutor.setLogPath("");
            xxlJobExecutor.setAdminAddresses("http://localhost:8080");
            xxlJobExecutor.setPort(7071);
            xxlJobExecutor.start();

            Log4netManager.GetLogger("11111").Info("started");

            Console.ReadKey();
        }





    }






}
