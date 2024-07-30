using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Swagger2Doc.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 取代所有　空白
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAllWhitespaces(this string str)
        {
            return str.ConvertWhitespacesToSingleSpaces().Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string ToHalfWidthString(this string str)
        {
            str = str.TrimAllWhitespaces();
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] > 65280 && chars[i] < 65375)
                {
                    chars[i] = (char)(chars[i] - 65248);
                }
            }
            return new string(chars);
        }

        public static T? GetObject<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static bool IsValidJson<T>(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) return false;

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<T>(strInput);
                    return true;
                }
                catch // not valid
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidJson(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                return false;
            }
        }

        public static bool IsValidEmailFormat(this string value)
        {
            try
            {
                MailAddress m = new MailAddress(value);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static IPAddress? GetClientIpAddress(this string clientIp)
        {
            if (String.IsNullOrWhiteSpace(clientIp))
            {
                return null;
            }
            IPAddress clientIPAddress;
            try
            {
                clientIPAddress = IPAddress.Parse(clientIp);
            }
            catch (Exception ex)
            {
                return null;
            }
            return clientIPAddress;
        }
    }
}
