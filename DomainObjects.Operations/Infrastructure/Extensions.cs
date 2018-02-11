using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal static class Extensions
    {
        private static Regex curlyBracketRegex = new Regex("\\{(.+?)\\}");
        public static string FormatOptional(this string s, params object[] args)
        {
            var numberOfArguments = curlyBracketRegex.Matches(s).Count;

            var missingArgumentCount = numberOfArguments - args.Length;
            if (missingArgumentCount <= 0) 
                return string.Format(s, args);

            args = args.Concat(Enumerable.Range(0, missingArgumentCount).Select(_ => string.Empty)).ToArray();
            return string.Format(s, args);
        }

            
    }
}
