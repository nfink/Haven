﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandExchangePlaces(Player player)
        {
            // add an exchange places action for each other player
            var game = Persistence.Connection.Get<Game>(player.GameId);
            foreach (Player p in game.Players.Where(x => x.Id != player.Id))
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.ExchangePlaces, OwnerId = player.Id, PlayerId = p.Id });
            }

            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Select a player to exchange places with." });
        }
    }
}
