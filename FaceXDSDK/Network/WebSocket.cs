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
    public class WebSocketClient
    {
        public HttpListenerContext http { get; set; }
        public HttpListenerWebSocketContext webSocket { get; set; }
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
        private bool isRunning = false;

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

        /// <summary>
        /// Key: User UUID String
        /// </summary>
        private Dictionary<String, WebSocketClient> clientMap = new Dictionary<string, WebSocketClient>();

        public WebSocket()
        {

        }

        public void Start(string listenUrl)
        {
            if (this.isRunning)
            {
                return;
            }

            this.listener.Prefixes.Add(listenUrl);
            this.listener.Start();

            OnStartServerAsync();
        }

        private async void OnStartServerAsync()
        {
            this.isRunning = true;
            while (this.isRunning)
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
            this.isRunning = false;
        }

        /// MARK: - Handler
        private async void OnReceiveWebSocketRequestAsync(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await context.AcceptWebSocketAsync(null);
                var guid = Guid.NewGuid().ToString();
                var client = new WebSocketClient();
                client.http = context;
                client.webSocket = webSocketContext;
                lock (this.clientMap)
                {
                    this.clientMap.Add(guid, client);
                }
                OnAcceptWebSocketAsync(webSocketContext, guid);
            }
            catch (Exception e)
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
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        lock (this.clientMap)
                        {
                            this.clientMap.Remove(guid);
                        }
                    }
                    else
                    {
                        await OnWebSocketReceiveDataAsync(webSocket, guid, buffer, result.Count);
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
                    lock (this.clientMap)
                    {
                        this.clientMap.Remove(guid);
                    }
                }
            }
        }
        private Task OnWebSocketReceiveDataAsync(System.Net.WebSockets.WebSocket webSocket, string guid, ArraySegment<byte> buffer, int count)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("receive from {0}: {1}", guid, Encoding.UTF8.GetString(buffer.Array, 0, count));
            });
        }
    }
}
