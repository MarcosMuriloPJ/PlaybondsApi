namespace PlaybondsApi.Models;

public class Player(string name)
{
  public string Name { get; set; } = name;
  public decimal TimeSpent { get; set; } = 0;
  public bool IsEliminated { get; set; } = false;
  public DateTime LastClick { get; set; } = DateTime.UtcNow;
}
