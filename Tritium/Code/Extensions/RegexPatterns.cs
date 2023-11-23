using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium
{
    public static class RegexPatterns
    {
        public static string IntegerOnly => @"-?\d+";
        public static string DecimalOnly => @"-?\d+(\.\d+)?";
    }
}
