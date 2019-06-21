using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Yolol.Execution;

namespace YololEmulator.Network.Http
{
    public class HttpClientDeviceNetwork
        : IDeviceNetwork
    {
        private readonly Uri _url;

        public HttpClientDeviceNetwork(string url)
        {
            _url = new Uri(url);
        }

        public IVariable Get(string name)
        {
            return new HttpVariable(name, _url);
        }

        private class HttpVariable
            : IVariable
        {
            private readonly string _name;
            private readonly Uri _baseUrl;

            public Value Value
            {
                get
                {
                    using (var client = new HttpClient())
                    {
                        var json = JObject.Parse(client.GetStringAsync(new Uri(_baseUrl, _name)).Result);
                        var value = json?.GetValue("value")?.TryAsYololValue();
                        if (value.HasValue)
                            return value.Value;

                        throw new ExecutionException($"Could not parse network response into a yolol value: `{json}`");
                    }
                }
                set
                {
                    using (var client = new HttpClient())
                    {
                        var body = JObject.FromObject(new { value = value.ToObject() });
                        client.PutAsync(new Uri(_baseUrl, _name), new StringContent(body.ToString())).Wait();
                    }
                }
            }

            public HttpVariable(string name, Uri baseUrl)
            {
                _name = name.ToLowerInvariant();
                _baseUrl = baseUrl;
            }
        }
    }
}
