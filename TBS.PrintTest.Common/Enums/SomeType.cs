using System;
using System.Collections.Generic;
using System.Text;

// Enums in the Common project are for values that may need to be referenced
// for projects in the various layers.
// all projects can reference the common project but common should not reference
// any other projects in the solution.

namespace TBS.PrintTest.Common.Enums
{
    public enum SomeType
    {
        Example1,
        Example2
    }
}
