﻿using CommandLine;
using GithubHelperTool.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace GithubHelperTool
{
    public partial class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<CopyOptions, MoveOptions>(args)
               .MapResult(
                 (CopyOptions copyOptions) => RunCopyCommand(copyOptions),
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
            if (opts.PAT == null)
            {
                Console.WriteLine("No Personal Access Token (PAT) was supplied, the tool will attempt to retrieve it manually.");
            }
            else
            {
                client.Credentials = new Credentials(opts.PAT);
            }
            var handler = new IssueHandler(client);

            (string, string, int) fromIssue = UriUtilities.GetIssueDetails(opts.FromIssue);
            (string, string) toRepo = UriUtilities.GetRepoDetails(opts.ToRepository);

            await handler.CopyIssue(fromIssue.Item1, fromIssue.Item2, fromIssue.Item3, toRepo.Item1, toRepo.Item2, opts.PAT != null);
            return 0;
        }

        private static int RunMoveCommand(MoveOptions opts)
        {
            return RunMoveCommandAsync(opts).GetAwaiter().GetResult();
        }

        private static async Task<int> RunMoveCommandAsync(MoveOptions opts)
        {
            var client = new GitHubClient(new ProductHeaderValue("github-helper-tool"));
            if (opts.PAT == null)
            {
                Console.WriteLine("No Personal Access Token (PAT) was supplied, the tool will attempt to retrieve it manually.");
            }
            else
            {
                client.Credentials = new Credentials(opts.PAT);
            }
            var handler = new IssueHandler(client);

            (string, string, int) fromIssue = UriUtilities.GetIssueDetails(opts.FromIssue);
            (string, string) toRepo = UriUtilities.GetRepoDetails(opts.ToRepository);

            await handler.MoveIssue(fromIssue.Item1, fromIssue.Item2, fromIssue.Item3, toRepo.Item1, toRepo.Item2, opts.PAT != null);
            return 0;
        }
    }
}