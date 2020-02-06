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


        public static async Task SendPushNotificationWithData(string title, string body, object data, string receiver, string _firebaseSeverApiKey)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", _firebaseSeverApiKey));
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
                    sound = "default",
                    payload = data
                },
                data = new
                {
                    notificationPayload = data
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
