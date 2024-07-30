using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swagger2Doc.Models.DTO;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Swagger2Doc.Libs
{
    public interface IRequest
    {
        public CancellationToken GetCancellationToken();

        public string AnonymizationLogBody(string input, string[]? noLogPatterns = null, string replaceStr = "***");

        public Task<T> GetJson<T>(string url, string[] noLogReqPatterns, string[] noLogResppatterns, CancellationToken? cancellationToken = null);

        public Task<T> GetJson<T>(string url, CancellationToken cancellationToken);

        public Task<T> GetJson<T>(string url);

        public Task<T> PostJson<T>(string url, string postBody, Dictionary<string, string> headers, string[] noLogReqPatterns, string[] noLogResppatterns, CancellationToken? cancellationToken = null);

        public Task<T> PostJson<T>(string url, string postBody, CancellationToken cancellationToken, Dictionary<string, string>? headers = null);

        public Task<T> PostJson<T>(string url, string postBody, Dictionary<string, string>? headers = null);

        public Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string> headers, string[] noLogReqPatterns, string[] noLogResppatterns, CancellationToken? cancellationToken = null);

        public Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, CancellationToken cancellationToken, Dictionary<string, string>? headers = null);

        public Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers = null);

        public Task<T> GetXml<T>(string url, string encoding, string[] noLogReqPatterns, string[] noLogResppatterns);

        public Task<T> GetXml<T>(string url, string encoding);

        public Task<byte[]?> GetImage(string url);
    }

    public class Request : IRequest
    {
        private readonly ILogger logger;

        private readonly int timeout;

        public CancellationToken GetCancellationToken()
        {
            return new CancellationTokenSource(timeout * 1000).Token;
        }



        private Guid ActionId;

        private readonly HttpClient client;

        public Request(ILogger<Request> logger, IConfiguration config, HttpClient client)
        {
            this.logger = logger;
            ConfigurationSection ApiSetting = (ConfigurationSection)config.GetSection("ApiSetting");
            this.timeout = ApiSetting.GetValue("RequestTimeoutSeconds", 120);
            string httpContextItemId = Guid.NewGuid().ToString();
            this.ActionId = new Guid(httpContextItemId);
            this.client = client;
        }

        public string AnonymizationLogBody(string input, string[]? noLogPatterns = null, string replaceStr = "***")
        {
            string output = Regex.Replace(input, @"\t|\n|\r", String.Empty);
            noLogPatterns = noLogPatterns ?? new string[] { };
            foreach (string regex in noLogPatterns)
            {
                output = Regex.Replace(output, regex, replaceStr);
            }
            return output;
        }

        public async Task<T> GetJson<T>(string url, string[] noLogReqPatterns, string[] noLogResppatterns, CancellationToken? cancellationToken = null)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetJson";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = this.AnonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            string result;

            Stopwatch sw = new Stopwatch();
            DateTime startTime = DateTime.Now;
            sw.Start();
            using HttpResponseMessage response = await this.client.GetAsync(url, cancellationToken ?? GetCancellationToken());
            logBody.ResponseStatusCode = (int)response.StatusCode;
            sw.Stop();

            var contentType = response.Content.Headers.ContentType;
            if (String.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response
            logBody.ResponseData = this.AnonymizationLogBody(result, noLogResppatterns);
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(string))
            {
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(HttpResponseMessage))
            {

                return (T)(object)response;
            }
            else if (typeof(T) == typeof(MyHttpResponseMessage))
            {
                MyHttpResponseMessage _response = new MyHttpResponseMessage();
                _response.ResponseRawData = response;
                _response.StartTime = startTime;
                _response.DurationMilliseconds = sw.ElapsedMilliseconds;
                _response.ResponseData = logBody.ResponseData;
                return (T)(object)_response;
            }
            else
            {
                try
                {
                    T resultObj = JsonConvert.DeserializeObject<T>(result);
                    if (resultObj is IStatusCodeResp)
                    {
                        ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                    }
                    return resultObj;
                }
                catch (Exception ex)
                {
                    throw new Exception($"result:{result};{JsonConvert.SerializeObject(ex)}");
                }
            }
        }

        public async Task<T> GetJson<T>(string url, CancellationToken cancellationToken)
        {
            return await GetJson<T>(url, new string[] { }, new string[] { }, cancellationToken);
        }

        public async Task<T> GetJson<T>(string url)
        {
            return await GetJson<T>(url, new string[] { }, new string[] { });
        }

        public async Task<T> PostJson<T>(
            string url,
            string postBody,
            Dictionary<string, string>? headers = null,
            string[]? noLogReqPatterns = null,
            string[]? noLogResppatterns = null,
            CancellationToken? cancellationToken = null)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "PostJson";
            logBody.Method = HttpMethod.Post.ToString();
            logBody.Url = url;
            logBody.RequestHeaders = headers;
            logBody.RequestModel = this.AnonymizationLogBody(postBody, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            string result;
            StringContent content = new StringContent(postBody, Encoding.UTF8, "application/json");
            #region Add Headers 
            if (headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    content.Headers.Add(header.Key, header.Value);
                }
            }
            #endregion

            Stopwatch sw = new Stopwatch();
            DateTime startTime = DateTime.Now;
            sw.Start();
            using HttpResponseMessage response = await this.client.PostAsync(url, content, cancellationToken ?? GetCancellationToken());
            sw.Stop();

            logBody.ResponseStatusCode = (int)response.StatusCode;
            logBody.DurationMilliseconds = sw.ElapsedMilliseconds;

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            if (String.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response
            logBody.ResponseData = this.AnonymizationLogBody(result, noLogResppatterns);
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(string))
            {
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(HttpResponseMessage))
            {

                return (T)(object)response;
            }
            else if (typeof(T) == typeof(MyHttpResponseMessage))
            {
                MyHttpResponseMessage _response = new MyHttpResponseMessage();
                _response.ResponseRawData = response;
                _response.StartTime = startTime;
                _response.DurationMilliseconds = sw.ElapsedMilliseconds;
                _response.ResponseData = logBody.ResponseData;
                return (T)(object)_response;
            }
            else
            {
                try
                {
                    T resultObj = JsonConvert.DeserializeObject<T>(result);
                    if (resultObj is IStatusCodeResp)
                    {
                        ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                    }
                    return resultObj;
                }
                catch (Exception ex)
                {
                    throw new Exception($"result:{result};{JsonConvert.SerializeObject(ex)}");
                }

            }
        }
        public async Task<T> PostJson<T>(string url, string postBody, CancellationToken cancellationToken, Dictionary<string, string>? headers = null)
        {
            return await PostJson<T>(url, postBody, headers, new string[] { }, new string[] { }, cancellationToken);
        }

        public async Task<T> PostJson<T>(string url, string postBody, Dictionary<string, string>? headers = null)
        {
            return await PostJson<T>(url, postBody, headers, new string[] { }, new string[] { });
        }

        public async Task<T> PostJson<T>(string url, string postBody)
        {
            return await PostJson<T>(url, postBody, null, new string[] { }, new string[] { });
        }

        public async Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string> headers, string[] noLogReqPatterns, string[] noLogResppatterns, CancellationToken? cancellationToken = null)
        {
            string result;
            #region Add Headers 
            if (headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    postBody.Headers.Add(header.Key, header.Value);
                }
            }
            #endregion
            string postBodyString = await postBody.ReadAsStringAsync();
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "PostJson";
            logBody.Method = HttpMethod.Post.ToString();
            logBody.Url = url;
            logBody.RequestHeaders = headers;
            logBody.RequestModel = this.AnonymizationLogBody(postBodyString, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            Stopwatch sw = new Stopwatch();
            DateTime startTime = DateTime.Now;
            sw.Start();
            using var response = await this.client.PostAsync(url, postBody, cancellationToken ?? GetCancellationToken());
            sw.Stop();

            logBody.ResponseStatusCode = (int)response.StatusCode;
            logBody.DurationMilliseconds = sw.ElapsedMilliseconds;

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            if (String.Equals(contentType?.CharSet, "BIG5", StringComparison.CurrentCultureIgnoreCase))
            {
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                result = Encoding.GetEncoding("BIG5").GetString(byteArray, 0, byteArray.Length);
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            #region Log Response
            logBody.ResponseData = this.AnonymizationLogBody(result, noLogResppatterns);
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            if (typeof(T) == typeof(HttpResponseMessage))
            {
                return (T)(object)response;
            }
            else if (typeof(T) == typeof(MyHttpResponseMessage))
            {
                MyHttpResponseMessage _response = new MyHttpResponseMessage();
                _response.ResponseRawData = response;
                _response.StartTime = startTime;
                _response.DurationMilliseconds = sw.ElapsedMilliseconds;
                _response.ResponseData = logBody.ResponseData;
                return (T)(object)_response;
            }
            else
            {
                try
                {
                    T resultObj = JsonConvert.DeserializeObject<T>(result);
                    if (resultObj is IStatusCodeResp)
                    {
                        ((IStatusCodeResp)resultObj).ResponseStatusCode = response.StatusCode;
                    }
                    return resultObj;
                }
                catch (Exception ex)
                {
                    throw new Exception($"result:{result};{JsonConvert.SerializeObject(ex)}");
                }
            }
        }

        public async Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, CancellationToken cancellationToken, Dictionary<string, string>? headers = null)
        {
            return await PostFrom<T>(url, postBody, headers, new string[] { }, new string[] { }, cancellationToken);
        }

        public async Task<T> PostFrom<T>(string url, FormUrlEncodedContent postBody, Dictionary<string, string>? headers = null)
        {
            return await PostFrom<T>(url, postBody, headers, new string[] { }, new string[] { });
        }


        public async Task<T> GetXml<T>(string url, string encoding, string[] noLogReqPatterns, string[] noLogResppatterns)
        {
            #region Log Request
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetXml";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = this.AnonymizationLogBody(url, noLogReqPatterns);
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            Stopwatch sw = new Stopwatch();
            DateTime startTime = DateTime.Now;
            sw.Start();
            using var response = await this.client.GetAsync(url, GetCancellationToken());
            sw.Stop();

            logBody.ResponseStatusCode = (int)response.StatusCode;
            logBody.DurationMilliseconds = sw.ElapsedMilliseconds;

            using Stream result = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(result, Encoding.GetEncoding(encoding));
            byte[]? byteArray = await response.Content.ReadAsByteArrayAsync();
            string resString = Encoding.GetEncoding(encoding).GetString(byteArray, 0, byteArray.Length);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(result);
        }

        public async Task<T> GetXml<T>(string url, string encoding)
        {
            return await GetXml<T>(url, encoding, new string[] { }, new string[] { });
        }

        public async Task<byte[]?> GetImage(string url)
        {
            #region Log Request 
            ApiLog logBody = new ApiLog();
            logBody.ActionId = this.ActionId;
            logBody.Action = "GetImage";
            logBody.Method = HttpMethod.Get.ToString();
            logBody.Url = url;
            logBody.CallApiActionId = Guid.NewGuid();
            logBody.CreateTime = DateTime.Now;
            this.LogMsg(logBody);
            #endregion

            Stopwatch sw = new Stopwatch();
            DateTime startTime = DateTime.Now;
            sw.Start();
            using var response = await this.client.GetAsync(url, GetCancellationToken());
            sw.Stop();

            logBody.ResponseStatusCode = (int)response.StatusCode;
            logBody.DurationMilliseconds = sw.ElapsedMilliseconds;

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            if (contentType == null || !(contentType.MediaType != null && contentType.MediaType.StartsWith("image")))
            {
                return null;
            }
            byte[] byteArray = await response.Content.ReadAsByteArrayAsync();
            #region Log Response
            logBody.ResponseData = $"imageLength={byteArray.Length}";
            logBody.LastUpdate = DateTime.Now;
            this.LogMsg(logBody);
            #endregion
            return byteArray;
        }

        private void LogMsg(ApiLog logBody)
        {
            logger.LogInformation(JsonConvert.SerializeObject(logBody));
        }
    }


}
