﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Converto
{
    internal class CachedConstructorInfo
    {
        public ConstructorInfo Constructor { get; }
        public List<ParameterInfo> Parameters { get; }

        public CachedConstructorInfo(ConstructorInfo constructorInfo)
        {
            Constructor = constructorInfo;
            Parameters = constructorInfo.GetParameters().ToList();
        }
    }
}
