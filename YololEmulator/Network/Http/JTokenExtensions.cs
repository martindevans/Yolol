using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Yolol.Execution;

namespace YololEmulator.Network.Http
{
    public static class JTokenExtensions
    {
        public static Value? TryAsYololValue([NotNull] this JToken token)
        {
            if (token.Type == JTokenType.String)
                return new Value(token.Value<string>());
            else if (token.Type == JTokenType.Integer)
                return new Value(token.Value<int>());
            else if (token.Type == JTokenType.Float)
                return new Value((decimal)token.Value<double>());

            return null;
        }
    }
}
