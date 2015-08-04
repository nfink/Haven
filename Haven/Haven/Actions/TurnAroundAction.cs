using System;

namespace Haven
{
    public partial class Action
    {
        private void TurnAroundAction(Object input)
        {
            Persistence.Connection.Delete(this);
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.EndTurn, this.OwnerId);
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            player.MovementDirection = !player.MovementDirection;
            Persistence.Connection.Update(player);
            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Turned around." });
        }
    }
}
