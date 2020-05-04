using FaceXDSDK.Model.NetworkPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FaceXDSDK.Api
{
    public class BaseApiService
    {
        public Action<string, BasePack> OnDispatchPack;
        virtual public void DispatchPack(BaseNetworkPack pack, string guid)
        {
            pack.Deserialized();
        }
    }

    public class BasePack : RawJson
    {
        public const string TypeSet = "set";
        public const string TypeGet = "get";
        public const string TypeAction = "action";
        public const string TypeTransaction = "transaction";

        public BasePack(byte[] data) : base(data)
        {

        }

        public string Type { get; set; } = "";
        public string Command { get; set; } = "";
        public string Parameter { get; set; } = "";

        public override byte[] Serialize()
        {
            this.JsonDictionary.Add("type", this.Type);
            this.JsonDictionary.Add("command", this.Command);
            this.JsonDictionary.Add("parameter", this.Parameter);

            var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, this.JsonDictionary);
            return stream.GetBuffer();
        }
    }
}
