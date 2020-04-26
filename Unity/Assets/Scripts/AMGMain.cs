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
        public static string DataLog;
    }
    public class AMGMain : MonoBehaviour
    {
        
        private AMGSocketManager mySocketServer;
        private ArrayList ModelList;

        void Start()
        {
            ModelList = new ArrayList();
            Globle.IPMessage = new Dictionary<string, string>();
        }

        public void onSocketSwitchSwitched()
        {
            if (SocketSwitch.isOn == true)
            {
                this.mySocketServer = new AMGSocketManager();
                this.mySocketServer.setSocketSwitch(SocketSwitch);
                this.mySocketServer.SocketStart();
            }
            else
            {
                this.mySocketServer.SocketStop();
                Globle.IPMessage = new Dictionary<string, string>();
            }
        }

        public void Update()
        {
            OnBindMessageAndIP();
        }

        public void OnBindMessageAndIP()
        {
            var ModelToIP = Globle.ModelToIP;
            var IPMessage = Globle.IPMessage;
            foreach (KeyValuePair<string, string> kvp in ModelToIP)
            {
                if (IPMessage.ContainsKey(kvp.Value) && IPMessage[kvp.Value] != "")
                {
                    try
                    {
                        foreach (CubismModel Model in ModelList)
                        {
                            if (Model.name == kvp.Key)
                            {
                                DoJsonPrase(IPMessage[kvp.Value], Model);
                                //在此解析json
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Log("发生错误 " + ex.Message + ":" + ex.StackTrace);
                    }
                }
            }
        }

        public void DoJsonPrase(string input, CubismModel model)
        {
            var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            model.GetComponent<AMGModelController>().paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
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
            model.GetComponent<AMGModelController>().paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());
        }
        
        public void Log(string text)
        {
            //DebugLogText.text = DebugLogText.text + "\n" + text;
        }
    }
}
