using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME
{
    internal class Quality
    {
        public readonly int width, height, maxIteration;
        public Quality(int width, int height, int maxIteration)
        {
            this.width = width;
            this.height = height;
            this.maxIteration = maxIteration;
        }
    }
}
