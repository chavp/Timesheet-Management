using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Support.Web.Models.Hubs
{
    using Microsoft.AspNet.SignalR;

    public class LogHub : Hub
    {
        public void Send(string id, string level, DateTime eventDate, int totalRecords)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.update(id, level, eventDate, totalRecords);
        }
    }
}