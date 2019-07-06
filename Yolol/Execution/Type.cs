using System;

namespace Yolol.Execution
{
    [Flags]
    public enum Type
        : byte
    {
        Unassigned = 0b00000000,   // Value type is not yet assigned
        Number     = 0b00000001,   // Value is a number
        String     = 0b00000010,   // Value is a string

        Error      = 0b00000100,   // Valur produced a runtime error
    }
}
