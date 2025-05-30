using CommandLine;

namespace GithubHelperTool.Options
{
    class BaseOptions
    {
        [Option("pat", Required = false, HelpText = "A Github PAT from a user with sufficient permissions to perform the invoked action." +
            "If not provided, the tool will attempt to use the GitHub Credential Provider to acquire a token.")]
        public string PAT { get; set; }
    }
}
