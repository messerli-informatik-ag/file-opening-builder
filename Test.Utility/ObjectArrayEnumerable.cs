using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Messerli.Test.Utility
{
    public class ObjectArrayEnumerable<T> : IEnumerable<object[]> where T : IEnumerable<object>, new()
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return new T()
                .Select(@interface => new [] {@interface})
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}