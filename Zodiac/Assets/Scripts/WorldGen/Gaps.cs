using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity;
using UnityEngine;
namespace WorldGen
{
    /// <summary>
    /// This struct represents gaps in the sides of screens that are used to connect screens together / make them walkable.
    /// </summary>
    struct Gaps
    {
        public InclusiveIntRange north;
        public InclusiveIntRange east;
        public InclusiveIntRange south;
        public InclusiveIntRange west;
    }

    struct InclusiveIntRange
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
