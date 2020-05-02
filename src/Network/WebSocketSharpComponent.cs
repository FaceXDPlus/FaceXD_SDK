using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FaceXDSDK.Network
{
    public class WebSocketSharpComponent : BaseNetworkComponent
    {
        internal class SDKBehavior : WebSocketBehavior
        {
            public string Guid;
            public Action<SDKBehavior> OnWebSocketOpen { get; set; }
            public Action<SDKBehavior, CloseEventArgs> OnWebSocketClose { get; set; }
            public Action<SDKBehavior, ErrorEventArgs> OnWebSocketError { get; set; }
            public Action<SDKBehavior, MessageEventArgs> OnWebSocketMessage { get; set; }

            protected override void OnOpen()
            {
                this.OnWebSocketOpen?.Invoke(this);
            }

            protected override void OnClose(CloseEventArgs e)
            {
                this.OnWebSocketClose?.Invoke(this, e);
            }

            protected override void OnError(ErrorEventArgs e)
            {
                this.OnWebSocketError?.Invoke(this, e);
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                this.OnWebSocketMessage?.Invoke(this, e);
            }

            public void CloseWebSocketAsync()
            {
                this.CloseAsync();
            }

            public void SendDataWebSocketAsync(ArraySegment<byte> buffer, int size)
            {
                var data = new ArraySegment<byte>(buffer.ToArray(), 0, size);
                this.SendAsync(data.ToArray(), (complete) =>
                {
                    /// [TODO] Send OK
                });
            }

        }

        public class WebSocketSharpClient: Client {

            public Action<string, bool> OnCloseAsyncNotifyServer;

            private SDKBehavior behavior;
            public WebSocketSharpClient()
            {

            }
            internal WebSocketSharpClient(SDKBehavior behavior, string guid)
            {
                this.behavior = behavior;
                this.behavior.Guid = guid;
                this.Guid = guid;
            }
            public override Task CloseAsync()
            {
                return Task.Run(() =>
                {
                    behavior.CloseWebSocketAsync();
                    OnCloseAsyncNotifyServer(this.Guid, false);
                });
            }

            public override Task SendDataAsync(ArraySegment<byte> buffer, int size)
            {
                return Task.Run(() =>
                {
                    this.behavior.SendDataWebSocketAsync(buffer, size);
                });
            }

            public override IPEndPoint UserEndpointAddress
            {
                get
                {
                    return this.behavior.Context.UserEndPoint;
                }
            }
        }

        private WebSocketServer webSocketServer;

        public override void Start(string listenUrl)
        {
            this.webSocketServer = new WebSocketServer(listenUrl);
            this.webSocketServer.AddWebSocketService<SDKBehavior>("/", (obj) => {
                obj.OnWebSocketClose = new Action<SDKBehavior, CloseEventArgs>(this.OnWebSocketClose);
                obj.OnWebSocketError = new Action<SDKBehavior, ErrorEventArgs>(this.OnWebSocketError);
                obj.OnWebSocketMessage = new Action<SDKBehavior, MessageEventArgs>(this.OnWebSocketMessageAsync);
                obj.OnWebSocketOpen = new Action<SDKBehavior>(this.OnWebSocketOpenAsync);
            });
            this.webSocketServer.Start();
        }

        public override void Stop()
        {
            this.webSocketServer.Stop();
        }

        protected async void RemoveClientAsync(string guid, bool closeClient)
        {
            Client client = this.ClientContainer[guid];
            if (client != null)
            {
                if (closeClient)
                {
                    await client.CloseAsync();
                }
                await Task.Run(() =>
                {
                    this.OnDisconnect?.Invoke(guid);
                });
            }
            lock (this.ClientContainer)
            {
                if (this.ClientContainer.ContainsKey(guid)) {
                    this.ClientContainer.Remove(guid);
                }
            }
            GC.Collect();
        }
        private async void OnWebSocketOpenAsync(SDKBehavior behavior)
        {
            var guid = Guid.NewGuid().ToString();
            WebSocketSharpClient client = new WebSocketSharpClient(behavior, guid);
            client.OnCloseAsyncNotifyServer = new Action<string, bool>(this.RemoveClientAsync);
            lock (this.ClientContainer)
            {
                this.ClientContainer.Add(guid, client);
            }
            await Task.Run(() =>
            {
                this.OnConnect?.Invoke(guid);
            });
        }

        private void OnWebSocketClose(SDKBehavior behavior, CloseEventArgs e)
        {
            this.RemoveClientAsync(behavior.Guid, false);
        }

        private void OnWebSocketError(SDKBehavior behavior, ErrorEventArgs e)
        {
            this.RemoveClientAsync(behavior.Guid, false);
        }

        private async void OnWebSocketMessageAsync(SDKBehavior behavior, MessageEventArgs e)
        {
            ArraySegment<byte> data = new ArraySegment<byte>(e.RawData);
            await Task.Run(() =>
            {
                this.OnReceiveData?.Invoke(behavior.Guid, data, data.Count);
            });
        }
    }
}
