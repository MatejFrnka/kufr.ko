using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Group
{
    public class GroupDetailInfo
    {
        public uint Id { get; set; }
        public string GroupName { get; set; }
        public bool HistoryVisibility { get; set; }
        public uint Id_Attachment { get; set; }
        public DateTime? IgnoreExpire { get; set; }
        public bool ReadOnly { get; set; }

        public string DisplayName { get; set; }

        public List<Group_UserInfo> Users { get; set; }
    }
}