using FaceXDSDK.Network;
using System;

namespace FaceXDSDK
{
    
    public class Server<NetworkComponent>: Object where NetworkComponent: BaseNetwork
    {
        private NetworkComponent NetowrkObject = null;
        public int Port { set; get; }


        public static Server<WebSocket> DefaultWebSocketServer()
        {
            return new Server<WebSocket>();
        }
        public Server()
        {

        }

        public void Run(int port)
        {
            this.Port = port;
        }

        public void Stop()
        {
            
        }
    }
}
