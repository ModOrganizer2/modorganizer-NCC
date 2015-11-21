using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.Client.Util
{
    public class Pair<F, S>
    {
        public Pair()
        {
        }

        public Pair(F first, S second)
        {
            this.First = first;
            this.Second = second;
        }

        public F First { get; set; }
        public S Second { get; set; }
    }
}