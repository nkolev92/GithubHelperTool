using CommandLine;

namespace GithubHelperTool.Options
{
    [Verb("move-issue", HelpText = "Move an issue and all of it's content & comments to a new repo. This action also closes the underlying issue if it's still open.")]
    class MoveOptions : CopyOptions
    {
    }
}