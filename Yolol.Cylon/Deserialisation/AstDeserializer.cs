﻿using System;
using Newtonsoft.Json.Linq;
using Semver;
using Yolol.Cylon.Deserialisation.Versions;
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

        public Program Parse(string json)
        {
            var jobj = JObject.Parse(json);

            var version = SemVersion.Parse(jobj.Tok("version").Value<string>()!);

            if (version == new SemVersion(0, 3, 0))
                return new V_0_3_0(_typeExtension).Parse(json);
            
            if (version.GreaterThanOrEqualTo(new SemVersion(1, 0, 0)) && version.LessThanOrEqualTo(new SemVersion(2, 0, 0)))
                return new V_1_X_X(_typeExtension).Parse(json);

            throw new NotSupportedException("Unsupported AST version");
        }
    }
}
