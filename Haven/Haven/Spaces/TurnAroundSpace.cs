using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandTurnAround(Player player)
        {
            // change player movement direction
            player.MovementDirection = !player.MovementDirection;

            Game.EndTurn(player.Id);

            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Turned around." });
        }
    }
}
