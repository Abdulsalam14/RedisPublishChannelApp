using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RedisPublicChannelApp.Hubs;
using RedisPublicChannelApp.Models;
using RedisPublicChannelApp.Services;
using StackExchange.Redis;
using System.Diagnostics;
using System.Threading.Channels;

namespace RedisPublicChannelApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageListener _messageListener;
        public HomeController(IConnectionMultiplexer redis, IHubContext<ChatHub> hubContext, IMessageListener messageListener)
        {
            _redis=redis;
            _database=redis.GetDatabase();
            _hubContext=hubContext;
            _messageListener=messageListener;
        }
        public async Task<IActionResult> Index()
        {
            var channels = await _database.ListRangeAsync("channels", 0, -1);
            if (channels.Any())
            {
                var channelNames = channels.Select(i => $"{i}").ToList();
                ViewBag.ChannelNames = channelNames;
            }
            return View();
        }

        public async Task<IActionResult> MakeChannel(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                await _database.ListRightPushAsync("channels", channelName);
                await _hubContext.Clients.All.SendAsync("newchannel");
            }
            return Ok();
        }

        public async Task<IActionResult> GetChannels()
        {
            var channels = await _database.ListRangeAsync("channels", 0, -1);
            if (channels.Any())
            {
                var channelNames = channels.Select(i => $"{i}").ToList();
                return Ok(channelNames);
            }
            return BadRequest();
        }

        public async Task<IActionResult> GetChannelMessages(string channelName)
        {

            var messages = await _database.StreamRangeAsync(channelName);
            List<object> data = new List<object>();
            if (messages.Any())
            {
                foreach (var entry in messages)
                {
                    var message = entry.Values.FirstOrDefault(x => x.Name == "message").Value.ToString();
                    var timeStamp = entry.Values.FirstOrDefault(x => x.Name == "timeStamp").Value.ToString();

                    var messageData = new
                    {
                        Message = message,
                        TimeStamp = timeStamp
                    };

                    data.Add(messageData);
                }
                return Ok(data);
            }
            return NoContent();

        }

        public async Task<IActionResult> Subscribe(string channelName)
        {
            try
            {
                await _messageListener.SubscribeChannel(channelName);
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        public async Task<IActionResult> AddMessage(string channelName, string message)
        {
            try
            {
                var subscriber = _redis.GetSubscriber();
                if (subscriber != null)
                {
                    await subscriber.PublishAsync(channelName, message);
                }
                _database.StreamAdd(channelName, new[] { new NameValueEntry("message", message), new NameValueEntry("timeStamp", DateTime.UtcNow.ToLongTimeString()) });
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

    }

}
