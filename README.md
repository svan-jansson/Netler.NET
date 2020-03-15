<p align="center">
    <img src="logo/netler.svg" alt="netler logo" height="150px">
</p>

[![Build Status](https://travis-ci.com/svan-jansson/Netler.NET.svg?branch=master)](https://travis-ci.com/svan-jansson/Netler.NET)
[![NuGet](https://img.shields.io/nuget/v/Netler.NET.svg?style=flat)](https://www.nuget.org/packages/Netler.NET)

# Netler.NET

A library for cross-process method calls over TCP. For instance for calling .NET methods from Elixir (see [https://hexdocs.pm/netler/](https://hexdocs.pm/netler/)). The library also contains a simple client that can be used to call a Netler server from another .NET application.

## Documentation

Auto generated documentation is available here: [DOCS.md](DOCS.md)

## Code Examples

### Creating a Server That Exposes a Method

This snippet creates a `Netler.Server` that listens on port `5544`. It exposes a single anonymous method on route `Add`.

```csharp
using Netler;

...

var server = Server.Create((config) =>
    {
        config.UsePort(5544);
        config.UseRoutes((routes) =>
        {
            routes.Add("Add", (param) =>
            {
                var a = Convert.ToInt32(param[0]);
                var b = Convert.ToInt32(param[1]);
                return a + b;
            });
            // More routes can be added here ...
        });
    });

server.Start();
```

### Calling a Method on The Server

This snippet calls the method in the example above. The client, in this example, is running on the same machine as the server, but in a different process.

```csharp
using Netler;

...

using (var client = new Client(5544))
{
    var response = client.Invoke("Add", new object[] { 2, 3 });
    var responseAsInt = Convert.ToInt32(response); // The response is 5
}
```
