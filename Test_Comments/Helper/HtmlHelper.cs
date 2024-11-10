using System.Text.RegularExpressions;

namespace Test_Comments.Helper
{
    public static class HtmlHelper
    {
        private static readonly string[] AllowedTags = { "b", "i", "u", "strong", "em", "p" };

        public static string SanitizeHTML(string input)
        {
            string pattern = $@"</?(?!({string.Join("|", AllowedTags)})\b)\w+.*?>";
            return Regex.Replace(input, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        public static bool ValidateHTMLTags(string input)
        {
            var tagStack = new Stack<string>();
            var tagRegex = new Regex(@"<(/?)(\w+)[^>]*>", RegexOptions.IgnoreCase);
            foreach (Match match in tagRegex.Matches(input))
            {
                var isClosingTag = match.Groups[1].Value == "/";
                var tagName = match.Groups[2].Value.ToLower();

                if (!AllowedTags.Contains(tagName)) continue;

                if (isClosingTag)
                {
                    if (tagStack.Count == 0 || tagStack.Pop() != tagName) return false;
                }
                else
                {
                    tagStack.Push(tagName);
                }
            }

            return tagStack.Count == 0;
        }
    }
}