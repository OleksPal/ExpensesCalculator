using ExpensesCalculator.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Services.Auth;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24);

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Token Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredTokens(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token cleanup");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Token Cleanup Service stopped");
    }

    private async Task CleanupExpiredTokens(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ExpensesContext>();

        var cutoffDate = DateTime.UtcNow;
        var revokedCutoff = DateTime.UtcNow.AddDays(-30);

        var expiredTokens = await context.RefreshTokens
            .Where(t => t.Expires < cutoffDate || (t.IsRevoked && t.Expires < revokedCutoff))
            .ToListAsync(cancellationToken);

        if (expiredTokens.Any())
        {
            context.RefreshTokens.RemoveRange(expiredTokens);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens", expiredTokens.Count);
        }
    }
}
