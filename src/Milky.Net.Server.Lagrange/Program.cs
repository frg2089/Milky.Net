using System.Net.Mime;
using System.Text.Json;

using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Options;

using Milky.Net.Model;
using Milky.Net.Server;
using Milky.Net.Server.Lagrange;
using Milky.Net.Server.Lagrange.ConsoleImage.Sixel;

using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSockets(options => builder.Configuration.Bind("WebSockets", options));
builder.Services.AddLagrange();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IFileFetcher, FileFetcher>();
builder.Services.AddTransient<IMilkyFileApiEndpoints, LagrangeFileApiEndpoints>();
builder.Services.AddTransient<IMilkyFriendApiEndpoints, LagrangeFriendApiEndpoints>();
builder.Services.AddTransient<IMilkyGroupApiEndpoints, LagrangeGroupApiEndpoints>();
builder.Services.AddTransient<IMilkyMessageApiEndpoints, LagrangeMessageApiEndpoints>();
builder.Services.AddTransient<IMilkySystemApiEndpoints, LagrangeSystemApiEndpoints>();
builder.Services.AddTransient<MilkyApiEndpoints>();

var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/event", async (
    HttpContext context,
    [FromServices] LagrangeEventScheduler eventScheduler,
    CancellationToken cancellationToken) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // WebSocket 请求
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        eventScheduler.OnReceived += async (data, jsonTypeInfo) =>
        {
            await using MemoryStream ms = new();
            await JsonSerializer.SerializeAsync(ms, data, jsonTypeInfo, cancellationToken);
            await ms.FlushAsync(cancellationToken);
            ms.Seek(0, SeekOrigin.Begin);
            await webSocket.SendAsync(ms.ToArray(), System.Net.WebSockets.WebSocketMessageType.Text, false, cancellationToken);
        };
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }
    else
    {
        // Server-Sent Events 请求
        context.Response.StatusCode = StatusCodes.Status206PartialContent;
        context.Response.ContentType = MediaTypeNames.Text.EventStream;
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";
        eventScheduler.OnReceived += async (data, jsonTypeInfo) =>
        {
            await using MemoryStream ms = new();
            await JsonSerializer.SerializeAsync(ms, data, jsonTypeInfo, cancellationToken);
            await ms.FlushAsync(cancellationToken);
            ms.Seek(0, SeekOrigin.Begin);
            using StreamReader sr = new(ms);
            while (!sr.EndOfStream)
            {
                var line = await sr.ReadLineAsync(cancellationToken);
                if (string.IsNullOrEmpty(line))
                    break;

                await context.Response.WriteAsync($"data: {line}\n", cancellationToken);
            }
            await context.Response.WriteAsync($"\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);
        };
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }
});


app.MapPost("/api/{endpoint}", async (
    HttpContext context,
    [FromRoute] string endpoint,
    [FromServices] MilkyApiEndpoints endpoints,
    CancellationToken cancellationToken) =>
{
    if (!endpoints.CanInvoke(endpoint))
    {
        await TypedResults.NotFound(endpoint).ExecuteAsync(context);
        return;
    }

    if (!context.Request.HasJsonContentType())
    {
        await TypedResults.BadRequest("Invalid content type").ExecuteAsync(context);
        return;
    }

    var json = await context.Request.ReadFromJsonAsync(MilkyJsonSerializerContext.Default.NullableJsonElement, cancellationToken);

    var result = await endpoints.InvokeAsync(endpoint, json, cancellationToken);

    context.Response.StatusCode = StatusCodes.Status200OK;
    if (result is not null)
        await context.Response.WriteAsJsonAsync(result.Value.Result, result.Value.Type, cancellationToken: cancellationToken);
    else
        await context.Response.WriteAsJsonAsync(new(), MilkyJsonSerializerContext.Default.Object, cancellationToken: cancellationToken);

    await context.Response.CompleteAsync();
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
        var task = bot.FetchQrCode();

        using StringWriter sw = new();
        Console.Write("\e[c");
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar is 'c')
                break;

            sw.Write(key.KeyChar);
        }
        var codes = sw.ToString()[3..].Split(';');

        var result = await task;
        if (codes.Contains("4"))
        {
            using var img = Image.Load(result.Value.QrCode);
            var sixel = Sixel.Process(img);

            app.Logger.LogInformation(
                """
                请扫码二维码登录
                {img}
                """,
                sixel);
        }
        else
        {
            app.Logger.LogInformation("""
                当前终端不支持 Sixel 协议展示图像，请手动生成二维码
                二维码链接: {url}
                """,
                result.Value.Url);
        }



        await bot.LoginByQrCode();
    }

    await options.Value.SaveDeviceInfoAsync(bot.UpdateDeviceInfo());
    await options.Value.SaveKeystoreAsync(bot.UpdateKeystore());
}

await app.RunAsync();