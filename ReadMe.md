![Milky.Net](https://socialify.git.ci/frg2089/Milky.Net/image?custom_description=SaltifyDev%2Fmilky+%E7%9A%84+.Net+%E5%AE%9E%E7%8E%B0&custom_language=.NET&description=1&font=Inter&forks=1&issues=1&language=1&logo=https%3A%2F%2Fmedia.githubusercontent.com%2Fmedia%2Ffrg2089%2FMilky.Net%2Fmaster%2FLogo.png&name=1&pattern=Circuit+Board&pulls=1&stargazers=1&theme=Auto)

## 什么是 Milky.Net

Milky.Net 是一个 .Net 框架，用于实现 [Milky](https://milky.ntqqrev.org/) 协议

|                                                                    正式版包                                                                    |                                                                     预览版包                                                                      | 描述                             |
| :--------------------------------------------------------------------------------------------------------------------------------------------: | :-----------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------- |
|   ![[Milky.Net.Model](https://www.nuget.org/packages/Milky.Net.Model)](https://img.shields.io/nuget/v/Milky.Net.Model?label=Milky.Net.Model)   |   ![[Milky.Net.Model](https://www.nuget.org/packages/Milky.Net.Model)](https://img.shields.io/nuget/vpre/Milky.Net.Model?label=Milky.Net.Model)   | Milky 协议的模型实现             |
| ![[Milky.Net.Client](https://www.nuget.org/packages/Milky.Net.Client)](https://img.shields.io/nuget/v/Milky.Net.Client?label=Milky.Net.Client) | ![[Milky.Net.Client](https://www.nuget.org/packages/Milky.Net.Client)](https://img.shields.io/nuget/vpre/Milky.Net.Client?label=Milky.Net.Client) | Milky 客户端（应用端）的具体实现 |
| ![[Milky.Net.Server](https://www.nuget.org/packages/Milky.Net.Server)](https://img.shields.io/nuget/v/Milky.Net.Server?label=Milky.Net.Server) | ![[Milky.Net.Server](https://www.nuget.org/packages/Milky.Net.Server)](https://img.shields.io/nuget/vpre/Milky.Net.Server?label=Milky.Net.Server) | Milky 服务端（协议端）的抽象实现 |

## 快速开始

```csharp
using System.Net.Http;

using Milky.Net.Client;

// ...

// 准备一个 HttpClient 实例
HttpClient client = new()
{
    // 设置服务端（实现端）地址
    BaseAddress = new("http://localhost:8080/"),
    DefaultRequestHeaders =
    {
        { "Authorization", "Bearer " },
    },
};

// 创建 MilkyClient 实例
MilkyClient milky = new(client);

// 监听机器人离线事件
milky.Events.BotOffline += (milky, e) => {
  // ...
};

// 通过 WebSocket 接收事件
_ = milky.ReceivingEventUsingWebSocketAsync();

// 获取服务端（实现端）信息
var result = await milky.System.GetImplInfoAsync();

/// ...
```
