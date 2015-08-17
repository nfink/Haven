using Newtonsoft.Json;
using System;
using System.Linq;

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
            var player = this.Owner;
            var game = this.Repository.Get<Game>(player.GameId);
            var samePiece = game.Players.Where(x => x.PieceId == pieceId && x.ColorId == colorId);
            if (samePiece.Count() > 0)
            {
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Another player has the same piece. Please choose another picture and/or color." });
            }
            else
            {
                this.Repository.Remove(this);

                // set piece
                var piece = this.Repository.Get<Piece>(pieceId);
                var color = this.Repository.Get<Color>(colorId);
                player.PieceId = pieceId;
                player.ColorId = colorId;
                this.Repository.Update(player);

                game.StartGame();
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("{0} {1} piece selected.", piece.Name, color.Name) });
            }
        }
    }
}
