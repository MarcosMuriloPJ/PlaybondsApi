using PlaybondsApi.Models;

namespace PlaybondsApi.Services;

public class GameService(NotificationService notificationService)
{
  private static readonly decimal _maxTime = 30;
  private State _state = new(string.Empty);
  private readonly NotificationService _notificationService = notificationService;

  /// <summary>
  /// Retorna o estado atual do jogo.
  /// </summary>
  /// <param name="start">Se ativa o jogo, define tempo e último clique dos jogadores.</param>
  /// <returns>Retorna o estado atual do jogo.</returns>
  public State GetState(bool start)
  {
    if (start)
    {
      _state.Active = true;

      foreach (var player in _state.Players)
      {
        player.TimeSpent = _maxTime;
        player.LastClick = DateTime.UtcNow;
      }
    }

    return _state;
  }

  /// <summary>
  /// Verifica se o jogo foi iniciado.
  /// </summary>
  /// <returns>Boleano indicando se iniciado.</returns>
  public bool IsGameStarted() => GetPlayers().Count == 4;

  /// <summary>
  /// Retorna a lista de jogadores.
  /// </summary>
  /// <returns></returns>
  public IReadOnlyList<Player> GetPlayers() => _state.Players;

  /// <summary>
  /// Adiciona um jogador ao jogo e defini-lo como jogador atual.
  /// </summary>
  /// <param name="name">Nome do jogador.</param>
  public void AddPlayer(string name)
  {
    var player = new Player(name);

    _state.Players.Add(player);
    _state.CurrentPlayer = name;
  }

  /// <summary>
  /// Verifica se algum jogador foi eliminado e se houve vencedor.
  /// </summary>
  public async Task CheckForEliminations()
  {
    if (_state.Active)
    {
      foreach (var player in _state.Players)
      {
        if (player.Name != _state.CurrentPlayer || player.IsEliminated)
          continue;

        player.TimeSpent -= (decimal)0.1;

        if (player.TimeSpent <= 0)
        {
          NextTurn();
          player.IsEliminated = true;

          await _notificationService.NotifyPlayerEliminated(_state, player.Name);
        }
      }

      await CheckWinner();
    }
  }

  /// <summary>
  /// Processa o clique de um jogador.
  /// </summary>
  /// <param name="name">Nome do jogador.</param>
  public void ProcessPlayerClick(string name)
  {
    foreach (var player in _state.Players)
      if (player.Name == name)
        player.LastClick = DateTime.UtcNow;

    NextTurn();
  }

  /// <summary>
  /// Define o próximo jogador como jogador atual.
  /// </summary>
  private void NextTurn()
  {
    var activePlayers = _state.Players.Where(p => !p.IsEliminated).ToList();
    var currentPlayerIndex = activePlayers.FindIndex(p => p.Name == _state.CurrentPlayer);
    var nextPlayerIndex = (currentPlayerIndex + 1) % activePlayers.Count;

    _state.CurrentPlayer = activePlayers[nextPlayerIndex].Name;
  }

  /// <summary>
  /// Verifica se há um vencedor.
  /// </summary>
  private async Task CheckWinner()
  {
    var activePlayers = GetPlayers().Where(p => !p.IsEliminated).ToList();

    if (activePlayers.Count == 1)
    {
      _state.Active = false;
      _state.Players = [];

      var winner = activePlayers.First();
      await _notificationService.NotifyWinner(_state, winner.Name);
    }
  }
}