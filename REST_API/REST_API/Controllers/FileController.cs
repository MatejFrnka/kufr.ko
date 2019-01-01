using REST_API.Authentication;
using REST_API.Models.Api;
using REST_API.Models.Api.Attachments;
using REST_API.Models.Database;
using REST_API.Repositories;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

namespace REST_API.Controllers
{
    [UserAuth]
    public class FileController : ApiController
    {
        private DbManager dbManager;
        private UserRepository userRepository;
        private AttachmentRepository attachmentRepository;
        private uint userId;
        private const string path = @"D:\kufrko\attachdb\";

        public FileController()
        {
            this.dbManager = new DbManager();
            this.userRepository = new UserRepository(this.dbManager);
            this.attachmentRepository = new AttachmentRepository(this.dbManager);
            userId = ((UserPrincipal)User).DbUser.Id;
        }


        //public static List<UserPublic> LoadProfilePictures (List<UserPublic> users)
        //{
        //    for (int i = 0; i < users.Count; i++)
        //    {
        //        string picture = LoadFile(users[i].Attachement);
        //        users[i].Attachement = picture;
        //    }
        //    return users;
        //}
        public static string LoadFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
        [HttpGet]
        public Response LoadAttachment(UInt32 IdAttachment)
        {
            try
            {
                Response response = new Response();
                Attachment attachment = attachmentRepository.FindByIdSecure(IdAttachment, userId);
                if (attachment == null)
                {
                    response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
                }
                else
                {
                    string attachmentFullPath = path + IdAttachment;
                    if (File.Exists(attachmentFullPath))
                    {
                        AttachmentData attachmentData = new AttachmentData();
                        attachmentData.Info = attachment;
                        attachmentData.Data = LoadFile(attachmentFullPath);
                        response.Data = attachmentData;
                        response.StatusCode = Models.Enums.StatusCode.OK;
                    }
                    else
                    {
                        response.StatusCode = Models.Enums.StatusCode.DATABASE_ERROR;
                    }

                    
                    
                }
                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        /// <summary>
        /// Returns MD5 hash for file specified in <paramref name="filename"/>
        /// </summary>
        /// <param name="filename">Path to file that will be loaded to calculate the hash</param>
        /// <returns>The MD5 hash of the file represented in string</returns>
        private string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
