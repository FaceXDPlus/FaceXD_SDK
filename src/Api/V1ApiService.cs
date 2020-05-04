using FaceXDSDK.Model.NetworkPack;
using System;
using System.Collections.Generic;

namespace FaceXDSDK.Api
{
    namespace v1
    {
        public class ApiService : BaseApiService
        {

            Dictionary<string, Type> packMap;
            public ApiService()
            {
                /// Register all api handler
                RegisterAllApiHandler();
            }

            private void RegisterAllApiHandler()
            {
                /// TODO 当前只支持 CaptureFaceParameterTransaction
                this.packMap = new Dictionary<string, Type>
                {
                    { "", typeof(CaptureFaceParameterTransaction)}
                };
            }

            public override void DispatchPack(BaseNetworkPack pack, string guid)
            {
                pack.Deserialized();
                if (pack.GetType() == typeof(RawJson))
                {
                    DispatchRawJsonPack((RawJson)pack, guid);
                }
            }

            private void DispatchRawJsonPack(RawJson jsonPack, string guid)
            {
                var command = "";
                if (jsonPack.JsonDictionary.ContainsKey("command"))
                {
                    command = (string)jsonPack.JsonDictionary["command"];
                }
                Type obj = this.packMap[command];
                var pack = (BasePack)Activator.CreateInstance(obj, jsonPack.RawData);
                pack.Deserialized();
                try
                {
                    this.OnDispatchPack?.Invoke(guid, pack);
                }
                catch
                {

                }
            }
        }

        public class CaptureFaceParameterTransaction : BasePack
        {
            public CaptureFaceParameterTransaction(byte[] data) : base(data)
            {

            }
            public string EyeROpen
            {
                get
                {
                    return (string)this.JsonDictionary["eyeROpen"];
                }
                set
                {
                    this.JsonDictionary["eyeROpen"] = value;
                }
            }

            public string EyeBrowYL
            {
                get
                {
                    return (string)this.JsonDictionary["eyeBrowYL"];
                }
                set
                {
                    this.JsonDictionary["eyeBrowYL"] = value;
                }
            }

            public string MouthOpenY
            {
                get
                {
                    return (string)this.JsonDictionary["mouthOpenY"];
                }
                set
                {
                    this.JsonDictionary["mouthOpenY"] = value;
                }
            }

            public string MouthForm
            {
                get
                {
                    return (string)this.JsonDictionary["mouthForm"];
                }
                set
                {
                    this.JsonDictionary["mouthForm"] = value;
                }
            }

            public string HeadPitch
            {
                get
                {
                    return (string)this.JsonDictionary["headPitch"];
                }
                set
                {
                    this.JsonDictionary["headPitch"] = value;
                }
            }

            public string HeadYaw
            {
                get
                {
                    return (string)this.JsonDictionary["headYaw"];
                }
                set
                {
                    this.JsonDictionary["headYaw"] = value;
                }
            }

            public string HeadRoll
            {
                get
                {
                    return (string)this.JsonDictionary["headRoll"];
                }
                set
                {
                    this.JsonDictionary["headRoll"] = value;
                }
            }

            public string EyeLOpen
            {
                get
                {
                    return (string)this.JsonDictionary["eyeLOpen"];
                }
                set
                {
                    this.JsonDictionary["eyeLOpen"] = value;
                }
            }

            public string EyeBrowAngleL
            {
                get
                {
                    return (string)this.JsonDictionary["eyeBrowAngleL"];
                }
                set
                {
                    this.JsonDictionary["eyeBrowAngleL"] = value;
                }
            }

            public string EyeY
            {
                get
                {
                    return (string)this.JsonDictionary["eyeY"];
                }
                set
                {
                    this.JsonDictionary["eyeY"] = value;
                }
            }

            public string EyeBrowAngleR
            {
                get
                {
                    return (string)this.JsonDictionary["eyeBrowAngleR"];
                }
                set
                {
                    this.JsonDictionary["eyeBrowAngleR"] = value;
                }
            }

            public string EyeX
            {
                get
                {
                    return (string)this.JsonDictionary["eyeX"];
                }
                set
                {
                    this.JsonDictionary["eyeX"] = value;
                }
            }

            public string EyeBrowYR
            {
                get
                {
                    return (string)this.JsonDictionary["eyeBrowYR"];
                }
                set
                {
                    this.JsonDictionary["eyeBrowYR"] = value;
                }
            }
        }
    }
}
