using Octokit;
using System;
using System.Threading.Tasks;

namespace GithubHelperTool
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expected 1 argument (github PAT). Found " + args.Length);
                return;
            }
            var client = new GitHubClient(new ProductHeaderValue("nuget-github-issue-tagger"));
            client.Credentials = new Credentials(args[0]);

            var handler = new IssueHandler(client);
            await handler.CopyIssue("nkolev92", "FromTestRepo", 1, "nkolev92", "ToTestRepo");
        }
    }
}
