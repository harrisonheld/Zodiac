using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    public struct InclusiveDoubleRange
    {
        public double Start { get; set; }
        public double End { get; set; }

        public InclusiveDoubleRange(double start, double end)
        {
            Start = start;
            End = end;
        }

        public bool Contains(double x)
        {
            return Start <= x && x <= End;
        }
    }
}
