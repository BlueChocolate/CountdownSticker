using CommunityToolkit.Mvvm.Messaging.Messages;
using CountdownSticker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.Messages
{
    public class StickersChangedMessage : ValueChangedMessage<IEnumerable<Countdown>>
    {
        public StickersChangedMessage(IEnumerable<Countdown> value) : base(value)
        {
        }
    }
}
