using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GithubHelperTool
{
    internal class IssueHandler
    {
        private readonly GitHubClient client;

        public IssueHandler(GitHubClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task MoveIssue(string fromOrganization, string fromRepo, int issueNumber, string toOrganization, string toRepo, bool providedCreds)
        {
            

            await CopyIssue(fromOrganization, fromRepo, issueNumber, toOrganization, toRepo, providedCreds);
            try
            {
                if (providedCreds == false)
                {
                    Dictionary<string, string>? credentuals = Get(new Uri("https://github.com/" + fromOrganization + "/" + fromRepo));
                    if (credentuals?.TryGetValue("password", out string? pat) == true)
                    {
                        client.Credentials = new Credentials(pat);
                    }
                }
                var fromIssue = await client.Issue.Get(fromOrganization, fromRepo, issueNumber);
                if (fromIssue.State == ItemState.Open)
                {
                    var issueUpdate = fromIssue.ToUpdate();
                    issueUpdate.State = ItemState.Closed;
                    await client.Issue.Update(fromOrganization, fromRepo, issueNumber, issueUpdate);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(Strings.Error_OperationFailed, nameof(CopyIssue), e.Message), e);
            }
        }

        public async Task CopyIssue(string fromOrganization, string fromRepo, int issueNumber, string toOrganization, string toRepo, bool providedCreds)
        {
            try
            {
                var fromIssue = await client.Issue.Get(fromOrganization, fromRepo, issueNumber);
                NewIssue newIssue = new NewIssue(fromIssue.Title)
                {
                    Body = GetCopiedIssueBody(fromIssue)
                };

                if (providedCreds == false)
                {
                    Dictionary<string, string>? credentuals = Get(new Uri("https://github.com/" + toOrganization + "/" + toRepo));
                    if (credentuals?.TryGetValue("password", out string? pat) == true)
                    {
                        client.Credentials = new Credentials(pat);
                    }
                }

                var toIssue = await client.Issue.Create(toOrganization, toRepo, newIssue);
                var fromComments = await client.Issue.Comment.GetAllForIssue(fromOrganization, fromRepo, issueNumber);
                foreach (var comment in fromComments)
                {
                    await client.Issue.Comment.Create(toOrganization, toRepo, toIssue.Number, GetCopiedIssueCommentBody(comment));
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(Strings.Error_OperationFailed, nameof(CopyIssue), e.Message), e);
            }
        }

        private static string GetCopiedIssueBody(Issue issue)
        {
            return string.Format(Strings.CommentedOn, GetUserString(issue.User), issue.CreatedAt.ToLocalTime().ToString("R"), issue.HtmlUrl) +
                Environment.NewLine +
                issue.Body;
        }

        private static string GetCopiedIssueCommentBody(IssueComment issueComment)
        {
            return string.Format(Strings.CommentedOn, GetUserString(issueComment.User), issueComment.CreatedAt.ToLocalTime().ToString("R"), issueComment.HtmlUrl) +
                Environment.NewLine +
                issueComment.Body;
        }

        private static string GetUserString(User user)
        {
            return "@" + user.Login;
        }

        // Implement https://git-scm.com/docs/git-credential#_typical_use_of_git_credential
        public static Dictionary<string, string>? Get(Uri uri)
        {
            string description = "url=" + uri.AbsoluteUri + "\n\n";

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "git",
                Arguments = "credential fill",
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            processStartInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";

            Process? process = Process.Start(processStartInfo);
            process?.StandardInput.Write(description);

            process?.WaitForExit();

            if (process?.ExitCode != 0)
            {
                // unable to get credentials
                return null;
            }

            Dictionary<string, string> result = new();
            string line;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                int index = line.IndexOf('=');
                if (index == -1)
                {
                    continue;
                }

                string key = line.Substring(0, index);
                string value = line.Substring(index + 1);
                result[key] = value;
            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return result.Count > 0 ? result : null;
        }
    }
}
