using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;

namespace FaceXDSDK.Model.NetworkPack
{
    public class BaseNetworkPack
    {
        public byte[] RawData { get; set; }
        public BaseNetworkPack()
        {

        }
        public BaseNetworkPack(byte[] data)
        {
            this.RawData = data;
        }

        virtual public byte[] Serialize()
        {
            return Array.Empty<byte>();
        }

        virtual public void Deserialized()
        {
            
        }
    }
}
