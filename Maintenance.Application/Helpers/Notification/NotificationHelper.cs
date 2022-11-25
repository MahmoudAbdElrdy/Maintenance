using Maintenance.Domain.Entities.Auth;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using FCMNet = FCM.Net;
namespace Maintenance.Application.Helpers.Notifications
{
    public class NotificationHelper
    {
        //AIzaSyAaB1yhtBsx6HqHxlsaNqbbBM3RqrqGVG8       241539094238
        //BAjcGgKwhivRrPBavpgF7xeDGCcmYRJ_UnWGVGsWFwaCB18rBHgTDVL_NBx1ZLJJHniJA1O8H3tRvNcdfboCNWM
        //private static string FirebaseApplicationID = "AAAALeyliNM:APA91bH0CTQajX1fVlbSd0HN-4Hf4VyiQEzwgj3C8Tzk1Gc0FAfxmi3AE-uFgDmg8u_53kaRenxWhCeE4okyjzsRLkmxexhcanxmbgz81s1sDDSUL_z7_1r3IRDCNiyVAh27nGPeZfwK";
        //private static string FirebaseSenderId = "197243799763";
        private static string FirebaseApplicationID = "AAAAdqL_0Ts:APA91bFuHEWOT3lzKiSdYVZSyLeQOZgqWf__mGKjubOnFw0Z-c2pUbt2q2lkjtYUQbPfIBkWOzDNJVKoDQSsPmtHvgIjXCjFYALEMSH4pEGwTtmptcwrvN581Wcly5lQwGACVraE1SQq";
        private static string FirebaseSenderId = "509540815163";
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
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Console.WriteLine(text);
                    }
                }
                return "Error";
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
        public static async Task FCMNotify(NotificationDto notification, string userFcmToken)
        {
            try
            {
                var serverKey = "AAAAdqL_0Ts:APA91bFuHEWOT3lzKiSdYVZSyLeQOZgqWf__mGKjubOnFw0Z-c2pUbt2q2lkjtYUQbPfIBkWOzDNJVKoDQSsPmtHvgIjXCjFYALEMSH4pEGwTtmptcwrvN581Wcly5lQwGACVraE1SQq";

                using (var sender = new FCMNet.Sender(serverKey))
                {
                    FCMNet.Message msg;
                    msg = new FCMNet.Message
                    {
                        RegistrationIds = new List<string> { userFcmToken },

                        Data = new FCMNet.Notification
                        {
                            Title = notification.Title,
                            Body = notification.Body,
                            Sound = "sound.caf",

                        },
                        Priority = FCMNet.Priority.High,
                        TimeToLive = 180,
                        ContentAvailable = true
                    };

                    var data = await sender.SendAsync(msg);
                }
            }
            catch (Exception e)
            {
                ;
            }
         
        }
       }
    public class NotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}