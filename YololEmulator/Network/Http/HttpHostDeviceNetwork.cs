using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using uhttpsharp;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;
using Yolol.Execution;

namespace YololEmulator.Network.Http
{
    public class HttpHostDeviceNetwork
        : IDeviceNetwork, IDisposable
    {
        private readonly ConcurrentDictionary<string, NetworkVariable> _variables = new ConcurrentDictionary<string, NetworkVariable>();
        private readonly HttpServer _httpServer;

        public HttpHostDeviceNetwork(ushort port = 9671)
        {
            _httpServer = new HttpServer(new HttpRequestProvider());
            _httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, port)));
            _httpServer.Use(OnHttpRequest);
            _httpServer.Start();
        }

        private async Task OnHttpRequest([NotNull] IHttpContext context, Func<Task> next)
        {
            var name = context.Request.Uri.OriginalString.Trim('/');

            var response = new JObject();
            if (context.Request.Method == HttpMethods.Get)
            {
                response = context.Request.Uri.OriginalString == "/"
                    ? HttpGetAll()
                    : HttpGet(name);
            }
            else if (context.Request.Method == HttpMethods.Put)
            {
                response = HttpPut(name, JObject.Parse(Encoding.UTF8.GetString(context.Request.Post.Raw)));
            }

            var m = new MemoryStream();
            var w = new StreamWriter(m);
            await w.WriteAsync(response.ToString());
            await w.FlushAsync();
            m.Seek(0, SeekOrigin.Begin);
            context.Response = new HttpResponse("application/json", m, true);
        }

        private JObject HttpGetAll()
        {
            return JObject.FromObject(
                new Dictionary<string, object>(
                    _variables.Select(a => new KeyValuePair<string, object>(a.Key, new { value = a.Value.Value.ToObject() }))
                )
            );
        }

        private JObject HttpGet([NotNull] string name)
        {
            var v = Get(name);
            return JObject.FromObject(new { value = v.Value.ToObject() });
        }

        private JObject HttpPut([NotNull] string name, [NotNull] JObject request)
        {
            var v = request.GetValue("value")?.TryAsYololValue() ?? throw new ExecutionException("Could not parse network request into a yolol value: `{json}`");
            Get(name).Value = v;

            return HttpGet(name);
        }

        private NetworkVariable Get([NotNull] string name)
        {
            return _variables.GetOrAdd(name.ToLowerInvariant(), _ => new NetworkVariable());
        }

        IVariable IDeviceNetwork.Get([NotNull] string name)
        {
            return _variables.GetOrAdd(name.ToLowerInvariant(), _ => new NetworkVariable());
        }

        public void Dispose()
        {
            _httpServer.Dispose();
        }

        private class NetworkVariable
            : IVariable
        {
            public Value Value { get; set; }

            public NetworkVariable()
            {
                Value = new Value(0);
            }
        }

        
    }
}
