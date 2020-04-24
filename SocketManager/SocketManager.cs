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
        private Socket _ServerSocket;
        private bool _IsListionContect;
        private Thread _thClientMsg;
        public void StopServer()
        {
            if (_ServerSocket != null)
            {
                _IsListionContect = false;
                _ServerSocket.Shutdown(SocketShutdown.Both);
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
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8040);
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _ServerSocket.Bind(endPoint);
                _ServerSocket.Listen(1);
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

        //public void GetStringJson(string tmpData, CubismModel model)
        public void GetStringJson(string tmpData)
        {
            int idxStart = tmpData.IndexOf("{");
            tmpData = tmpData.Substring(idxStart, tmpData.Length - idxStart);
            idxStart = 0;
            int idxEnd = 0;
            while (tmpData.Contains("}}"))
            {
                idxEnd = tmpData.IndexOf("}}", idxEnd) + 2;
                //Console.WriteLine("{}=>" + idxStart.ToString() + "--" + idxEnd.ToString());
                if (idxStart >= idxEnd)
                {
                    continue;// 找下一个 "}"
                }

                var sJSON = tmpData.Substring(idxStart, idxEnd);

                
                //DoJsonPrase(sJSON, model);
                DoJsonPrase(sJSON);
                //break;

                tmpData = tmpData.Substring(idxEnd); //剩余未解析部分
                idxEnd = 0; //复位

                if (tmpData.Contains("{") && tmpData.Contains("}}") && (tmpData.Length > 2))
                {
                    GetStringJson(tmpData, model);
                    break;
                }
            }
        }

        //public void DoJsonPrase(string input, CubismModel model)
        public void DoJsonPrase(string input)
        {
            if (input.Substring(0, 1) == "{" && input.Substring(input.Length - 2, 2) == "}}")
            {
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
                /*model.GetComponent<AMGModelController>().paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
                model.GetComponent<AMGModelController>().ParamEyeBallXValue = float.Parse(jsonResult["eyeX"].ToString());
                model.GetComponent<AMGModelController>().ParamEyeBallYValue = float.Parse(jsonResult["eyeY"].ToString());
                model.GetComponent<AMGModelController>().paramAngleXValue = float.Parse(jsonResult["headYaw"].ToString());
                model.GetComponent<AMGModelController>().paramAngleYValue = float.Parse(jsonResult["headPitch"].ToString());
                model.GetComponent<AMGModelController>().paramAngleZValue = float.Parse(jsonResult["headRoll"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleXValue = float.Parse(jsonResult["bodyAngleX"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleYValue = float.Parse(jsonResult["bodyAngleY"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleZValue = float.Parse(jsonResult["bodyAngleZ"].ToString());
                model.GetComponent<AMGModelController>().paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
                model.GetComponent<AMGModelController>().paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
                model.GetComponent<AMGModelController>().paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
                model.GetComponent<AMGModelController>().paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
                model.GetComponent<AMGModelController>().paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
                model.GetComponent<AMGModelController>().paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
                model.GetComponent<AMGModelController>().paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());*/
            }
        }

    }

}