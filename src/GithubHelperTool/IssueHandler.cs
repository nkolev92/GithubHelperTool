using Octokit;
using System;
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

        public async Task MoveIssue(string fromOrganization, string fromRepo, int issueNumber, string toOrganization, string toRepo)
        {
            await CopyIssue(fromOrganization, fromRepo, issueNumber, toOrganization, toRepo);
            try
            {
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

        public async Task CopyIssue(string fromOrganization, string fromRepo, int issueNumber, string toOrganization, string toRepo)
        {
            try
            {
                var fromIssue = await client.Issue.Get(fromOrganization, fromRepo, issueNumber);
                NewIssue newIssue = new NewIssue(fromIssue.Title)
                {
                    Body = GetCopiedIssueBody(fromIssue)
                };
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
    }
}
