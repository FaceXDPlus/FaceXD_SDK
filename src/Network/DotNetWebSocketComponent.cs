using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaceXDSDK.Network
{
    public class DotNetWebSocketComponent : BaseNetworkComponent
    {
        public class DotNetWebSocketClient : Client
        {
            public Action<string, bool> OnCloseAsyncNotifyServer;
            public DotNetWebSocketClient(HttpListenerContext httpContext, HttpListenerWebSocketContext webSocketContext, string guid)
            {
                this.Guid = guid;
                this.httpContext = httpContext;
                this.webSocketContext = webSocketContext;
            }
            private HttpListenerContext httpContext;
            private HttpListenerWebSocketContext webSocketContext;

            public override Task CloseAsync()
            {
                var task = webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                OnCloseAsyncNotifyServer(this.Guid, false);
                return task;
            }

            public override Task SendDataAsync(ArraySegment<byte> buffer, int size)
            {
                var data = new ArraySegment<byte>(buffer.Array, 0, size);
                return this.webSocketContext.WebSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            public override IPEndPoint UserEndpointAddress
            {
                get
                {
                    return this.httpContext.Request.RemoteEndPoint;
                }
            }
        }

        private const int ServerBufferSize = 4 * 1024 * 1024;
        private enum HttpStatusCode
        {
            OK = 200,

            InternalServerError = 500,
            NotImplemented = 501,
        }

        private HttpListener _listern;
        private HttpListener listener
        {
            get
            {
                if (_listern == null)
                {
                    _listern = new HttpListener();
                }
                return _listern;
            }
        }


        public DotNetWebSocketComponent()
        {

        }

        public new void Dispose()
        {
            this.Stop();
        }

        public override void Start(string listenUrl)
        {
            if (this.IsRunning)
            {
                return;
            }

            this.listener.Prefixes.Add(listenUrl);
            this.listener.Start();

            OnStartServerAsync();
        }

        public override void Stop()
        {
            OnStopServerAsync();
        }

        protected void RemoveClientAsync(string guid)
        {
            RemoveClientAsync(guid, true);
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
                if (this.ClientContainer.ContainsKey(guid))
                {
                    this.ClientContainer.Remove(guid);
                }
            }
            GC.Collect();
        }

        /// MARK: - Handler
        private async void OnStartServerAsync()
        {
            this.IsRunning = true;
            while (this.IsRunning)
            {
                HttpListenerContext context = await this.listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    /// Receive websocket request
                    OnReceiveWebSocketRequestAsync(context);
                }
                else
                {
                    /// Not support, just close ret unsupport
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    context.Response.Close();
                }
            }
            this.IsRunning = false;
        }

        private async void OnStopServerAsync()
        {
            this.listener.Stop();
            List<Task> tasks = new List<Task>();

            foreach (var kv in this.ClientContainer)
            {
                var task = kv.Value.CloseAsync();
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            this.IsRunning = false;
        }
        private async void OnReceiveWebSocketRequestAsync(HttpListenerContext context)
        {
            try
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                var guid = Guid.NewGuid().ToString();
                var client = new DotNetWebSocketComponent.DotNetWebSocketClient(context, webSocketContext, guid)
                {
                    OnCloseAsyncNotifyServer = new Action<string, bool>(this.RemoveClientAsync)
                };
                lock (this.ClientContainer)
                {
                    this.ClientContainer.Add(guid, client);
                }
                await Task.Run(() =>
                {
                    this.OnConnect?.Invoke(guid);
                });
                OnAcceptWebSocketAsync(webSocketContext, guid);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
        }

        private async void OnAcceptWebSocketAsync(WebSocketContext context, string guid)
        {
            var webSocket = context.WebSocket;
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var buffer = WebSocket.CreateServerBuffer(DotNetWebSocketComponent.ServerBufferSize);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        await Task.Run(() =>
                        {
                            this.OnReceiveData?.Invoke(guid, buffer, result.Count);
                        });
                    }
                }
            }
            catch
            {
                /// Just catch
            }
            finally
            {
                if (webSocket != null)
                {
                    webSocket.Dispose();
                    RemoveClientAsync(guid);
                }
            }
        }
    }
}
