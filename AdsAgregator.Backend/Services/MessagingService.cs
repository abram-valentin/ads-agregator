using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdsAgregator.Backend.Services
{
    public static class MessagingService
    {
        private const string BaseFcmUrl = "https://fcm.googleapis.com/fcm/send";
        private const string _firebaseServerApiKey = "AAAAtc0y0OE:APA91bFyopTw_73HY9KatdjbGV_cYLWGtmLakga-yHeyO7kV6XPXLinpPGZ1gqQTx0MQc90O4QTkjPspm_5CcNSf0exSmUtNsnOTdUTLCAGTOP7RPkM9WXcBBFGgGr-_rEFK46I9vcUi";


        public static async Task SendPushNotificationWithData(string title, string body,  object data, string receiver)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", _firebaseServerApiKey));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "notificationtest-d1d18"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = receiver,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = body,
                    title = title,
                    badge = 1,
                    sound = "default"
                },
                data = new
                {
                    key1 = "value1",
                    key2 = "value2"
                }

            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
        }

      
    }
}
