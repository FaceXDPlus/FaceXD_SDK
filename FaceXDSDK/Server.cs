using FaceXDSDK.Network;
using System;
using System.Text;
namespace FaceXDSDK
{

    public class Server<NetworkComponent>: Object where NetworkComponent: BaseNetwork, new()
    {
        private NetworkComponent networkObject = null;
        public string ListenUrl { set; get; }


        public static Server<WebSocket> DefaultWebSocketServer()
        {
            return new Server<WebSocket>();
        }
        public Server()
        {
            
        }

        public void Run(string listenUrl)
        {            this.ListenUrl = listenUrl;
            this.networkObject = new NetworkComponent();
            this.networkObject.Start(listenUrl);

            this.networkObject.onConnect = new BaseNetwork.Delegate.OnConnect(this.OnNetworkObjectClientConnnect);
            this.networkObject.OnReceiveData = new BaseNetwork.Delegate.OnReceiveData(this.OnNetworkObjectClientReceiveData);
            this.networkObject.OnDisconnect = new BaseNetwork.Delegate.OnDisconnect(this.OnNetworkObjectClientDisconnect);
        }

        public void Stop()
        {
            this.networkObject.Stop();
        }

        /// MARK: - Handler
        
        private void OnNetworkObjectClientConnnect(string guid)
        {
            Console.WriteLine("Client {0} connected", guid);
        }
        private void OnNetworkObjectClientReceiveData(string guid, ArraySegment<byte> buffer, int size)
        {
            Console.WriteLine("receive from {0}: {1}", guid, Encoding.UTF8.GetString(buffer.Array, 0, size));
        }

        private void OnNetworkObjectClientDisconnect(string guid)
        {
            Console.WriteLine("Client {0} disconnect", guid);

        }
    }
}
