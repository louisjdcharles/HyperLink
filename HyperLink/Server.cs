using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Reflection.Metadata;

namespace HyperLink
{
    using Request = HttpListenerRequest;
    using Response = HttpListenerResponse;

    class Server
    {

        private HttpListener Listener;
        public bool Running { get; private set; } = false;
        public RequestRouter Router { get; private set; }

        public Server() {

            Listener = new HttpListener();
            Router = new RequestRouter();
        }

        public void Close()
        {
            Listener.Close();
            Running = false;
        }

        public void Listen(ushort port = 3000) {

            Listener.Prefixes.Add("http://localhost:" + port.ToString() + "/");

            Listener.Start();
            Running = true;

            Console.WriteLine("Server listening at http://localhost:" + port.ToString());

            while (Running)
            {
                HttpListenerContext ctx = Listener.GetContext();

                // starts new thread to handle response, main thread becomes available to 
                new Thread(() => Router.HandleRequest(ctx.Request, ctx.Response)).Start();
            }
        }
    }
}
