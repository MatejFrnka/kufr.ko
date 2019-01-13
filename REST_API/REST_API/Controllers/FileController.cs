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
        private const string path = @"\attachdb\";

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
        private static string LoadFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
        [HttpGet]
        public Response LoadAttachment(uint IdAttachment)
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
                        response.Data = LoadFile(attachmentFullPath);
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
            }
        }

        //[HttpGet]
        //public Response LoadAttachmentInfo(uint IdAttachment)
        //{
        //    Response response = new Response();
        //    AttachmentMessage attachment = attachmentRepository.FindByIdSecure(IdAttachment, userId);
        //    if (attachment == null)
        //    {
        //        response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
        //    }
        //    else
        //    {
        //        response.Data = attachment;
        //        response.StatusCode = Models.Enums.StatusCode.OK;
        //    }
        //    return response;
        //}

        [HttpPost]
        
        public Response SaveAttachment(string attachment)
        {
            Response response = new Response();
            try
            {
                byte[] bytes = Convert.FromBase64String(attachment);
                var md5 = HashAlgorithm.Create();
                string hash = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
                uint? id = attachmentRepository.FindIdByHash(hash);
                if (id!=null)
                {
                    response.Data = id;
                }
                else
                {
                    id = attachmentRepository.CreateAttachment(hash);
                    response.Data = id;
                    File.WriteAllBytes(path+id.ToString(), bytes);
                }
                
                response.StatusCode = Models.Enums.StatusCode.OK;
            }
            catch (Exception)
            {
                response.StatusCode = Models.Enums.StatusCode.DATABASE_ERROR;
            }
            return response;
        }

        [HttpGet]



        /// <summary>
        /// Checks if attachment already exists in the database
        /// </summary>
        /// <param name="hash">MD5 Hash of the attachment</param>
        /// <returns>Response with ID of the attachment if attachment exists, otherwise null</returns>
        public Response AttachmentExists(string hash)
        {
            Response response = new Response();
            try
            {
                uint? id = attachmentRepository.FindIdByHash(hash);
                if (id != null)
                {
                    response.Data = id;
                }
                else
                {
                    response.Data = null;
                }

                response.StatusCode = Models.Enums.StatusCode.OK;
            }
            catch (Exception)
            {
                response.StatusCode = Models.Enums.StatusCode.DATABASE_ERROR;
            }
            return response;
        }


        ///// <summary>
        ///// Returns MD5 hash for file specified in <paramref name="filename"/>
        ///// </summary>
        ///// <param name="filename">Path to file that will be loaded to calculate the hash</param>
        ///// <returns>The MD5 hash of the file represented in string</returns>
        //private string CalculateMD5(string filename)
        //{
        //    using (var md5 = MD5.Create())
        //    {
        //        using (var stream = File.OpenRead(filename))
        //        {
        //            var hash = md5.ComputeHash(stream);
        //            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        //        }
        //    }
        //}
        private static string GetMime(string filename)
        {
            if (filename.Contains('.'))
            {
                if (filename.EndsWith("jpg"))
                {
                    return "image/jpg";
                }
                else if (filename.EndsWith("jpeg"))
                {
                    return "image/jpeg";
                }
                else if (filename.EndsWith("png"))
                {
                    return "image/png";
                }
                else if (filename.EndsWith("gif"))
                {
                    return "image/gif";
                }
                else if (filename.EndsWith("bmp"))
                {
                    return "image/bmp";
                }
                else if (filename.EndsWith("avi"))
                {
                    return "video/x-msvideo";
                }
                else if (filename.EndsWith("mp3"))
                {
                    return "audio/mpeg";
                }
                else if (filename.EndsWith("txt"))
                {
                    return "text/plain";
                }
                else
                {
                    return "application/unknown";
                }
            }
            else
            {
                return "application/octet-stream";
            }
        }
    }
}
