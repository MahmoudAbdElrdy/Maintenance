using Unifonic;

namespace Maintenance.Application.Helpers.SendSms
{
    public static class SendSMS
    {
        public static async Task<int> SendMessageUnifonic(string message, string toNumber)
        {
            try
            {
                return 1234;
                var urc = new UnifonicRestClient("cUbBK3Be7cZkjIC5NSkvtfAsjP0CTq");

                var sendSmsMessageResult = urc.SendSmsMessage(toNumber, message);

                //var sendVerificationCode = urc.SendVerificationCode(toNumber, message, securityType: VerificationSecurityType.OTP);
                //var status = sendVerificationCode.Status;

                var status = (SmsMessageStatus)sendSmsMessageResult.Status.Value;

                if (status == SmsMessageStatus.Sent || status == SmsMessageStatus.Queued || status == SmsMessageStatus.Scheduled)
                {
                    return (int)status;
                }
                else
                {
                    return (int)status;
                }
            }
            catch (Exception ex)
            {
                var Message = " : " + ex.Message;
                return 1;
            }
        }
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
}