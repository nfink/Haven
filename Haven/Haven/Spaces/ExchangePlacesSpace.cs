using System.Linq;

namespace Haven
{
    public partial class Space
    {
        private void OnLandExchangePlaces(Player player)
        {
            // add an exchange places action for each other player
            foreach (Player p in Game.GetGame(player.Id).Players.Where(x => x.Id != player.Id))
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.ExchangePlaces, OwnerId = player.Id, PlayerId = p.Id });
            }

            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Select a player to exchange places with." });
        }
    }
}
