using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcDaClient
{
    internal class Sequence
    {
        private int _value;
        private int _initial;
        private int _MaxValue;
        public Sequence(int seed, int maxValue)
        {
            _initial = _value = seed;
            _MaxValue = maxValue;
        }
        public int GetNext()
        {
            Interlocked.CompareExchange(ref _value, _initial, _MaxValue);
            return Interlocked.Increment(ref _value);
        }
    }
}
