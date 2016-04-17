using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onism.Cldr.Extensions
{
    public static class MatchExtensions
    {
        public static bool IsGroupMatched(this Match match, string groupName)
        {
            return !string.IsNullOrEmpty(match.GetMatchedGroup(groupName));
        }

        /// <summary>
        /// Gets the captured substring from groups matched by a regular expression.
        /// </summary>
        public static string GetMatchedGroup(this Match match, string groupName)
        {
            return match.Groups[groupName]?.Value;
        }
    }
}
