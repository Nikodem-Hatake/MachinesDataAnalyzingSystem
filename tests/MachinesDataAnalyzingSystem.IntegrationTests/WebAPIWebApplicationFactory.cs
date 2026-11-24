using MachinesDataAnalyzingSystem.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testcontainers.Seq;

namespace MachinesDataAnalyzingSystem.IntegrationTests;

public class WebAPIWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public string ValidAPIKey { get; } = Guid.NewGuid().ToString();
    private readonly RedisContainer _cacheContainer = new RedisBuilder("eqalpha/keydb:latest").Build();
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("MachinesData")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private readonly SeqContainer _seqContainer = new SeqBuilder("datalust/seq")
        .WithAcceptLicenseAgreement(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("APIKey", ValidAPIKey);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IDbContextFactory<ApplicationDbContext>));
            if(descriptor is not null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IDistributedCache));
            if(descriptor is not null)
            {
                services.Remove(descriptor);
            }
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _cacheContainer.GetConnectionString();
            });
        });

        builder.UseSetting("Serilog:WriteTo:1:Args:serverUrl", _seqContainer.GetConnectionString());
    }

    public new async Task DisposeAsync()
    {
        await _cacheContainer.StopAsync();
        await _dbContainer.StopAsync();
        await _seqContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _cacheContainer.StartAsync();
        await _dbContainer.StartAsync();
        await _seqContainer.StartAsync();
    }
}