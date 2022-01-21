using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class HttpHelperTemplate : ClassTemplate
    {
        public const string ClassName = "HttpHelper";

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace {Namespace}
{{
    public static class {Name}
    {{
        private static readonly int _httpRequestRetryTimes = ConfigurationManager.AppSettings[""HttpRequestRetryTimes""].ToInt(0);

        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri)
        {{
            return await httpClient.SendAsync<T>(HttpMethod.Get, requestUri);
        }}

        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string requestUri, object param)
        {{
            return await httpClient.SendAsync<T>(HttpMethod.Post, requestUri, param);
        }}

        public static async Task<T> PutAsync<T>(this HttpClient httpClient, string requestUri, object param)
        {{
            return await httpClient.SendAsync<T>(HttpMethod.Put, requestUri, param);
        }}

        public static async Task<T> DeleteAsync<T>(this HttpClient httpClient, string requestUri)
        {{
            return await httpClient.SendAsync<T>(HttpMethod.Delete, requestUri);
        }}

        public static async Task<T> SendAsync<T>(this HttpClient httpClient, HttpMethod method, string requestUri, object param = null, int retry = 0)
        {{
            try
            {{
                HttpContent content = null;
                if (param != null)
                {{
                    var paramJson = JsonHelper.Serialize(param);
                    content = new StringContent(paramJson, Encoding.UTF8, ""application/json"");
                }}
                using (var request = new HttpRequestMessage(method, requestUri) {{ Content = content }})
                using (var response = await httpClient.SendAsync(request))
                {{
                    if (!response.IsSuccessStatusCode)
                    {{
                        Logger.Info($""[{{method.ToString().ToUpper()}}] \""{{requestUri}}\""{{(param == null ? """" : ("" ["" + param.GetHashCode() + ""]""))}} Faild!"");
                        if (retry++ < _httpRequestRetryTimes)
                        {{
                            Logger.Info($""Retrying...[{{retry}}]"");
                            return await httpClient.SendAsync<T>(method, requestUri, param, retry);
                        }}
                        throw new HttpRequestFaildException(response);
                    }}
                    var resultJson = await response.Content.ReadAsStringAsync();
                    return JsonHelper.Deserialize<T>(resultJson);
                }}
            }}
            catch (Exception ex)
            {{
                Logger.Error(ex);
                return default(T);
            }}
        }}
    }}

    [Serializable]
    public class HttpRequestFaildException : HttpRequestException
    {{
        public HttpRequestFaildException(HttpResponseMessage response) : base(GetMessage(response))
        {{

        }}

        private static string GetMessage(HttpResponseMessage response)
        {{
            string reqMethod = null, reqUri = null, resContent = null;
            try
            {{
                var request = response.RequestMessage;
                reqMethod = request.Method.Method;
                reqUri = request.RequestUri.AbsoluteUri;
                resContent = response.Content.ReadAsStringAsync().Result.Replace(""\\/"", ""/"");
            }}
            catch {{ }}
            return $""Http Request Faild: [{{reqMethod}}] \""{{reqUri}}\"" => {{resContent}}"";
        }}
    }}
}}";
    }
}
