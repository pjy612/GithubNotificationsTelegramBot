using System.Globalization;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.Deployment;
using Octokit.Webhooks.Events.DeploymentStatus;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.Issues;
using Octokit.Webhooks.Events.WorkflowJob;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace GithubNotificationsTelegramBot;

public class AppWebhookEventProcessor : WebhookEventProcessor
{
    private ILogger<AppWebhookEventProcessor> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly long _chatId;

    public AppWebhookEventProcessor(ILogger<AppWebhookEventProcessor> logger,
        ITelegramBotClient bot,
        IConfiguration configuration)
    {
        _logger = logger;
        _chatId = configuration.GetValue<long>("TelegramChatId");
        _bot = bot;
    }

    private string ParseDate(string date)
    {
        return !string.IsNullOrEmpty(date)
            ? DateTime.Parse(date).ToString(CultureInfo.InvariantCulture)
            : "";
    }
    
    protected override async Task ProcessWorkflowJobWebhookAsync(
        WebhookHeaders headers, WorkflowJobEvent workflowJobEvent,
        WorkflowJobAction action)
    {
        var text =
            $@"[{workflowJobEvent.Repository.Name}]({workflowJobEvent.Repository.HtmlUrl})
Job: [{workflowJobEvent.WorkflowJob.Name}]({workflowJobEvent.WorkflowJob.HtmlUrl}) started by: [{workflowJobEvent.Sender.Login}]({workflowJobEvent.Sender.HtmlUrl})
Status: {workflowJobEvent.WorkflowJob.Status} 
started at: {ParseDate(workflowJobEvent.WorkflowJob.StartedAt)}  
completed at: {ParseDate(workflowJobEvent.WorkflowJob.CompletedAt)} 
";
        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    protected override async Task ProcessDeploymentStatusWebhookAsync(
        WebhookHeaders headers, DeploymentStatusEvent deploymentStatusEvent,
        DeploymentStatusAction action)
    {
        var text =
            $@"[{deploymentStatusEvent.Repository.Name}]({deploymentStatusEvent.Repository.HtmlUrl})
New deployment status by: [{deploymentStatusEvent.Sender.Login}]({deploymentStatusEvent.Sender.HtmlUrl})

[{deploymentStatusEvent.DeploymentStatus.State.ToString()}]({deploymentStatusEvent.DeploymentStatus.Url})
";
        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    protected override
        async Task ProcessDeploymentWebhookAsync(WebhookHeaders headers,
            DeploymentEvent deploymentEvent,
            DeploymentAction action)
    {
        var text =
            $@"[{deploymentEvent.Repository.Name}]({deploymentEvent.Repository.HtmlUrl})
New deployment by: [{deploymentEvent.Sender.Login}]({deploymentEvent.Sender.HtmlUrl})

[{deploymentEvent.Deployment.Task}]({deploymentEvent.Deployment.Url})
";
        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers, IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        var text =
            $@"[{issueCommentEvent.Repository.Name}]({issueCommentEvent.Repository.HtmlUrl})
New issue comment by: [{issueCommentEvent.Sender.Login}]({issueCommentEvent.Sender.HtmlUrl})

[{issueCommentEvent.Issue.Title}]({issueCommentEvent.Issue.HtmlUrl})
{issueCommentEvent.Comment.Body}
";
        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    protected override async Task ProcessIssuesWebhookAsync(
        WebhookHeaders headers, IssuesEvent issuesEvent, IssuesAction action)
    {
        var text = $@"[{issuesEvent.Repository.Name}]({issuesEvent.Repository.HtmlUrl})
New issue by: [{issuesEvent.Sender.Login}]({issuesEvent.Sender.HtmlUrl})

[{issuesEvent.Issue.Title}]({issuesEvent.Issue.HtmlUrl})
{issuesEvent.Issue.Body}".Replace("![image]", "[]");
        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    protected override async Task ProcessPushWebhookAsync(WebhookHeaders headers,
        PushEvent pushEvent)
    {
        var text = $@"[{pushEvent.Repository.Name}]({pushEvent.Repository.HtmlUrl})\
Push by: [{pushEvent.Sender.Login}]({pushEvent.Sender.HtmlUrl})\

";
        if (pushEvent?.Commits != null)
        {
            foreach (var commit in pushEvent?.Commits)
            {
                text +=
                    $@"Commit: [{commit.Message.Replace(".", "\\.")}]({commit.Url}) by {commit.Author.Name}\
";
            }
        }

        await _bot.SendTextMessageAsync(_chatId, text,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    // public override async Task ProcessWebhookAsync(WebhookHeaders headers,
    //     WebhookEvent webhookEvent)
    // {
    //     _logger.LogInformation("Webhook handled: {WebhookEventAction}",
    //         webhookEvent.Action);
    //
    //     _logger.LogInformation(webhookEvent.Action);
    //
    //     var text =
    //         $@"[{webhookEvent.Repository?.Name}]({webhookEvent.Repository?.Url}) by: [{webhookEvent.Sender?.Login}]({webhookEvent.Sender?.HtmlUrl})";
    //
    //     try
    //     {
    //         await _bot.SendTextMessageAsync(_chatId, text,
    //             parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    //         await base.ProcessWebhookAsync(headers, webhookEvent);
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e.Message);
    //         _logger.LogError(text);
    //     }
    // }
}