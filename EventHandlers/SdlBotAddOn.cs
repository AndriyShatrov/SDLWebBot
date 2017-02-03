using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Extensibility;
using Tridion.ContentManager.Extensibility.Events;
using Tridion.ContentManager.Publishing;


namespace TcmEventHandlers
{
    [TcmExtension("SdlBotAddOn")]
    public class SdlBotAddOn : TcmExtension
    {
        static string _url = @"http://ua1dt160pl82.global.sdl.corp:21500/Bot";

        public SdlBotAddOn()
        {
            Subscribe();
        }

        public void Subscribe()
        {
            EventSystem.Subscribe<PublishTransaction, SaveEventArgs>(SendPublishInfo, EventPhases.TransactionCommitted);
        }

        public void SendPublishInfo(PublishTransaction publishTransaction, SaveEventArgs args, EventPhases phase)
        {
            //Get Tcm Uri of publisher user and published Url of page and send info to WebService
            if (publishTransaction.State == PublishTransactionState.Success)
            {
                var page = publishTransaction.Items.First() as Page;

                var publishedInfo = new
                {
                    TCMUri = publishTransaction.Creator.Id,
                    URL = page.GetPublishUrl(publishTransaction.TargetType).ToString()
                };
                
                string json = JsonConvert.SerializeObject(publishedInfo, Formatting.Indented);
                SendJson(json);
            }
        }
        
        private static void SendJson(string json)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(_url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
