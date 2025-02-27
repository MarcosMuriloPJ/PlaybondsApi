namespace PlaybondsApi.Services;

public class GameTimerService(GameService gameService) : IHostedService, IDisposable
{
  private readonly GameService _gameService = gameService;
  private Timer? _timer;

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _timer = new Timer(ExecuteCheckForEliminations, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.1));
    return Task.CompletedTask;
  }

  private void ExecuteCheckForEliminations(object state)
  {
    _gameService.CheckForEliminations();
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _timer?.Change(Timeout.Infinite, 0);
    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _timer?.Dispose();
  }
}
