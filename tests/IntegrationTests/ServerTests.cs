using MessagePack;
using Netler;
using Netler.Contracts;
using Netler.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class ServerTests
    {
        [Fact]
        public async Task ClientServerCommunication()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.Add("Add", (param) =>
                        {
                            var a = Convert.ToInt32(param[0]);
                            var b = Convert.ToInt32(param[1]);
                            return a + b;
                        });
                    });
                });


            var expected = 5;
            int? actual = null;

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    actual = Convert.ToInt32(await client.InvokeAsync("Add", new object[] { 2, 3 }));
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.Equal(expected, actual);
        }

        [Fact(Timeout = 10_000)]
        public async Task LargeContent()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.Add("Large", (param) =>
                        {
                            var size = Convert.ToInt32(param[0]);
                            var payload = new List<object>();
                            for (var i = 0; i < size; i++)
                            {
                                payload.Add(new
                                {
                                    Index = i,
                                    Value = "This is a string"
                                });
                            }
                            return payload;
                        });
                    });
                });


            var expectedSize = 10_000;
            object[] actual = null;

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    actual = await client.InvokeAsync("Large", new object[] { expectedSize }) as object[];
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.Equal(expectedSize, actual.Length);
        }

        [Fact]
        public async Task ClientIsReusable()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.Add("Add", (param) =>
                        {
                            var a = Convert.ToInt32(param[0]);
                            var b = Convert.ToInt32(param[1]);
                            return a + b;
                        });
                    });
                });


            var firstExected = 5;
            var secondExected = 37;
            int? firstActual = null;
            int? secondActual = null;

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    firstActual = Convert.ToInt32(await client.InvokeAsync("Add", new object[] { 2, 3 }));
                    secondActual = Convert.ToInt32(await client.InvokeAsync("Add", new object[] { 30, 7 }));
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.Equal(firstExected, firstActual);
            Assert.Equal(secondExected, secondActual);
        }

        [Fact]
        public async Task ClientCatchesServerExceptions()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.Add("Add", (param) =>
                        {
                            throw new Exception("This is an exception thrown from the server");
                        });
                    });
                });


            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    await Assert.ThrowsAsync<RemoteInvokationFailed>(() => client.InvokeAsync("Add", new object[] { 2, 3 }));
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact(Skip = "CI")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Cannot be reliably run in CI environment")]
        public async Task ServerCanListenToClientProcessStatus()
        {
            var port = FreeTcpPort();
            var clientPid = StartProcessThatRunsFiveSeconds();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseClientPid(clientPid);
                    config.UseClientDisconnectBehaviour(Netler.Contracts.ClientDisconnectBehaviour.DisposeServer);
                });

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using var client = new Client(port);
                await Task.CompletedTask;
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.True(true);
        }

        [Fact]
        public async Task AddTyped_Primitives()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.AddTyped("Add",    (int a, int b) => a + b);
                        routes.AddTyped("Double", (int x) => x * 2);
                        routes.AddTyped("Ping",   () => "pong");
                    });
                });

            int? sum = null;
            int? doubled = null;
            string ping = null;

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    sum     = await client.InvokeAsync<int>("Add",    new object[] { 3, 4 });
                    doubled = await client.InvokeAsync<int>("Double", new object[] { 6 });
                    ping    = await client.InvokeAsync<string>("Ping", new object[] { });
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.Equal(7, sum);
            Assert.Equal(12, doubled);
            Assert.Equal("pong", ping);
        }

        [Fact]
        public async Task AddTyped_VoidAction()
        {
            var port = FreeTcpPort();
            var logged = string.Empty;

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.AddTyped("Log", (string msg) => { logged = msg; });
                    });
                });

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    await client.InvokeAsync("Log", new object[] { "hello" });
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.Equal("hello", logged);
        }

        [Fact]
        public async Task AddTyped_MessagePackObject()
        {
            var port = FreeTcpPort();

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(port);
                    config.UseRoutes((routes) =>
                    {
                        routes.AddTyped("Echo", (EchoRequest req) =>
                            new EchoResponse { Message = req.Text, Length = req.Text.Length });
                    });
                });

            EchoResponse actual = null;

            var serverTask = server.Start();
            var clientTask = Task.Run(async () =>
            {
                using (var client = new Client(port))
                {
                    var request = new EchoRequest { Text = "netler" };
                    actual = await client.InvokeAsync<EchoResponse>("Echo", new object[] { request });
                }
                server.Stop();
            });

            await Task.WhenAll(serverTask, clientTask);

            Assert.NotNull(actual);
            Assert.Equal("netler", actual.Message);
            Assert.Equal(6, actual.Length);
        }

        [MessagePackObject]
        public class EchoRequest
        {
            [Key(0)] public string Text { get; set; }
        }

        [MessagePackObject]
        public class EchoResponse
        {
            [Key(0)] public string Message { get; set; }
            [Key(1)] public int Length { get; set; }
        }

        private int StartProcessThatRunsFiveSeconds()
        {
            string scriptFile;
            var root = Environment.CurrentDirectory;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                scriptFile = "sleep.sh";
            }
            else
            {
                scriptFile = "sleep.cmd";
            }

            var fullPath = Path.Combine(root, scriptFile);

            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = fullPath;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            return process.Id;
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
