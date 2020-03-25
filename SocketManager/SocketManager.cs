using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;
//using UnityEngine;
//using UnityEngine.UI;

namespace FaceXD
{
    class SocketManager
    {
        private Socket _ServerSocket;                       //服务器监听套接字
        private bool _IsListionContect;                     //是否在监听
        private Thread _thClientMsg;
        public void StopServer()
        {
            if (_ServerSocket != null)
            {
                _IsListionContect = false;
                //_ServerSocket.Shutdown(SocketShutdown.Both)
                _ServerSocket.Close();
                if (_thClientMsg != null)
                {
                    _thClientMsg.Abort();
                }
            }
        }
        public SocketManager()
        {
            try
            {
                //定义网络终节点（封装IP和端口）
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8040);
                //实例化套接字
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //服务端绑定地址
                _ServerSocket.Bind(endPoint);
                //开始监听
                _ServerSocket.Listen(1);
                //监听的最大长度
                Debug.Log("启动监听{0}成功" + _ServerSocket.LocalEndPoint.ToString());

                //我们建议你使用Loom
                /*Loom.RunAsync(
                () =>
                {
                    _thClientMsg = new Thread(ClientMsg);
                    _IsListionContect = true;
                    _thClientMsg.Start();
                    Debug.Log("服务器已经启动...");
                });*/
                _thClientMsg = new Thread(BuildSever);
                _IsListionContect = true;
                _thClientMsg.Start();
                Debug.Log("服务器已经启动...");
            }
            catch (Exception ex)
            {
                Debug.Log("发生错误" + ex.Message);
            }
        }

        private void BuildSever()
        {
            while (_IsListionContect)
            {
                Socket socket = _ServerSocket.Accept();
                Debug.Log("连接到" + socket.RemoteEndPoint);
                ClientMsg(socket);
            }
        }

            /// <summary>
            /// 服务器端和客户端通信的后天线程
            /// </summary>
            /// <param name="?"></param>
        public void ClientMsg(Socket socketMsg)
        {
            while (true)
            {
                if (!socketMsg.Connected)
                {
                    socketMsg.Shutdown(SocketShutdown.Both);
                    socketMsg.Close();
                    break;
                }
                Debug.Log("连接到" + socketMsg.RemoteEndPoint);
                try
                {
                    byte[] msyArray = new byte[0124 * 0124];
                    int receiveNumber = socketMsg.Receive(msyArray);
                    Debug.Log("接收客户端" + socketMsg.RemoteEndPoint.ToString() + "消息， 长度为" + receiveNumber);
                    if (receiveNumber == 0)
                    {
                        Debug.Log("接收失败 关闭连接");
                        socketMsg.Shutdown(SocketShutdown.Both);
                        socketMsg.Close();
                        break;
                    }
                    string strMsg = Encoding.UTF8.GetString(msyArray, 0, receiveNumber);
                    GetStringJson(strMsg);
                }
                catch (Exception ex)
                {
                    Debug.Log("错误：" + ex.Message);
                    socketMsg.Shutdown(SocketShutdown.Both);
                    socketMsg.Close();
                    break;
                    /*Debug.Log("接收失败 关闭连接" + ex.Message);
                    Debug.Log("\n");
                    Loom.QueueOnMainThread((param) =>
                    {
                        _statusText.text = "等待连接";
                    }, null);
                    socketMsg.Shutdown(SocketShutdown.Both);
                    socketMsg.Close();
                    break;*/
                }
            }
        }

        public void GetStringJson(string tmpData)
        {
            List<string> outputList = new List<string>();

            int idxStart = tmpData.IndexOf("{");
            int idxEnd = 0;
            while (tmpData.Contains("}"))
            {
                idxEnd = tmpData.IndexOf("}", idxEnd) + 1;
                Console.WriteLine("{}=>" + idxStart.ToString() + "--" + idxEnd.ToString());
                if (idxStart >= idxEnd)
                {
                    continue;// 找下一个 "}"
                }

                var sJSON = tmpData.Substring(idxStart, idxEnd);

                doJsonPrase(sJSON);

                tmpData = tmpData.Substring(idxEnd); //剩余未解析部分
                idxEnd = 0; //复位

                if (tmpData.Contains("{") && tmpData.Contains("}") && (tmpData.Length > 2))
                {
                    GetStringJson(tmpData);
                    break;
                }
            }
        }

        public void doJsonPrase(string input)
        {
            Debug.Log("解析 JSON ...." + input);

            /*var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            _D2LiveModelController.paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
            _D2LiveModelController.ParamEyeBallXValue = float.Parse(jsonResult["eyeX"].ToString());
            _D2LiveModelController.ParamEyeBallYValue = float.Parse(jsonResult["eyeY"].ToString());
            _D2LiveModelController.paramAngleXValue = float.Parse(jsonResult["headYaw"].ToString());
            _D2LiveModelController.paramAngleYValue = float.Parse(jsonResult["headPitch"].ToString());
            _D2LiveModelController.paramAngleZValue = float.Parse(jsonResult["headRoll"].ToString());
            _D2LiveModelController.ParamBodyAngleXValue = float.Parse(jsonResult["bodyAngleX"].ToString());
            _D2LiveModelController.ParamBodyAngleYValue = float.Parse(jsonResult["bodyAngleY"].ToString());
            _D2LiveModelController.ParamBodyAngleZValue = float.Parse(jsonResult["bodyAngleZ"].ToString());
            _D2LiveModelController.paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
            _D2LiveModelController.paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
            _D2LiveModelController.paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
            _D2LiveModelController.paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
            _D2LiveModelController.paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
            _D2LiveModelController.paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
            _D2LiveModelController.paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());*/
        }

    }

}