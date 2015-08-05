using System;

namespace Haven
{
    public partial class Action
    {
        private void RollToGoAction(Object input)
        {
            Persistence.Connection.Delete(this);

            // roll dice
            var roll = Dice.RollDice(1, 6);
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Rolled a {0} to go.", roll.Sum) });

            // move player forward if even, backward if odd
            var player = this.Owner;
            var board = Game.GetGame(player.Id).Board;
            var newSpace = board.GetNewSpace(player.SpaceId, roll.Sum, (roll.Sum % 2 == 0));
            player.Move(newSpace.Id);
        }
    }
}
