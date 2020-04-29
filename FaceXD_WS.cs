using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using LitJson;
using System.Net;

public class FaceXD_WS : MonoBehaviour
{
    TcpListener listener;
    Thread mThread;
    public FaceXD_json faceXD_Json_data;

    void Update()
    {
        //set faceXD_Json_data to Live2D here..
    }

    private void Start()
    {
        StartWS_Thread();
    }
    private void StartWS_Thread()
    {
        ThreadStart ts = new ThreadStart(FetchScoketData);
        mThread = new Thread(ts);
        mThread.Start();
    }
    private void OnDestroy()
    {
        EndThread();
    }
    void EndThread()
    {
        if (mThread.IsAlive)
        {
            listener.Stop();
            mThread.Abort();
        }
    }

    public async void FetchScoketData()
    {
        string ip = "0.0.0.0";
        int port = 8040;
        listener = new TcpListener(IPAddress.Parse(ip), port); ;//添加需要监听的url


        listener.Start();   //开始监听         
        TcpClient client = await listener.AcceptTcpClientAsync();


        NetworkStream stream = client.GetStream();
        while (true)
        {

            while (!stream.DataAvailable) ;
            while (client.Available < 3) ;//明明是我先来的

            Byte[] bytes = new Byte[client.Available];
            stream.Read(bytes, 0, bytes.Length);
            String data = Encoding.UTF8.GetString(bytes);
            if (new System.Text.RegularExpressions.Regex("^GET").IsMatch(data))
            {
                const string eol = "\r\n"; // HTTP/1.1 将CR LF的顺序定义为任何协议元素的行尾标志

                Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                    + "Connection: Upgrade" + eol
                    + "Upgrade: websocket" + eol
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        System.Security.Cryptography.SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + eol
                    + eol);

                stream.Write(response, 0, response.Length);
            }
            else
            {
                bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // 必须为真，"所有从客户端到服务器的消息都有此位设置"

                int opcode = bytes[0] & 0b00001111, // 文本数据
                    msglen = bytes[1] - 128, // & 0111 1111
                    offset = 2;

                if (msglen == 126)
                {
                    // 为 ToUInt16(bytes, offset) 但结果不对
                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                }
                else if (msglen == 127)
                {
                    Debug.Log("TODO: msglen == 127, needs qword to store msglen");
                    // 我不是很清楚字节顺序，请编辑一下这个
                    // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                    // offset = 10;
                }

                if (msglen == 0)
                    Debug.Log("msglen == 0");
                else if (mask)
                {
                    byte[] decoded = new byte[msglen];
                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (int i = 0; i < msglen; ++i)
                        decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                    string text = Encoding.UTF8.GetString(decoded);
                    faceXD_Json_data = JsonMapper.ToObject<FaceXD_json>(text);//可以用别的json库。

                }
                else
                    Debug.Log("mask bit not set");//掩码位未设置 "https://en.wikipedia.org/wiki/Mask_(computing)"

            }

        }



    }
    public class BlendShapes
    {
        //Todo?
    }

    public class FaceXD_json
    {
        /// <summary>
        /// 
        /// </summary>
        public string eyeROpen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeBrowYL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mouthOpenY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mouthForm { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string headPitch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string headYaw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string headRoll { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeLOpen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeBrowAngleL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeBrowAngleR { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string eyeBrowYR { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BlendShapes blendShapes { get; set; }
    }

}
