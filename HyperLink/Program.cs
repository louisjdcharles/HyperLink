using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using HyperLink;
using System.IO;
using System.Linq;

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
            Server server = new Server();

            server.Router.ServeFile("/", @"Public\index.html");
            server.Router.ServeFile("/styles.css", @"Public\styles.css");
            server.Router.ServeFile("/script.js", @"Public\script.js");

            server.Router.AddRoute("GET", "/time", (req, res) =>
            {
                res.ContentType = "application/json";
                res.OutputStream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Time(DateTime.Now))));
                res.OutputStream.Close();
            });

            server.Listen(3000);

            
        }
    }
}
