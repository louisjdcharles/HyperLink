using System;
using System.Text;
using System.Text.Json;
using HyperLink;

namespace Main
{
    class Program
    {
        [Serializable]
        class Time {
            public int hours { get; set; } = 0;
            public int miniutes { get; set; } = 0;
            public int seconds { get; set; } = 0;

            public Time(DateTime dt) { hours = dt.Hour; miniutes = dt.Minute; seconds = dt.Second; }
            public Time() { }
        };

        static void Main(string[] args)
        {
            // create new server instance
            Server server = new Server();

            // add routes for static files
            server.Router.ServeFile("/", @"Public\index.html");
            server.Router.ServeFile("/styles.css", @"Public\styles.css");
            server.Router.ServeFile("/script.js", @"Public\script.js");
            server.Router.ServeFile("/favicon.ico", @"Public\favicon.ico");

            // Api endpoint that returns the current time
            server.Router.AddRoute("GET", "/time", (req, res) =>
            {
                res.ContentType = "application/json";
                res.OutputStream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Time(DateTime.Now))));
                res.OutputStream.Close();
            });

            // start the server
            server.Listen(3000);

            
        }
    }
}
