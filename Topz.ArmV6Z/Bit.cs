using System;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Specific bits of an instruction.
    /// </summary>
    [Flags]
    internal enum Bit
    {
        /// <summary>
        /// The instruction has no bits turned on.
        /// </summary>
        None = 0,

        /// <summary>
        /// The L bit is turned on.
        /// </summary>
        L = 1,
        
        /// <summary>
        /// The S bit is turned on.
        /// </summary>
        S = 2,

        /// <summary>
        /// The H bit is turned on.
        /// </summary>
        H = 4,

        /// <summary>
        /// The D bit is turned on.
        /// </summary>
        D = 8,

        /// <summary>
        /// The B bit is turned on.
        /// </summary>
        B = 16
    }
}