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
    internal struct Gaps
    {
        public InclusiveIntRange north;
        public InclusiveIntRange east;
        public InclusiveIntRange south;
        public InclusiveIntRange west;
    }
}
