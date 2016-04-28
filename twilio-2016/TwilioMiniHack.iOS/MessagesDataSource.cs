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
    public class MessagesDataSource : UITableViewSource
    {
        public List<Message> Messages { get; private set; } = new List<Message>();

        public void AddMessage(Message msg)
        {
            Messages.Add(msg);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return Messages.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var message = Messages[indexPath.Row];
            var cell = tableView.DequeueReusableCell("MessageCell") as MessageCell;
            cell.Message = message;
            cell.SetNeedsUpdateConstraints();
            cell.UpdateConstraintsIfNeeded();

            return cell;
        }
    }
}
