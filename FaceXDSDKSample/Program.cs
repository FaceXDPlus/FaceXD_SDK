using System;
using System.Threading;
using FaceXDSDK.Network;
namespace FaceXDSDKSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ready to create websocket server");
            WebSocket websocket = new WebSocket();
            websocket.Start("http://127.0.0.1:12001/");
            Console.WriteLine("Websocket server start");
            Console.ReadKey();
        }
    }
}
