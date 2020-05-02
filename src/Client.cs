using System;
using System.Threading.Tasks;
using System.Net;

namespace FaceXDSDK
{
    public class Client
    {
        public virtual IPEndPoint UserEndpointAddress
        {
            get
            {
                return null;
            }
        }
        public string Guid { get; set; }
        virtual public Task CloseAsync()
        {
            return Task.Run(() =>
            {

            });
        }

        virtual public Task SendDataAsync(ArraySegment<byte> buffer, int size)
        {
            return Task.Run(() =>
            {

            });
        }
    }
}
