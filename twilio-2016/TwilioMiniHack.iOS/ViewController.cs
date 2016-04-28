using System;
using System.Linq;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;
using System.Collections.Generic;
using CoreGraphics;
using Twilio.Common;
using Twilio.IPMessaging;


namespace TwilioMiniHack.iOS
{
    public partial class ViewController : UIViewController, IUITextFieldDelegate, ITwilioIPMessagingClientDelegate, ITwilioAccessManagerDelegate
    {
        // Our chat client
        TwilioIPMessagingClient client;
        // The channel we'll chat in
        Channel generalChannel;
        // Our username when we connect
        string identity;
        MessagesDataSource dataSource;

        public ViewController(IntPtr handle)
            : base(handle)
        {
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardWillShow);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyboardDidShow);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillHide);

            this.View.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        this.messageTextField.ResignFirstResponder();
                    }));

            dataSource = new MessagesDataSource();
            tableView.Source = dataSource;
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 70;

            var token = await GetToken();
            this.NavigationItem.Prompt = $"Logged in as {identity}";
            var accessManager = TwilioAccessManager.Create(token, this);
            client = TwilioIPMessagingClient.Create(accessManager, this);

            client.GetChannelsList((result, channels) =>
                {
                    generalChannel = channels.GetChannelWithUniqueName("general");

                    if (generalChannel != null)
                    {
                        generalChannel.Join(r =>
                            {
                                Console.WriteLine("successfully joined general channel!");
                            });
                    }
                    else
                    {
                        var options = new NSDictionary("TWMChannelOptionFriendlyName", "General Chat Channel", "TWMChannelOptionType", 0);

                        channels.CreateChannel(options, (creationResult, channel) =>
                            {
                                if (creationResult.IsSuccessful())
                                {
                                    generalChannel = channel;
                                    generalChannel.Join(r =>
                                        {
                                            generalChannel.SetUniqueName("general", res =>
                                                {
                                                });
                                        });
                                }
                            });
                    }

                });
        }

        [Foundation.Export("accessManagerTokenExpired:")]
        public void TokenExpired(Twilio.Common.TwilioAccessManager accessManager)
        {
            Console.WriteLine("token expired");
        }

        [Foundation.Export("accessManager:error:")]
        public void Error(Twilio.Common.TwilioAccessManager accessManager, Foundation.NSError error)
        {
            Console.WriteLine("access manager error");
        }

        #region Keyboard Management

        private void KeyboardWillShow(NSNotification notification)
        {
            var keyboardHeight = ((NSValue)notification.UserInfo.ValueForKey(UIKeyboard.FrameBeginUserInfoKey)).RectangleFValue.Height;
            UIView.Animate(0.1, () =>
                {
                    this.messageTextFieldBottomConstraint.Constant = keyboardHeight + 8;
                    this.sendButtonBottomConstraint.Constant = keyboardHeight + 8;
                    this.View.LayoutIfNeeded();
                });
        }

        private void KeyboardDidShow(NSNotification notification)
        {
            this.ScrollToBottomMessage();
        }

        private void KeyboardWillHide(NSNotification notification)
        {
            UIView.Animate(0.1, () =>
                {
                    this.messageTextFieldBottomConstraint.Constant = 8;
                    this.sendButtonBottomConstraint.Constant = 8;
                });
        }

        #endregion

        public void ScrollToBottomMessage()
        {
            if (dataSource.Messages.Count == 0)
            {
                return;
            }

            var bottomIndexPath = NSIndexPath.FromRowSection(this.tableView.NumberOfRowsInSection(0) - 1, 0);
            this.tableView.ScrollToRow(bottomIndexPath, UITableViewScrollPosition.Bottom, true);
        }

        async Task<string> GetToken()
        {
            var deviceId = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            // If you are using PHP it will be $"https://YOUR_TOKEN_SERVER_URL/token.php?device={deviceId}"
            var tokenEndpoint = $"https://twilio-drmtmxamarin.rhcloud.com/token?device={deviceId}";

            var http = new HttpClient();
            var data = await http.GetStringAsync(tokenEndpoint);

            var json = JsonObject.Parse(data);
            // Set the identity for use later, this is our username
            identity = json["identity"]?.ToString()?.Trim('"');

            return json["token"]?.ToString()?.Trim('"');
        }

        partial void SendButton_TouchUpInside(UIButton sender)
        {
            var msg = generalChannel.Messages.CreateMessage(messageTextField.Text);
            sendButton.Enabled = false;
            generalChannel.Messages.SendMessage(msg, r =>
                {

                    BeginInvokeOnMainThread(() =>
                        {
                            messageTextField.Text = string.Empty;
                            sendButton.Enabled = true;
                        });

                });
        }

        [Foundation.Export("ipMessagingClient:channel:messageAdded:")]
        public void MessageAdded(TwilioIPMessagingClient client, Channel channel, Message message)
        {
            dataSource.AddMessage(message);
            tableView.ReloadData();
            if (dataSource.Messages.Count > 0)
            {
                ScrollToBottomMessage();
            }
        }
    }
}
