using System;
using System.Threading;
namespace FaceXDSDKSample
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceXDSDK.Server<FaceXDSDK.Network.DotNetWebSocketComponent> server = new FaceXDSDK.Server<FaceXDSDK.Network.DotNetWebSocketComponent>();
            server.Run("http://127.0.0.1:12001/");

            Console.ReadKey();
        }
    }
}
