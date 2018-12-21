using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class Group
    {
        public uint Id { get; set; }
        public string GroupName { get; set; }
        public bool HistoryVisibility { get; set; }
        public uint Id_Attachment { get; set; }
        
    }
}