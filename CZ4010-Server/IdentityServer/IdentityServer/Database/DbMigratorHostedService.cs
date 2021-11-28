using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Database
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbMigratorHostedService(IServiceProvider serviceProvider,
            IHostApplicationLifetime lifetime)
        {
            Console.WriteLine("Initialising hosted service");
            _serviceProvider = serviceProvider;
        }
        
        public DbMigratorHostedService(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Initialising hosted service");
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                var timeout = 5;
                var tries = 0;
                var canConnect = false;
                while (!canConnect)
                {
                    if (tries > timeout)
                    {
                        Environment.ExitCode = 1;
                        break;
                    }

                    try
                    {
                        canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
                    }
                    catch (Exception e)
                    {
                    }

                    tries++;
                }


                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Environment.ExitCode = 1;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}