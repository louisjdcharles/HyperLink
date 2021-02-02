using System;
using System.Collections.Generic;
using System.Net;
using System.Resources;
using System.Text;
using System.IO;

namespace HyperLink
{
    using Request = HttpListenerRequest;
    using Response = HttpListenerResponse;

    class RequestRouter
    {
        private Dictionary<string, Action<Request, Response>> Routes; 

        public RequestRouter() {
            Routes = new Dictionary<string, Action<Request, Response>>();
        }

        public void AddRoute (string Method, string Path, Action<Request, Response> Handler) {
            string RouteIdentifier = Method + " " + Path;

            Routes.Add(RouteIdentifier, Handler);
        }

        public void HandleRequest(Request req, Response res) {
            Console.WriteLine("{0} {1}", req.HttpMethod, req.RawUrl);

            string RouteIdentifier = req.HttpMethod + " " + req.RawUrl;
            try
            {
                try
                {
                    Routes[RouteIdentifier].Invoke(req, res);
                }
                catch (KeyNotFoundException) // Handle 404
                {
                    Console.WriteLine("ERROR: Route {0} {1} not found!", req.HttpMethod, req.RawUrl);

                    res.ContentType = "text/html";
                    res.StatusCode = 404;
                    res.OutputStream.Write(Encoding.UTF8.GetBytes(
                        string.Format("<html><body>Error 404: Api Endpoint '{0}' Not Found!</body></html>", RouteIdentifier)
                    ));
                    res.OutputStream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: An error occured processing the request");
                Console.WriteLine(e.Message);
            }
        }

        public void ServeFile(string EndpointPath, string FilePath) {
            AddRoute("GET", EndpointPath, (req, res) =>
            {
                res.ContentType = GetContentType(FilePath);

                FileStream f = File.OpenRead(FilePath);
                byte[] buffer = new byte[f.Length];
                f.Read(buffer);
                f.Close();

                res.OutputStream.Write(buffer);
                res.OutputStream.Close();
            });
        }

        public static string GetContentType(string FilePath) {

            string ext = Path.GetExtension(FilePath);
            switch (ext)
            {
                case ".css":
                    return "text/css";
                case ".json":
                    return "application/json";
                case ".js":
                    return "text/javascript";
                default:
                    return "text/html";
            }
        }
    }
}
