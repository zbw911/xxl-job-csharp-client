using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.glue
{
    public enum ExecutorBlockStrategyEnum
    {
        /// <summary>
        /// ("单机串行")
        /// </summary>
        SERIAL_EXECUTION,
        /*CONCURRENT_EXECUTION("并行"),*/
        /// <summary>
        /// ("丢弃后续调度")
        /// </summary>
        DISCARD_LATER,
        /// <summary>
        /// ("覆盖之前调度")
        /// </summary>
        COVER_EARLY

        //private final String title;
        //private ExecutorBlockStrategyEnum(String title)
        //{
        //    this.title = title;
        //}
        //public String getTitle()
        //{
        //    return title;
        //}

        //public static ExecutorBlockStrategyEnum match(String name, ExecutorBlockStrategyEnum defaultItem)
        //{
        //    if (name != null)
        //    {
        //        for (ExecutorBlockStrategyEnum item:ExecutorBlockStrategyEnum.values())
        //        {
        //            if (item.name().equals(name))
        //            {
        //                return item;
        //            }
        //        }
        //    }
        //    return defaultItem;
        //}
    }
}
