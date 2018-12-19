using REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace REST_API.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Seen()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public Test Get(int id)
        {
            return new Test() { Id = 1, Name = "aaa" } ;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
