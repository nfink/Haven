using System;

namespace Haven
{
    public partial class Action
    {
        private void DeclineWarAction(Object input)
        {
            // remove all war actions
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.DeclareWar, ActionType.DeclineWar, this.OwnerId);

            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);

            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Declined to declare war." });
        }
    }
}
