using System;

namespace Haven
{
    public partial class Action
    {
        private void RollAction(Object input)
        {
            this.Repository.Remove(this);

            // roll dice
            var roll = Dice.RollDice(1, 6);
            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Rolled a {0}.", roll.Sum) });

            // move player to next space
            var player = this.Owner;
            var board = Game.GetGame(player.Id).Board;
            var newSpace = board.GetNewSpace(player.SpaceId, roll.Sum, player.MovementDirection);
            player.Move(newSpace.Id);
        }
    }
}
