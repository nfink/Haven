using System.Linq;

namespace Haven
{
    public partial class Space
    {
        private void OnLandSafeHaven(Player player)
        {
            // add the safe haven card if the player has all name cards
            var safeHaven = Persistence.Connection.Get<SafeHavenCard>(this.SafeHavenCardId);
            var game = Game.GetGame(player.Id);
            if ((player.NameCards.Count() >= game.Board.NameCards.Count()) && (!player.SafeHavenCards.Select(x => x.Id).Contains(safeHaven.Id)))
            {
                Persistence.Connection.Insert(new PlayerSafeHavenCard() { PlayerId = player.Id, SafeHavenCardId = this.SafeHavenCardId });
                Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("{0} protects you this turn and gives you an award", safeHaven.Name) });
            }
            else
            {
                Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("{0} protects you this turn.", safeHaven.Name) });
            }

            game.EndTurn(player.Id);
        }
    }
}
