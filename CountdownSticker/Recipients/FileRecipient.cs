using CommunityToolkit.Mvvm.Messaging;
using CountdownSticker.Messages;
using System.Diagnostics;

namespace CountdownSticker.Recipients
{
    class FileRecipient : IRecipient<CountdownsRequestMessage>
    {
        public FileRecipient()
        {
            
        }
        
        public void Receive(CountdownsRequestMessage message)
        {
            Debug.WriteLine($"FileRecipient 收到了一条消息来自{message.Count()}");
        }
    }
}
