using System;

namespace Haven
{
    public partial class Action
    {
        private void TurnAroundAction(Object input)
        {
            // remove actions
            this.Repository.Remove(this);
            this.RemoveActions(ActionType.EndTurn);

            // flip movement direction
            var player = this.Owner;
            player.MovementDirection = !player.MovementDirection;
            this.Repository.Update(player);
            Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);
            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Turned around." });
        }
    }
}
