using System;
using Xunit;

namespace GithubHelperTool.Tests
{
    public class UriUtilitiesTests
    {
        [Fact]
        public void ValidateGithubUrl_WithNonGithubUrl_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => UriUtilities.ValidateGithubUrl(new Uri(@"https://notgithub.com/fake/repo/issues/1")));
        }

        [Fact]
        public void ValidateGithubUrl_WithGithubUrl_Succeeds()
        {
            UriUtilities.ValidateGithubUrl(new Uri(@"https://github.com/fake/repo/issues/1"));
        }

        [Theory]
        [InlineData("https://github.com/org/repo/issues/1", "org", "repo", 1)]
        [InlineData("https://github.com/fancy-org-name/fancier-repo-name/issues/120", "fancy-org-name", "fancier-repo-name", 120)]
        public void GetIssueDetails_WithValidIssueUrl_Succeeds(string uri, string org, string repo, int issueNumber)
        {
            var issueDetails = UriUtilities.GetIssueDetails(new Uri(uri));

            Assert.Equal(org, issueDetails.Item1);
            Assert.Equal(repo, issueDetails.Item2);
            Assert.Equal(issueNumber, issueDetails.Item3);
        }

        [Theory]
        [InlineData("https://github.com/org/repo/", "org", "repo")]
        [InlineData("https://github.com/fancy-org-name/fancier-repo-name/issues/", "fancy-org-name", "fancier-repo-name")]
        public void GetRepoDetails_WithValidIssueUrl_Succeeds(string uri, string org, string repo)
        {
            var issueDetails = UriUtilities.GetRepoDetails(new Uri(uri));

            Assert.Equal(org, issueDetails.Item1);
            Assert.Equal(repo, issueDetails.Item2);
        }

        [Theory]
        [InlineData("https://github.com/org/repo/pulls/1")]
        [InlineData("https://github.com/fancy-org-name/fancier-repo-name/random/wiki/120")]
        public void GetIssueDetails_WithInvalidIssueUrl_ThrowsArgumentException(string uri)
        {
            Assert.Throws<ArgumentException>(() => UriUtilities.GetIssueDetails(new Uri(uri)));
        }

        [Theory]
        [InlineData("https://github.com/org/repo/idontknowsomethingweird/thisisnotavalidrepolink/")]
        [InlineData("https://github.com/fancy-org-name/fancier-repo-name/pulls/")]
        public void GetRepoDetails_WithInvalidIssueUrl_ThrowsArgumentException(string uri)
        {
            Assert.Throws<ArgumentException>(() => UriUtilities.GetRepoDetails(new Uri(uri)));
        }
    }
}
