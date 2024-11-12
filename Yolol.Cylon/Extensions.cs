using System;
using Newtonsoft.Json.Linq;
using Semver;

namespace Yolol.Cylon
{
    internal static class Extensions
    {
        internal static JToken Tok(this JToken token, string field)
        {
            return token[field] ?? throw new ArgumentException($"`{field}` field is missing");
        }

        internal static bool LessThanOrEqualTo(this SemVersion a, SemVersion b)
        {
            return a.ComparePrecedenceTo(b) < 1;
        }

        internal static bool LessThan(this SemVersion a, SemVersion b)
        {
            return a.ComparePrecedenceTo(b) < 0;
        }

        internal static bool GreaterThanOrEqualTo(this SemVersion a, SemVersion b)
        {
            return a.ComparePrecedenceTo(b) > -1;
        }

        internal static bool GreaterThan(this SemVersion a, SemVersion b)
        {
            return a.ComparePrecedenceTo(b) > 0;
        }
    }
}
