<p align="center">
    <img src="logo/netler.svg" alt="netler logo" height="150px">
</p>

[![Build Status](https://github.com/svan-jansson/Netler.NET/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/svan-jansson/Netler.NET/actions/workflows/build-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/Netler.NET.svg?style=flat)](https://www.nuget.org/packages/Netler.NET)

# Netler.NET

A library for cross-process method calls over TCP. Designed for calling .NET methods from Elixir (see [hexdocs.pm/netler](https://hexdocs.pm/netler/)), and usable anywhere you need fast binary RPC between processes. All messages are serialised with [MessagePack](https://msgpack.org).

## Getting Started

```sh
dotnet add package Netler.NET
```

## Server

### Basic route registration

```csharp
using Netler;

var server = Server.Create(config =>
{
    config.UsePort(5544);
    config.UseRoutes(routes =>
    {
        routes.Add("Add", param =>
        {
            var a = Convert.ToInt32(param[0]);
            var b = Convert.ToInt32(param[1]);
            return a + b;
        });
    });
});

await server.Start();
```

### Typed routes with `AddTyped`

`AddTyped` infers parameter and return types from the lambda. The C# compiler enforces the types; the wire format is unchanged.

```csharp
config.UseRoutes(routes =>
{
    routes.AddTyped("Add",    (int a, int b) => a + b);
    routes.AddTyped("Double", (int x) => x * 2);
    routes.AddTyped("Ping",   () => "pong");
    routes.AddTyped("Log",    (string msg) => { /* void handler */ });
});
```

### Explicit composition with `Params.Decode`

`Params.Decode` wraps a typed delegate into the `Func<object[], object>` signature that `IRoutes.Add` expects. Useful when you want to separate the handler from the route registration, or pass an existing method reference.

```csharp
static int Add(int a, int b) => a + b;

config.UseRoutes(routes =>
{
    routes.Add("Add",    Params.Decode<int, int, int>(Add));
    routes.Add("Double", Params.Decode((int x) => x * 2));
    routes.Add("Ping",   Params.Decode(() => "pong"));
});
```

### Complex types

Any type annotated with `[MessagePackObject]` can be used as a parameter or return value.

```csharp
using MessagePack;

[MessagePackObject]
public class EchoRequest  { [Key(0)] public string Text { get; set; } }

[MessagePackObject]
public class EchoResponse { [Key(0)] public string Message { get; set; }
                            [Key(1)] public int    Length  { get; set; } }

config.UseRoutes(routes =>
{
    routes.AddTyped("Echo", (EchoRequest req) =>
        new EchoResponse { Message = req.Text, Length = req.Text.Length });
});
```

### Monitoring the client process

The server can watch a client OS process and react automatically when it exits.

```csharp
Server.Create(config =>
{
    config.UsePort(5544);
    config.UseClientPid(clientProcessId);
    config.UseClientDisconnectBehaviour(ClientDisconnectBehaviour.DisposeServer);
});
```

| `ClientDisconnectBehaviour` | Effect |
|---|---|
| `ShutdownApplication` | Calls `Environment.Exit(0)` — default |
| `DisposeServer` | Stops the server; application keeps running |
| `KeepAlive` | No action |

### Custom logger

```csharp
using Microsoft.Extensions.Logging;

ILogger logger = loggerFactory.CreateLogger<MyApp>();

Server.Create(config =>
{
    config.UsePort(5544);
    config.UseLogger(logger);
    config.UseRoutes(routes => { /* ... */ });
});
```

## Client

### Typed calls (recommended)

```csharp
using Netler;

using var client = new Client(5544);

var sum  = await client.InvokeAsync<int>("Add",    new object[] { 2, 3 });
var pong = await client.InvokeAsync<string>("Ping", new object[0]);

// Complex return type — EchoResponse must carry [MessagePackObject]
var reply = await client.InvokeAsync<EchoResponse>("Echo",
    new object[] { new EchoRequest { Text = "hello" } });
```

### Untyped calls

```csharp
var raw = await client.InvokeAsync("Add", new object[] { 2, 3 });
var sum = Convert.ToInt32(raw);
```

### Remote to another machine

```csharp
using var client = new Client("192.168.1.10", 5544);
```

### Error handling

When a route handler throws an unhandled exception on the server, the client receives a `RemoteInvokationFailed` exception containing the server-side error message.

```csharp
using Netler.Exceptions;

try
{
    await client.InvokeAsync("Divide", new object[] { 10, 0 });
}
catch (RemoteInvokationFailed ex)
{
    Console.WriteLine($"Server error: {ex.Message}");
}
```
