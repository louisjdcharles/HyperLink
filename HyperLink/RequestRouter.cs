using System;
using System.Collections.Generic;
using System.Net;
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

        /// <summary>
        /// Create a new API endpoint
        /// </summary>
        /// <param name="Method">HTTP method</param>
        /// <param name="Path">Path of API endpoint</param>
        /// <param name="Handler">Function to handle request</param>
        public void AddRoute (string Method, string Path, Action<Request, Response> Handler) {
            string RouteIdentifier = Method + " " + Path;

            Routes.Add(RouteIdentifier, Handler);
        }

        /// <summary>
        /// Passes request to the required API endpoint handler, handling 404 or error processing the request
        /// </summary>
        /// <param name="req">request</param>
        /// <param name="res">response</param>
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

        /// <summary>
        /// Creates a GET api endpoint that returns the specified file
        /// </summary>
        /// <param name="EndpointPath">Path of api endpoint</param>
        /// <param name="FilePath">Local path to file</param>
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

        /// <summary>
        /// Returns the content-type for a file
        /// </summary>
        /// <param name="FilePath">Local Path to file</param>
        /// <returns>content-type</returns>
        private static string GetContentType(string FilePath) {

            string ext = Path.GetExtension(FilePath);
            switch (ext)
            {
                case ".css":
                    return "text/css";
                case ".json":
                    return "application/json";
                case ".js":
                    return "text/javascript";
                case ".ico":
                    return "image/webp";
                default:
                    return "text/html";
            }
        }
    }
}
