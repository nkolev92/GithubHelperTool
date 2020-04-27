using CommandLine;

namespace GithubHelperTool.Options
{
    class BaseOptions
    {
        [Option("pat", Required = true, HelpText = "A Github PAT from a user with sufficient permissions to perform the invoked action.")]
        public string PAT { get; set; }
    }
}
