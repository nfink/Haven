using System;

namespace Haven
{
    public partial class Action
    {
        private void EndTurnAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where OwnerId=?", this.OwnerId);

            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);

            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Ended turn." });
        }
    }
}
