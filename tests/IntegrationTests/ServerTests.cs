using Netler;
using Netler.Exceptions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class ServerTests
    {
        [Fact]
        public void ClientServerCommunication()
        {
            const int Port = 53666;

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(Port);
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
                using (var client = new Client(Port))
                {
                    actual = Convert.ToInt32(client.Invoke("Add", new object[] { 2, 3 }));
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ClientCatchesServerExceptions()
        {
            const int Port = 53667;

            var server = Server
                .Create((config) =>
                {
                    config.UsePort(Port);
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
                using (var client = new Client(Port))
                {
                    Assert.Throws<RemoteInvokationFailed>(() => { client.Invoke("Add", new object[] { 2, 3 }); });
                }
                server.Stop();
            });

            Task.WaitAll(serverTask, clientTask);
        }
    }
}
