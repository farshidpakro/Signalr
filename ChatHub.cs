using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace signalr
{
    public class ChatHub : Hub
    {
        private static List<ChatUser> chatUsers = new List<ChatUser>();

        public async Task Connect(string username)
        {
            var chatUser = new ChatUser
            {
                ConnectionId = Context.ConnectionId,
                Username = username
            };
            chatUsers.Add(chatUser);

            await Clients.Caller.SendAsync("onConnected", chatUser.ConnectionId, chatUser.Username, chatUsers.Select(u => u.Username).ToList());
            await Clients.AllExcept(chatUser.ConnectionId).SendAsync("onNewUserConnected", chatUser.ConnectionId, chatUser.Username);
        }

        public async Task SendMessage(string toUser, string message)
        {
            var fromUser = chatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var toUserConnectionId = chatUsers.FirstOrDefault(u => u.Username == toUser)?.ConnectionId;

            if (toUserConnectionId != null)
            {
                await Clients.Client(toUserConnectionId).SendAsync("receivePrivateMessage", fromUser.Username, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var chatUser = chatUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (chatUser != null)
            {
                chatUsers.Remove(chatUser);
                await Clients.All.SendAsync("onUserDisconnected", chatUser.ConnectionId, chatUser.Username);
            }

            await base.OnDisconnectedAsync(exception);
        }


    }
}