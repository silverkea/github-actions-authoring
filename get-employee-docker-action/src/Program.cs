using Octokit;
using Slack.NetStandard;

var githubUsername = Environment.GetEnvironmentVariable("INPUT_GITHUB-USERNAME");
var githubToken = Environment.GetEnvironmentVariable("INPUT_GITHUB-TOKEN");
var slackToken = Environment.GetEnvironmentVariable("INPUT_SLACK-TOKEN");

try
{
    var slackUserId = await LookupSlackUserbyGithubUsername(githubUsername, githubToken, slackToken);

    Console.WriteLine($"Found Slack user ID: {slackUserId}");

    await File.AppendAllLinesAsync(Environment.GetEnvironmentVariable("GITHUB_OUTPUT"), 
        [$"slack-user-id={slackUserId}"]
    );
}
catch (Exception ex)
{
    Console.WriteLine($"::error:: Failed to lookup Slack ID for GitHub user: {ex.Message}");
    Environment.Exit(1);
}


async Task<string> LookupSlackUserbyGithubUsername(string githubUsername, string githubToken, string slackToken)
{
    var ockokit = new GitHubClient(new ProductHeaderValue("get-employee-docker-action"));
    var tokenAuth = new Credentials(githubToken);
    ockokit.Credentials = tokenAuth;

    var githubUser = await ockokit.User.Get(githubUsername);
    var email = githubUser?.Email;

    if (email == null)
    {
        throw new Exception($"GitHub user '{githubUsername}' does not have a public email address.");
    }

    var client = new SlackWebApiClient(slackToken);
    var slackResponse = await client.Users.LookupByEmail(email);

    if(slackResponse?.User == null)
    {
        throw new Exception($"No Slack user found with email '{email}'.");
    }

    return slackResponse.User.ID;
}