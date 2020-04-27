using CommandLine;

using System;

namespace GithubHelperTool.Options
{
    [Verb("move-issue", HelpText = "Move an issue and all of it's content & comments to a new repo. This action also closes the underlying issue if it's still open.")]
    class MoveOptions : BaseOptions
    {
        [Option("from", Required = true, HelpText = "The link to the issue being transferred.")]
        public Uri FromIssue { get; set; }

        [Option("to", Required = true, HelpText = "The link to the destination repository.")]
        public Uri ToRepository { get; set; }
    }
}