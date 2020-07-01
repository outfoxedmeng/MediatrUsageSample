using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace MediatrUsageSample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            await Host.CreateDefaultBuilder()
                  .ConfigureWebHostDefaults(builder =>
                  {
                      builder.ConfigureServices(services =>
                      {
                          services.AddMediatR(typeof(Ping));
                          services.AddSingleton<IFoo, Foo>();
                      })
                      .Configure(app =>
                      {
                          app.UseRouting();
                          app.UseEndpoints(endpoint =>
                          {
                              endpoint.Map("/", async context =>
                              {
                                  await app.ApplicationServices.GetRequiredService<IFoo>().ShowMsgAsync(context);
                              });
                          });
                      });
                  })
                  .Build()
                  .RunAsync();

        }
    }

    public interface IFoo
    {
        Task ShowMsgAsync(HttpContext context);
    }
    public class Foo : IFoo
    {
        private readonly IMediator _mediator;

        public Foo(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task ShowMsgAsync(HttpContext context)
        {
            var pong = await _mediator.Send(new Ping { Message = "Hi" });
            await context.Response.WriteAsync(pong.Message);
        }
    }

    public class PingHandler : IRequestHandler<Ping, Pong>
    {
        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong { Message = request.Message + " There~~" });
        }
    }

    public class Ping : IRequest<Pong>
    {
        public string Message { get; set; }
    }
    public class Pong
    {
        public string Message { get; set; }
    }
}
