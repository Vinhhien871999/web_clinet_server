using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CNWeb.Controllers
{
    public class TestApiController : ApiController
    {
        DBContext myModel = new DBContext();
        // GET: api/TestApi
        public IEnumerable<Product> Get()
        {
            return myModel.Products.ToList();
        }

        // GET: api/TestApi/5
        public string Get(int id)
        {
            
            return "value";
        }

        // POST: api/TestApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TestApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TestApi/5
        public void Delete(int id)
        {
        }
    }
}
