using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private Message OnLandOptionalTurnAround(Player player)
        {
            Persistence.Connection.Insert(new Action() { Type = ActionType.TurnAround, OwnerId = player.Id });
            Persistence.Connection.Insert(new Action() { Type = ActionType.EndTurn, OwnerId = player.Id });

            return new Message("You may choose to turn around.");
        }
    }
}
