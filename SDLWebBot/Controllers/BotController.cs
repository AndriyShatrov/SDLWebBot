using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDLWebBot.Controllers
{
    public class BotController : ApiController
    {

        public class PublicationModel
        {
            public string TCMUri { get; set; }
            public string URL { get; set; }
            public string TYPE { get; set; }
            public string childTCMUri { get; set; }
            public string childTitle { get; set; }
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
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
            switch (model.TYPE)
            {
                case "PagePublished":
                    WebApiApplication.PulicationFinished(model.TCMUri, model.URL);
                    break;
                case "ComponentUpdated":
                    WebApiApplication.ComponentUpdated(model.TCMUri, model.URL);
                    break;
                case "ComponentNeedTranslation":
                    WebApiApplication.ComponentNeedTranslation(model.TCMUri, model.URL, model.childTCMUri, model.childTitle);
                    break;
            }
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
