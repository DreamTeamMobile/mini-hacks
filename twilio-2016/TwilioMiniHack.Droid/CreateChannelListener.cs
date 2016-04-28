using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;
using System;
using System.Collections.Generic;
using Twilio.Common;
using Twilio.IPMessaging;

namespace TwilioMiniHack.Droid
{

    public class CreateChannelListener : ConstantsCreateChannelListener
    {
        public Action<IChannel> OnCreatedHandler { get; set; }
        public Action OnErrorHandler { get; set; }

        public override void OnCreated(IChannel channel)
        {
            OnCreatedHandler?.Invoke(channel);
        }

        public override void OnError(IErrorInfo errorInfo)
        {
            base.OnError(errorInfo);
        }
    }

}
