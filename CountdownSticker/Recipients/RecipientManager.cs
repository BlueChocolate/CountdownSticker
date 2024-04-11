using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownSticker.Recipients
{
    public class RecipientManager
    {
        private readonly List<object> _recipients = new List<object>();

        public RecipientManager Register<TRecipient, TMessage>() where TRecipient : IRecipient<TMessage> where TMessage : class
        {
            var recipient = Activator.CreateInstance<TRecipient>();
            if (recipient is not null and IRecipient<TMessage>)
            {
                WeakReferenceMessenger.Default.Register((IRecipient<TMessage>)recipient);
                _recipients.Add(recipient);
            }
            return this;
        }
    }
}