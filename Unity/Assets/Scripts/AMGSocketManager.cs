using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Networking;
using NetworkSocket;
using NetworkSocket.Plugs;
using System.Threading.Tasks;
using NetworkSocket.Core;
using NetworkSocket.Fast;
using NetworkSocket.Tasks;
using NetworkSocket.WebSocket;
using System.Linq;
using System.Collections.Generic;
using NetworkSocket.Http;
using Newtonsoft.Json.Linq;

namespace AMG
{
    public class CustomPlug : PlugBase
    {
        protected sealed override void OnConnected(object sender, IContenxt context)
        {
            var log = string.Format("时间:{0} 用户:{1} 连接", DateTime.Now.ToString("mm:ss"), context.Session.ToString());
            //Debug.Log(log);
            Globle.DataLog = Globle.DataLog + log;
        }

        protected sealed override void OnDisconnected(object sender, IContenxt context)
        {
            var log = string.Format("时间:{0} 用户:{1} 断开连接", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString());
            //Debug.Log(log);
            Globle.DataLog = Globle.DataLog + log;
            Globle.IPMessage.Remove(context.Session.RemoteEndPoint.ToString());
        }

        protected sealed override void OnException(object sender, Exception exception)
        {
            var log = string.Format("时间:{0} 发生错误：{1} {2}", DateTime.Now.ToString("mm:ss"), exception.Message, exception.StackTrace);
            //Debug.Log(log);
            Globle.DataLog = Globle.DataLog + log;
            //Globle.IPMessage.Remove(context.Session.RemoteEndPoint.ToString());
        }
    }

    public class AMGSocketManager : MonoBehaviour
    {
        private Toggle SocketSwitch;

        private NetworkSocket.TcpListener listener;


        public void setSocketSwitch(Toggle socketSwitch)
        {
            this.SocketSwitch = socketSwitch;
        }
        
        public void SocketStart()
        {
            try
            {
                listener = new NetworkSocket.TcpListener();
                listener.Use<WebSocketMiddleware>();
                listener.UsePlug<CustomPlug>();
                listener.Start(8040);
                Globle.DataLog = Globle.DataLog + "服务器已经启动";

            }
            catch (Exception ex)
            {
                SocketSwitch.isOn = false;
                Globle.DataLog = Globle.DataLog + "发生错误 " + ex.Message + " : " + ex.StackTrace;
            }
        }

        public void SocketStop()
        {
            listener.Dispose();
            Globle.DataLog = Globle.DataLog + "服务器已经关闭";
        }

    }
}