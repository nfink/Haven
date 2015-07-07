using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            var board = Persistence.Connection.Query<Board>("select Board.* from Board join Game on Game.BoardId=Board.Id where Game.Id=?", player.GameId).First();
            var newSpace = board.GetNewSpace(player.SpaceId, roll.Sum, (roll.Sum % 2 == 0));
            player.Move(newSpace.Id);
        }
    }
}
