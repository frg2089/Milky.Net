using System.Text.Json;
using System.Threading;

using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;

using Microsoft.Extensions.Options;

namespace Milky.Net.Server.Lagrange;

public sealed class LagrangeBotOptions
{
    public string DeviceInfoPath { get; set; } = "Lagrange.device.json";
    public string KeystorePath { get; set; } = "Lagrange.keystore.json";

    public async Task SaveDeviceInfoAsync(BotDeviceInfo deviceInfo, CancellationToken cancellationToken = default)
    {
        await using var fs = File.Create(DeviceInfoPath);
        await JsonSerializer.SerializeAsync(fs, deviceInfo, cancellationToken: cancellationToken);
    }

    public async Task SaveKeystoreAsync(BotKeystore keystore, CancellationToken cancellationToken = default)
    {
        await using var fs = File.Create(KeystorePath);
        await JsonSerializer.SerializeAsync(fs, keystore, cancellationToken: cancellationToken);
    }

    public async Task<BotDeviceInfo> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
    {
        BotDeviceInfo? info;
        if (!File.Exists(DeviceInfoPath))
        {
            info = BotDeviceInfo.GenerateInfo();
            await SaveDeviceInfoAsync(info, cancellationToken);
        }
        else
        {
            await using var fs = File.OpenRead(DeviceInfoPath);
            info = await JsonSerializer.DeserializeAsync<BotDeviceInfo>(fs, cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException();
        }

        return info;
    }

    public async Task<BotKeystore> GetKeystoreAsync(CancellationToken cancellationToken = default)
    {
        BotKeystore? result;
        if (!File.Exists(KeystorePath))
        {
            result = new();
            await SaveKeystoreAsync(result, cancellationToken);
        }
        else
        {
            await using var fs = File.OpenRead(KeystorePath);
            result = await JsonSerializer.DeserializeAsync<BotKeystore>(fs, cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException();
        }

        return result;
    }
}

public static class LagrangeExtensions
{
    public static IServiceCollection AddLagrange(this IServiceCollection services)
    {
        services
            .AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<BotContext>>();
                var config = provider.GetRequiredService<IOptions<BotConfig>>();
                var options = provider.GetRequiredService<IOptions<LagrangeBotOptions>>();
                var bot = BotFactory.Create(config.Value, options.Value.GetDeviceInfoAsync().Result, options.Value.GetKeystoreAsync().Result);
                bot.Invoker.OnBotLogEvent += (_, args) => logger.Log(args.Level switch
                {
                    global::Lagrange.Core.Event.EventArg.LogLevel.Debug => LogLevel.Trace,
                    global::Lagrange.Core.Event.EventArg.LogLevel.Verbose => LogLevel.Debug,
                    global::Lagrange.Core.Event.EventArg.LogLevel.Information => LogLevel.Information,
                    global::Lagrange.Core.Event.EventArg.LogLevel.Warning => LogLevel.Warning,
                    global::Lagrange.Core.Event.EventArg.LogLevel.Exception => LogLevel.Error,
                    global::Lagrange.Core.Event.EventArg.LogLevel.Fatal => LogLevel.Critical,
                    _ => throw new NotImplementedException(),
                }, args.EventMessage);

                return bot;
            });

        services
            .AddOptions<BotConfig>()
            .BindConfiguration("Lagrange:Config");

        services
            .AddOptions<LagrangeBotOptions>()
            .BindConfiguration("Lagrange");

        return services;
    }
}