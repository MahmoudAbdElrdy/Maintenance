using Unifonic;

namespace Maintenance.Application.Helpers.SendSms
{
    public static class SendSMS
    {
        public static async Task<int> SendMessageUnifonic(string message, string toNumber)
        {
            try
            {

                var urc = new UnifonicRestClient("YMRzS2kHKsV620TJckD3uKNT2llkPZ");

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
                return -1;
            }
        }

    }
}