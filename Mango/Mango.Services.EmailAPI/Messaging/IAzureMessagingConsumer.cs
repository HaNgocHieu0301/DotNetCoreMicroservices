namespace Mango.Services.EmailAPI.Messaging
{
    public interface IAzureMessagingConsumer
    {
        Task Start();
        Task Stop();
    }
}
