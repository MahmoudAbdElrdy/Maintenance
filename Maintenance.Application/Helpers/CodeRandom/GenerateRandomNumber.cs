using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Helpers.CodeRandom
{
    public static class GenerateRandomNumber
    {

        public const string LowerCaseAlphabet = "abcdefghijklmnopqrstuvwyxz";
        public const string UpperCaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GetSerial(long Id)
        {
            string DefaultSerial = "00001";
            string Serial = "";
            Int64 NextSerial = Id;
            if (Id != 0)
                Serial = (NextSerial + 1).ToString();
            else
                Serial = DefaultSerial;
            if (Serial.Length > 0 && Serial.Length <= DefaultSerial.Length)
            {
                DefaultSerial = DefaultSerial.Remove(DefaultSerial.Length - Serial.Length);
                Serial = DefaultSerial + Serial;
            }
            return Serial;
        }
        public static string GenerateCustomRandomString(string prefix = "", int size = 4, bool lowerCase = false, bool isWithTime = true, bool isReadable = false)
        {
            DateTime ksaDt = DateTime.UtcNow.AddHours(3);
            var day = ksaDt.Day.ToString("D2");
            var month = ksaDt.Month.ToString("D2");
            var year = ksaDt.Year.ToString().Substring(2, 2);
            var time = isWithTime ? ksaDt.Hour.ToString("D2") + ksaDt.Minute.ToString("D2") + ksaDt.Second.ToString("D2") : string.Empty;
            var code = prefix + year + month + day + time;
            if (isReadable)
                return code + "-" + GenerateString(size, lowerCase) + RandomNumber();
            return code + GenerateString(size, lowerCase);
        }

        public static string GenerateString(int size, bool lowerCase)
        {
            string alphabet = lowerCase ? LowerCaseAlphabet : UpperCaseAlphabet;
            var random = new Random();
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = alphabet[random.Next(alphabet.Length)];
            }
            return new string(chars);
        }


        private static string RandomNumber(int size = 3)
        {
            Random rand = new Random();
            int value = rand.Next(1000);
            return value.ToString($"D{size}");
        }

    }
}
