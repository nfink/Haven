using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void EnterPasswordAction(Object input)
        {
            Persistence.Connection.Delete(this);
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            player.SetPassword((string)input);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            game.StartGame();
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Password saved." });
        }
    }
}
