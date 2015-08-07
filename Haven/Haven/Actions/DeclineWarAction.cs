using System;

namespace Haven
{
    public partial class Action
    {
        private void DeclineWarAction(Object input)
        {
            // remove all war actions
            this.RemoveActions(ActionType.DeclareWar);
            this.RemoveActions(ActionType.DeclineWar);

            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);

            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Declined to declare war." });
        }
    }
}
