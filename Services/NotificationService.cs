using Microsoft.AspNetCore.SignalR;
using PlaybondsApi.Hubs;
using PlaybondsApi.Models;

namespace PlaybondsApi.Services;

public class NotificationService(IHubContext<GameHub> hubContext)
{
  private readonly IHubContext<GameHub> _hubContext = hubContext;

  public async Task NotifyPlayerEliminated(State gameState, string playerName)
  {
    await _hubContext.Clients.All.SendAsync("PlayerEliminated", playerName);
    await Updates(gameState);
  }

  public async Task NotifyWinner(State gameState, string playerName)
  {
    await _hubContext.Clients.All.SendAsync("Winner", playerName);
    await Updates(gameState);
  }

  private async Task Updates(State gameState)
  {
    await _hubContext.Clients.All.SendAsync("UpdatePlayerList", gameState.Players);
    await _hubContext.Clients.All.SendAsync("UpdateGameState", gameState);
  }
}
