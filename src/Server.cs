using FaceXDSDK.Network;
using System;
using System.Text;
namespace FaceXDSDK
{

    public class Server<NetworkComponent>: Object, IDisposable where NetworkComponent: BaseNetworkComponent, new()
    {
        private NetworkComponent networkObject = null;
        public string ListenUrl { set; get; }

        public Server()
        {
            
        }

        public void Dispose()
        {
            this.networkObject.Dispose();
        }
        public void Run(string listenUrl)
        {            this.ListenUrl = listenUrl;
            this.networkObject = new NetworkComponent();
            this.networkObject.Start(listenUrl);

            this.networkObject.OnConnect = new BaseNetworkComponent.Delegate.OnConnect(this.OnNetworkObjectClientConnnect);
            this.networkObject.OnReceiveData = new BaseNetworkComponent.Delegate.OnReceiveData(this.OnNetworkObjectClientReceiveData);
            this.networkObject.OnDisconnect = new BaseNetworkComponent.Delegate.OnDisconnect(this.OnNetworkObjectClientDisconnect);
        }

        public void Stop()
        {
            this.networkObject.Stop();
        }

        public Client GetClient(string guid)
        {
            return this.networkObject.ClientContainer[guid];
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
