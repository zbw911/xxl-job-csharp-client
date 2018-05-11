using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.utils
{
    public class Log4netManager
    {
        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory + "/log4net.config";

        /// <summary>
        ///     静态构造函数，在首次使用log4net前，加载log4net配置
        /// </summary>
        static Log4netManager()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        /// <summary>
        ///     根据name获取logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }

        public static ILog GetLogger(Type name)
        {
            return LogManager.GetLogger(name);
        }
    }
}
