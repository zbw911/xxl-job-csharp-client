using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobClient.utils
{
    public class ShardingUtil
    {

        private static ThreadLocal<ShardingVO> contextHolder = new ThreadLocal<ShardingVO>();

        public class ShardingVO
        {

            private int index;  // sharding index
            private int total;  // sharding total

            public ShardingVO(int index, int total)
            {
                this.index = index;
                this.total = total;
            }

            public int getIndex()
            {
                return index;
            }

            public void setIndex(int index)
            {
                this.index = index;
            }

            public int getTotal()
            {
                return total;
            }

            public void setTotal(int total)
            {
                this.total = total;
            }
        }

        public static void setShardingVo(ShardingVO shardingVo)
        {
            contextHolder.Value = (shardingVo);
        }

        public static ShardingVO getShardingVo()
        {
            return contextHolder.Value;
        }

    }
}
