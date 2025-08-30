
using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;
using System.Threading;

using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Shimakaze.Milky;
using Shimakaze.Milky.Server;
using Shimakaze.Milky.Server.Lagrange;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLagrange();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IFileFetcher, FileFetcher>();
builder.Services.AddTransient<LagrangeFileApiEndpoints>();
builder.Services.AddTransient<LagrangeFriendApiEndpoints>();
builder.Services.AddTransient<LagrangeGroupApiEndpoints>();
builder.Services.AddTransient<LagrangeMessageApiEndpoints>();
builder.Services.AddTransient<LagrangeSystemApiEndpoints>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/event", async (
    HttpContext context,
    [FromServices] BotContext bot,
    CancellationToken cancellationToken) =>
{
    using LagrangeEventManager manager = new(bot, context, cancellationToken);
    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
});


app.MapPost("/api/{endpoint}", async (
    HttpContext context,
    [FromRoute] string endpoint,
    [FromServices] LagrangeFileApiEndpoints file,
    [FromServices] LagrangeFriendApiEndpoints friend,
    [FromServices] LagrangeGroupApiEndpoints group,
    [FromServices] LagrangeMessageApiEndpoints message,
    [FromServices] LagrangeSystemApiEndpoints system,
    CancellationToken cancellationToken) =>
{
    Debug.Assert(context.Request.HasJsonContentType());
    var json = await context.Request.ReadFromJsonAsync(MilkyJsonSerializerContext.Default.NullableJsonElement, cancellationToken);
    var (status, result, typeInfo) = await MatchAsync();
    if (!status)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    context.Response.StatusCode = StatusCodes.Status200OK;
    if (typeInfo is not null)
        await context.Response.WriteAsJsonAsync(result, typeInfo, cancellationToken: cancellationToken);
    else
        await context.Response.WriteAsJsonAsync(new(), MilkyJsonSerializerContext.Default.Object, cancellationToken: cancellationToken);

    await context.Response.CompleteAsync();

    async Task<(bool Success, object? Result, JsonTypeInfo? Type)> MatchAsync()
    {
        var tmp = await file.TryInvokeAsync(endpoint, json, cancellationToken);
        if (tmp.Success)
            return tmp;
        tmp = await friend.TryInvokeAsync(endpoint, json, cancellationToken);
        if (tmp.Success)
            return tmp;
        tmp = await group.TryInvokeAsync(endpoint, json, cancellationToken);
        if (tmp.Success)
            return tmp;
        tmp = await message.TryInvokeAsync(endpoint, json, cancellationToken);
        if (tmp.Success)
            return tmp;
        tmp = await system.TryInvokeAsync(endpoint, json, cancellationToken);
        return tmp;
    }
});

await using (var scope = app.Services.CreateAsyncScope())
{
    var bot = scope.ServiceProvider.GetRequiredService<BotContext>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<LagrangeBotOptions>>();
    var keystore = await options.Value.GetKeystoreAsync();
    if (keystore.Uin is not 0)
    {
        await bot.LoginByPassword();
    }
    else
    {
        var result = await bot.FetchQrCode();

        app.Logger.LogInformation("二维码链接: {url}", result.Value.Url);

        await bot.LoginByQrCode();
    }

    await options.Value.SaveDeviceInfoAsync(bot.UpdateDeviceInfo());
    await options.Value.SaveKeystoreAsync(bot.UpdateKeystore());
}

await app.RunAsync();