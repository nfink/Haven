using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void RollAction(Object input)
        {
            Persistence.Connection.Delete(this);

            // roll dice
            var roll = Dice.RollDice(1, 6);
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Rolled a {0}.", roll.Sum) });

            // move player to next space
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var newSpace = game.Board.GetNewSpace(player.SpaceId, roll.Sum, player.MovementDirection);
            game.Board.MovePlayer(game, player, newSpace.Id);
        }
    }
}
