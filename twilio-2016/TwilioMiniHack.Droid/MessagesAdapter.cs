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
    public class MessagesAdapter : BaseAdapter<IMessage>
    {
        private List<IMessage> messages = new List<IMessage>();
        private Activity activity;

        public MessagesAdapter(Activity parentActivity)
        {
            activity = parentActivity;
        }


        public void AddMessage(IMessage msg)
        {
            lock (messages)
            {
                messages.Add(msg);
            }

            activity.RunOnUiThread(() =>
                NotifyDataSetChanged());
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var view = convertView as LinearLayout ?? activity.LayoutInflater.Inflate(Resource.Layout.MessageItemLayout, null) as LinearLayout;
            var message = messages[position];
            view.FindViewById<TextView>(Resource.Id.authorTextView).Text = message.Author;
            view.FindViewById<TextView>(Resource.Id.messageTextView).Text = message.MessageBody;
            return view;
        }

        public override int Count => messages.Count;

        public override IMessage this[int index] 
        { 
            get 
            {
                return messages[index]; 
            }
        }
    }
}
