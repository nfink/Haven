using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message ExchangePlacesAction(Object input)
        {
            // remove all exchange places actions
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.ExchangePlaces, this.OwnerId);

            // swap locations
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            var playerToExchangeWith = Persistence.Connection.Get<Player>(this.PlayerId);
            var swapLocation = player.SpaceId;
            player.SpaceId = playerToExchangeWith.SpaceId;
            playerToExchangeWith.SpaceId = swapLocation;
            Persistence.Connection.Update(player);
            Persistence.Connection.Update(playerToExchangeWith);

            Game.EndTurn(this.OwnerId);
            return new Message(string.Format("Exchanged places with {0}.", playerToExchangeWith.Name));
        }
    }
}
