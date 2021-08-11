using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    public enum powerUpType
    {
        NONE,
        SUPERBALL,
        TRIPLEBALL,
        SPEEDUP,
        SLOWDOWN
    }

    public class PowerUp
    {
        int x;
        int y;
        int speed;
        powerUpType type;

        public PowerUp() { }
    }
}
