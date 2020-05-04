using FaceXDSDK.Api;
using FaceXDSDK.Model.NetworkPack;
using FaceXDSDK.Network;
using System;
using System.Collections.Generic;

namespace FaceXDSDK
{

    public class Server<NetworkComponent, ApiComponent> : Object, IDisposable where NetworkComponent : BaseNetworkComponent, new() where ApiComponent : BaseApiService, new()
    {

        private NetworkComponent networkObject = null;
        private ApiComponent apiComponentObject = null;

        public Action<string, BasePack> OnReceivePack;
        public Action<string> OnConnectedClient;
        public Action<string> OnDisconnectedClient;

        public string ListenUrl { set; get; }

        public Server()
        {

        }

        public void Dispose()
        {
            this.networkObject.Dispose();
        }
        public void Run(string listenUrl)
        {
            this.ListenUrl = listenUrl;
            this.networkObject = new NetworkComponent();
            this.networkObject.Start(listenUrl);

            this.networkObject.OnConnect = new BaseNetworkComponent.Delegate.OnConnect(this.OnNetworkObjectClientConnnect);
            this.networkObject.OnReceiveData = new BaseNetworkComponent.Delegate.OnReceiveData(this.OnNetworkObjectClientReceiveData);
            this.networkObject.OnDisconnect = new BaseNetworkComponent.Delegate.OnDisconnect(this.OnNetworkObjectClientDisconnect);

            this.apiComponentObject = new ApiComponent
            {
                OnDispatchPack = OnReceivePack
            };
        }

        public void Stop()
        {
            this.networkObject.Stop();
        }

        public Client GetClient(string guid)
        {
            return this.networkObject.ClientContainer[guid];
        }

        public Dictionary<string, Client>.ValueCollection GetAllClients()
        {
            return this.networkObject.ClientContainer.Values;
        }

        /// MARK: - Handler

        private void OnNetworkObjectClientConnnect(string guid)
        {
#if DEBUG
            Console.WriteLine("Connect: {0}", guid);
#endif
            this.OnConnectedClient?.Invoke(guid);
        }
        private void OnNetworkObjectClientReceiveData(string guid, ArraySegment<byte> buffer, int size)
        {
            var data = new ArraySegment<byte>(buffer.Array, 0, size);
            var pack = new RawJson(data.Array);
            this.apiComponentObject.DispatchPack(pack, guid);
        }

        private void OnNetworkObjectClientDisconnect(string guid)
        {
#if DEBUG
            Console.WriteLine("Disconnet: {0}", guid);
#endif
            this.OnDisconnectedClient?.Invoke(guid);
        }
    }
}
