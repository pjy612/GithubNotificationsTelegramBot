namespace GithubNotificationsTelegramBot.Extensions;

/// <summary>
///
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseDefaultConfiguration(this WebApplicationBuilder builder, string[]? args = null)
    {
        builder
            .UseDefaultConfiguration("Settings", args);

        var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (string.IsNullOrEmpty(envName))
        {
            Console.WriteLine(
                "Warning, no DOTNET_ENVIRONMENT environment configured!");
        }
        else
            builder.Environment.EnvironmentName = envName;

        return builder;
    }

    // ReSharper disable once MemberCanBePrivate.Global, HeapView.ClosureAllocation
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="folder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseDefaultConfiguration(this WebApplicationBuilder builder, string folder, string[]? args)
    {
        var user = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USERNAME"))
            ? Environment.GetEnvironmentVariable("USERNAME")
            : Environment.GetEnvironmentVariable("USER");

        Console.WriteLine("Environment:");
        Console.WriteLine($"  USER: {user}");
        Console.WriteLine(
            $"  DOTNET_ENVIRONMENT: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");
        Console.WriteLine(
            $"  HostingEnvironment.EnvironmentName: {builder.Environment.EnvironmentName}");
        Console.WriteLine(
            $"  HostingEnvironment.ApplicationName: {builder.Environment.ApplicationName}");

        var confFile = $"{folder}/Settings.json";
        var fullPathConfFile = new FileInfo(confFile).FullName;
        if (File.Exists(confFile))
        {
            Console.WriteLine($"Parsing configuration file: {fullPathConfFile}");
            builder.Configuration.AddJsonFile(confFile, true, false);
        }

        confFile = $"{folder}/Settings.{builder.Environment.EnvironmentName}.json";
        fullPathConfFile = new FileInfo(confFile).FullName;
        if (File.Exists(confFile))
        {
            Console.WriteLine($"Parsing configuration file: {fullPathConfFile}");
            builder.Configuration.AddJsonFile(confFile, true, false);
        }

        confFile = $"{folder}/Settings.{user}.json";
        fullPathConfFile = new FileInfo(confFile).FullName;
        if (File.Exists(confFile))
        {
            Console.WriteLine($"Parsing configuration file: {fullPathConfFile}");
            builder.Configuration.AddJsonFile(confFile, true, false);
        }

        builder.Configuration.AddCommandLine(args ?? new string[] { });
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }
}