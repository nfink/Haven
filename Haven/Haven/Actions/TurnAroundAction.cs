using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message TurnAroundAction(Object input)
        {
            Persistence.Connection.Delete(this);
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.EndTurn, this.OwnerId);
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            player.MovementDirection = !player.MovementDirection;
            Persistence.Connection.Update(player);
            Game.EndTurn(this.OwnerId);
            return new Message("Turned around.");
        }
    }
}
