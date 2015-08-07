using Newtonsoft.Json;
using System;

namespace Haven
{
    public partial class Action
    {
        private void SelectPieceAction(Object input)
        {
            var selection = JsonConvert.DeserializeObject<dynamic>((string)input);
            var pieceId = (int)selection.PieceId;
            var colorId = (int)selection.ColorId;

            // make sure no other player in the game has the same piece
            var samePiece = Persistence.Connection.Query<Player>("select * from Player where PieceId=? and ColorId=?", pieceId, colorId);
            if (samePiece.Count > 0)
            {
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Another player has the same piece. Please choose another picture and/or color." });
            }
            else
            {
                this.Repository.Remove(this);

                // set piece
                var piece = this.Repository.Get<Piece>(pieceId);
                var color = this.Repository.Get<Color>(colorId);
                var player = this.Owner;
                player.PieceId = pieceId;
                player.ColorId = colorId;
                this.Repository.Update(player);

                Game.GetGame(this.OwnerId).StartGame();
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("{0} {1} piece selected.", piece.Name, color.Name) });
            }
        }
    }
}
