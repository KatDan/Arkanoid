using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    [Serializable]
    public class LevelFormatException : Exception
    {
        public const string lineError = ": The file the level is loaded from is in wrong format.";

        public LevelFormatException()
            : base(lineError)
        { }

        public LevelFormatException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
