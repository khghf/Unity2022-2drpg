using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem
{
    internal sealed class AbilityLog
    {
        [Conditional("DEBUG")]
        public static void LogInfo(string message)
        {
            Console.WriteLine($"[GameAbilitySystem]Info:{message}");
        }
        [Conditional("DEBUG")]
        public static void LogWarning(string message)
        {
            Console.WriteLine($"[GameAbilitySystem]Warning:{message}");

        }
        [Conditional("DEBUG")]
        public static void LogError(string message)
        {
            Console.WriteLine($"[GameAbilitySystem]Error:{message}");

        }
    }
}
