using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class Attachment
    {
        public uint Id { get; set; }
        public string Filename { get; set; }
        public string Mime { get; set; }
        public string Hash { get; set; }
    }
}