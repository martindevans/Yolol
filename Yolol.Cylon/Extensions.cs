using System;
using Newtonsoft.Json.Linq;

namespace Yolol.Cylon
{
    internal static class Extensions
    {
        internal static JToken Tok(this JToken token, string field)
        {
            return token[field] ?? throw new ArgumentException($"`{field}` field is missing");
        }
    }
}
