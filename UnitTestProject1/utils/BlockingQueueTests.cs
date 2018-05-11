using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobClient.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.utils.Tests
{
    [TestClass()]
    public class BlockingQueueTests
    {
        [TestMethod()]
        public void EnqueueTest()
        {
            BlockingQueue<int> blockingQueue = new BlockingQueue<int>();

            blockingQueue.Enqueue(1);

            var d = blockingQueue.Dequeue();

            d = blockingQueue.Dequeue();

            int i = 0;
        }

        [TestMethod()]
        public void DequeueTest()
        {
            Assert.Fail();
        }
    }
}