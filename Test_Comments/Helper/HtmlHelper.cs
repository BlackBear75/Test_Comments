using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Test_Comments.Helpers
{
    public static class HtmlHelper
    {
        private static readonly List<string> AllowedTags = new List<string> { "a", "code", "i", "strong" };

        public static string SanitizeHTML(string inputText)
        {
            string tagPattern = @"<\/?(?!a|code|i|strong\b)[^>]*>";
            return Regex.Replace(inputText, tagPattern, string.Empty);
        }

        public static bool ValidateHTMLTags(string inputText)
        {
            var tagPattern = new Regex(@"<\/?([a-z]+)(?:\s+[^>]*)?>", RegexOptions.IgnoreCase);
            var stack = new Stack<string>();
            var matches = tagPattern.Matches(inputText);

            foreach (Match match in matches)
            {
                var tagName = match.Groups[1].Value.ToLower();

                if (AllowedTags.Contains(tagName))
                {
                    if (match.Value.StartsWith("</"))
                    {
                        if (stack.Count == 0 || stack.Pop() != tagName)
                            return false;
                    }
                    else
                    {
                        stack.Push(tagName);
                    }
                }
            }

            return stack.Count == 0; 
        }
    }
}