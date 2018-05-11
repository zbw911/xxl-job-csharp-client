using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobClient.utils
{
    /// <summary>
    /// https://blogs.msdn.microsoft.com/toub/2006/04/12/blocking-queues/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockingQueue<T> : IDisposable

    {

        private Queue<T> _queue = new Queue<T>();

        private Semaphore _semaphore = new Semaphore(0, int.MaxValue);

        public void Enqueue(T data)

        {

            if (data == null) throw new ArgumentNullException("data");

            lock (_queue) _queue.Enqueue(data);

            _semaphore.Release();

        }


        public T Dequeue()

        {

            _semaphore.WaitOne();

            lock (_queue) return _queue.Dequeue();

        }


        void IDisposable.Dispose()

        {

            if (_semaphore != null)

            {

                _semaphore.Close();

                _semaphore = null;

            }

        }

    }
}
