using Newtonsoft.Json;
using System.Reflection;
using System.Web;

namespace Swagger2Doc.Helpers
{
    public class CommonHelper
    {
        public static string EncodeBase64(string input)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeBase64(string input)
        {
            var base64EncodeBytes = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(base64EncodeBytes);
        }

        public static string DecodeBase64Url(string input)
        {
            return DecodeBase64(HttpUtility.UrlDecode(input));
        }

        public static string? SetObject(object? data)
        {
            return data == null ? null : JsonConvert.SerializeObject(data);
        }

        public static T? GetObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

    }
}
