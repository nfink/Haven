using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void SelectPieceAction(Object input)
        {
            // remove all other select piece actions
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.SelectPiece, this.OwnerId);

            // set piece
            var piece = Persistence.Connection.Get<Piece>(this.PieceId);
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            player.PieceId = this.PieceId;
            Persistence.Connection.Update(player);

            var game = Persistence.Connection.Get<Game>(player.GameId);
            game.StartGame();
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("{0} piece selected.", piece.Name) });
        }
    }
}
