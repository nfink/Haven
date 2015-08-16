using System;

namespace Haven
{
    public partial class Action
    {
        private void ExchangePlacesAction(Object input)
        {
            // remove all exchange places actions
            this.RemoveActions(ActionType.ExchangePlaces);

            // swap locations
            var player = this.Owner;
            var playerToExchangeWith = this.Player;
            var swapLocation = player.SpaceId;
            player.SpaceId = playerToExchangeWith.SpaceId;
            playerToExchangeWith.SpaceId = swapLocation;
            this.Repository.Update(player);
            this.Repository.Update(playerToExchangeWith);

            player.Game.EndTurn(this.OwnerId);
            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Exchanged places with {0}.", playerToExchangeWith.Name) });
        }
    }
}
