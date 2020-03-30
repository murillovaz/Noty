using System;
using System.Collections.Generic;
using System.Text;

namespace Noty.SqlServer
{
    public static class Parameter
    {
        public static KeyValuePair<string, object> WithValue(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }
    }
}
