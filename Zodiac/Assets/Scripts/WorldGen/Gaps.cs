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
    /// The gaps are ranges of doubles from 0.0 to 1.0, indicating the proportion of the screen that is a gap.
    /// </summary>
    public struct Gaps
    {
        public InclusiveDoubleRange north;
        public InclusiveDoubleRange east;
        public InclusiveDoubleRange south;
        public InclusiveDoubleRange west;
    }
}
