using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smsforwarder.server
{
    internal interface IServerApi
    {
    }

    internal class ServerException : Exception
    {
        public ServerException(string message) : base(message) { }
    }
}
