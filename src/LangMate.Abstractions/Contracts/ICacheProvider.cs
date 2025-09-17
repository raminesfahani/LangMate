using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangMate.Abstractions;

namespace LangMate.Abstractions.Contracts
{
    public interface ICacheProvider
    {
        void Set(string key, string value, TimeSpan ttl);
        bool TryGet(string key, out string value);
        void Remove(string key);
    }
}
