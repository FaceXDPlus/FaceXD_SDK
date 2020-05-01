using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace FaceXDSDK.Network
{
    public class WebSocketClient: Client
    {
        public HttpListenerContext http { get; set; }
        public HttpListenerWebSocketContext webSocket { get; set; }

        public override Task CloseAsync()
        {
           return webSocket.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }

    public class WebSocket : BaseNetwork
    {
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


        public WebSocket()
        {

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

        protected async void RemoveClientAsync(string guid)
        {
            Client client = this.ClientContainer[guid];
            if (client != null)
            {
                await client.CloseAsync();
                await Task.Run(() => {
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
                var client = new WebSocketClient
                {
                    http = context,
                    webSocket = webSocketContext
                };
                lock (this.ClientContainer)
                {
                    this.ClientContainer.Add(guid, client);
                }
                await Task.Run(() => {
                    this.onConnect?.Invoke(guid);
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
                    var buffer = System.Net.WebSockets.WebSocket.CreateServerBuffer(WebSocket.ServerBufferSize);
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        await Task.Run(() => {
                            this.OnReceiveData?.Invoke(guid, buffer, result.Count);
                        });
                    }
                }
            }
            catch (Exception e)
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
