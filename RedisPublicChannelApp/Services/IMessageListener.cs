namespace RedisPublicChannelApp.Services
{
    public interface IMessageListener
    {
        public Task SubscribeChannel(string channel);
    }
}
