using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace Example2
{
  public class Program
  {
    public static void Main (string[] args)
    {
      // Create a new instance of the WebSocketServer class.
      //
      // If you would like to provide the secure connection, you should
      // create a new instance with the "secure" parameter set to true or
      // with a wss scheme WebSocket URL.

      var wssv = new WebSocketServer (4649);
      //var wssv = new WebSocketServer (5963, true);

      //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 4649);
      //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 5963, true);

      //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 4649);
      //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 5963, true);

      //var wssv = new WebSocketServer ("ws://0.0.0.0:4649");
      //var wssv = new WebSocketServer ("wss://0.0.0.0:5963");

      //var wssv = new WebSocketServer ("ws://[::0]:4649");
      //var wssv = new WebSocketServer ("wss://[::0]:5963");

      //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 4649);
      //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 5963, true);

      //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 4649);
      //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 5963, true);

      //var wssv = new WebSocketServer ("ws://localhost:4649");
      //var wssv = new WebSocketServer ("wss://localhost:5963");

      //var wssv = new WebSocketServer ("ws://127.0.0.1:4649");
      //var wssv = new WebSocketServer ("wss://127.0.0.1:5963");

      //var wssv = new WebSocketServer ("ws://[::1]:4649");
      //var wssv = new WebSocketServer ("wss://[::1]:5963");
#if DEBUG
      // To change the logging level.
      wssv.Log.Level = LogLevel.Trace;

      // To provide the HTTP Authentication (Basic/Digest).
      /*
      wssv.AuthenticationSchemes = AuthenticationSchemes.Basic;
      wssv.Realm = "WebSocket Test";
      wssv.UserCredentialsFinder =
        id => {
          var name = id.Name;

          // Return user name, password, and roles.
          return name == "nobita"
                 ? new NetworkCredential (name, "password", "gunfighter")
                 : null; // If the user credentials are not found.
        };
       */

      // To remove the inactive sessions periodically.
      //wssv.KeepClean = true;

      // To resolve to wait for socket in TIME_WAIT state.
      //wssv.ReuseAddress = true;

      // To provide the secure connection.
      /*
      var cert = ConfigurationManager.AppSettings["ServerCertFile"];
      var passwd = ConfigurationManager.AppSettings["CertFilePassword"];

      wssv.SslConfiguration.ServerCertificate = new X509Certificate2 (
                                                  cert,
                                                  passwd
                                                );
       */

      // To change the wait time for the response to the WebSocket Ping or Close.
      //wssv.WaitTime = TimeSpan.FromSeconds (2);
#endif
      // Add the WebSocket services.

      wssv.AddWebSocketService<Echo> ("/Echo");

      // With initializing.
      wssv.AddWebSocketService<Chat> (
        "/Chat",
        s => {
          s.Prefix = "Anon#";
#if DEBUG
          // To validate the cookies.
          /*
          s.CookiesValidator =
            (req, res) => {
              // Check the cookies in "req", and set the cookies to send to
              // the client with "res" if necessary.

              foreach (var cookie in req) {
                cookie.Expired = true;

                res.Add (cookie);
              }

              return true; // If valid.
            };
           */

          // To emit a WebSocket.OnMessage event when receives a ping.
          //s.EmitOnPing = true;

          // To ignore the Sec-WebSocket-Extensions header.
          //s.IgnoreExtensions = true;

          // To disable a delay when send or receive buffer of the underlying
          // TCP socket is not full.
          s.NoDelay = true;

          // To validate the Origin header.
          /*
          s.OriginValidator =
            val => {
              // Check the value of the Origin header, and return true if valid.

              Uri origin;

              return !val.IsNullOrEmpty ()
                     && Uri.TryCreate (val, UriKind.Absolute, out origin)
                     && origin.Host == "localhost";
            };
           */

          // To send the Sec-WebSocket-Protocol header that has a subprotocol
          // name.
          //s.Protocol = "chat";
#endif
        }
      );

      // Start the server.
      wssv.Start ();

      if (wssv.IsListening) {
        var fmt = "Listening on port {0}, and providing WebSocket services:";

        Console.WriteLine (fmt, wssv.Port);

        foreach (var path in wssv.WebSocketServices.Paths)
          Console.WriteLine ("- {0}", path);
      }

      Console.WriteLine ("\nPress Enter key to stop the server...");

      Console.ReadLine ();

      // Stop the server.
      wssv.Stop ();
    }
  }
}
