using Microsoft.AspNetCore.SignalR;
using PlaybondsApi.Services;

namespace PlaybondsApi.Hubs;

public class GameHub(GameService gameService) : Hub
{
  private readonly GameService _gameService = gameService;

  public async Task ConnectPlayer(string playerName)
  {
    _gameService.AddPlayer(playerName);
    await Clients.All.SendAsync("UpdatePlayerList", _gameService.GetPlayers());

    if (_gameService.IsGameStarted())
      await StartGame();
  }

  public async Task PlayerClicked(string playerName)
  {
    _gameService.ProcessPlayerClick(playerName);

    await UpdateState(false);
  }

  private async Task StartGame()
  {
    await UpdateState(true);
  }

  private async Task UpdateState(bool start)
  {
    var gameState = _gameService.GetState(start);
    if (gameState != null)
      await Clients.All.SendAsync("UpdateGameState", gameState);
  }
}