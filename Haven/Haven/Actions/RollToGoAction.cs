using System;

namespace Haven
{
    public partial class Action
    {
        private void RollToGoAction(Object input)
        {
            this.Repository.Remove(this);

            // roll dice
            var roll = Dice.RollDice(1, 6);
            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Rolled a {0} to go.", roll.Sum) });

            // move player forward if even, backward if odd
            var player = this.Owner;
            var newSpace = player.Game.Board.GetNewSpace(player.SpaceId, roll.Sum, (roll.Sum % 2 == 0));
            player.Move(newSpace.Id);
        }
    }
}
