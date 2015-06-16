using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message RollAction(Object input)
        {
            Persistence.Connection.Delete(this);

            // roll dice
            var roll = Dice.RollDice(1, 6);

            // move player to next space
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var newSpace = game.Board.GetNewSpace(player.SpaceId, roll.Sum, player.MovementDirection);
            var message = game.Board.MovePlayer(game, player, newSpace.Id);

            return new Message(string.Format("Rolled a {0}. {1}", roll.Sum, message.Text));
        }
    }
}
