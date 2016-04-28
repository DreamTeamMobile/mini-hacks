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
    [Activity(Label = "#general", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, IPMessagingClientListener, IChannelListener, ITwilioAccessManagerListener
    {
       
        internal const string TAG = "TWILIO";

        Button sendButton;
        EditText textMessage;
        ListView listView;
        MessagesAdapter adapter;

        ITwilioIPMessagingClient client;
        IChannel generalChannel;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.ActionBar.Subtitle = "logging in...";
            SetContentView(Resource.Layout.Main);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);
            textMessage = FindViewById<EditText>(Resource.Id.messageTextField);
            listView = FindViewById<ListView>(Resource.Id.listView);
            adapter = new MessagesAdapter(this);
            listView.Adapter = adapter;

            TwilioIPMessagingSDK.SetLogLevel((int)Android.Util.LogPriority.Debug);
            if (!TwilioIPMessagingSDK.IsInitialized)
            {
                Console.WriteLine("Initialize");

                TwilioIPMessagingSDK.InitializeSDK(this, new InitListener
                    {
                        InitializedHandler = async delegate
                        {
                            await Setup();
                        },
                        ErrorHandler = err =>
                        {
                            Console.WriteLine(err.Message);
                        }
                    });
            }
            else
            {
                await Setup();
            }
            sendButton.Click += ButtonSend_Click;
        }

        async Task Setup()
        {
            var token = await GetIdentity();
            var accessManager = TwilioAccessManagerFactory.CreateAccessManager(token, this);
            client = TwilioIPMessagingSDK.CreateIPMessagingClientWithAccessManager(accessManager, this);

            client.Channels.LoadChannelsWithListener(new StatusListener
                {
                    SuccessHandler = () =>
                    {
                        generalChannel = client.Channels.GetChannelByUniqueName("general");

                        if (generalChannel != null)
                        {
                            generalChannel.Listener = this;
                            JoinGeneralChannel();
                        }
                        else
                        {
                            CreateAndJoinGeneralChannel();
                        }
                    }
                });
        }

        async Task<string> GetIdentity()
        {
            var androidId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var tokenEndpoint = $"https://twilio-drmtmxamarin.rhcloud.com/token?device={androidId}";
            var http = new HttpClient();
            var data = await http.GetStringAsync(tokenEndpoint);
            var json = System.Json.JsonObject.Parse(data);
            var identity = json["identity"]?.ToString()?.Trim('"');
            this.ActionBar.Subtitle = $"Logged in as {identity}";
            var token = json["token"]?.ToString()?.Trim('"');
            return token;
        }

        public void OnTokenExpired(ITwilioAccessManager p0)
        {
            Console.WriteLine("token expired");
        }

        public void OnTokenUpdated(ITwilioAccessManager p0)
        {
            Console.WriteLine("token updated");
        }

        private void JoinGeneralChannel()
        {
            generalChannel.Join(new StatusListener
                {
                    SuccessHandler = () =>
                    {
                        RunOnUiThread(() => Toast.MakeText(this, "Joined general channel!", ToastLength.Short).Show());
                    }
                });
        }

        private void CreateAndJoinGeneralChannel()
        {
            var options = new Dictionary<string, Java.Lang.Object>();
            options["friendlyName"] = "General Chat Channel";
            options["ChannelType"] = ChannelChannelType.ChannelTypePublic;
            client.Channels.CreateChannel(options, new CreateChannelListener
                {
                    OnCreatedHandler = channel =>
                    {
                        generalChannel = channel;
                        channel.SetUniqueName("general", new StatusListener
                            {
                                SuccessHandler = () =>
                                {
                                    Console.WriteLine("set unique name successfully!");
                                }
                            });
                        this.JoinGeneralChannel();
                    },
                    OnErrorHandler = () =>
                    {
                    }
                });
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textMessage.Text))
            {
                var message = generalChannel.Messages.CreateMessage(textMessage.Text);
                generalChannel.Messages.SendMessage(message, new StatusListener
                    {
                        SuccessHandler = () =>
                        {
                            RunOnUiThread(() =>
                                {
                                    textMessage.Text = string.Empty;
                                });
                        }
                    });
            }
        }

        public void OnMessageAdd(IMessage message)
        {
            adapter.AddMessage(message);
            listView.SmoothScrollToPosition(adapter.Count - 1);
        }

        #region IChannelListener, IPMessagingClientListener,ITwilioAccessManagerListener

        public void OnError(ITwilioAccessManager p0, string p1)
        {
        }

        public void OnAttributesChange(IDictionary<string, string> p0)
        {
        }

        public void OnMemberChange(IMember p0)
        {
        }

        public void OnMemberDelete(IMember p0)
        {
        }

        public void OnMemberJoin(IMember p0)
        {
        }

        public void OnMessageChange(IMessage p0)
        {
        }

        public void OnMessageDelete(IMessage p0)
        {
        }

        public void OnTypingEnded(IMember p0)
        {
        }

        public void OnTypingStarted(IMember p0)
        {
        }

        public void OnAttributesChange(string p0)
        {
        }

        public void OnChannelAdd(IChannel p0)
        {
        }

        public void OnChannelChange(IChannel p0)
        {
        }

        public void OnChannelDelete(IChannel p0)
        {
        }

        public void OnChannelHistoryLoaded(IChannel p0)
        {
        }

        public void OnError(IErrorInfo p0)
        {
        }

        public void OnUserInfoChange(IUserInfo p0)
        {
        }
        #endregion

    }
}



