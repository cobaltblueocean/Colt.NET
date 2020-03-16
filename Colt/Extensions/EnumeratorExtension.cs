using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EnumeratorExtension
    {
        public static T Next<T>(this IEnumerator<T> source)
        {
            source.MoveNext();
            return source.Current;
        }
    }
}
