using System;

namespace Stop.FileSystems
{
    [Flags]
    internal enum EMethod : uint
    {
        Buffered = 0,
        InDirect = 1,
        OutDirect = 2,
        Neither = 3
    }
}
