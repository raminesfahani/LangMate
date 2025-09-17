using System;

namespace LangMate.Extensions.Tools.Annotations
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]  // Multiuse attribute.  
    public class EnvName(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}
