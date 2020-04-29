using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using MaterialUI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class Globle : MonoBehaviour
    {
        public static Dictionary<string, string> IPMessage;
    }
    
    public class AMGMain : MonoBehaviour
    {
        
        private AMGSocketManager mySocketServer;

        void Start()
        {
            Globle.IPMessage = new Dictionary<string, string>();
            
            this.mySocketServer = new AMGSocketManager();
            this.mySocketServer.setSocketSwitch(SocketSwitch);
            this.mySocketServer.SocketStart();
            
            /*
            关闭
                this.mySocketServer.SocketStop();
                Globle.IPMessage = new Dictionary<string, string>();
            */
        }

        public void Update()
        {
            OnBindMessageAndIP();
        }

        public void OnBindMessageAndIP()
        {
            var IPMessage = Globle.IPMessage;
            foreach (KeyValuePair<string, string> kvp in IPMessage)
            {
                Debug.Log(kvp.Key + ":" +  kvp.Value);
                DoJsonPrase(kvp.Value);
            }
        }

        public void DoJsonPrase(string input)
        {
            var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            //在此进行JSON解析
            /*paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
            ParamEyeBallXValue = float.Parse(jsonResult["eyeX"].ToString());
            ParamEyeBallYValue = float.Parse(jsonResult["eyeY"].ToString());
            paramAngleXValue = float.Parse(jsonResult["headYaw"].ToString());
            paramAngleYValue = float.Parse(jsonResult["headPitch"].ToString());
            paramAngleZValue = float.Parse(jsonResult["headRoll"].ToString());
            paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
            paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
            paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
            paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
            paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
            paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
            paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());*/
        }
    }
}
