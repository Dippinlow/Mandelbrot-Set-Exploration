using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME
{
    internal class Location
    {
        public readonly Complex centre;
        public readonly float zoom;

        public Location(Complex centre, float zoom)
        {
            this.centre = centre;
            this.zoom = zoom;
        }
    }
}
