using CommandLine;
using GithubHelperTool.Options;
using Octokit;
using System.Threading.Tasks;

namespace GithubHelperTool
{
    public partial class Program
    {
        static int Main(string[] args)
        {
            // TODO NK - figure out how to parse stuff better.
            return Parser.Default.ParseArguments<MoveOptions>(args)
               .MapResult(
                 (MoveOptions moveOptions) => RunMoveCommand(moveOptions),
                 errs => 1);
        }

        private static int RunCopyCommand(CopyOptions opts)
        {
            return RunCopyCommandAsync(opts).GetAwaiter().GetResult();
        }

        private static async Task<int> RunCopyCommandAsync(CopyOptions opts)
        {
            var client = new GitHubClient(new ProductHeaderValue("github-helper-tool"));
            client.Credentials = new Credentials(opts.PAT);
            var handler = new IssueHandler(client);

            (string, string, int) fromIssue = UriUtilities.GetIssueDetails(opts.FromIssue);
            (string, string) toRepo = UriUtilities.GetRepoDetails(opts.ToRepository);

            await handler.CopyIssue(fromIssue.Item1, fromIssue.Item2, fromIssue.Item3, toRepo.Item1, toRepo.Item2);
            return 0;
        }

        private static int RunMoveCommand(MoveOptions opts)
        {
            return RunMoveCommandAsync(opts).GetAwaiter().GetResult();
        }

        private static async Task<int> RunMoveCommandAsync(MoveOptions opts)
        {
            var client = new GitHubClient(new ProductHeaderValue("github-helper-tool"));
            client.Credentials = new Credentials(opts.PAT);
            var handler = new IssueHandler(client);

            (string, string, int) fromIssue = UriUtilities.GetIssueDetails(opts.FromIssue);
            (string, string) toRepo = UriUtilities.GetRepoDetails(opts.ToRepository);

            await handler.MoveIssue(fromIssue.Item1, fromIssue.Item2, fromIssue.Item3, toRepo.Item1, toRepo.Item2);
            return 0;
        }
    }
}