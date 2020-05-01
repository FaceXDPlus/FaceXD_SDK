using System;
using System.Threading;
using FaceXDSDK;
namespace FaceXDSDKSample
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceXDSDK.Server<FaceXDSDK.Network.WebSocket> server = new FaceXDSDK.Server<FaceXDSDK.Network.WebSocket>();
            server.Run("http://127.0.0.1:12001/");

            Console.ReadKey();
        }
    }
}
