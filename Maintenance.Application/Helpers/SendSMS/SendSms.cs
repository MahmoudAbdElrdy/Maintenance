using Newtonsoft.Json;
using System.Text;
using Unifonic;

namespace Maintenance.Application.Helpers.SendSms
{
    public static class SendSMS
    {
        public static int SendMessageUnifonic(string message, string toNumber)
        {
            try
            {
                var AppSid = "Yqb3hxK3pPgJnorip1ZFc6e8mDbeKV";
                var SenderID = "Camelclub";
                var Body = message;
                var Recipient = toNumber;

                var url1 = "https://api.unifonic.com/rest/Messages/Send";
                using (var client = new HttpClient())
                {
                    var sbsRequest = new
                    {
                        AppSid = AppSid,
                        Recipient = Recipient,
                        Body = Body,
                        SenderID = SenderID,
                    };
                    //var apiResult = jss.Deserialize<dynamic>(jsonMessage);

                    //var jsonRequest = jss.SerializeObject(sbsRequest);
                    var jsonRequest = JsonConvert.SerializeObject(sbsRequest);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var httpResponse = client.PostAsync(url1, content);


                    if (httpResponse.Result.IsSuccessStatusCode)
                    {
                        using (var requestResponse = httpResponse.Result.Content)
                        {
                            var responseStr = requestResponse.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(responseStr))
                            {
                                //string s = responseStr.Replace(@"\", string.Empty);
                                //string final = s.Trim().Substring(1, (s.Length) - 2);
                                var sendingResult = JsonConvert.DeserializeObject<UnifonicResult>(responseStr);

                                if (sendingResult.data.Status == "Sent" || sendingResult.data.Status == "Queued" || sendingResult.data.Status == "Scheduled")
                                {
                                    return 0;
                                }
                                else
                                {
                                    return 10;
                                }
                            }
                        }
                    }
                }

                return 10;
            }
            catch (Exception ex)
            {

                var Message = ex.Message;
                return -1;
            }
        }
        //public static async Task<int> SendMessageUnifonic(string message, string toNumber)
        //{
        //    try
        //    {
        //        return 1234;
        //        var urc = new UnifonicRestClient("cUbBK3Be7cZkjIC5NSkvtfAsjP0CTq");

        //        var sendSmsMessageResult = urc.SendSmsMessage(toNumber, message);

        //        //var sendVerificationCode = urc.SendVerificationCode(toNumber, message, securityType: VerificationSecurityType.OTP);
        //        //var status = sendVerificationCode.Status;

        //        var status = (SmsMessageStatus)sendSmsMessageResult.Status.Value;

        //        if (status == SmsMessageStatus.Sent || status == SmsMessageStatus.Queued || status == SmsMessageStatus.Scheduled)
        //        {
        //            return (int)status;
        //        }
        //        else
        //        {
        //            return (int)status;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var Message = " : " + ex.Message;
        //        return 1;
        //    }
        //}
        public static string GenerateCode()
        {
            var characters = "0123456789";
            var charsArr = new char[4];
            var random = new Random();
            for (int i = 0; i < charsArr.Length; i++)
            {
                charsArr[i] = characters[random.Next(characters.Length)];
            }
            var segmentString = new String(charsArr);
            return segmentString;
        }
     
    }
    public class SMSService
    {

        public async Task<int> SendMessageUnifonic(string message, string toNumber)
        {
            try
            {
                var AppSid = "kBYwlwBLTnQDMx1DjJFOk40Y8uvjMg";
                var SenderID = "Camelclub";
                var Body = message;
                var Recipient = toNumber;

                var url1 = $"https://api.goinfinito.me/unified/v2/send?clientid=camelclube8p4oqiimngqr0n&clientpassword=b0ob1q431yb3k7u42i1wsssyv8yb7z53&to={Recipient}&from=CamelClub&text={message}";

                using var client = new HttpClient();
                var content = await client.GetAsync(url1);

                if (content.IsSuccessStatusCode)
                {
                    return 1;
                }
                return 10;
            }
            catch (Exception ex)
            {

                var Message = ex.Message;
                return -1;
            }
        }
    }
    public class UnifonicResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public UnifonicResultData data { get; set; }
    }
    public class UnifonicResultData
    {
        public long MessageID { get; set; }
        public string CorrelationID { get; set; }
        public string Status { get; set; }
        public int NumberOfUnits { get; set; }
        public int Cost { get; set; }
        public int Balance { get; set; }
        public string Recipient { get; set; }
        public DateTime TimeCreated { get; set; }
        public string CurrencyCode { get; set; }
    }
}