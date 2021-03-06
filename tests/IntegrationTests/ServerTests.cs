using Netler;
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
        public void ClientServerCommunication()
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
            var clientTask = Task.Run(() =>
            {
                using (var client = new Client(port))
                {
                    actual = Convert.ToInt32(client.Invoke("Add", new object[] { 2, 3 }));
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LargeContent()
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
            var clientTask = Task.Run(() =>
            {
                using (var client = new Client(port))
                {
                    actual = client.Invoke("Large", new object[] { expectedSize }) as object[];
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);

            Assert.Equal(expectedSize, actual.Length);
        }

        [Fact]
        public void ClientIsReusable()
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
            var clientTask = Task.Run(() =>
            {
                using (var client = new Client(port))
                {
                    firstActual = Convert.ToInt32(client.Invoke("Add", new object[] { 2, 3 }));
                    secondActual = Convert.ToInt32(client.Invoke("Add", new object[] { 30, 7 }));
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);

            Assert.Equal(firstExected, firstActual);
            Assert.Equal(secondExected, secondActual);
        }

        [Fact]
        public void ClientCatchesServerExceptions()
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
            var clientTask = Task.Run(() =>
            {
                using (var client = new Client(port))
                {
                    Assert.Throws<RemoteInvokationFailed>(() => { client.Invoke("Add", new object[] { 2, 3 }); });
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);
        }

        [Fact(Skip = "CI")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Cannot get Travis CI to run this test")]
        public void ServerCanListenToClientProcessStatus()
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
            var clientTask = Task.Run(() =>
            {
                using var client = new Client(port);
            });

            Task.WaitAll(serverTask, clientTask);

            Assert.True(true);
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
