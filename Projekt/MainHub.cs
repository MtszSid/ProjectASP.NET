using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Projekt
{
    public class MainHub : Hub
    {
        static Dictionary<string, GroupInfo> _dictionary = new Dictionary<string, GroupInfo>();
        public override Task OnConnected()
        {
            
            return base.OnConnected();
        }
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
        public void Hello()
        {
            Clients.All.hello();
        }
        public void JoinARoom(string RoomName)
        {
            
        }
    }

    public class GroupInfo
    {

    }
}