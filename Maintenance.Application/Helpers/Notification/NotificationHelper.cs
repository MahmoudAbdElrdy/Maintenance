using Maintenance.Domain.Entities.Auth;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using FCMNet = FCM.Net;
namespace Maintenance.Application.Helpers.Notifications
{
    public class NotificationHelper
    {
        //private static string FirebaseApplicationID = "AAAALeyliNM:APA91bH0CTQajX1fVlbSd0HN-4Hf4VyiQEzwgj3C8Tzk1Gc0FAfxmi3AE-uFgDmg8u_53kaRenxWhCeE4okyjzsRLkmxexhcanxmbgz81s1sDDSUL_z7_1r3IRDCNiyVAh27nGPeZfwK";
        //private static string FirebaseSenderId = "197243799763";
        private static string FirebaseApplicationID = "AAAAsLU-Bsc:APA91bEjQCvUtuI2hrfUIpDq8w7icKxygNYhfTeeYv1tMOttQgMZniTpGFMaUb_9SqppLvAnTijiwUUeUOb9xR1jfj99i9njjDIeyr_STDteyTJckf_GK4S9FZzJppcsTVp18IaFfVPq";
        private static string FirebaseSenderId = "758954985159";
        public static string PushNotificationByFirebase(string txtmsg, string txttitle, int badgeCounter, string deviceId, string iamgeURL = null)
        {
            try
            {
                //Create the web request with fire base API  
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", FirebaseApplicationID));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", FirebaseSenderId));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = deviceId,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = txtmsg,
                        title = txttitle,
                        sound = "sound.caf",
                        badge = badgeCounter,
                        image = iamgeURL
                    },
                };

                // BinaryFormatter bf = new BinaryFormatter();
                //MemoryStream ms = new MemoryStream();
                //  bf.Serialize(ms, payload);

                //var test = Convert.ToByte(payload);
               // var ser = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(payload);
               // var json = ser.Serialize(payload);
                //JsonConvert.SerializeObject(serializedData);

                byte[] originalArray = Encoding.UTF8.GetBytes(json);

                //var serializer = new JavaScriptSerializer();
                // Byte[] byteArray = ObjectToByteArray(payload);// ms.ToArray();
                tRequest.ContentLength = originalArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(originalArray, 0, originalArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    return sResponseFromServer;
                                }
                            return "fail";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
        public static async Task FCMNotify(Notification notification, string userFcmToken)
        {
            var serverKey = "AAAAAzeHsvc:APA91bGzQXZ2pWbom40rIb0CLcWI4CPmduQAz0GA3q1-LzQiuzQqnLi0VaUYHBPuKCrYxBupkIixhrxDGgqGCJhOsG7N4v2TLqtmQa0v-mpKDdf1_gTXYFjZPmFjzVtz7cF-IDpTQOEp";
            
            using (var sender = new FCMNet.Sender(serverKey))
            {
                FCMNet.Message msg;
                msg = new FCMNet.Message
                {
                    RegistrationIds = new List<string> { userFcmToken },

                    Notification = new FCMNet.Notification
                    {
                        Title = notification.SubjectAr,
                        Body = notification.BodyAr,
                        Sound = "sound.caf"
                    },
                    Priority = FCMNet.Priority.High,
                };

                var data = await sender.SendAsync(msg);
            }
        }
        //public static void PushNotificationByFirebase(string englishMessage, string title, List<string> player_Id, Dictionary<string, object> AdditionalData, int second = 0)
        //{
        //    try
        //    {
        //        if (AdditionalData == null)
        //        {
        //            AdditionalData = new Dictionary<string, object>()
        //            {
        //                { "message" , englishMessage },
        //                { "other_key" , true },
        //                { "title" , title },
        //                { "body", englishMessage },
        //                { "badge" , 1 },
        //                { "sound" ,"default" },
        //                { "content_available" , true },
        //                { "timestamp" , DateTime.UtcNow.AddHours(2).ToString() }
        //            };
        //        }
        //        else
        //        {
        //            AdditionalData.Add("message", englishMessage);
        //            AdditionalData.Add("other_key", true);
        //            AdditionalData.Add("title", title);
        //            AdditionalData.Add("body", englishMessage);
        //            AdditionalData.Add("badge", 1);
        //            AdditionalData.Add("sound", "default");
        //            AdditionalData.Add("content_available", true);
        //            AdditionalData.Add("timestamp", DateTime.UtcNow.AddHours(2).ToString());
        //        }
        //        player_Id = player_Id.Where(pId => pId != null && pId.Length > 9).ToList();
        //        foreach (var deviceId in player_Id)
        //        {
        //            try
        //            {
        //                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //                tRequest.Method = "post";
        //                tRequest.ContentType = "application/json";
        //                AdditionalData.Add("userToken", deviceId);
        //                var data = new
        //                {
        //                    to = deviceId,
        //                    priority = "high",
        //                    content_available = true,
        //                    notification = new
        //                    {
        //                        body = englishMessage,
        //                        title = title,
        //                        badge = 1,
        //                        sound = "default",
        //                        content_available = true
        //                    },
        //                    data = AdditionalData,
        //                    apns = new
        //                    {
        //                        payload = new
        //                        {
        //                            aps = new
        //                            {
        //                                sound = "default",
        //                                content_available = true,
        //                                body = englishMessage,
        //                                message = englishMessage,
        //                                title = title,
        //                                badge = 1,
        //                            },
        //                        },
        //                        customKey = "test app",
        //                    }

        //                };
        //                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //                tRequest.Headers.Add(string.Format("Authorization: key={0}", FirebaseApplicationID));
        //                tRequest.Headers.Add(string.Format("Sender: id={0}", FirebaseSenderId));
        //                tRequest.ContentLength = byteArray.Length;
        //                using (Stream dataStream = tRequest.GetRequestStream())
        //                {
        //                    dataStream.Write(byteArray, 0, byteArray.Length);
        //                    using (WebResponse tResponse = tRequest.GetResponse())
        //                    {
        //                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //                        {
        //                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                            {
        //                                String sResponseFromServer = tReader.ReadToEnd();
        //                                string str = sResponseFromServer;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                string Error = string.Format("{0} - {1} ", ex.Message, ex.InnerException != null ? ex.InnerException.FullMessage() : "");
        //                //System.Diagnostics.Debug.WriteLine(Error);
        //                string tokens = "tokens is : (" + deviceId + ")";
        //                System.Diagnostics.Debug.WriteLine(string.Format("{0} :::: {1}", Error, tokens), DateTime.Now);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string Error = string.Format("{0} - {1} ", ex.Message, ex.InnerException != null ? ex.InnerException.FullMessage() : "");
        //        //System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1} ", ex.Message, ex.InnerException != null ? ex.InnerException.FullMessage() : ""));
        //        string tokens = "tokens is : (";
        //        foreach (var item in player_Id)
        //        {
        //            tokens += "{" + item + "}   ";
        //        }
        //        tokens += "  )";
        //        System.Diagnostics.Debug.WriteLine(string.Format("{0} :::: {1}", Error, tokens), DateTime.Now);
        //    }
        //}
    }
}