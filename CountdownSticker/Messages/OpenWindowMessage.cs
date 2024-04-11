using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.Messages
{
    public class OpenWindowMessage : ValueChangedMessage<Type>
    {
        public OpenWindowMessage(Type value) : base(value)
        {
        }
    }
}
