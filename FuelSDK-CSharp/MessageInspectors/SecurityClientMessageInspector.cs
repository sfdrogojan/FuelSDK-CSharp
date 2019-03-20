using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using FuelSDK.MessageHeaders;
using FuelSDK.MessageHeaders.Models;

namespace FuelSDK.MessageInspectors
{
    public class SecurityClientMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            SecurityMessageHeader securityMessageHeader = new SecurityMessageHeader
            {
                UsernameToken = new UsernameToken()
                {
                    Id = $"uuid-{Guid.NewGuid()}-1", Username = "*", Password = "*"
                }
            };
            request.Headers.Add(securityMessageHeader);
            return null;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            
        }
    }
}