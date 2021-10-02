using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Reacto.Web.Hubs
{
    public class StageHub : Hub
    {
        public async Task Send(string name, string message)
        {
            var messageId = Guid.NewGuid();
            // Call the broadcastMessage method to update clients.
            await Clients.All.SendAsync("broadcastMessage", messageId, name, message);
        }
    }
}
