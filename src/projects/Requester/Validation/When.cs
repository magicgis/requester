using System;
using System.Net.Http;
using EnsureThat;

namespace Requester.Validation
{
    /// <summary>
    /// This is purely a construct intended for use of validation of WebAPIs.
    /// If you want something for performing HTTP-requests for application use,
    /// look at <see cref="HttpRequester"/>.
    /// </summary>
    public class When
    {
        public static HttpTextResponse Head(string url, Action<HttpRequest> configurer = null)
        {
            return DoRequest(HttpMethod.Head, url, configurer);
        }

        public static HttpTextResponse Put(string url, Action<HttpRequest> configurer = null)
        {
            return DoRequest(HttpMethod.Put, url, configurer);
        }

        public static HttpTextResponse Delete(string url, Action<HttpRequest> configurer = null)
        {
            return DoRequest(HttpMethod.Delete, url, configurer);
        }

        public static HttpTextResponse PostAsJson<TContent>(string url, TContent content, Action<HttpRequest> configurer = null) where TContent : class
        {
            Ensure.That(content, "content").IsNotNull();

            return DoRequest(HttpMethod.Post, url, configurer, serializer => serializer.Serialize(content));
        }

        public static HttpTextResponse PutAsJson<TContent>(string url, TContent content, Action<HttpRequest> configurer = null) where TContent : class
        {
            Ensure.That(content, "content").IsNotNull();

            return DoRequest(HttpMethod.Put, url, configurer, serializer => serializer.Serialize(content));
        }

        public static HttpTextResponse PostOfJson(string url, string content, Action<HttpRequest> configurer = null)
        {
            Ensure.That(content, "content").IsNotNull();

            return DoRequest(HttpMethod.Post, url, configurer, serializer => content);
        }

        public static HttpTextResponse PutOfJson(string url, string content, Action<HttpRequest> configurer = null)
        {
            Ensure.That(content, "content").IsNotNull();

            return DoRequest(HttpMethod.Put, url, configurer, serializer => content);
        }

        public static HttpTextResponse GetOfJson(string url, Action<HttpRequest> configurer = null)
        {
            return DoRequest(HttpMethod.Get, url, configurer);
        }

        private static HttpTextResponse DoRequest(HttpMethod method, string url, Action<HttpRequest> configurer = null, Func<IJsonSerializer, string> contentFn = null)
        {
            Ensure.That(url, "url").IsNotNullOrWhiteSpace();

            var request = new HttpRequest(method);

            using (var requester = new HttpRequester(url))
            {
                var content = contentFn?.Invoke(requester.JsonSerializer);
                if (!string.IsNullOrWhiteSpace(content))
                    request.WithJsonContent(content);

                configurer?.Invoke(request);

                return requester.SendAsync(request).Result;
            }
        }
    }
}