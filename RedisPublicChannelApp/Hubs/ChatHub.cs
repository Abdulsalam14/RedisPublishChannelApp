using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace RedisPublicChannelApp.Hubs
{
    public class ChatHub:Hub
    {
        public override async Task OnConnectedAsync()
        {

            await Clients.Others.SendAsync("Connect");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("Disconnect");
        }

        //public async Task NewMessagePublished(string channelName)
        //{
        //    await Clients.All.SendAsync(channelName);
        //}
    }
}
