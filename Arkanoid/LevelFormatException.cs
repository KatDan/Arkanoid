﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    /// <summary>
    /// An exception that is thrown when the file the brick maps are loaded from is in wrong format.
    /// </summary>
    [Serializable]
    public class LevelFormatException : Exception
    {
        public const string errorMsg = ": The file that level is loaded from is in wrong format.";

        public LevelFormatException()
            : base(errorMsg)
        { }

        public LevelFormatException(string msg)
            : base(errorMsg+" "+msg)
        { }

        public LevelFormatException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
