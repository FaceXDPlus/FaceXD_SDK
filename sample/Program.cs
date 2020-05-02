using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
namespace FaceXDSDKSample
{
    class Program
    {
        static private Dictionary<Type, Action<string, FaceXDSDK.Api.BasePack>> handlerMap;
        static FaceXDSDK.Server<FaceXDSDK.Network.WebSocketSharpComponent, FaceXDSDK.Api.v1.ApiService> server;
        static void Main(string[] args)
        {
            /// Create handler map
            handlerMap = new Dictionary<Type, Action<string, FaceXDSDK.Api.BasePack>>
            {
                { typeof(FaceXDSDK.Api.v1.CaptureFaceParameterTransaction), new Action<string, FaceXDSDK.Api.BasePack>(HandleCaptureFaceParameter)},
            };

            /// Setup server
            server = new FaceXDSDK.Server<FaceXDSDK.Network.WebSocketSharpComponent, FaceXDSDK.Api.v1.ApiService>();
            server.OnReceivePack = (guid, pack) =>
            {
                var handler = handlerMap[pack.GetType()];
                handler.Invoke(guid, pack);
            };

            /// Must after register delegate;
            server.Run("ws://127.0.0.1:12003/");
            Console.ReadKey();
        }

        static void HandleCaptureFaceParameter(string guid, FaceXDSDK.Api.BasePack pack)
        {
           var captureFaceParameter = (FaceXDSDK.Api.v1.CaptureFaceParameterTransaction)pack;
            Console.WriteLine("[{0}][CaptureFaceParameter]: headPitch: {1}, headRoll: {2}, headYaw: {3}", guid, captureFaceParameter.HeadPitch, captureFaceParameter.HeadRoll, captureFaceParameter.HeadYaw);
        }
    }
}
