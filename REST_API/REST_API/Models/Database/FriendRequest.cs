using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class FriendRequest
    {
        public uint Id_UserSender { get; set; }
        public uint Id_UserReceiver { get; set; }
        public FriendRequestState State { get; set; }
    }
}