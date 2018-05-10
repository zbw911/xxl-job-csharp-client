using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class HttpServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly Thread[] _workers;
        private readonly ManualResetEvent _stop, _ready;
        private Queue<HttpListenerContext> _queue;

        public HttpServer(int maxThreads)
        {
            this._workers = new Thread[maxThreads];
            this._queue = new Queue<HttpListenerContext>();
            this._stop = new ManualResetEvent(false);
            this._ready = new ManualResetEvent(false);
            this._listener = new HttpListener();
            this._listenerThread = new Thread(this.HandleRequests);
            this._listenerThread.SetApartmentState(ApartmentState.STA);
        }

        public int Start(int port)
        {
            if (port == 0)
            {
                port = GetRandomUnusedPort();
            }


            Console.WriteLine($"Port=>{port}");

            this._listener.Prefixes.Add(String.Format(@"http://+:{0}/", port == 0 ? GetRandomUnusedPort() : port));


            this._listener.IgnoreWriteExceptions = true;
            this._listener.Start();
            this._listenerThread.Start();

            for (int i = 0; i < this._workers.Length; i++)
            {
                var j = i;
                this._workers[i] = new Thread(() => this.Worker(j));
                //this._workers[i].IsBackground = true;
                this._workers[i].SetApartmentState(ApartmentState.STA);
                this._workers[i].Start();
            }

            return port;
        }


        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public void Dispose()
        {
            this.Stop();
        }

        public void Stop()
        {
            this._stop.Set();
            this._listenerThread.Join();
            foreach (Thread worker in this._workers)
                worker.Join();
            this._listener.Stop();
        }

        private void HandleRequests()
        {

            while (this._listener.IsListening)
            {

                var context = this._listener.BeginGetContext(this.ContextReady, null);

                if (0 == WaitHandle.WaitAny(new[] { this._stop, context.AsyncWaitHandle }))
                    return;
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (this._queue)
                {
                    this._queue.Enqueue(this._listener.EndGetContext(ar));



                    this._ready.Set();
                }
            }
            catch (Exception e)
            {

                Console.Error.WriteLine(e);
            }
        }

        private void Worker(int threadindex)
        {
            WaitHandle[] wait = new[] { this._ready, this._stop };
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (this._queue)
                {
                    if (this._queue.Count > 0)
                        context = this._queue.Dequeue();
                    else
                    {
                        this._ready.Reset();
                        continue;
                    }
                }

                try
                {
                    this.ProcessRequest(context);
                }
                catch (Exception e)
                {

                    Console.Error.WriteLine(e);
                }
            }
        }

        public event Action<HttpListenerContext> ProcessRequest;


    }
}
