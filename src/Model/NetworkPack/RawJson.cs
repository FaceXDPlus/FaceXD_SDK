using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace FaceXDSDK.Model.NetworkPack
{
    public class RawJson: BaseNetworkPack
    {
        public Dictionary<string, object> JsonDictionary { get; set; }
        public RawJson(byte[] data): base(data)
        {
            
        }

        public override byte[] Serialize()
        {
            return Array.Empty<byte>();
        }

        public override void Deserialized()
        {
            var buffer = Array.ConvertAll<byte, char>(this.RawData, s => (char)s);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(buffer)))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>), settings);
                try
                {
                    Dictionary<string, object> results = (Dictionary<string, object>)serializer.ReadObject(ms);
                    this.JsonDictionary = results;
                }
                catch
                {
                    this.JsonDictionary = new Dictionary<string, object>
                    {

                    };
                }
            }
        }
    }
}
