using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tridion.ContentManager;

namespace SDLWebBot.Controllers
{
    public class BotController : ApiController
    {
        public class PublicationModel {
            public string TCMUri { get; set; }
            public string URL { get; set; }
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            string TCMUri = "sfdf";
            if (TCMUri != null)
                using (Session session = new Session())
                {
                    bool isExist = session.IsExistingObject(TCMUri);
                }


            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post(PublicationModel model)
        {
            WebApiApplication.PulicationFinished(model.TCMUri, model.URL);
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
