using REST_API.Models.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace REST_API.Controllers
{
    public class FileController : ApiController
    {
        private readonly string path = @"D:\kufrko\attachdb\";
        public static List<UserPublic> LoadAttachements (List<UserPublic> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                string picture = LoadFile(users[i].Attachement);
                users[i].Attachement = picture;
            }
            return users;
        }
        public static string LoadFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
    }
}
