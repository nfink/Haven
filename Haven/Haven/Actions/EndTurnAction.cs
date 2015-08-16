using System;

namespace Haven
{
    public partial class Action
    {
        private void EndTurnAction(Object input)
        {
            // remove all actions
            foreach (Action action in this.Owner.Actions)
            {
                this.Repository.Remove(action);
            }

            this.Owner.Game.EndTurn(this.OwnerId);

            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Ended turn." });
        }
    }
}
