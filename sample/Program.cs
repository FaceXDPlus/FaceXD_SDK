using System;
using System.Threading;
namespace FaceXDSDKSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new FaceXDSDK.Server<FaceXDSDK.Network.WebSocketSharpComponent>();
            server.Run("ws://127.0.0.1:12001/");

            Console.ReadKey();
        }
    }
}
