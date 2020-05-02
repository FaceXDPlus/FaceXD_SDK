using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FaceXDSDK
{
    public class Client
    {
        virtual public Task CloseAsync()
        {
            return Task.Run(() =>
            {

            });
        }
    }
}
