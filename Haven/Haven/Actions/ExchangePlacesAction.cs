using System;

namespace Haven
{
    public partial class Action
    {
        private void ExchangePlacesAction(Object input)
        {
            // remove all exchange places actions
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.ExchangePlaces, this.OwnerId);

            // swap locations
            var player = this.Owner;
            var playerToExchangeWith = this.Player;
            var swapLocation = player.SpaceId;
            player.SpaceId = playerToExchangeWith.SpaceId;
            playerToExchangeWith.SpaceId = swapLocation;
            Persistence.Connection.Update(player);
            Persistence.Connection.Update(playerToExchangeWith);

            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Exchanged places with {0}.", playerToExchangeWith.Name) });
        }
    }
}
