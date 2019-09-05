using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Yolol.Cylon.Versions;
using Yolol.Grammar.AST;

namespace Yolol.Cylon.Deserialisation
{
    public class AstDeserializer
    {
        private readonly bool _typeExtension;

        public AstDeserializer(bool typeExtension = false)
        {
            _typeExtension = typeExtension;
        }

        [NotNull] public Program Parse([NotNull] string json)
        {
            var jobj = JObject.Parse(json);

            var version = Semver.SemVersion.Parse(jobj["version"].Value<string>());

            if (version == "0.3.0")
                return new V_0_3_0(_typeExtension).Parse(json);

            if (version >= "1.0.0" && version <= "2.0.0")
                return new V_1_X_X(_typeExtension).Parse(json);

            throw new NotSupportedException("Unsupported AST version");
        }
    }
}
