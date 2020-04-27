using CommandLine;

using System;

namespace GithubHelperTool.Options
{
    [Verb("copy-issue", HelpText = "Copy an issue and all of it's content & comments to a new repo.")]
    class CopyOptions : BaseOptions
    {
        [Option("from", Required = true, HelpText = "The link to the issue being transferred.")]
        public Uri FromIssue { get; set; }

        [Option("to", Required = true, HelpText = "The link to the destination repository.")]
        public Uri ToRepository { get; set; }
    }
}