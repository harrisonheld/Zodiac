using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    internal struct InclusiveIntRange
    {
        public int start;
        public int end;
        public InclusiveIntRange(int _start, int _end)
        {
            start = _start;
            end = _end;
        }

        public bool Contains(int x)
        {
            return start <= x && x <= end;
        }
    }
}
