using System;
using System.Linq;

namespace GithubHelperTool
{
    internal class UriUtilities
    {
        internal static (string, string, int) GetIssueDetails(Uri uri)
        {
            ValidateGithubUrl(uri);
            var segments = uri.Segments.Select(e => e.Trim('/').Trim('\\')).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (segments.Count != 4 || !segments[2].Equals("issues", StringComparison.OrdinalIgnoreCase) || !int.TryParse(segments[3], out int issueNumber))
            {
                throw new ArgumentException(string.Format(Strings.Error_UnparsableIssueUrl, uri.OriginalString));
            }

            return (segments[0], segments[1], issueNumber);
        }

        internal static (string, string) GetRepoDetails(Uri uri)
        {
            ValidateGithubUrl(uri);
            var segments = uri.Segments.Select(e => e.Trim('/').Trim('\\')).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (!(segments.Count == 2 || (segments.Count == 3 && segments[2].Equals("issues", StringComparison.OrdinalIgnoreCase))))
            {
                throw new ArgumentException(string.Format(Strings.Error_UnparsableIssueUrl, uri.OriginalString));
            }

            return (segments[0], segments[1]);
        }

        internal static void ValidateGithubUrl(Uri uri)
        {
            if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(Strings.Error_UriHostNotSupported);
            }
        }
    }
}
