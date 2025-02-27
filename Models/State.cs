namespace PlaybondsApi.Models;

public class State(string currentPlayer)
{
  public string CurrentPlayer { get; set; } = currentPlayer;
  public List<Player> Players { get; set; } = [];
  public bool Active { get; set; } = false;
}
