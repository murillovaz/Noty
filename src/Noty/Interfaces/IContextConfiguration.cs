using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Noty.Interfaces
{
    public interface IContextConfiguration
    {
        string GetConnectionString();
        AsyncPolicyWrap<T> GetPolicy<T>();
    }
}
